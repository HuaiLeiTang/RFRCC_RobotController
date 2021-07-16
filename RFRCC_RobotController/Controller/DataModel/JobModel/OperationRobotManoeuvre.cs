using RFRCC_RobotController.Controller.DataModel.OperationData;
using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Action step involving Robot cutting process
    /// </summary>
    public class OperationRobotManoeuvre : OperationAction
    {

        // --- INTERNAL PROPERTIES ---
        RobotComputedFeatures _featureData;
        private int _RequiredStockDX;

        // --- EVENTS ---
        public event EventHandler RequiredStockDXUpdate;

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
        public int RequiredStockDX { get;}


        // --- CONSTRUCTORS ---
        public OperationRobotManoeuvre()
        {
            this.ActionStarted += OnRobotActionStarted;
            this.ActionPaused += OnRobotActionPaused;
            this.ActionCanceled += OnRobotActionCanceled;
            
        }

        // --- METHODS ---



        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnRobotActionStarted(object sender = null, EventArgs args = null)
        {
            if (!PauseOn)
            {
                featureData.StartWhenReady = true;
            }
        }
        protected virtual void OnRobotActionPaused(object sender = null, EventArgs args = null)
        {

        }
        protected virtual void OnRobotActionCanceled(object sender = null, EventArgs args = null)
        {

        }



    }
}
