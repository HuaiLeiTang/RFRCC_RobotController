using ABB.Robotics.Math;
using ABB.Robotics.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using System.Diagnostics;
using System.Net;

namespace RFRCC_RobotController
{
    public class PC_RobotMove_Register
    {
        private List<RobotComputedFeatures> _ComputedFeatures = new List<RobotComputedFeatures>();
        private int _NumberOfFeatures = 0;
        private RAPID_OM_List _OpManArrayBuffer;
        private RAPID_OH_List _OpHeadArrayBuffer;
        public class RobotComputedFeatures
        {
            private OperationHeader _FeatureHeader = new OperationHeader();
            private List<OperationManoeuvre> _FeatureManoeuvres = new List<OperationManoeuvre>();
            public RobotComputedFeatures()
            {
            }
            public RobotComputedFeatures(OperationHeader NewHeader)
            {
                _FeatureHeader = NewHeader;
            }
            public RobotComputedFeatures(OperationHeader NewHeader, List<OperationManoeuvre> NewManoeuvres)
            {
                _FeatureHeader = NewHeader;
                _FeatureManoeuvres.AddRange(NewManoeuvres);
            }
            public OperationHeader FeatureHeader
            {
                get
                {
                    return _FeatureHeader;
                }
                set
                {
                    _FeatureHeader = value;
                }
            }
            public List<OperationManoeuvre> FeatureManoeuvres
            {
                get
                {
                    return _FeatureManoeuvres;
                }
                set
                {
                    _FeatureManoeuvres = value;
                }
            }
        }
        public PC_RobotMove_Register(RAPID_OM_List OperationManoeuvres, RAPID_OH_List OperationHeaders)
        {
            _OpManArrayBuffer = OperationManoeuvres;
            _OpHeadArrayBuffer = OperationHeaders;
            OperationHeaders.StructChange += OnRobotStructChange;
        }

        public RobotComputedFeatures Feature(int FeatureNum)
        {
            if (FeatureNumberInComputedFeatures(FeatureNum))
            {
                for (int i = 0; i < _ComputedFeatures.Count; i++)
                {
                    if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                        return _ComputedFeatures[i];
                }
                return new RobotComputedFeatures();
            }
            else
                return new RobotComputedFeatures();
        }
        private int FeatureIndex(int FeatureNum)
        {
            if (FeatureNumberInComputedFeatures(FeatureNum))
            {
                for (int i = 0; i < _ComputedFeatures.Count; i++)
                {
                    if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                        return i;
                }
                return -1;
            }
            else
                return -1;
        }
        public int NumberOfFeaturesComplete
        {
            get
            {
                return _NumberOfFeatures;
            }
        }
        protected virtual void OnRobotStructChange(object source, EventArgs e)
        {
            UpdateFeatureList();
        }
        public void UpdateFeatureList()
        {
            _OpHeadArrayBuffer.DownloadData();
            List<int> FeaturesToBeUpdated = new List<int>();

            // Check for instructions new in list (not in currently in 'ComputedFeatures')
            // & check if instruction has updated / changed
            for (int i = 0; i < _OpHeadArrayBuffer.SizeOfBuffer; i++)
            {
                if (_OpHeadArrayBuffer.Data(i, false).FeatureNum != 0)
                {
                    // if not in current buffer, add to list
                    if (!FeatureNumberInComputedFeatures(_OpHeadArrayBuffer.Data(i, false).FeatureNum))
                        FeaturesToBeUpdated.Add(_OpHeadArrayBuffer.Data(i, false).FeatureNum);
                    // if in buffer & different from current header, update
                    else
                        if (_OpHeadArrayBuffer.Data(i, false) != Feature(_OpHeadArrayBuffer.Data(i, false).FeatureNum).FeatureHeader)
                        FeaturesToBeUpdated.Add(_OpHeadArrayBuffer.Data(i, false).FeatureNum);
                }
            }

            _OpManArrayBuffer.DownloadData();
            // update computed register with new headers
            for (int i = 0; i < FeaturesToBeUpdated.Count; i++)
            {
                // go through buffer to find the corresponding header
                for (int j = 0; j < _OpHeadArrayBuffer.SizeOfBuffer; j++)
                {
                    if (_OpHeadArrayBuffer.Data(j, false).FeatureNum == FeaturesToBeUpdated[i] && _OpHeadArrayBuffer.Data(j, false).PathComplete)
                    {
                        UpdateComputedFeatureHeader(_OpHeadArrayBuffer.Data(j, false));
                    }
                }
            }

            return;
        }
        private void UpdateComputedFeatureHeader(OperationHeader Input)
        {
            int index = FeatureIndex(Input.FeatureNum);
            List<OperationManoeuvre> moves = new List<OperationManoeuvre>();
            for (int i = Input.ManoeuvreIndex; i < Input.ManoeuvreIndex + Input.NumManoeuvres; i++)
                moves.Add(_OpManArrayBuffer.Data(i, false).Clone());

            if (index == -1) // make a new one
            {

                _ComputedFeatures.Add(new RobotComputedFeatures(Input, moves));
            }
            else
            {
                _ComputedFeatures[index].FeatureHeader = Input.Clone();
                _ComputedFeatures[index].FeatureManoeuvres = moves;
            }
        }
        // checks if FeatureNum already contained within ComputedFeatures
        private bool FeatureNumberInComputedFeatures(int FeatureNum)
        {
            for (int i = 0; i < _ComputedFeatures.Count; i++)
                if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                    return true;
            return false;
        }

    }
    public class RAPID_OM_List
    {
        private OperationManoeuvre[] _OpManArrayBuffer;
        private int _BufferSize;
        private RapidData _RAPIDdata;
        private Controller _Controller;
        public event EventHandler<EventArgs> StructChange;
        public int SizeOfBuffer
        {
            get
            {
                return _BufferSize;
            }
        }
        public OperationManoeuvre Data(int index, bool update = true)
        {
            if (update)
                DownloadData();
            return _OpManArrayBuffer[index];
        }
        public void Data(int index, OperationManoeuvre Append)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            DownloadData();
            timer.Stop();
            Debug.WriteLine("download Op Movoeuvre data in: " + timer.ElapsedMilliseconds);
            timer.Restart();
            _OpManArrayBuffer[index] = Append;
            timer.Stop();
            Debug.WriteLine("appended Op Movoeuvre data  in: " + timer.ElapsedMilliseconds);
            timer.Restart();
            UploadData();
            timer.Stop();
            Debug.WriteLine("uploaded Op Movoeuvre data in: " + timer.ElapsedMilliseconds);
        }
        protected virtual void OnStructChange(object source, DataValueChangedEventArgs e)
        {
            StructChange?.Invoke(this, e);
        }
        public RAPID_OM_List(int SizeOfArray, Controller controller, Task task, string tool, string workobject)
        {
            _BufferSize = SizeOfArray;
            _RAPIDdata = task.GetRapidData(tool, workobject);
            _Controller = controller;
            _RAPIDdata.ValueChanged += OnStructChange;
            _OpManArrayBuffer = new OperationManoeuvre[SizeOfArray];

            // check
            DownloadData();

        }
        // TODO: make this quicker
        public void FromString(string String)
        {

            string[] inputArray = String.Trim('[', ']').Split(',');
            for (int i = 0; i < _BufferSize; i++)
            {
                if (_OpManArrayBuffer[i] == null)
                    _OpManArrayBuffer[i] = new OperationManoeuvre();
                string InsertMe = string.Join(",", inputArray[(i * 51)..((i + 1) * 51)]);
                _OpManArrayBuffer[i].FromString(InsertMe);
            }
        }
        public override string ToString()
        {
            string output = "[";

            for (int i = 0; i < _BufferSize; i++)
            {
                output += _OpManArrayBuffer[i].ToString() + ",";
            }

            return output.Trim(',') + "]";

        }
        // upload function takes 693ms
        public void UploadData()
        {
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller.Rapid))
                    {
                        _RAPIDdata.StringValue = this.ToString();
                    }
                }
                catch
                {
                    complete = false;
                }
                finally
                {
                    complete = true;
                }
            }
        }
        // Download function takes 21032ms
        public void DownloadData()
        {


            FromString(_RAPIDdata.StringValue);
        }

    }
    public class RAPID_OH_List
    {
        private OperationHeader[] _OpHeadArrayBuffer;
        private int _BufferSize;
        private RapidData _RAPIDdata;
        private Controller _Controller;
        public event EventHandler<EventArgs> StructChange;
        public int SizeOfBuffer
        {
            get
            {
                return _BufferSize;
            }
        }
        public OperationHeader Data(int index, bool update = true)
        {
            if (update)
                DownloadData();
            return _OpHeadArrayBuffer[index];
        }
        public void Data(int index, OperationHeader Append)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            DownloadData();
            timer.Stop();
            Debug.WriteLine("download Op Header data in: " + timer.ElapsedMilliseconds);
            timer.Restart();
            _OpHeadArrayBuffer[index] = Append;
            timer.Stop();
            Debug.WriteLine("appended Op Header data  in: " + timer.ElapsedMilliseconds);
            timer.Restart();
            UploadData();
            timer.Stop();
            Debug.WriteLine("uploaded Op Header data in: " + timer.ElapsedMilliseconds);
        }
        protected virtual void OnStructChange(object source, DataValueChangedEventArgs e)
        {
            StructChange?.Invoke(this, e);
        }
        public RAPID_OH_List(int SizeOfArray, Controller controller, Task task, string tool, string workobject)
        {
            _BufferSize = SizeOfArray;
            _RAPIDdata = task.GetRapidData(tool, workobject);
            _Controller = controller;
            _RAPIDdata.ValueChanged += OnStructChange;
            _OpHeadArrayBuffer = new OperationHeader[SizeOfArray];

            // check
            DownloadData();

        }
        // TODO: make this quicker
        public void FromString(string String)
        {

            string[] inputArray = String.Trim('[', ']').Split(',');
            for (int i = 0; i < _BufferSize; i++)
            {
                if (_OpHeadArrayBuffer[i] == null)
                    _OpHeadArrayBuffer[i] = new OperationHeader();
                _OpHeadArrayBuffer[i].FromString(string.Join(",", inputArray[(i * 17)..((i + 1) * 17)]).Trim('[', ']'));
            }
        }
        public override string ToString()
        {
            string output = "[";

            for (int i = 0; i < _BufferSize; i++)
            {
                output += _OpHeadArrayBuffer[i].ToString() + ",";
            }

            return output.Trim(',') + "]";

        }
        // upload function takes 511ms
        public void UploadData()
        {
            bool complete = false;
            string sendme = this.ToString();
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller.Rapid))
                    {

                        _RAPIDdata.StringValue = sendme;
                    }
                }
                catch
                {
                    complete = false;
                }
                finally
                {
                    complete = true;
                }
            }
        }
        // download function takes 3708ms
        public void DownloadData()
        {
            this.FromString(_RAPIDdata.StringValue);
        }
    }
    public class OperationManoeuvre
    {
        private string _Movement;
        private string _Type;
        private string _Dim2Ref;
        private bool _StartCut;
        private bool _EndCut;
        private bool _WristFirst;
        private CS_RobTarget _ManRobT;
        private CS_RobTarget _ManEndRobT;
        private CS_speeddata _ManSpeed;
        private CS_zonedata _ManZone;

        public CS_zonedata ManZone
        {
            get
            {
                return _ManZone;
            }
            set
            {
                _ManZone = value;
            }
        }
        public CS_speeddata ManSpeed
        {
            get
            {
                return _ManSpeed;
            }
            set
            {
                _ManSpeed = value;
            }
        }
        public CS_RobTarget ManEndRobT
        {
            get
            {
                return _ManEndRobT;
            }
            set
            {
                _ManEndRobT = value;
            }
        }
        public CS_RobTarget ManRobT
        {
            get
            {
                return _ManRobT;
            }
            set
            {
                _ManRobT = value;
            }
        }
        public bool WristFirst
        {
            get
            {
                return _WristFirst;
            }
            set
            {
                _WristFirst = value;
            }
        }
        public bool EndCut
        {
            get
            {
                return _EndCut;
            }
            set
            {
                _EndCut = value;
            }
        }
        public bool StartCut
        {
            get
            {
                return _StartCut;
            }
            set
            {
                _StartCut = value;
            }
        }
        public string Dim2Ref
        {
            get
            {
                return _Dim2Ref;
            }
            set
            {
                _Dim2Ref = value;
            }
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public string Movement
        {
            get
            {
                return _Movement;
            }
            set
            {
                _Movement = value;
            }
        }
        public OperationManoeuvre()
        {
            _Movement = "";
            _Type = "";
            _Dim2Ref = "";
            _StartCut = false;
            _EndCut = false;
            _WristFirst = false;
            _ManRobT = new CS_RobTarget();
            _ManEndRobT = new CS_RobTarget();
            _ManSpeed = new CS_speeddata();
            _ManZone = new CS_zonedata();
        }
        public OperationManoeuvre(string Input)
        {
            new OperationManoeuvre().FromString(Input);
        }
        public void FromString(string String)
        {
            string[] inputArray = String.Trim('[', ']').Split(',');
            _Movement = inputArray[0].ToLower().Trim('\"');
            _Type = inputArray[1].ToLower().Trim('\"');
            _Dim2Ref = inputArray[2].ToLower().Trim('\"');
            _StartCut = Bool.Parse(inputArray[3].ToLower());
            _EndCut = Bool.Parse(inputArray[4].ToLower());
            _WristFirst = Bool.Parse(inputArray[5].ToLower());
            _ManRobT.FromString(string.Join(",", inputArray[6..23]).ToLower()); // 17 variables
            _ManEndRobT.FromString(string.Join(",", inputArray[23..40]).ToLower()); // 17 variables
            _ManSpeed.FromString(string.Join(",", inputArray[40..44]).ToLower()); // 4 variables
            _ManZone.FromString(string.Join(",", inputArray[44..51]).ToLower()); // 7 varables
        }
        public override string ToString()
        {
            return "[\"" +
               _Movement + "\",\"" +
               _Type + "\",\"" +
               _Dim2Ref + "\"," +
               _StartCut.ToString() + "," +
               _EndCut.ToString() + "," +
               _WristFirst.ToString() + "," +
               _ManRobT.ToString() + "," +
               _ManEndRobT.ToString() + "," +
               _ManSpeed.ToString() + "," +
               _ManZone.ToString() + "]";
        }
        public OperationManoeuvre Clone()
        {
            return (OperationManoeuvre)this.MemberwiseClone();
        }
    }
    public class OperationHeader
    {
        private int _FeatureNum;
        private double _IdealXDisplacement;
        private string _TaskCode;
        private string _Face;
        private CS_pos _LocationMin;
        private CS_pos _LocationMax;
        private int _NumInstructions;
        private int _NumManoeuvres;
        private int _ManoeuvreIndex;
        private bool _PathComplete;
        private bool _Ready;
        private bool _LeadInstruction;
        private bool _Complete;


        public bool Complete
        {
            get
            {
                return _Complete;
            }
            set
            {
                _Complete = value;
            }
        }
        public bool LeadInstruction
        {
            get
            {
                return _LeadInstruction;
            }
            set
            {
                _LeadInstruction = value;
            }
        }
        public bool Ready
        {
            get
            {
                return _Ready;
            }
            set
            {
                _Ready = value;
            }
        }
        public bool PathComplete
        {
            get
            {
                return _PathComplete;
            }
            set
            {
                _PathComplete = value;
            }
        }
        public int ManoeuvreIndex
        {
            get
            {
                return _ManoeuvreIndex;
            }
            set
            {
                _ManoeuvreIndex = value;
            }
        }
        public int NumManoeuvres
        {
            get
            {
                return _NumManoeuvres;
            }
            set
            {
                _NumManoeuvres = value;
            }
        }
        public int NumInstructions
        {
            get
            {
                return _NumInstructions;
            }
            set
            {
                _NumInstructions = value;
            }
        }
        public CS_pos LocationMax
        {
            get
            {
                return _LocationMax;
            }
            set
            {
                _LocationMax = value;
            }
        }
        public CS_pos LocationMin
        {
            get
            {
                return _LocationMin;
            }
            set
            {
                _LocationMin = value;
            }
        }
        public string Face
        {
            get
            {
                return _Face;
            }
            set
            {
                _Face = value;
            }
        }
        public string TaskCode
        {
            get
            {
                return _TaskCode;
            }
            set
            {
                _TaskCode = value;
            }
        }
        public double IdealXDisplacement
        {
            get
            {
                return _IdealXDisplacement;
            }
            set
            {
                _IdealXDisplacement = value;
            }
        }
        public int FeatureNum
        {
            get
            {
                return _FeatureNum;
            }
            set
            {
                _FeatureNum = value;
            }
        }

        public OperationHeader()
        {
            _FeatureNum = 0;
            _IdealXDisplacement = 0;
            _TaskCode = "";
            _Face = "";
            _LocationMin = new CS_pos();
            _LocationMax = new CS_pos();
            _NumInstructions = 0;
            _NumManoeuvres = 0;
            _ManoeuvreIndex = 0;
            _PathComplete = false;
            _Ready = false;
            _LeadInstruction = false;
            _Complete = false;
        }
        public OperationHeader(string input)
        {
            new OperationHeader().FromString(input);
        }

        public void FromString(string String)
        {
            string[] inputArray = String.Trim('[', ']').Split(',');
            _FeatureNum = Int32.Parse(inputArray[0].ToLower());
            _IdealXDisplacement = Double.Parse(inputArray[1].ToLower());
            _TaskCode = inputArray[2].ToLower().Trim('\"');
            _Face = inputArray[3].ToLower().Trim('\"');
            _LocationMin.FromString(string.Join(",", inputArray[4..7]).ToLower());
            _LocationMax = new CS_pos(string.Join(",", inputArray[7..10]).ToLower());
            _NumInstructions = Int32.Parse(inputArray[10].ToLower());
            _NumManoeuvres = Int32.Parse(inputArray[11].ToLower());
            _ManoeuvreIndex = Int32.Parse(inputArray[12].ToLower());
            _PathComplete = Bool.Parse(inputArray[13].ToLower());
            _Ready = Bool.Parse(inputArray[14].ToLower());
            _LeadInstruction = Bool.Parse(inputArray[15].ToLower());
            _Complete = Bool.Parse(inputArray[16].ToLower());
        }
        public override string ToString()
        {
            return "[" +
               _FeatureNum.ToString() + "," +
               _IdealXDisplacement.ToString() + ",\"" +
               _TaskCode + "\",\"" +
               _Face + "\"," +
               _LocationMin.ToString() + "," +
               _LocationMax.ToString() + "," +
               _NumInstructions.ToString() + "," +
               _NumManoeuvres.ToString() + "," +
               _ManoeuvreIndex.ToString() + "," +
               _PathComplete.ToString() + "," +
               _Ready.ToString() + "," +
               _LeadInstruction.ToString() + "," +
               _Complete.ToString() + "]";
        }

        public OperationHeader Clone()
        {
            return (OperationHeader)this.MemberwiseClone();
        }

    }


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
        private Controller ControllerConnection;
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
                Debug.WriteLine("struct OnValueUpdate fired if handler == null");
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

        public void ConnectToRAPID(Controller controller, Task RobotTask, string Module, string RAPID_Name)
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


    // C# libraries of RobotStudio Classes
    public class CS_pos
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public override string ToString()
        {
            return ("[" + X.ToString("0.00") + "," + Y.ToString("0.00") + "," + Z.ToString("0.00") + "]");
        }

        public CS_pos()
        {
            X = 0.00f;
            Y = 0.00f;
            Z = 0.00f;
        }
        public CS_pos(CS_orient PosFromQuat)
        {
            X = PosFromQuat.q2;
            Y = PosFromQuat.q3;
            Z = PosFromQuat.q4;
        }

        public CS_pos(double _X, double _Y, double _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
        public CS_pos(string input)
        {
            new CS_pos().FromString(input);
        }

        public void FromString(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            this.X = double.Parse(inputArray[0]);
            this.Y = double.Parse(inputArray[1]);
            this.Z = double.Parse(inputArray[2]);
        }


    }
    public class CS_orient
    {

        public double q1 { get; set; } // W
        public double q2 { get; set; } // X
        public double q3 { get; set; } // Y
        public double q4 { get; set; } // Z

        public CS_orient()
        {
            q1 = 1;
            q2 = 0;
            q3 = 0;
            q4 = 0;
        }

        public CS_orient(double _q1, double _q2, double _q3, double _q4)
        {
            q1 = _q1;
            q2 = _q2;
            q3 = _q3;
            q4 = _q4;
        }

        public CS_orient(CS_pos from_pos)
        {
            q1 = 0.00f;
            q2 = from_pos.X;
            q3 = from_pos.Y;
            q4 = from_pos.Z;
        }

        public CS_orient(CS_pos Vector, double Angle)
        {
            q1 = (float)Math.Cos(Angle / 2);
            q2 = (float)(Vector.X * Math.Sin(Angle / 2));
            q3 = (float)(Vector.Y * Math.Sin(Angle / 2));
            q4 = (float)(Vector.Z * Math.Sin(Angle / 2));
        }
        public CS_orient(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            q1 = double.Parse(inputArray[0]);
            q2 = double.Parse(inputArray[1]);
            q3 = double.Parse(inputArray[2]);
            q4 = double.Parse(inputArray[3]);
        }
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            q1 = double.Parse(InputArray[0]);
            q2 = double.Parse(InputArray[1]);
            q3 = double.Parse(InputArray[2]);
            q4 = double.Parse(InputArray[3]);
        }

        public override string ToString()
        {
            return ("[" + q1.ToString("0.0000") + "," + q2.ToString("0.0000") + "," + q3.ToString("0.0000") + "," + q4.ToString("0.0000") + "]");
        }

    }
    public class CS_confdata
    {
        public double cf1 { get; set; }
        public double cf4 { get; set; }
        public double cf6 { get; set; }
        public double cfx { get; set; }
        public CS_confdata()
        {
            cf1 = 0f;
            cf4 = 0f;
            cf6 = 0f;
            cfx = 0f;
        }
        public CS_confdata(float _cf1, float _cf4, float _cf6, float _cfx)
        {
            cf1 = _cf1;
            cf4 = _cf4;
            cf6 = _cf6;
            cfx = _cfx;
        }
        public CS_confdata(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            cf1 = Single.Parse(inputArray[0]);
            cf4 = Single.Parse(inputArray[1]);
            cf6 = Single.Parse(inputArray[2]);
            cfx = Single.Parse(inputArray[3]);
        }

        public override string ToString()
        {
            return ("[" + cf1.ToString("0.00000000") + "," + cf4.ToString("0.00000000") + "," + cf6.ToString("0.00000000") + "," + cfx.ToString("0.00000000") + "]");
        }

        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            cf1 = double.Parse(InputArray[0]);
            cf4 = double.Parse(InputArray[1]);
            cf6 = double.Parse(InputArray[2]);
            cfx = double.Parse(InputArray[3]);
        }
    }
    public class CS_extjoint
    {
        double _eax_a;
        double _eax_b;
        double _eax_c;
        double _eax_d;
        double _eax_e;
        double _eax_f;
        public double eax_a
        {
            get
            {
                return _eax_a;
            }
            set
            {
                _eax_a = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[0] = true;
            }
        }
        public double eax_b
        {
            get
            {
                return _eax_b;
            }
            set
            {
                _eax_b = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[1] = true;
            }
        }
        public double eax_c
        {
            get
            {
                return _eax_c;
            }
            set
            {
                _eax_c = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[2] = true;
            }
        }
        public double eax_d
        {
            get
            {
                return _eax_d;
            }
            set
            {
                _eax_d = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[3] = true;
            }
        }
        public double eax_e
        {
            get
            {
                return _eax_e;
            }
            set
            {
                _eax_e = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[4] = true;
            }
        }
        public double eax_f
        {
            get
            {
                return _eax_f;
            }
            set
            {
                _eax_f = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[5] = true;
            }
        }
        private bool[] ConnectedAxis = new bool[6];

        public CS_extjoint()
        {
            eax_a = 0;
            eax_b = 0;
            eax_c = 0;
            eax_d = 0;
            eax_e = 0;
            eax_f = 0;
            ConnectedAxis = new bool[6];
            ConnectedAxis[0] = false;
            ConnectedAxis[1] = false;
            ConnectedAxis[2] = false;
            ConnectedAxis[3] = false;
            ConnectedAxis[4] = false;
            ConnectedAxis[5] = false;
        }
        public CS_extjoint(string _eax_a, string _eax_b, string _eax_c, string _eax_d, string _eax_e, string _eax_f)
        {
            eax_a = _eax_a == "9E+09" ? 0f : Double.Parse(_eax_a);
            eax_b = _eax_b == "9E+09" ? 0f : Double.Parse(_eax_b);
            eax_c = _eax_c == "9E+09" ? 0f : Double.Parse(_eax_c);
            eax_d = _eax_d == "9E+09" ? 0f : Double.Parse(_eax_d);
            eax_e = _eax_e == "9E+09" ? 0f : Double.Parse(_eax_e);
            eax_f = _eax_f == "9E+09" ? 0f : Double.Parse(_eax_f);
            ConnectedAxis = new bool[6];
            ConnectedAxis[0] = _eax_a == "9E+09" ? true : false;
            ConnectedAxis[1] = _eax_b == "9E+09" ? true : false;
            ConnectedAxis[2] = _eax_c == "9E+09" ? true : false;
            ConnectedAxis[3] = _eax_d == "9E+09" ? true : false;
            ConnectedAxis[4] = _eax_e == "9E+09" ? true : false;
            ConnectedAxis[5] = _eax_f == "9E+09" ? true : false;
        }
        public CS_extjoint(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            eax_a = inputArray[0] == "9E+09" ? 0 : Double.Parse(inputArray[0]);
            eax_b = inputArray[1] == "9E+09" ? 0 : Double.Parse(inputArray[1]);
            eax_c = inputArray[2] == "9E+09" ? 0 : Double.Parse(inputArray[2]);
            eax_d = inputArray[3] == "9E+09" ? 0 : Double.Parse(inputArray[3]);
            eax_e = inputArray[4] == "9E+09" ? 0 : Double.Parse(inputArray[4]);
            eax_f = inputArray[5] == "9E+09" ? 0 : Double.Parse(inputArray[5]);
            ConnectedAxis = new bool[6];
            ConnectedAxis[0] = inputArray[0] == "9E+09" ? false : true;
            ConnectedAxis[1] = inputArray[1] == "9E+09" ? false : true;
            ConnectedAxis[2] = inputArray[2] == "9E+09" ? false : true;
            ConnectedAxis[3] = inputArray[3] == "9E+09" ? false : true;
            ConnectedAxis[4] = inputArray[4] == "9E+09" ? false : true;
            ConnectedAxis[5] = inputArray[5] == "9E+09" ? false : true;
        }
        public CS_extjoint(bool[] BinaryInput)
        {
            int i = 0;

            foreach (bool check in BinaryInput)
            {
                ConnectedAxis[i] = check;
                i++;
                if (i > 6)
                {
                    break;
                }
            }
        }
        public void ConnectExtAxis(bool[] BinaryInput)
        {
            int i = 0;

            foreach (bool check in BinaryInput)
            {
                ConnectedAxis[i] = check;
                i++;
                if (i > 6)
                {
                    break;
                }
            }
        }

        public void ConnectExtAxis(string input)
        {
            ConnectedAxis[0] = input.Contains("eax_a") ? true : ConnectedAxis[0];
            ConnectedAxis[1] = input.Contains("eax_b") ? true : ConnectedAxis[1];
            ConnectedAxis[2] = input.Contains("eax_c") ? true : ConnectedAxis[2];
            ConnectedAxis[3] = input.Contains("eax_d") ? true : ConnectedAxis[3];
            ConnectedAxis[4] = input.Contains("eax_e") ? true : ConnectedAxis[4];
            ConnectedAxis[5] = input.Contains("eax_f") ? true : ConnectedAxis[5];
        }

        public override string ToString()
        {
            return ("[" + (ConnectedAxis[0] ? _eax_a.ToString() : "9E+09") + "," + (ConnectedAxis[1] ? _eax_b.ToString() : "9E+09") + "," + (ConnectedAxis[2] ? _eax_c.ToString() : "9E+09") + "," + (ConnectedAxis[3] ? _eax_d.ToString() : "9E+09") + "," + (ConnectedAxis[4] ? _eax_e.ToString() : "9E+09") + "," + (ConnectedAxis[5] ? _eax_f.ToString() : "9E+09") + "]");
        }

        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            _eax_a = double.Parse(InputArray[0]);
            _eax_b = double.Parse(InputArray[1]);
            _eax_c = double.Parse(InputArray[2]);
            _eax_d = double.Parse(InputArray[3]);
            _eax_e = double.Parse(InputArray[4]);
            _eax_f = double.Parse(InputArray[5]);
        }
    }
    public class CS_RobTarget
    {
        public CS_pos trans { get; set; }
        public CS_orient rot { get; set; }
        public CS_confdata robconf { get; set; }
        public CS_extjoint extax { get; set; }

        public CS_RobTarget()
        {
            trans = new CS_pos();
            rot = new CS_orient();
            robconf = new CS_confdata();
            extax = new CS_extjoint();
        }
        public CS_RobTarget(bool[] ExtAx)
        {
            trans = new CS_pos();
            rot = new CS_orient();
            robconf = new CS_confdata();
            extax = new CS_extjoint(ExtAx);
        }
        public CS_RobTarget(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(new string[] { "],[" }, StringSplitOptions.None);
            trans = new CS_pos(inputArray[0]);
            rot = new CS_orient(inputArray[1]);
            robconf = new CS_confdata(inputArray[2]);
            extax = new CS_extjoint(inputArray[3]);
        }
        public CS_RobTarget(DataNode[] input)
        {
            trans = new CS_pos(Single.Parse(input[0].Children[0].Value), Single.Parse(input[0].Children[1].Value), Single.Parse(input[0].Children[2].Value));
            rot = new CS_orient(Single.Parse(input[1].Children[0].Value), Single.Parse(input[1].Children[1].Value), Single.Parse(input[1].Children[2].Value), Single.Parse(input[1].Children[3].Value));
            robconf = new CS_confdata(Single.Parse(input[2].Children[0].Value), Single.Parse(input[2].Children[1].Value), Single.Parse(input[2].Children[2].Value), Single.Parse(input[2].Children[3].Value));
            extax = new CS_extjoint(input[3].Children[0].Value, input[3].Children[1].Value, input[3].Children[2].Value, input[3].Children[3].Value, input[3].Children[4].Value, input[3].Children[5].Value);
        }

        public override string ToString()
        {
            return ("[" + trans.ToString() + "," + rot.ToString() + "," + robconf.ToString() + "," + extax.ToString() + "]");
        }

        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            trans.FromString(string.Join(",", InputArray[0..3]));
            rot.FromString(string.Join(",", InputArray[3..7]));
            robconf.FromString(string.Join(",", InputArray[7..11]));
            extax.FromString(string.Join(",", InputArray[11..17]));
        }
    }
    public class CS_wobjdata
    {
        public bool robhold { get; set; }
        public bool ufprog { get; set; }
        public string ufmec { get; set; }
        public CS_pos uframe { get; set; }
        public CS_pos oframe { get; set; }

        public CS_wobjdata()
        {
            robhold = false;
            ufprog = false;
            ufmec = "";
            uframe = new CS_pos();
            oframe = new CS_pos();
        }
        public CS_wobjdata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            robhold = inputArray[0].ToLower() == "true" ? true : false;
            ufprog = inputArray[1].ToLower() == "true" ? true : false;
            ufmec = inputArray[2].Trim('"');
            uframe = new CS_pos(inputArray[3]);
            oframe = new CS_pos(inputArray[4]);
        }
        public CS_wobjdata(bool _robhold, bool _ufprog, string _ufmec, CS_pos _uframe, CS_pos _oframe)
        {
            robhold = _robhold;
            ufprog = _ufprog;
            ufmec = _ufmec;
            uframe = _uframe;
            oframe = _oframe;
        }



    }
    public class CS_speeddata
    {
        private double _v_tcp;
        private double _v_ori;
        private double _v_leax;
        private double _v_reax;

        public double v_reax
        {
            get { return _v_reax; }
            set { _v_reax = value; }
        }

        public double v_leax
        {
            get { return _v_leax; }
            set { _v_leax = value; }
        }

        public double v_ori
        {
            get { return _v_ori; }
            set { _v_ori = value; }
        }


        public double v_tcp
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }

        public CS_speeddata()
        {
            _v_tcp = 0;
            _v_ori = 0;
            _v_leax = 0;
            _v_reax = 0;
        }
        public CS_speeddata(double v_tcp, double v_ori, double v_leax, double v_reax)
        {
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
        }
        public CS_speeddata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _v_tcp = Double.Parse(inputArray[0].ToLower());
            _v_ori = Double.Parse(inputArray[1].ToLower());
            _v_leax = Double.Parse(inputArray[2].ToLower());
            _v_reax = Double.Parse(inputArray[3].ToLower());
        }

        public override string ToString()
        {
            return ("[" + _v_tcp.ToString("0.00") + "," + _v_ori.ToString("0.00") + "," + _v_leax.ToString("0.00") + "," + _v_reax.ToString("0.00") + "]");
        }
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            _v_tcp = double.Parse(InputArray[0]);
            _v_ori = double.Parse(InputArray[1]);
            _v_leax = double.Parse(InputArray[2]);
            _v_reax = double.Parse(InputArray[3]);
        }

    }
    public class CS_zonedata
    {
        private bool _finep;
        private double _pzone_tcp;
        private double _pzone_ori;
        private double _pzone_eax;
        private double _zone_ori;
        private double _zone_leax;
        private double _zone_reax;

        public double zone_reax
        {
            get { return _zone_reax; }
            set { _zone_reax = value; }
        }
        public double zone_leax
        {
            get { return _zone_leax; }
            set { _zone_leax = value; }
        }
        public double zone_ori
        {
            get { return _zone_ori; }
            set { _zone_ori = value; }
        }
        public double pzone_eax
        {
            get { return _pzone_eax; }
            set { _pzone_eax = value; }
        }
        public double pzone_ori
        {
            get { return _pzone_ori; }
            set { _pzone_ori = value; }
        }
        public double pzone_tcp
        {
            get { return _pzone_tcp; }
            set { _pzone_tcp = value; }
        }
        public bool finep
        {
            get { return _finep; }
            set { _finep = value; }
        }

        public CS_zonedata()
        {
            _finep = false;
            _pzone_tcp = 0;
            _pzone_ori = 0;
            _pzone_eax = 0;
            _zone_ori = 0;
            _zone_leax = 0;
            _zone_reax = 0;
        }
        public CS_zonedata(bool finep, double pzone_tcp, double pzone_ori, double pzone_eax, double zone_ori, double zone_leax, double zone_reax)
        {
            _finep = finep;
            _pzone_tcp = pzone_tcp;
            _pzone_ori = pzone_ori;
            _pzone_eax = pzone_eax;
            _zone_ori = zone_ori;
            _zone_leax = zone_leax;
            _zone_reax = zone_reax;
        }
        public CS_zonedata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _finep = Bool.Parse(inputArray[0].ToLower());
            _pzone_tcp = Double.Parse(inputArray[1].ToLower());
            _pzone_ori = Double.Parse(inputArray[2].ToLower());
            _pzone_eax = Double.Parse(inputArray[3].ToLower());
            _zone_ori = Double.Parse(inputArray[4].ToLower());
            _zone_leax = Double.Parse(inputArray[5].ToLower());
            _zone_reax = Double.Parse(inputArray[6].ToLower());
        }
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            _finep = bool.Parse(InputArray[0]);
            _pzone_tcp = double.Parse(InputArray[1]);
            _pzone_ori = double.Parse(InputArray[2]);
            _pzone_eax = double.Parse(InputArray[3]);
            _zone_ori = double.Parse(InputArray[4]);
            _zone_leax = double.Parse(InputArray[5]);
            _zone_reax = double.Parse(InputArray[6]);
        }

        public override string ToString()
        {
            return ("[" + _finep.ToString() + "," + _pzone_tcp.ToString("0.00") + "," + _pzone_ori.ToString("0.00") + "," + _pzone_eax.ToString("0.00") + "," + _zone_ori.ToString("0.00") + "," + _zone_leax.ToString("0.00") + "," + _zone_reax.ToString("0.00") + "]");
        }
    }
}
