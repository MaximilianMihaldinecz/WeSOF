using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace wesof
{
    class CommandLineInterface
    {
        Controller _Controller;

        public void ProgramStartWelcomeMessage()
        {
            Console.WriteLine("Welcome to WeSOF!");
            Console.WriteLine("Type 'help' to get around in the command line.");
        }

        public void DisplayCurrentConfiguration()
        {
            Console.WriteLine("Current configuration:");
            Console.WriteLine("listenerPortNumber: " + Configuration.ListenerPortNumber.ToString());
            Console.WriteLine("localAddress: " + Configuration.LocalAddress.ToString());
            Console.WriteLine("webServerPortNumber: " + Configuration.WebServerPortNumber.ToString());
            Console.WriteLine("webServerAddress: " + Configuration.WebServerAddress.ToString());
            Console.WriteLine("readingBufferLength: " + Configuration.ReadingBufferLength.ToString());
            Console.WriteLine("IsDebugModeOn: " + Configuration.IsDebugModeOn.ToString());
            Console.WriteLine("isDisplayRawTraffic: " + Configuration.IsDisplayRawTraffic.ToString());
            Console.WriteLine("isMVTEnabled: " + Configuration.IsMVTEnabled.ToString());
            Console.WriteLine("isDashboardOn: " + Configuration.IsDashboardOn.ToString());
            Console.WriteLine("isAnalysisRunWithNewGuid: " + Configuration.IsAnalysisRunWithNewGuid.ToString());

            
            Console.WriteLine();
        }

        internal void SetCurrentController(Controller controller)
        {
            _Controller = controller;
        }

        public void DisplayHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("help: Show this help");
            Console.WriteLine("exit: Exit WeSOF");
            Console.WriteLine("showconfig: Displays current configuration");
            Console.WriteLine("clear: Clears the screen");
            Console.WriteLine("SET: Sets a configuration variable. Example: 'SET listenerportnumber=80'");
            Console.WriteLine("start: Starts WeSOF to listen and serve incoming connections.");
            Console.WriteLine("sendtoserver: Sends a custom string as HTTP message to the server. Closes with \\r\\n\\r\\n");
            Console.WriteLine("stat: Show MVT statistics.");
            Console.WriteLine("writeToCsv [FILE]: Writes all test results to csv file (absolute path, no spaces).");
            Console.WriteLine("addhypo [type] [params]: Adds a hypothesis, which will be used to generate variants. ");
        }


        public void WaitingForCommand()
        {
            string lastCommand = "";

            while(lastCommand.ToUpper() != "EXIT")
            {
                Console.Write("WeSOF>");
                lastCommand = Console.ReadLine();
                ParseLastCommand(lastCommand);
            }

            Console.WriteLine("Goodbye!");
        }

        private void ParseLastCommand(string lastCommand)
        {
            if (lastCommand.ToUpper().StartsWith("SET "))
            {
                SetVariable(lastCommand);
                return;
            }

            if (lastCommand.ToUpper().StartsWith("SENDTOSERVER:"))
            {
                SendToServer(lastCommand);
                return;
            }

            if (lastCommand.ToUpper().StartsWith("WRITETOCSV"))
            {
                WriteToCSV(lastCommand);
                return;
            }

            if (lastCommand.ToUpper().StartsWith("ADDHYPO"))
            {
                AddHypothesis(lastCommand);
                return;
            }

            switch (lastCommand.ToUpper())
            {
                case "HELP":
                    DisplayHelp();
                    break;
                case "STAT":
                     DisplayStat();
                     break;
                case "SHOWCONFIG":
                    DisplayCurrentConfiguration();
                    break;
                case "":
                    break;
                case "CLEAR":
                    Console.Clear();
                    break;
                case "START":
                    _Controller.LaunchConnectionManager();
                    break;                
                default:
                    Console.WriteLine("Sorry. I don't understand.");
                    break;
            }
        }

        private void AddHypothesis(string lastCommand)
        {
            string[] tags = lastCommand.Split(' ');
            
            if(tags.Length >= 3)
            {
                if(tags[1].ToUpper() == "ADDTOCARTBTN")
                {
                    string colour = null;
                    string text = null;

                    if(tags[2].Contains('#'))
                    {
                        colour = tags[2];
                    }

                    for(int i=2; i<tags.Length; i++)
                    {
                        if(!tags[i].Contains('#'))
                        {

                            if(text == null)
                            {
                                text = tags[i];
                            }
                            else
                            {
                                text += tags[i];
                            }

                            if (i != tags.Length)
                            {
                                text += " ";
                            }
                                
                        }
                    }

                    MVTModules.AddToCartButton.AddToCartButton.AddNewHypothesis(colour, text);
                }
            }
           
            string errormsg = "The command uses wrong syntax. Here are some valid examples:\n" +
                    "addhypo addtocartbtn #fff Basket it\n" + 
                    "addhypo addtocartbtn #aaa\n" + 
                    "addhypo addtocartbtn Place to basket";

            Out(errormsg, true, true);            

        }

        private void WriteToCSV(string lastCommand)
        {
            string[] parts = lastCommand.Split(' ');
            if(parts.Length == 2)
            {
                string analysis = MVTManager.Stats.TestHistoryAsCSV();
                bool result = FileManager.WriteToFile(analysis, parts[1], this);

                if(result)
                {
                    Out("File created.", true, true);
                }
            }
            else
            {
                Out("Sorry, the command is not using the right syntax. Try something like: \nWRITETOCSC C:\\logfolder\\testhistory.csv");
            }
        }

        private void DisplayStat()
        {
            MVTManager.Stats.PrintStats(this);
        }

        public void RefreshStatDashboard()
        {
            if(Configuration.IsDashboardOn)
            {
                Console.Clear();
                MVTManager.Stats.PrintDashboard(this);
            }
        }


        private void SendToServer(string lastCommand)
        {
            string theRequest = lastCommand.Remove(0, "sendtoserver:".Length);
            _Controller.SendFixedRequest(theRequest);
        }

        private void SetVariable(string lastCommand)
        {
            string value = "";
            if(lastCommand.Contains("="))
            {
                value = lastCommand.Remove(0, lastCommand.LastIndexOf("=")+1);
            }
            else
            {
                Console.WriteLine("Sorry. I don't understand. You may missing a '=' sign.");
                return;
            }

            if (lastCommand.ToUpper().Contains("listenerPortNumber".ToUpper()))
            {
                try
                {
                    Configuration.ListenerPortNumber = Int32.Parse(value);
                } catch (Exception e)
                {
                    Console.WriteLine("Sorry. I don't understand. Maybe something is wrong with the value");
                }
                return;
            }

            if (lastCommand.ToUpper().Contains("webServerPortNumber".ToUpper()))
            {
                try
                {
                    Configuration.WebServerPortNumber = Int32.Parse(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sorry. I don't understand. Maybe something is wrong with the value");
                }
                return;
            }


            if (lastCommand.ToUpper().Contains("readingBufferLength".ToUpper()))
            {
                try
                {
                    Configuration.ReadingBufferLength = Int32.Parse(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sorry. I don't understand. Maybe something is wrong with the value");
                }
                return;
            }


            if (lastCommand.ToUpper().Contains("localAddress".ToUpper()))
            {
                try
                {
                    Configuration.LocalAddress = IPAddress.Parse(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sorry. I don't understand. Maybe something is wrong with the value");
                }
                return;
            }

            if (lastCommand.ToUpper().Contains("webServerAddress".ToUpper()))
            {
                try
                {
                    Configuration.WebServerAddress = IPAddress.Parse(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sorry. I don't understand. Maybe something is wrong with the value");
                }
                return;
            }

            if (lastCommand.ToUpper().Contains("isDebugModeOn".ToUpper()))
            {
               if(value.ToUpper().Contains("TRUE") ||
                   value.ToUpper().Contains("YES") ||
                   value.ToUpper().Contains("1"))
                {
                    Configuration.IsDebugModeOn = true;
                }
               else
                {
                    Configuration.IsDebugModeOn = false;
                }

                return;
            }
            if (lastCommand.ToUpper().Contains("isMVTEnabled".ToUpper()))
            {
                if (value.ToUpper().Contains("TRUE") ||
                    value.ToUpper().Contains("YES") ||
                    value.ToUpper().Contains("1"))
                {
                    Configuration.IsMVTEnabled = true;
                }
                else
                {
                    Configuration.IsMVTEnabled = false;
                }

                return;
            }

            if (lastCommand.ToUpper().Contains("isDashboardOn".ToUpper()))
            {
                if (value.ToUpper().Contains("TRUE") ||
                    value.ToUpper().Contains("YES") ||
                    value.ToUpper().Contains("1"))
                {
                    Configuration.IsDashboardOn = true;
                }
                else
                {
                    Configuration.IsDashboardOn = false;
                }

                return;
            }

            if (lastCommand.ToUpper().Contains("isAnalysisRunWithNewGuid".ToUpper()))
            {
                if (value.ToUpper().Contains("TRUE") ||
                    value.ToUpper().Contains("YES") ||
                    value.ToUpper().Contains("1"))
                {
                    Configuration.IsAnalysisRunWithNewGuid = true;
                }
                else
                {
                    Configuration.IsAnalysisRunWithNewGuid = false;
                }

                return;
            }

            Console.WriteLine("Sorry. I don't understand. You may tried to set a variable which does not exist.");
        }


        public void Out(string message)
        {
            Console.WriteLine(message);
        }

        public void Out(string message, bool withNewLine)
        {
            if (withNewLine)
                Console.WriteLine(message);
            else
                Console.Write(message);
        }

        public void Out(string message, bool withNewLine, bool showWesofPromptAfterMsg)
        {
            if (withNewLine)
                Console.WriteLine(message);
            else
                Console.Write(message);

            if(showWesofPromptAfterMsg)
            {
                Console.Write("WeSOF>");
            }
        }


    }
}
