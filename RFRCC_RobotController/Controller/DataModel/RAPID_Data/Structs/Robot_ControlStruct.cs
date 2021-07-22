using ABB.Robotics.Controllers;
using System;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using System.Diagnostics;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    /// <summary>
    /// Custom event arguments for OnValueUpdate of Robot_ControlStruct
    /// </summary>
    public class ControlStrucEventArgs : EventArgs
    {
        /// <summary>
        /// Initialise with Element Name
        /// </summary>
        /// <param name="valueName">Element Name</param>
        public ControlStrucEventArgs(string valueName)
        {
            ValueName = valueName;
        }
        /// <summary>
        /// Element Name
        /// </summary>
        public string ValueName { get; set; }
    }
    /// <summary>
    /// RAPID integrated Control Structure for Robot communication, control and status during processing
    /// </summary>
    public class Robot_ControlStruct
    {
        // --- INTERNAL PROPERTIES ---
        private bool _JobInProgress;
        private string _JobID;
        private bool _TorchEnable;
        private bool _ManualControl_Req;
        private double _StockXDisplacement;
        private bool _RobotEnabled;
        private bool _Park_Req;
        private string _PC_Message;
        private ABB.Robotics.Controllers.Controller ControllerConnection;

        // -- EVENTS ---
        /// <summary>
        /// Event raised when an object value is updated
        /// </summary>
        public event EventHandler<ControlStrucEventArgs> ValueUpdate;
        /// <summary>
        /// Event raised when RAPID Data structure value is updated
        /// </summary>
        public event EventHandler<ControlStrucEventArgs> PC_MessageUpdate;

        // --- PROPERTIES ---
        /// <summary>
        /// RAPIDData on connect Network Controller
        /// </summary>
        public RapidData Robot_Control_RAPID { get; set; }
        /// <summary>
        /// Current Job In progress
        /// </summary>
        public bool JobInProgress
        {
            get { return _JobInProgress; }
            set
            {
                if (_JobInProgress != value)
                {
                    _JobInProgress = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("JobInProgress"));
                }
            }
        }
        /// <summary>
        /// Current Job processing ID
        /// </summary>
        public string JobID
        {
            get { return _JobID; }
            set
            {
                if (_JobID != value)
                {
                    _JobID = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("JobID"));
                }
            }
        }
        /// <summary>
        /// Plasma Torch Enabled
        /// </summary>
        public bool TorchEnable
        {
            get { return _TorchEnable; }
            set
            {
                if (_TorchEnable != value)
                {
                    _TorchEnable = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("TorchEnable"));
                }
            }
        }
        /// <summary>
        /// Manual Control from operator Requested
        /// </summary>
        public bool ManualControl_Req
        {
            get { return _ManualControl_Req; }
            set
            {
                if (_ManualControl_Req != value)
                {
                    _ManualControl_Req = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("ManualControl"));
                }
            }
        }
        /// <summary>
        /// Current Stock X Displacement
        /// to be populated from PLC
        /// </summary>
        public double StockXDisplacement
        {
            get { return _StockXDisplacement; }
            set
            {
                if (_StockXDisplacement != value)
                {
                    _StockXDisplacement = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("StockXDisplacement"));
                }
            }
        }
        /// <summary>
        /// Robot Movement Enabled
        /// </summary>
        public bool RobotEnabled
        {
            get { return _RobotEnabled; }
            set
            {
                if (_RobotEnabled != value)
                {
                    _RobotEnabled = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("RobotEnabled"));
                }
            }
        }
        /// <summary>
        /// Park Position requested
        /// </summary>
        public bool Park_Req
        {
            get { return _Park_Req; }
            set
            {
                if (_Park_Req != value)
                {
                    _Park_Req = value;
                    Update_Rapid();
                    OnValueUpdate(new ControlStrucEventArgs("Park_Req"));
                }
            }
        }
        /// <summary>
        /// Message to Network Controller from PC
        /// </summary>
        public string PC_Message
        {
            get { return _PC_Message; }
            set
            {
                if (_PC_Message != value)
                {
                    _PC_Message = value;
                    Update_Rapid();
                    OnPC_MessageUpdate(new ControlStrucEventArgs(value));
                }
            }
        }

        // --- CONSTRUCTORS ---

        // --- METHODS ---
        /// <summary>
        /// Initialise object with connection to controller
        /// </summary>
        /// <param name="controller">Network Controller</param>
        /// <param name="RobotTask">Controller Task</param>
        /// <param name="Module">Module housing Control structure</param>
        /// <param name="RAPID_Name">name of Control structure</param>
        public void ConnectToRAPID(ABB.Robotics.Controllers.Controller controller, Task RobotTask, string Module, string RAPID_Name)
        {
            ControllerConnection = controller;
            Robot_Control_RAPID = RobotTask.GetRapidData(Module, RAPID_Name);
            ToString();
            InitialUpdateStruct();
            Robot_Control_RAPID.Subscribe(Update_Struct, EventPriority.High);
        }
        /// <summary>
        /// Object Information displayed in string
        /// </summary>
        public string DisplayInfo
        {
            get { return $"{JobInProgress}, { JobID }, { TorchEnable }, { ManualControl_Req }, { StockXDisplacement }, { RobotEnabled }, {Park_Req}, { PC_Message } "; }

        }
        /// <summary>
        /// Download Data from Network Controller
        /// </summary>
        public void GetFromRapidData()
        {
            DataNode[] RapidStruct = Robot_Control_RAPID.Value.ToStructure().Children.ToArray();

            if (JobInProgress != bool.Parse(RapidStruct[0].Value)) JobInProgress = bool.Parse(RapidStruct[0].Value);
            if (JobID != "\"" + RapidStruct[1].Value + "\"") JobID = RapidStruct[1].Value[1..^1];
            if (TorchEnable != bool.Parse(RapidStruct[2].Value)) TorchEnable = bool.Parse(RapidStruct[2].Value);
            if (ManualControl_Req != bool.Parse(RapidStruct[3].Value)) ManualControl_Req = bool.Parse(RapidStruct[3].Value);
            if (StockXDisplacement != double.Parse(RapidStruct[4].Value)) StockXDisplacement = double.Parse(RapidStruct[4].Value);
            if (RobotEnabled != bool.Parse(RapidStruct[5].Value)) RobotEnabled = bool.Parse(RapidStruct[5].Value);
            if (Park_Req != bool.Parse(RapidStruct[6].Value)) Park_Req = bool.Parse(RapidStruct[6].Value);
            if (PC_Message != "\"" + RapidStruct[7].Value + "\"") PC_Message = RapidStruct[7].Value[1..^1];
        }
        /// <summary>
        /// Output Structure in string representation 
        /// format: "[JobInProgress, JobID, TorchEnabled, ManualControl_Req, StockXDisplacement, RobotEnabled, Park_Req, PC_Message]"
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            string output = "[" +
                _JobInProgress.ToString() + ",\"" +
                _JobID + "\"," +
                _TorchEnable.ToString() + "," +
                _ManualControl_Req.ToString() + "," +
                _StockXDisplacement.ToString() + "," +
                _RobotEnabled.ToString() + "," +
                _Park_Req.ToString() + ",\"" +
                _PC_Message +
                "\"]";
            return output;
        }


        // --- INTERNAL EVENTS AND AUTOMATION ---
        /// <summary>
        /// Update Object from controller when data changed 
        /// </summary>
        /// <param name="sender">Network Controller</param>
        /// <param name="e">Data Changed Specification</param>
        private void Update_Struct(object sender, DataValueChangedEventArgs e)
        {

            GetFromRapidData();
        }
        /// <summary>
        /// Initialise object with network controller data
        /// </summary>
        private void InitialUpdateStruct()
        {
            DataNode[] RapidStruct = Robot_Control_RAPID.Value.ToStructure().Children.ToArray();
            if (JobInProgress != bool.Parse(RapidStruct[0].Value)) _JobInProgress = bool.Parse(RapidStruct[0].Value);
            if (JobID != "\"" + RapidStruct[1].Value + "\"") _JobID = RapidStruct[1].Value[1..^1];
            if (TorchEnable != bool.Parse(RapidStruct[2].Value)) _TorchEnable = bool.Parse(RapidStruct[2].Value);
            if (ManualControl_Req != bool.Parse(RapidStruct[3].Value)) _ManualControl_Req = bool.Parse(RapidStruct[3].Value);
            if (StockXDisplacement != float.Parse(RapidStruct[4].Value)) _StockXDisplacement = float.Parse(RapidStruct[4].Value);
            if (RobotEnabled != bool.Parse(RapidStruct[5].Value)) _RobotEnabled = bool.Parse(RapidStruct[5].Value);
            if (Park_Req != bool.Parse(RapidStruct[6].Value)) _Park_Req = bool.Parse(RapidStruct[6].Value);
            if (PC_Message != "\"" + RapidStruct[7].Value + "\"") _PC_Message = RapidStruct[7].Value[1..^1];
        }
        /// <summary>
        /// Update Network controller with object data
        /// </summary>
        private void Update_Rapid()
        {
            bool complete = false;

            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(ControllerConnection.Rapid))
                    {
                        Robot_Control_RAPID.StringValue = ToString();
                    }
                }
                catch
                {
                    Debug.Print("mastership failed while attempting to update control register");
                    complete = false;
                }
                finally
                {
                    complete = true;
                }
            }
            complete = false;

        }
        /// <summary>
        /// Method to raise ValueUpdate Event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueUpdate(object sender = null, ControlStrucEventArgs args = null)
        {
            if (ValueUpdate != null)
            {
                ValueUpdate?.Invoke(this, args);
            }
            else // null
            {
                Debug.WriteLine("No Listeners found for onvalueupdate on Robot Control structure");
            }
        }
        /// <summary>
        /// Method to raise PC_MessageUpdate event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPC_MessageUpdate(ControlStrucEventArgs e)
        {
            EventHandler<ControlStrucEventArgs> handler = PC_MessageUpdate;
            if (handler != null)
            {
                handler(this, e);
            }
            else // null
            {
                Debug.WriteLine("struct OnValueUpdate handler not assigned");
            }
        }
    }

}
