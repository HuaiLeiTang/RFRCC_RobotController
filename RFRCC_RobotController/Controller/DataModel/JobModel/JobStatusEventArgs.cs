using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Event arguments for JobStatusChange Event holding the relevant job to the event
    /// </summary>
    public class JobStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Relevant Job to the event
        /// </summary>
        public JobModel Job;

        /// <summary>
        /// construct object with the relevant job to the event
        /// </summary>
        /// <param name="job"></param>
        public JobStatusEventArgs(JobModel job)
        {
            Job = job;
        }
    }
}
