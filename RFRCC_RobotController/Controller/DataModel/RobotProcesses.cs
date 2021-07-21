using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class RobotProcesses
    {
        // --- INTERNAL ---
        internal RobotController _parentController;

        // --- EVENTS ---


        // --- PARAMETERS ---


        // --- CONSTRUCTORS ---
        public RobotProcesses(RobotController ParentController)
        {
            _parentController = ParentController;
        }

        // --- METHODS ---

        // just make methods for now
        /// <summary>
        /// Stops robot in current process, destroying process.
        /// NOT A SUBSTITUTE FOR EMERGENCY STOP. EMERGENCY STOP MUST BE ENNACTED PHYSICALLY
        /// </summary>
        /// <returns>success</returns>
        public bool ImmediateStop()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Stops robot in current process, destroying process.
        /// NOT A SUBSTITUTE FOR EMERGENCY STOP. EMERGENCY STOP MUST BE ENNACTED PHYSICALLY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void ImmediateStop(object sender = null, EventArgs args = null)
        {
            ImmediateStop();
        }
        public bool RecoverFromIMStop()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Loads Job Data onto robot and begins processing
        /// </summary>
        /// <returns>success</returns>
        public bool StartJob()
        {
            throw new NotImplementedException();
        }

        public bool PauseProcess()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Verify Stock Dimensions and returns measurements fit stock size
        /// </summary>
        /// <returns>If Robot takes stock for dimensions required</returns>
        public bool VerifyStockDimensions()
        {
            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Robot performing dummy Verify Stock: Process to be implemented"));
            return true;
        }

        // --- INTERNAL EVENT TRIGGERS AND AUTOMATION ---


    }

}
