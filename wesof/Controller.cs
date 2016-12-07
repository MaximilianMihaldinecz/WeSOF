using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTManager;
using wesof.MVTModules.AddToCartButton;

namespace wesof
{
    class Controller
    {
        private CommandLineInterface _CLI;
        private ConnectionManager _CM = null;
        private TestManager _TM;

        public Controller()
        {
            _CLI = new CommandLineInterface();
            _CLI.SetCurrentController(this);
            _TM = new TestManager(_CLI);            
        }

        public void StartApplication()
        {
            Configuration.MainController = this;
            AddToCartButton.Initialise();
            MVTHistory.SetCLI(ref _CLI);

            _CLI.ProgramStartWelcomeMessage();
            _CLI.SetCurrentController(this);
            _CLI.WaitingForCommand();
        }

        public void LaunchConnectionManager()
        {
            if(_CM == null)
            {
                _CM = new ConnectionManager(_CLI, this);
                _CM.Start();
            }
        }

        public void SendFixedRequest(string req)
        {
            LaunchConnectionManager();
            _CM.CustomSendAndListenServer(req);
        }

        public TestManager GetTestManager()
        {
            return _TM; 
        }

        public CommandLineInterface GetCLI()
        {
            return _CLI;
        }


    }
}
