using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Event arguments for event passing process execution request
    /// </summary>
    public class CallForRobotProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Name of process requested
        /// </summary>
        public string ProcessName;
        /// <summary>
        /// Start or stop of process requested
        /// </summary>
        public bool Start;

        public CallForRobotProcessEventArgs(string processName, bool start)
        {
            ProcessName = processName;
            Start = start;
        }
    }
}
