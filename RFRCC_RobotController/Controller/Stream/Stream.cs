﻿using System;
using System.Collections.Generic;
using System.Text;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;
using RapidBool = ABB.Robotics.Controllers.RapidDomain.Bool;
using RFRCC_RobotController.Controller.DataModel.RAPID_Data;
using RFRCC_RobotController.ABB_Data;

namespace RFRCC_RobotController.Controller
{
    /// <summary>
    /// handles network access, connection, and related events for the ABB Controllers, specifically intended for use with the 
    /// </summary>
    public class Stream
    {
        // --- INTERNAL ---
        private RobotController _parentController;
        private NetworkScanner scanner = null;
        private NetworkWatcher networkwatcher = null;
        private ControllerCollection _AvailableControllers = null;

        // --- EVENTS ---
        /// <summary>
        /// Update if any controllers are discovered or lost on the network
        /// </summary>
        public event EventHandler<AvailableControllersEventArgs> OnAvailableControllersChange;
        /// <summary>
        /// controller connection connected or disconnected
        /// </summary>
        public event EventHandler ControllerConnectedChange;

        // --- PARAMETERS ---
        /// <summary>
        /// Provide list of Controllers available on the network
        /// </summary>
        public ControllerCollection AvailableControllers => _AvailableControllers;

        // --- CONSTRUCTORS ---
        /// <summary>
        /// Initialise stream functionality with a parent controller class
        /// </summary>
        /// <param name="parentController">Controller class to house any connection made and access events</param>
        public Stream(RobotController parentController)
        {
            _parentController = parentController;
            scanner = new NetworkScanner();
            scanner.Scan();
            _AvailableControllers = new ControllerCollection(scanner.Controllers);

            //Network Watcher Setup // TODO: verify this is functioning properly
            // NetworkWatcher setup
            networkwatcher = new NetworkWatcher(scanner.Controllers);
            networkwatcher.Found += new EventHandler<NetworkWatcherEventArgs>(HandleNWCChangeEvent);
            networkwatcher.Lost += new EventHandler<NetworkWatcherEventArgs>(HandleNWCChangeEvent);
            networkwatcher.EnableRaisingEvents = true;
        }

        // --- METHODS ---
        /// <summary>
        /// Connect controller object to desired controller using ABB classes
        /// </summary>
        /// <param name="controller">Controller to be connected to</param>
        public void ConnectToABBController(ABB.Robotics.Controllers.Controller controller)
        {
            if (_parentController._ControllerConnected)
                Dispose();
            _parentController.controller = controller;
            _parentController._controllerInfo = new NetworkControllerInfo(controller);
            _parentController.controller.Logon(UserInfo.DefaultUser);
            _parentController.tRob1 = controller.Rapid.GetTask("T_ROB1");
            _parentController.dataModel.InitDataStream();
            _parentController.ConnectedToController();
            _parentController.dataModel.PCConnected.Subscribe(OnControllerConnectedChange, EventPriority.High);
        }
        /// <summary>
        /// Connect controller object to desired controller using ABB classes
        /// </summary>
        /// <param name="controllerInfo">Controller Info collected by ABB net scanner</param>
        public void ConnectToABBController(ControllerInfo controllerInfo)
        {
            ConnectToABBController(ABB.Robotics.Controllers.Controller.Connect(controllerInfo, ConnectionType.Standalone));

            _parentController._controllerInfo = new NetworkControllerInfo(controllerInfo);
        }
        /// <summary>
        /// Connect controller object to desired controller
        /// </summary>
        /// <param name="controllerInfo">Controller info collected by stream.AvailableControllers</param>
        public void ConnectToController(NetworkControllerInfo controllerInfo)
        {
            ConnectToABBController(ABB.Robotics.Controllers.Controller.Connect(controllerInfo._ABBControllerInfo, ConnectionType.Standalone));
            _parentController._controllerInfo = controllerInfo;
        }
        // TODO: change this to a close data stream of robot controller? 
        /// <summary>
        /// Logs off network controller and disposes of all connections and memory holds regarding this object 
        /// </summary>
        public void Dispose()
        {
            // TODO Stop current task on disconnect?
            _parentController.controller.Logoff();
            _parentController.controller.Dispose();
            _parentController.controller = null;
            _parentController.DisconnectedFromController();
        }

        // --- INTERNAL EVENTS AND AUTOMATION ---
        /// <summary>
        /// Event executing method for raising event when Controllers connection status changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnControllerConnectedChange(object sender, EventArgs args)
        {
            ControllerConnectedChange?.Invoke(sender, args);
        }
        protected virtual void OnControllerConnectedChange(object sender, ABB.Robotics.Controllers.RapidDomain.DataValueChangedEventArgs args)
        {
            bool complete = false;
            if (_parentController.ControllerConnected)
            {
                while (!complete)
                {
                    try
                    {
                        using (Mastership m = Mastership.Request(_parentController.controller))
                        {
                            _parentController.dataModel.PCConnected.Value = RapidBool.Parse("TRUE");
                            m.Release();
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
        }
        /// <summary>
        /// Updates list of Network Controllers Available systematically, and raises OnAvailableControllersChange for any listeners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleNWCChangeEvent(object sender, NetworkWatcherEventArgs e)
        {
            scanner.Scan();
            ControllerInfoCollection AvailableControllers = scanner.Controllers;
            List<NetworkControllerInfo> removeList = new List<NetworkControllerInfo>();

            foreach (NetworkControllerInfo existing in _AvailableControllers)
            {
                if (!AvailableControllers.Contains(existing._ABBControllerInfo))
                {
                    removeList.Add(existing);
                }
            }

            foreach (NetworkControllerInfo remove in removeList)
            {
                _AvailableControllers.Remove(remove);
            }

            foreach (ControllerInfo newController in AvailableControllers)
            {
                NetworkControllerInfo check = new NetworkControllerInfo(newController);
                if (!_AvailableControllers.Contains(check))
                {
                    _AvailableControllers.Add(check);
                }
            }

            OnAvailableControllersChange(this, new AvailableControllersEventArgs(_AvailableControllers));
        }
        /// <summary>
        /// Event raising method used to trigger when any network controllers change status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AvailableControllersChange(object sender, AvailableControllersEventArgs e)
        {
            EventHandler<AvailableControllersEventArgs> handler = OnAvailableControllersChange;
            if (handler != null)
                handler(sender, e);
        }

    }

    /// <summary>
    /// Contains updated list of controllers available on the network
    /// </summary>
    public class AvailableControllersEventArgs : EventArgs
    {
        /// <summary>
        /// generate new AvailableControllersEventArgs with list of controlers
        /// </summary>
        /// <param name="AvalableList">Collection of controllers available on the network</param>
        public AvailableControllersEventArgs(ControllerCollection AvalableList)
        {
            AvailableControllers = AvalableList;
        }
        /// <summary>
        /// Collection of controllers available on the network
        /// </summary>
        public ControllerCollection AvailableControllers { get; set; }
    }
}
