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

        // --- EVENTS ---
        public event EventHandler RequiredStockDXUpdate;
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


        // --- CONSTRUCTORS ---
        public OperationRobotManoeuvre(RobotComputedFeatures featureData)
        {
            _featureData = featureData;
            this.ActionStarted += OnRobotActionStarted;
            this.ActionPaused += OnRobotActionPaused;
            this.ActionCanceled += OnRobotActionCanceled;
            _featureData.FeatureCompletedChange += _featureData_FeatureCompletedChange;
        }

        // --- METHODS ---

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
                default:
                    break;
            }
            
        }


        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnRobotActionStarted(object sender = null, EventArgs args = null)
        {
            featureData.StartWhenReady = true;
            CallForRobotProcess(this, new CallForProcessEventArgs("StartManoeuvre"));
        }
        protected virtual void OnRobotActionPaused(object sender = null, EventArgs args = null)
        {
            
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

    }
}
