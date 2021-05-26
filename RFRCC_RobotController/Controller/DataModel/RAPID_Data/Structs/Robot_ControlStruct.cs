using ABB.Robotics.Controllers;
using System;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using System.Diagnostics;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
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
            ControllerConnection = controller;
            Robot_Control_RAPID = RobotTask.GetRapidData(Module, RAPID_Name);
            ToString();
            InitialUpdateStruct();
            Robot_Control_RAPID.ValueChanged += Update_Struct;
        }
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

        private void Update_Struct(object sender, DataValueChangedEventArgs e)
        {

            GetFromRapidData();
        }

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

        public string DisplayInfo
        {
            get { return $"{JobInProgress}, { JobID }, { TorchEnable }, { ManualControl_Req }, { StockXDisplacement }, { RobotEnabled }, {Park_Req}, { PC_Message } "; }

        }
        public void GetFromRapidData()
        {
            DataNode[] RapidStruct = Robot_Control_RAPID.Value.ToStructure().Children.ToArray();

            if (JobInProgress != bool.Parse(RapidStruct[0].Value)) JobInProgress = bool.Parse(RapidStruct[0].Value);
            if (JobID != "\"" + RapidStruct[1].Value + "\"") JobID = RapidStruct[1].Value[1..^1];
            if (TorchEnable != bool.Parse(RapidStruct[2].Value)) TorchEnable = bool.Parse(RapidStruct[2].Value);
            if (ManualControl_Req != bool.Parse(RapidStruct[3].Value)) ManualControl_Req = bool.Parse(RapidStruct[3].Value);
            if (StockXDisplacement != float.Parse(RapidStruct[4].Value)) StockXDisplacement = float.Parse(RapidStruct[4].Value);
            if (RobotEnabled != bool.Parse(RapidStruct[5].Value)) RobotEnabled = bool.Parse(RapidStruct[5].Value);
            if (Park_Req != bool.Parse(RapidStruct[6].Value)) Park_Req = bool.Parse(RapidStruct[6].Value);
            if (PC_Message != "\"" + RapidStruct[7].Value + "\"") PC_Message = RapidStruct[7].Value[1..^1];
        }

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



    }

}
