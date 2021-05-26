using System;
using System.Collections.Generic;
using System.Text;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;
using RapidBool = ABB.Robotics.Controllers.RapidDomain.Bool;
using RFRCC_RobotController.Controller.DataModel.RAPID_Data;

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
            _parentController.dataModel.InitDataStream();
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
            _parentController.dataModel.InitDataStream();
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
