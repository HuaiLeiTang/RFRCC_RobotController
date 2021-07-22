using System;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Action step involving Robot process
    /// </summary>
    public class OperationRobotProcess : OperationAction
    {
        // --- INTERNAL PROPERTIES ---

        // --- EVENTS ---
        public event EventHandler RequiredStockDXUpdate;
        // TODO: connect RequiredStockDXUpdate to something?
        public event EventHandler<CallForRobotProcessEventArgs> CallForRobotProcess;
        // TODO: connect CallForProcess to RobotProcess

        

        // --- PUBLIC PROPERTIES ---
        public double RequiredStockDX
        {
            get 
            {
                if (Attributes.ContainsKey("RequiredStockDX"))
                {
                    return int.Parse(Attributes.Where(stockDX => stockDX.Key == "RequiredStockDX").FirstOrDefault().Value);
                }
                else return 9999;
            }
        }

        // --- CONSTRUCTORS ---
        public OperationRobotProcess()
        {
            this.ActionStarted += OnRobotProcessStarted;
            this.ActionPaused += OnRobotProcessPaused;
            this.ActionCanceled += OnRobotProcessCanceled;
        }


        // --- METHODS ---


        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnRobotProcessStarted(object sender = null, EventArgs args = null)
        {
            if (CallForRobotProcess != null)
            {
                CallForRobotProcess?.Invoke(this, new CallForRobotProcessEventArgs(Attributes.Where(stockDX => stockDX.Key == "RobotProcess").FirstOrDefault().Value, true));
            }
            else throw new Exception("No listeners subscribed to Robot Process request");
             
        }
        protected virtual void OnRobotProcessPaused(object sender = null, EventArgs args = null)
        {

        }
        protected virtual void OnRobotProcessCanceled(object sender = null, EventArgs args = null)
        {

        }
    }
}
