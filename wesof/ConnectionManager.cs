using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace wesof
{
    class ConnectionManager
    {
        private Controller controller;
        private CommandLineInterface _CLI;

        private TcpListener _Listener = null;
        private bool _IsListening = false;
        private List<TcpClient> _Clients = new List<TcpClient>();
        private List<ClientHandler> _ClientHandlers = new List<ClientHandler>();

        private int _ClientIdCounter = 1;

        public ConnectionManager(CommandLineInterface _CLI, Controller controller)
        {
            this._CLI = _CLI;
            this.controller = controller;
        }

        public void Start()
        {
            Stop(false);
            
            if (_Listener == null)
            {
                try
                {
                    _Listener = new TcpListener(Configuration.LocalAddress, Configuration.ListenerPortNumber);
                    _CLI.Out("Listener created.");
                }
                catch (Exception e)
                {
                    _CLI.Out("Error while creating listener." + e.Message);
                }
            }

            if (_Listener != null)
            {
                try
                {
                    _Listener.Start();
                    _CLI.Out("Listening on " + Configuration.LocalAddress + ":" + Configuration.ListenerPortNumber);
                    _IsListening = true;
                    WaitForTcpClients();
                }
                catch (Exception e)
                {
                    _CLI.Out("Error while starting listener." + e.Message);
                }
            }
        }


        private async void WaitForTcpClients()
        {
            while(_Listener != null && _IsListening)
            {
                TcpClient newClient = await _Listener.AcceptTcpClientAsync();
                ClientHandler newHandler = new ClientHandler(newClient, _ClientIdCounter, this, _CLI);
                _Clients.Add(newClient);
                _ClientHandlers.Add(newHandler);

                if (Configuration.IsDisplayConnections)
                {
                    _CLI.Out("New client accepted from " + ((IPEndPoint)newClient.Client.RemoteEndPoint).Address.ToString()
                        + " . Client ID: " + _ClientIdCounter.ToString(), true, true);
                }
                newHandler.StartHandling();

                _ClientIdCounter++;               
            }
           
        }

        internal void ClientGracefullShut(int _ClientIdCounter, ClientHandler clientHandler, TcpClient _Client)
        {
            _Clients.Remove(_Client);
            _ClientHandlers.Remove(clientHandler);

        }

        public void Stop(bool showConfirmationMessage)
        {
            
            if (_Listener != null)
            {
                try
                {
                    _Listener.Stop();
                    _IsListening = false;
                    _Clients = new List<TcpClient>();
                    _ClientHandlers = new List<ClientHandler>();
                    if (showConfirmationMessage)
                        _CLI.Out("Listener stopped.");
                }
                catch (Exception e)
                {
                    _CLI.Out("Error. Could not stop the listener." + e.Message);
                }
            }
            else
            {
                _IsListening = false;
            }
        }



        internal void CustomSendAndListenServer(string sendFixedRequest)
        {
            TcpClient customClient = new TcpClient(Configuration.WebServerAddress.ToString(), Configuration.WebServerPortNumber);
            string strmsg = sendFixedRequest + "\r\n\r\n";
            byte[] msg = Encoding.UTF8.GetBytes(strmsg);

            NetworkStream stream = customClient.GetStream();

            stream.Write(msg,0,msg.Length);
            stream.Flush();

            _CLI.Out("Your custom message sent. Waiting for response.", true, false);

            int tmp = stream.ReadByte();
            LinkedList<byte> response = null; 
            while(tmp != -1 && stream.DataAvailable)
            {
                if(response == null)
                {
                    response = new LinkedList<byte>();
                }
                tmp = stream.ReadByte();
                response.AddLast((byte)tmp);
            }

            if(response != null)
            {
                byte[] bResult = response.ToArray();
                string sResult = Encoding.UTF8.GetString(bResult);

                _CLI.Out("Server response:", true, false);
                _CLI.Out(sResult, true, true);

            }

        }

    }
}
