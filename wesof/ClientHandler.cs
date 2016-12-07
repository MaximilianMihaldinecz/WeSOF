using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using wesof.MVTManager;

namespace wesof
{
    class ClientHandler
    {

        Timer _HeartBeat = new Timer(Configuration.ClientThreadRefreshInterval);

        private ConnectionManager _ConnectionManager;
        private TcpClient _Client;
        private TcpClient _WebServer = null;
        private int _ClientIdCounter;
        private CommandLineInterface _CLI;        

        private NetworkStream _CS = null;
        private NetworkStream _SS = null;

        private bool ContinueReadWrite = false;
        private bool InRWMode = false;
        private int closeDownCounter = 0;

        private ParserMatcher _MVTManager = null;

        private ManagedCookie _Cookie = null;

        private string _LastRequestedURL = null;


        public ClientHandler(TcpClient newClient, int _ClientIdCounter, ConnectionManager connectionManager, CommandLineInterface CLI)
        {
            this._Client = newClient;
            this._ClientIdCounter = _ClientIdCounter;
            this._ConnectionManager = connectionManager;
            this._CLI = CLI;

            _HeartBeat.Elapsed += HeartBeatHandler;
        }

        private void HeartBeatHandler(object sender, ElapsedEventArgs e)
        {
            if(InRWMode == false)
            {
                
                ContinueReadWrite = ShouldContinueRW();
                if(ContinueReadWrite)
                {
                    StartReadWrite();
                    closeDownCounter = 0;
                }

                closeDownCounter += Configuration.ClientThreadRefreshInterval;
                if(closeDownCounter >= Configuration.CloseConnectionAfterTime)
                {
                    Shutdown();
                }
            }
        }

        private void Shutdown()
        {
            _HeartBeat.Stop();

            //Try Graceful close of client
            /*
            try
            {
                byte[] lastWord = { 13, 10, 13, 10, 0, 13, 10, 13, 10 };
                WriteToStream(_CS, lastWord);
            } catch
            {
                if (Configuration.IsDebugModeOn)
                    _CLI.Out("Could not send graceful closing message. (Client ID: " + _ClientIdCounter.ToString() + ")");
            }
            */

            _CS.Close();
            _Client.Close();
            _SS.Close();
            _WebServer.Close();

            _WebServer = null;
            _SS = null;

            _ConnectionManager.ClientGracefullShut(_ClientIdCounter, this, _Client);
        }

        public async void StartHandling()
        {            
            
            //Opening connection towards the client
            if(_CS == null)
            {
                try
                {
                    _CS = _Client.GetStream();

                    if (Configuration.IsDisplayConnections)
                        _CLI.Out("Client stream created. (Client ID: " + _ClientIdCounter.ToString() + ")");
                } catch (Exception e)
                {
                    _CLI.Out("Error. Client stream cannot be created. (Client ID: " + _ClientIdCounter.ToString() + ")");
                }                
            }

            //Opening connection towards the webserver
            if (_CS != null)
            {

                try
                {
                    _WebServer = new TcpClient(Configuration.WebServerAddress.ToString(), Configuration.WebServerPortNumber);
                    _SS = _WebServer.GetStream();

                    if (Configuration.IsDisplayConnections)
                        _CLI.Out("Server stream created. (Client ID: " + _ClientIdCounter.ToString() + ")");

                }
                catch (Exception e)
                {
                    _CLI.Out("Error. Server stream cannot be created. (Client ID: " + _ClientIdCounter.ToString() + ")");
                }
            }


            if (_CS != null && _SS != null)
            {
                ContinueReadWrite = true;
                StartReadWrite();                
                _HeartBeat.Start();          
            }
        }

        public async void StartReadWrite()
        {
            int counter = 0;              
                    
            while (ContinueReadWrite)
            {
                InRWMode = true;
                _Cookie = new ManagedCookie();

                //READ the client request in full
                byte[] lastRequest = ReadUntilEnd(_CS);

                if (Configuration.IsDisplayRawTraffic)
                    DisplayTraffic("RAW incoming traffic", lastRequest);


                //FORWARD the request to the server     
                byte[] msgToSendToServer = ManipulateRequestHeaderBeforeForwarding(lastRequest);
                WriteToStream(_SS, msgToSendToServer);

                if (Configuration.IsDisplayRawTraffic)
                    DisplayTraffic("RAW data to server", msgToSendToServer);


                //GET response from the server
                byte[] serverResponse = ReadUntilEnd(_SS);

                if (Configuration.IsDisplayRawTraffic)
                    DisplayTraffic("RAW response from the server", serverResponse);


                //FORWARD server response to the client
                byte[] msgToClient = ManipulateServerResponseBeforeForwarding(serverResponse);
                WriteToStream(_CS, msgToClient);

                if (Configuration.IsDisplayRawTraffic)
                    DisplayTraffic("RAW forwarding server response to client", msgToClient);



                _Cookie = null;
                //Should the loop continue?
                ContinueReadWrite = ShouldContinueRW();                
                InRWMode = false;
                counter++;
            }

        }

        private bool ShouldContinueRW()
        {
            bool result = true;

            result = _Client.Connected;
            result = result && _CS.CanRead;            
            result = result && _CS.DataAvailable;                               
            
            return result;
        }


        private ServerResponse SplitServerResponse(byte[] inputResponse, bool chkForContentType)
        {
            ServerResponse Result = new ServerResponse();

            //Let's find the first CRLFCRLF \r\n\r\n
            int iRnRn = 0;
            for (iRnRn = 2; iRnRn < inputResponse.Length; iRnRn++)
            {
                if(iRnRn + 3 >= (inputResponse.Length))
                {
                    iRnRn = inputResponse.Length;
                    break;
                }
                else
                {
                    if (inputResponse[iRnRn] == 13 && inputResponse[iRnRn + 1] == 10 && inputResponse[iRnRn + 2] == 13 && inputResponse[iRnRn + 3] == 10)
                       break;
                }
                
            }

            if (iRnRn == inputResponse.Length)
            {
                if (Configuration.IsDebugModeOn)
                {
                    _CLI.Out("Error. Cannot find header in the byte array response.(Client ID: " + _ClientIdCounter.ToString() + ")");
                }
                return Result;
            }

            //Create byte array with original body
            if (chkForContentType)
            {
                Result.Body = new byte[inputResponse.Length - (iRnRn + 4)];
                Array.Copy(inputResponse, iRnRn + 4, Result.Body, 0, inputResponse.Length - (iRnRn + 4));
            }
            

            //Create byte array with the original response
            Result.Header = new byte[iRnRn + 4];
            Array.Copy(inputResponse, 0, Result.Header, 0, iRnRn + 4);

            //Convert header to Str
            Result.StrHeader = Encoding.UTF8.GetString(Result.Header);

            //Check if the content type is html
            if (chkForContentType)
            {             
                int iContentType = Result.StrHeader.IndexOf("content-type:", StringComparison.InvariantCultureIgnoreCase);
                if (iContentType > 0)
                {
                    int iEoL = Result.StrHeader.IndexOf("\r\n", iContentType);
                    int isHtml = Result.StrHeader.IndexOf(@"text/html", iContentType, (iEoL - iContentType) + 2, StringComparison.InvariantCultureIgnoreCase);

                    //If the content is HTML, mark it down
                    if (isHtml > 0)                
                        Result.isHtmlTextContent = true;
                    else
                        Result.isHtmlTextContent = false;
                }
                else
                {
                    //If no content-type found.
                    if (Configuration.IsDebugModeOn && Configuration.IsDisplayRawTraffic)
                    {
                        _CLI.Out("Error. Cannot find Content-Type in the http response. (Client ID: " + _ClientIdCounter.ToString() + ")");
                    }                   
                }
            }

            return Result;
        }

        private void DeleteContentLength(ref ServerResponse SRe)
        {
            if(SRe.ModifiedHeader == null || SRe.ModifiedHeader == "")
            {
                SRe.ModifiedHeader = SRe.StrHeader;
            }


            int iClength = SRe.ModifiedHeader.IndexOf("content-length:", StringComparison.InvariantCultureIgnoreCase);
            if (iClength > 0)
            {
                int iEoL = SRe.ModifiedHeader.IndexOf("\r\n", iClength);
                SRe.ModifiedHeader = SRe.ModifiedHeader.Remove(iClength, (iEoL - iClength) + 2);
            }
            else
            {
                if (Configuration.IsDebugModeOn && Configuration.IsDisplayRawTraffic)
                {
                    _CLI.Out("Error. Cannot find Content-Length in the http response. (Client ID: " + _ClientIdCounter.ToString() + ")");
                }                
            }
        }

        private void DeleteContentLength(ref string SRe)
        {           
            int iClength = SRe.IndexOf("content-length:", StringComparison.InvariantCultureIgnoreCase);
            if (iClength > 0)
            {
                int iEoL = SRe.IndexOf("\r\n", iClength);
                SRe = SRe.Remove(iClength, (iEoL - iClength) + 2);
            }
            else
            {
                if (Configuration.IsDebugModeOn && Configuration.IsDisplayRawTraffic)
                {
                    _CLI.Out("Error. Cannot find Content-Length in the http response. (Client ID: " + _ClientIdCounter.ToString() + ")");
                }
            }
        }

        private void ReplaceAddressInServerResponse(ref ServerResponse SRe)
        {
            string hostNameToReplace = Configuration.WebServerAddress.ToString();
            string newHostName = Configuration.LocalAddress.ToString() + ":" + Configuration.ListenerPortNumber.ToString(); ;

            //Replace hostname in the header
            SRe.ModifiedHeader = SRe.ModifiedHeader.Replace(hostNameToReplace, newHostName);

            //Convert body to string
            SRe.ModifiedBody = Encoding.UTF8.GetString(SRe.Body);
            SRe.ModifiedBody = SRe.ModifiedBody.Replace(hostNameToReplace, newHostName);
        }

        private void MergeModifiedBodyAndHeader(ref ServerResponse SRe)
        {
            string nHeader;
            if (SRe.ModifiedHeader != null)
                nHeader = SRe.ModifiedHeader;
            else
                nHeader = SRe.StrHeader;

            if(SRe.ModifiedBody != null)
            {
                //If body was not modified
                string nResponse = nHeader + SRe.ModifiedBody;
                SRe.MergedModified = Encoding.UTF8.GetBytes(nResponse);
            }
            else
            {
                //If body was modified

                //Convert header to byte array
                byte[] bnHeader = Encoding.UTF8.GetBytes(nHeader);
                //Copy byte header to the merged array
                SRe.MergedModified = new byte[bnHeader.Length + SRe.Body.Length];

                //Copy header to the merged array
                Array.Copy(bnHeader, SRe.MergedModified, bnHeader.Length);
                //Copy unmodified body to the merged array
                Array.Copy(SRe.Body, 0, SRe.MergedModified, bnHeader.Length, SRe.Body.Length);
            }

        }

        private byte[] ManipulateServerResponseBeforeForwarding(byte[] serverResponse)
        {
                        
            //Chk for too short (invalid) server responses
            if(serverResponse.Length < 4)
            {
                if (Configuration.IsDebugModeOn)
                {
                    _CLI.Out("Error. Too short server response. Nothing to manipulate. (Client ID: " + _ClientIdCounter.ToString() + ")");
                }
                return serverResponse;
            }

            ServerResponse SRep = null;
            //First, check if every response needs to be modified or only HTMLs
            if (Configuration.ProxyHtmlFilesOnly)
            {
                //Split the response to header and body if only HTML files need modification
                SRep = SplitServerResponse(serverResponse, Configuration.ProxyHtmlFilesOnly);
            }
            else
            {
                //If all response needs to be modified, then convert the response to string
                string asStr = Encoding.UTF8.GetString(serverResponse);

                // Replace hostname in every request
                string hostNameToReplace = Configuration.WebServerAddress.ToString();
                string newHostName = Configuration.LocalAddress.ToString() + ":" + Configuration.ListenerPortNumber.ToString(); ;
                string strResult = asStr.Replace(hostNameToReplace, newHostName);

                //Remove content length
                DeleteContentLength(ref strResult);

                //Convert the result back to bytes, and return it
                return Encoding.UTF8.GetBytes(strResult);
            }
            
            //This runs when only HTMLs whould be modified.
            //If header exists, then manipulate it
            if(SRep.Header != null)
            {
                
                if (SRep.isHtmlTextContent == true)
                {
                    //If only HTML files should be modified an this is a html file in the response
                    
                    //Remove content length section from the response
                    DeleteContentLength(ref SRep);

                    //Replace the server's address with proxy address in both header and body
                    ReplaceAddressInServerResponse(ref SRep);

                    //Add WeSOF Guid if not present
                    _Cookie.EnsureGUIDisPresent(ref SRep);

                    //Pass it to the MVT manager
                    PassToMVTManager(ref SRep);

                    //Check if conversion is triggered by the last url
                    //This may also result in a test completion, if it reaches significance
                    ConversionManager.CheckUrlAgainstConversionTriggers(_LastRequestedURL, SRep.Cookie.Wesof_GUID);

                    //Merge the modified header and body too
                    MergeModifiedBodyAndHeader(ref SRep);
                    return SRep.MergedModified;
                }

                if (SRep.isHtmlTextContent == null || SRep.isHtmlTextContent == false)
                {

                    //Remove content length section from the response
                    DeleteContentLength(ref SRep);

                    //Merge the modified header and body too
                    MergeModifiedBodyAndHeader(ref SRep);
                    return SRep.MergedModified;

                    //Return server response as is
                    //return serverResponse;
                }               

            }
            else
            {
                //The response does not have a header, or the splitter method failed. :-/
                //Return the original message without any manipulation
                return serverResponse;
            }


            //This should never run
            if (Configuration.IsDebugModeOn)
            {
                _CLI.Out("Error. Could not analyse the server response. Forwarding as-is. (Client ID: " + _ClientIdCounter.ToString() + ")");
            }
            return serverResponse;
        }

        private string GetRequestedUrl(ref string request)
        {
            string searchFor = "get ";            
            int getStart = request.IndexOf(searchFor, 0, StringComparison.InvariantCultureIgnoreCase) + 4;
            if(getStart >-1)
            {
                int getEnd = request.IndexOf(" ", getStart);
                if(getEnd >-1)
                {
                    string result = request.Substring(getStart, getEnd - getStart).Trim();
                    if(result.Length > 0)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private void PassToMVTManager(ref ServerResponse sRep)
        {
            if(_MVTManager == null)
            {
                _MVTManager = new ParserMatcher(_CLI);
            }

            sRep.Cookie = _Cookie;
            MVTHistory.UnAsssignGUIDfromFinishedTests(ref sRep);
            _MVTManager.ParseHtml(ref sRep);
        }

        private void DisplayTraffic(string prefix, byte[] data)
        {
            string asStr = Encoding.UTF8.GetString(data);
            _CLI.Out("(Client ID: " + _ClientIdCounter.ToString() + ") " + prefix + ":");
            _CLI.Out(asStr, true, true);
        }



        private byte[] ManipulateRequestHeaderBeforeForwarding(byte[] lastRequest)
        {
            string asStr = Encoding.UTF8.GetString(lastRequest);
           
            //Save the last requested URL
            _LastRequestedURL = GetRequestedUrl(ref asStr);            

            //Replacing hostname
            string hostNameToReplace = Configuration.LocalAddress.ToString() + ":" + Configuration.ListenerPortNumber.ToString();
            //string newHostName = Configuration.WebServerAddress.ToString() + ":" + Configuration.WebServerPortNumber.ToString();
            string newHostName = Configuration.WebServerAddress.ToString();
            string result = asStr.Replace(hostNameToReplace, newHostName);
            
            //Removing support for gzip
            int iEncod = result.IndexOf("Accept-Encoding:", StringComparison.InvariantCultureIgnoreCase);
            if(iEncod > 0)
            {
                int iEnd = result.IndexOf("\n",iEncod);
                if(iEnd>0)
                {
                    result = result.Remove(iEncod, (iEnd - (iEncod))+1  );
                }
            }

            //Remove keepalive
            if (Configuration.IsKeepAliveAlowed == false)
            {
                int iKeep = result.IndexOf("Connection:", StringComparison.InvariantCultureIgnoreCase);
                if (iKeep > 0)
                {
                    int iEnd = result.IndexOf("\n", iKeep);
                    if (iEnd > 0)
                    {
                        result = result.Remove(iKeep, (iEnd - (iKeep)) + 1);
                    }
                }
            }

            //Look for cookies in the sender request
            int iCookie = asStr.IndexOf("Cookie:");
            if(iCookie > -1)
            {
                int end = asStr.IndexOf("\r\n", iCookie);
                string tmp = asStr.Substring(iCookie, end - iCookie);
                _Cookie.SetOriginalRequestCookie(tmp);
            }            
            

            //Ensure proper \r\n\r\n ending
            result = result + "\n";


            //Return result
            return Encoding.UTF8.GetBytes(result);
        }

        public byte[] ReadUntilEnd(NetworkStream streamToUse)
        {
            byte[] result = null;

            byte[] buffer = new byte[10000000]; //~10MB
            int i = 0;

            try
            {                
                int temp = streamToUse.ReadByte();
                while (temp != -1 && streamToUse.DataAvailable)
                {                   

                    buffer[i] = (byte)temp;
                    i++;
                    
                    temp = streamToUse.ReadByte();
                }    
            }

            catch (Exception e)
            {
                result = null;
                ContinueReadWrite = ShouldContinueRW();

                if (Configuration.IsDebugModeOn)
                    _CLI.Out("Error during ReadUntilEnd(). (Client ID: " + _ClientIdCounter.ToString() + "):\n" + e.Message.ToString(),true,false);

                return result;
            }

            result = new byte[i];
            Array.Copy(buffer, 0, result, 0, i);

            return result;
        }

        public void WriteToStream(NetworkStream streamToUse, byte[] data)
        {
            streamToUse.Write(data, 0, data.Length);
            streamToUse.Flush();
        }


        private void CloseServer()
        {
            _SS.Close();
            _WebServer.Close();
        }

        private void CloseClient()
        {
            _CS.Close();
            _Client.Close();
        }

    }
}
