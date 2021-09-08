using RFRCC_RobotController.Controller.DataModel.OperationData;
using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Action step involving Robot cutting process
    /// </summary>
    public class OperationRobotManoeuvre : OperationAction, ICallForProcess, IRobotOperation, IOperationAction
    {

        // --- INTERNAL PROPERTIES ---
        RobotComputedFeatures _featureData;
        bool _SkippedByRobot = false;
        bool _RequestedRobotSkip = false;

        // --- EVENTS ---
        public event OperationRobotManoeuvreEventHandler RequiredStockDXUpdate;
        public event CallForProcessEventHandler CallForRobotProcess;

        // TODO: connect RequiredStockDXUpdate to something?

        // --- PUBLIC PROPERTIES ---
        /// <summary>
        /// Data of feature to be cut during process
        /// </summary>
        public RobotComputedFeatures featureData 
        { 
            get
            {
                return _featureData;
            } 
            set
            {
                _featureData = value;
            }
        }
        public double RequiredStockDX 
        {
            get { return featureData.FeatureHeader.IdealXDisplacement; }
        }

        public bool SkippedByRobot
        {
            get
            {
                return _SkippedByRobot;
            }
        }
        public bool RequestedRobotSkip
        {
            get
            {
                return _RequestedRobotSkip;
            }
        }

        // --- CONSTRUCTORS ---
        public OperationRobotManoeuvre(RobotComputedFeatures featureData)
        {
            _featureData = featureData;
            this.ActionStarted += OnRobotActionStarted;
            this.ActionContinued += OnRobotActionContinue;
            this.ActionPaused += OnRobotActionPaused;
            this.ActionCanceled += OnRobotActionCanceled;
            this.InternalAbortEvent += OnActionAbort;
            _featureData.FeatureIdealXDisplacementChange += _featureData_FeatureIdealXDisplacementChange; ;
            _featureData.FeatureCompletedChange += _featureData_FeatureCompletedChange;
        }

        // --- METHODS ---
        public void ActionSkippedByRobot()
        {
            _SkippedByRobot = true;
        }
        public void CallForProcessResponse(bool success, object process = null, object response = null)
        {
            switch (process.GetType().Name)
            {
                case "EnableRobotCut":
                    if (success && response != null)
                    {
                        // TODO: indicate processing
                    }
                    else
                    {
                        // TODO: some failure handling process
                    }
                    break;
                case "RobotCompleteCut":
                    if (success && response != null)
                    {
                        this.Complete(bool.Parse(response.ToString()));
                    }
                    else
                    {
                        // TODO: some failure handling process
                    }
                    break;
                case "SkipManoeuvre":
                    _RequestedRobotSkip = true;
                    return;
                default:
                    break;
            }
        }

        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnActionAbort(object sender = null, EventArgs args = null)
        {
            RequiredStockDXUpdate = null;
            CallForRobotProcess = null;
        }
        protected virtual void OnRobotActionStarted(object sender = null, EventArgs args = null)
        {
            if (!(Paused || PauseOn))
            {
                featureData.StartWhenReady = true;
                CallForRobotProcess?.Invoke(this, new CallForProcessEventArgs("StartManoeuvre"));
            }
        }
        protected virtual void OnRobotActionContinue(object sender = null, EventArgs args = null)
        {
            featureData.StartWhenReady = true;
            CallForRobotProcess?.Invoke(this, new CallForProcessEventArgs("ContinueManoeuvre"));
        }
        protected virtual void OnRobotActionPaused(object sender = null, EventArgs args = null)
        {
            CallForRobotProcess?.Invoke(this, new CallForProcessEventArgs("PauseProcess"));
        }
        protected virtual void OnRobotActionCanceled(object sender = null, EventArgs args = null)
        {

        }
        private void _featureData_FeatureCompletedChange(RobotComputedFeatures sender, EventArgs args)
        {
            if (_featureData.CompletedByRobot)
            {
                this.Complete(true);
            }
        }
        private void _featureData_FeatureIdealXDisplacementChange(RobotComputedFeatures sender, EventArgs args)
        {
            this.Attributes["RequiredStockDX"] = RequiredStockDX.ToString();
            RequiredStockDXUpdate?.Invoke(this, new EventArgs());
        }
        
    }

    public delegate void OperationRobotManoeuvreEventHandler(OperationRobotManoeuvre sender, EventArgs args);
}
