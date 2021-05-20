using ABB.Robotics.Controllers;
using System;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using System.Diagnostics;

namespace RFRCC_RobotController.ABB_Data
{
    public class ControlStrucEventArgs : EventArgs
    {
        public ControlStrucEventArgs(string valueName)
        {
            ValueName = valueName;
        }

        public string ValueName { get; set; }
    }
    public class Robot_ControlStruct
    {
        public RapidData Robot_Control_RAPID { get; set; }
        private bool _JobInProgress;
        private string _JobID;
        private bool _TorchEnable;
        private bool _ManualControl_Req;
        private float _StockXDisplacement;
        private bool _RobotEnabled;
        private bool _Park_Req;
        private string _PC_Message;
        private ABB.Robotics.Controllers.Controller ControllerConnection;
        public event EventHandler<ControlStrucEventArgs> ValueUpdate;
        public event EventHandler<ControlStrucEventArgs> PC_MessageUpdate;



        protected virtual void OnValueUpdate(ControlStrucEventArgs e)
        {
            EventHandler<ControlStrucEventArgs> handler = ValueUpdate;
            if (handler != null)
            {
                handler(this, e);
            }
            else // null
            {
                Debug.WriteLine("struct OnValueUpdate fired if handler == null");
            }
        }
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
        public float StockXDisplacement
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

        public void ConnectToRAPID(ABB.Robotics.Controllers.Controller controller, Task RobotTask, string Module, string RAPID_Name)
        {
            this.ControllerConnection = controller;
            this.Robot_Control_RAPID = RobotTask.GetRapidData(Module, RAPID_Name);
            this.ToString();
            InitialUpdateStruct();
            this.Robot_Control_RAPID.ValueChanged += Update_Struct;
        }
        private void InitialUpdateStruct()
        {
            DataNode[] RapidStruct = this.Robot_Control_RAPID.Value.ToStructure().Children.ToArray();
            if (this.JobInProgress != bool.Parse(RapidStruct[0].Value)) this._JobInProgress = bool.Parse(RapidStruct[0].Value);
            if (this.JobID != "\"" + RapidStruct[1].Value + "\"") this._JobID = RapidStruct[1].Value[1..^1];
            if (this.TorchEnable != bool.Parse(RapidStruct[2].Value)) this._TorchEnable = bool.Parse(RapidStruct[2].Value);
            if (this.ManualControl_Req != bool.Parse(RapidStruct[3].Value)) this._ManualControl_Req = bool.Parse(RapidStruct[3].Value);
            if (this.StockXDisplacement != float.Parse(RapidStruct[4].Value)) this._StockXDisplacement = float.Parse(RapidStruct[4].Value);
            if (this.RobotEnabled != bool.Parse(RapidStruct[5].Value)) this._RobotEnabled = bool.Parse(RapidStruct[5].Value);
            if (this.Park_Req != bool.Parse(RapidStruct[6].Value)) this._Park_Req = bool.Parse(RapidStruct[6].Value);
            if (this.PC_Message != "\"" + RapidStruct[7].Value + "\"") this._PC_Message = RapidStruct[7].Value[1..^1];
        }

        private void Update_Struct(object sender, DataValueChangedEventArgs e)
        {

            this.GetFromRapidData();
        }

        private void Update_Rapid()
        {
            bool complete = false;

            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(this.ControllerConnection.Rapid))
                    {
                        this.Robot_Control_RAPID.StringValue = this.ToString();
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

        public string DisplayInfo
        {
            get { return $"{JobInProgress}, { JobID }, { TorchEnable }, { ManualControl_Req }, { StockXDisplacement }, { RobotEnabled }, {Park_Req}, { PC_Message } "; }

        }
        public void GetFromRapidData()
        {
            DataNode[] RapidStruct = this.Robot_Control_RAPID.Value.ToStructure().Children.ToArray();

            if (this.JobInProgress != bool.Parse(RapidStruct[0].Value)) this.JobInProgress = bool.Parse(RapidStruct[0].Value);
            if (this.JobID != "\"" + RapidStruct[1].Value + "\"") this.JobID = RapidStruct[1].Value[1..^1];
            if (this.TorchEnable != bool.Parse(RapidStruct[2].Value)) this.TorchEnable = bool.Parse(RapidStruct[2].Value);
            if (this.ManualControl_Req != bool.Parse(RapidStruct[3].Value)) this.ManualControl_Req = bool.Parse(RapidStruct[3].Value);
            if (this.StockXDisplacement != float.Parse(RapidStruct[4].Value)) this.StockXDisplacement = float.Parse(RapidStruct[4].Value);
            if (this.RobotEnabled != bool.Parse(RapidStruct[5].Value)) this.RobotEnabled = bool.Parse(RapidStruct[5].Value);
            if (this.Park_Req != bool.Parse(RapidStruct[6].Value)) this.Park_Req = bool.Parse(RapidStruct[6].Value);
            if (this.PC_Message != "\"" + RapidStruct[7].Value + "\"") this.PC_Message = RapidStruct[7].Value[1..^1];
        }

        override public string ToString()
        {
            string output = "[" +
                this._JobInProgress.ToString() + ",\"" +
                this._JobID + "\"," +
                this._TorchEnable.ToString() + "," +
                this._ManualControl_Req.ToString() + "," +
                this._StockXDisplacement.ToString() + "," +
                this._RobotEnabled.ToString() + "," +
                this._Park_Req.ToString() + ",\"" +
                this._PC_Message +
                "\"]";
            return output;
        }



    }

}
