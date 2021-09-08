using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Event arguments for event passing process execution request
    /// </summary>
    public class CallForProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Name of process requested
        /// </summary>
        public string ProcessName;
        /// <summary>
        /// Start or stop of process requested
        /// </summary>
        public bool Start;
        public object ProcessParameters;

        public CallForProcessEventArgs(string processName, bool start = true, object processParams = null)
        {
            ProcessName = processName;
            Start = start;
            if (processParams != null) ProcessParameters = processParams;
        }
    }
}
