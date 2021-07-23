using System;
using System.Reflection;

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
        public bool StartManoeuvre()
        {
            _parentController.dataModel.Robot_Control.RobotEnabled = true;
            return _parentController.dataModel.Robot_Control.RobotEnabled;
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

        public void CallForProcessHandler(ICallForProcess sender, CallForProcessEventArgs args)
        {
            MethodInfo Process = this.GetType().GetMethod(args.ProcessName != null ? args.ProcessName : "");
            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Robot Called for Process: " + (args.ProcessName != null? args.ProcessName : "NOT PROVIDED")));

            if (Process != null)
            {
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Invoking process: " + Process.Name));
                object response = Process.Invoke(this,null);
                if (response != null)
                {
                    _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("process responded: " + response.ToString()));
                    // TODO: respond to sender of response
                    sender.CallForProcessResponse(true, Process, response);
                }
                else
                {
                    _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("process invoked with no response"));
                    //TODO: respond to sender of completion (success)
                    sender.CallForProcessResponse(true, Process, response);
                }
            }
            else
            {
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("No Such Process Found"));
                // TODO: respond to sender of failure
                sender.CallForProcessResponse(false);
            }
        }

        // --- INTERNAL EVENT TRIGGERS AND AUTOMATION ---


    }

}
