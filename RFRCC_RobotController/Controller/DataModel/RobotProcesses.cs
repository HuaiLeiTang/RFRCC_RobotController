using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class RobotProcesses
    {
        // --- INTERNAL ---


        // --- EVENTS ---


        // --- PARAMETERS ---


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

        public bool VerifyStockDimensions()
        {
            throw new NotImplementedException();
        }

        // --- INTERNAL EVENT TRIGGERS AND AUTOMATION ---


    }

}
