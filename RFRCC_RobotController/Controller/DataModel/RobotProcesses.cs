using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.Reflection;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class RobotProcesses
    {
        // --- INTERNAL ---
        internal RobotController _parentController;

        internal RapidData _OperationsToSkip;
        internal string _OperationsToSkipModule = "PC_Manoeuvre_Register";
        internal string _OperationsToSkipVAR = "OperationsToSkip";

        internal RapidData _Request_IMStop;
        internal string _Request_IMStopModule = "SAFETY";
        internal string _Request_IMStopVAR = "TRAP_PC_IMStop";
        internal bool _RobotPaused = false;

        internal RapidData _Request_Abort;
        internal string _Request_AbortModule = "SAFETY";
        internal string _Request_AbortVAR = "TRAP_PC_Abort";

        // --- EVENTS ---


        // --- PARAMETERS ---


        // --- CONSTRUCTORS ---
        public RobotProcesses(RobotController ParentController)
        {
            _parentController = ParentController;
            _parentController.ControllerConnectionChange += OnControllerConnectionChange;
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
            // TODO: Make recovery Process
            // TODO: make robot status update
            // Immediate stop
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_parentController.controller))
                    {
                        _Request_IMStop.StringValue = "TRUE";
                        m.Release();
                    }
                }
                finally
                {
                    complete = true;
                    _RobotPaused = true;
                }
            }
            return complete;
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
        public bool AbortJob(AbortCode Code = 0)
        {

            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs(string.Format("Robot Job has been aborted with code {0}", Code.ToString())));

            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_parentController.controller))
                    {
                        _Request_Abort.StringValue = "TRUE";
                        m.Release();
                    }
                }
                finally
                {
                    complete = true;
                }
            }
            return complete;
        }
        public bool ContinueManoeuvre()
        {
            if (_RobotPaused)
            {
                bool complete = false;
                // Remove Immediate stop
                while (!complete)
                {
                    try
                    {
                        using (Mastership m = Mastership.Request(_parentController.controller))
                        {
                            _Request_IMStop.StringValue = "FALSE";
                            m.Release();
                        }
                    }
                    finally
                    {
                        complete = true;
                        _RobotPaused = false;
                    }
                }
                
            }
            while (!_parentController.dataModel.Robot_Control.RobotEnabled)
            {
                _parentController.dataModel.Robot_Control.RobotEnabled = true;
            }

            return _parentController.dataModel.Robot_Control.RobotEnabled;
        }
        public bool StartManoeuvre()
        {
            while (!_parentController.dataModel.Robot_Control.RobotEnabled)
            {
                _parentController.dataModel.Robot_Control.RobotEnabled = true;
            }
            
            return _parentController.dataModel.Robot_Control.RobotEnabled;
        }
        /// <summary>
        /// This will imediately pause any robot motion unrecoverably
        /// </summary>
        /// <returns></returns>
        public bool PauseProcess()
        {
            bool complete = false;
            if (_parentController.dataModel.ProcessSettings.RobotIMStopOnPause)
            {
                complete = ImmediateStop();
            }
            else
            {
                // TODO: method to pause at next point
                complete = ImmediateStop();
            }
            return complete;
        }
        public void SkipManoeuvre(int FeatureNum)
        {
            bool complete = false;
            string Current = _OperationsToSkip.StringValue.Trim('"');
            Current += FeatureNum.ToString() + ",";
            Current = "\"" + Current + "\"";

            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_parentController.controller))
                    {
                        _OperationsToSkip.StringValue = Current;
                        m.Release();
                    }
                }
                finally
                {
                    complete = true;
                }
            }
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
                object response;
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Invoking process: " + Process.Name));
                if (args.ProcessParameters != null)
                {
                    response = Process.Invoke(this, new object[] { args.ProcessParameters });
                }
                else
                {
                    response = Process.Invoke(this, null);
                }
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
        protected virtual void OnControllerConnectionChange(object sender = null, EventArgs args = null)
        {
            if (_parentController.ControllerConnected)
            {
                _OperationsToSkip = _parentController.tRob1.GetRapidData(_OperationsToSkipModule, _OperationsToSkipVAR);
                _Request_IMStop = _parentController.tRob1.GetRapidData(_Request_IMStopModule, _Request_IMStopVAR);
                _Request_Abort = _parentController.tRob1.GetRapidData(_Request_AbortModule, _Request_AbortVAR);
            }
            else
            {
                _OperationsToSkip.Dispose();
                _Request_IMStop.Dispose();
                _Request_Abort.Dispose();
            }
        }


    }

    public enum AbortCode
    {
        UserAbort = 0,
        UnrecoverdError = 8,
        FatalError = 9

    }

}
