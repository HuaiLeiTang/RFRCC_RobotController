using System;
using System.Collections.Generic;
using System.Text;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;
using RapidBool = ABB.Robotics.Controllers.RapidDomain.Bool;
using RFRCC_RobotController.RAPID_Data;

namespace RFRCC_RobotController.Controller
{
    /* This class is to hanble network access, connection, and related events for the controller
     * 
     */
    public class Stream
    {
        private RobotController _parentController;
        // Housekeeping and networking  
        private NetworkScanner scanner = null;
        private NetworkWatcher networkwatcher = null;
        private ControllerInfoCollection _AvailableControllers = null;
        private bool FetchedData;

        public Stream(RobotController parentController)
        {
            _parentController = parentController;
            scanner = new NetworkScanner();
            scanner.Scan();
            _AvailableControllers = scanner.Controllers;

            //Network Watcher Setup // TODO: verify this is functioning properly
            // NetworkWatcher setup
            networkwatcher = new NetworkWatcher(scanner.Controllers);
            networkwatcher.Found += new EventHandler<NetworkWatcherEventArgs>(HandleNWCChangeEvent);
            networkwatcher.Lost += new EventHandler<NetworkWatcherEventArgs>(HandleNWCChangeEvent);
            networkwatcher.EnableRaisingEvents = true;
        }

        public event EventHandler<AvailableControllersEventArgs> OnAvailableControllersChange;
        public class AvailableControllersEventArgs : EventArgs
        {
            public AvailableControllersEventArgs(ControllerInfoCollection AvalableList)
            {
                AvailableControllers = AvalableList;
            }

            public ControllerInfoCollection AvailableControllers { get; set; }
        }
        protected virtual void AvailableControllersChange(object sender, AvailableControllersEventArgs e)
        {
            EventHandler<AvailableControllersEventArgs> handler = OnAvailableControllersChange;
            if (handler != null)
                handler(sender, e);
        }
        public ControllerInfoCollection AvailableControllers
        {
            get
            {
                return _AvailableControllers;
            }
        }
        void HandleNWCChangeEvent(object sender, NetworkWatcherEventArgs e)
        {
            scanner.Scan();
            _AvailableControllers = scanner.Controllers;
            OnAvailableControllersChange(this, new AvailableControllersEventArgs(_AvailableControllers));
        }
        public void ConnectToController(ABB.Robotics.Controllers.Controller controller)
        {
            if (_parentController._ControllerConnected)
                Dispose();
            _parentController.controller = controller;
            _parentController.controller.Logon(UserInfo.DefaultUser);
            _parentController.tRob1 = controller.Rapid.GetTask("T_ROB1");
            InitDataStream();
            _parentController._ControllerConnected = true;
            ControllerConnectedChange(this, new ControllerConnectedEventArgs(_parentController._ControllerConnected));
            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Connected to controller"));
        }
        public void ConnectToController(ControllerInfo controllerInfo)
        {
            if (_parentController._ControllerConnected)
                Dispose();
            _parentController.controller = ABB.Robotics.Controllers.Controller.Connect(controllerInfo, ConnectionType.Standalone);
            _parentController.controller.Logon(UserInfo.DefaultUser);
            _parentController.tRob1 = _parentController.controller.Rapid.GetTask("T_ROB1");
            InitDataStream();
            _parentController._ControllerConnected = true;
            ControllerConnectedChange(this, new ControllerConnectedEventArgs(_parentController._ControllerConnected));
            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Connected to controller"));
        }
        public event EventHandler<ControllerConnectedEventArgs> OnControllerConnectedChange;
        public class ControllerConnectedEventArgs : EventArgs
        {
            public ControllerConnectedEventArgs(bool Connected)
            {
                ControllerConnected = Connected;
            }

            public bool ControllerConnected { get; set; }
        }
        protected virtual void ControllerConnectedChange(object sender, ControllerConnectedEventArgs e)
        {
            EventHandler<ControllerConnectedEventArgs> handler = OnControllerConnectedChange;
            if (handler != null)
                handler(sender, e);
        }

        // TODO: move this to Datamodel of Robot Controller 
        public void InitDataStream()
        {
            bool complete;
            if (_parentController.tRob1 != null)
            {
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Connecting to controller"));
                _parentController.SQLMessageRecieve = _parentController.tRob1.GetRapidData("SQL_Comm", "SQLMessageRecieve");
                _parentController.RapidJobData = _parentController.tRob1.GetRapidData("Module1", "Sys_JobData");
                _parentController.RapidFeatureData = _parentController.tRob1.GetRapidData("Module1", "Sys_FeatureData");
                _parentController.PCSDK_Complete = _parentController.tRob1.GetRapidData("SQL_Comm", "PCSDK_Complete");
                _parentController.SQLMessageError = _parentController.tRob1.GetRapidData("SQL_Comm", "SQLMessageError");
                _parentController.PCConnected = _parentController.tRob1.GetRapidData("SQL_Comm", "PCConnected");
                _parentController.Robot_Status = _parentController.tRob1.GetRapidData("Module1", "Rob_Status");

                _parentController.TopCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Top_CutChart");
                _parentController.BottomCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Bottom_CutChart");
                _parentController.FrontCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Front_CutChart");
                _parentController.BackCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Back_CutChart");

                _parentController.Robot_Control.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Rob_Control");
                _parentController.OperationManeouvres = new RAPID_OM_List(99, _parentController.controller, _parentController.tRob1, "Module1", "OperationManoeuvres");
                _parentController.OperationHeaders = new RAPID_OH_List(20, _parentController.controller, _parentController.tRob1, "Module1", "OperationHeaders");
                _parentController.OperationBuffer = new RAPID_OperationBuffer(_parentController.controller, _parentController.tRob1, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
                _parentController.OperationBuffer.DescendingOrder = true;
                _parentController.jobHeader = new RAPIDJob_Header(_parentController.controller, _parentController.tRob1, "Module1", "Sys_JobData");

                _parentController.Robot_Control.ValueUpdate += _parentController.OnControlValueUpdate; // Maybe update to enable Interrupts
                _parentController.Robot_Control.PC_MessageUpdate += _parentController.RobotPC_MessageChanged;

                _parentController.NextDX = _parentController.tRob1.GetRapidData("Module1", "NextDX");
                _parentController.NextDX.ValueChanged += _parentController.NextDXChange;

                complete = false;
                while (!complete)
                {
                    try
                    {
                        using (Mastership m = Mastership.Request(_parentController.controller.Rapid))
                        {
                            _parentController.PCConnected.Value = RapidBool.Parse("TRUE");
                        }
                    }
                    catch
                    {
                        complete = false;
                    }
                    finally
                    {
                        complete = true;
                    }
                }

            }
            else
            {
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("'targets' data does not exist!"));
            }


        }

        // TODO: change this to a close data stream of robot controller? 
        public void Dispose()
        {
            // TODO Stop current task on disconnect?
            _parentController.controller.Logoff();
            _parentController.controller.Dispose();
            _parentController.controller = null;
            _parentController._ControllerConnected = false;
            ControllerConnectedChange(this, new ControllerConnectedEventArgs(_parentController._ControllerConnected));
            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Disconnected from controller"));
        }

    }
}
