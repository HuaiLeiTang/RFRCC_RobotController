using System;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Action step involving Robot process
    /// </summary>
    public class OperationRobotProcess : OperationAction, ICallForProcess, IRobotOperation, IOperationAction
    {
        // --- INTERNAL PROPERTIES ---

        // --- EVENTS ---
        public event EventHandler RequiredStockDXUpdate;
        public event CallForProcessEventHandler CallForRobotProcess;
        



        // --- PUBLIC PROPERTIES ---
        public double RequiredStockDX
        {
            get
            {
                if (Attributes.ContainsKey("RequiredStockDX"))
                {
                    return int.Parse(Attributes.Where(stockDX => stockDX.Key == "RequiredStockDX").FirstOrDefault().Value);
                }
                else return 99999;
            }
        }
        // --- CONSTRUCTORS ---
        public OperationRobotProcess()
        {
            this.ActionStarted += OnRobotProcessStarted;
            this.ActionPaused += OnRobotProcessPaused;
            this.ActionCanceled += OnRobotProcessCanceled;
            this.InternalAbortEvent += OnActionAbort;
        }


        // --- METHODS ---
        public void CallForProcessResponse(bool success, object process = null, object response = null)
        {
            if (success && response != null)
            {
                switch (process.GetType().Name)
                {
                    case "StartManoeuvre":
                        this.Complete(bool.Parse(response.ToString()));
                        return;
                    default:
                        break;
                }
                
            }
            else
            {
                // TODO: some failure handling process
            }
        }

        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnRobotProcessStarted(object sender = null, EventArgs args = null)
        {
            if (CallForRobotProcess != null)
            {
                CallForRobotProcess?.Invoke(this, new CallForProcessEventArgs("StartManoeuvre"));
            }
            else throw new Exception("No listeners subscribed to Robot Process request");

        }
        protected virtual void OnRobotProcessPaused(object sender = null, EventArgs args = null)
        {

        }
        protected virtual void OnRobotProcessCanceled(object sender = null, EventArgs args = null)
        {

        }
        protected virtual void OnActionAbort(object sender = null, EventArgs args = null)
        {
            RequiredStockDXUpdate = null;
            CallForRobotProcess = null;
        }


    }

    public delegate void CallForProcessEventHandler(ICallForProcess sender, CallForProcessEventArgs args);
}
