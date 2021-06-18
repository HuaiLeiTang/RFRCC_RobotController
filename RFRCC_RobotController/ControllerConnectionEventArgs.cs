using System;

namespace RFRCC_RobotController
{
    public class ControllerConnectionEventArgs : EventArgs
    {
        public ControllerConnectionEventArgs()
        {

        }
        public ControllerConnectionEventArgs(ABB.Robotics.Controllers.Controller controller, ABB.Robotics.Controllers.RapidDomain.Task task)
        {
            Controller = controller;
            Task = task;
        }
        public ABB.Robotics.Controllers.Controller Controller { get; set; }
        public ABB.Robotics.Controllers.RapidDomain.Task Task { get; set; }
    }
}
