using ABB.Robotics.Controllers;
using System.Collections.Generic;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using static RFRCC_RobotController.Controller.DataModel.OperationData.PC_RobotMove_Register;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using System;
using System.Diagnostics;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    // RAPID Data connection to Operation Buffer for PC generation of paths for manoeuvres
    public class RAPID_OperationBuffer
    {
        // --- PRIVATE PROPERTIES ---- 
        private RobotController _ParentController;
        private int _SizeOfManBuffer;
        private RapidData _ManBufferRD;
        private RapidData _HeadBufferRD;
        private RapidData _RobSystemData;
        private RapidData _OperationStarted;
        private RapidData _OperationCompleted;
        private RapidData _OperationWaitingForStart;

        private PC_RobotMove_Register _Operations = new PC_RobotMove_Register();
        private bool _sortAscending = false;
        private string _OpManModule;
        private string _OpManVARName;
        private string _OpHeadModule;
        private string _OpHeadVARName;
        private string _RobSysDataModule = "Module1";
        private string _RobSysDataVARName = "Sys_RobData";
        private string _OpStartedModule = "PC_Manoeuvre_Register";
        private string _OpStartedVARName = "OperationStarted";
        private string _OpCompletedModule = "PC_Manoeuvre_Register";
        private string _OpCompletedVARName = "OperationCompleted";
        private string _OperationWaitingForStartModule = "PC_Manoeuvre_Register";
        private string _OperationWaitingForStartVARName = "WaitingOnStart";
        private bool _connected = false;
        private bool _currentJob = false;
        private bool _RobotWaiting = false;

        // --- EVENTS ---
        public event EventHandler NewCurrentOperation;
        public event EventHandler NewUploadedOperation;
        public event EventHandler OperationStarted;
        public event EventHandler OperationCompleted;
        public event EventHandler OperationWaitingForStart;


        // --- PROPERTIES ---
        /// <summary>
        /// Operation
        /// </summary>
        public PC_RobotMove_Register Operation => _Operations;
        /// <summary>
        /// Status if this object is currently uploaded and maintained on associated robot controller
        /// </summary>
        public bool CurrentJob 
        {
            get
            {
                return _currentJob;
            }
            set
            {
                _currentJob = value;
                if (_currentJob)
                {
                    // If controller is already specified
                    _ParentController.ControllerConnectionChange += ConnectToController;
                    if (_ParentController.ControllerConnected)
                    {
                        ConnectToController(_ParentController, new ControllerConnectionEventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Size of maneouvre buffer on network controller
        /// </summary>
        public int SizeOfManBuffer
        {
            get
            {
                return _SizeOfManBuffer;
            }
        }
        /// <summary>
        /// Set manoeuvre order in ascending order in distance from x = 0
        /// </summary>
        public bool AscendingOrder
        {
            get
            {
                return _sortAscending;
            }
            set
            {
                _sortAscending = value;
            }
        }
        /// <summary>
        /// Set manoeuvre order in descending order in distance from x = 0
        /// </summary>
        public bool DescendingOrder
        {
            get
            {
                return !_sortAscending;
            }
            set
            {
                _sortAscending = !value;
            }
        }

        // --- CONSTRUCTORS ---
        /// <summary>
        /// initialise empty object
        /// </summary>
        public RAPID_OperationBuffer()
        {

        }
        /// <summary>
        /// Initialise object
        /// </summary>
        /// <param name="ParentController">Network Controller</param>
        /// <param name="OpManModule">Module name housing Manoeuvre RAPID structure</param>
        /// <param name="OpManVARName">Manoeuvre RAPID structure name</param>
        /// <param name="OpHeadModule">Module name housing Header RAPID structure</param>
        /// <param name="OpHeadVARName">Header RAPID structure name</param>
        public RAPID_OperationBuffer(RobotController ParentController, string OpManModule, string OpManVARName, string OpHeadModule, string OpHeadVARName)
        {
            _ParentController = ParentController;
            _OpManModule = OpManModule;
            _OpManVARName = OpManVARName;
            _OpHeadModule = OpManModule;
            _OpHeadVARName = OpHeadVARName;

            if (_currentJob)
            {
                // If controller is already specified
                _ParentController.ControllerConnectionChange += ConnectToController;
                if (_ParentController.ControllerConnected)
                {
                    ConnectToController(_ParentController, new ControllerConnectionEventArgs());
                }
            }
        }
        
        // --- METHODS ---
        /// <summary>
        /// Connect network controller to this object
        /// </summary>
        /// <param name="ParentController">Network Controller</param>
        /// <param name="OpManModule">Module name housing Manoeuvre RAPID structure</param>
        /// <param name="OpManVARName">Manoeuvre RAPID structure name</param>
        /// <param name="OpHeadModule">Module name housing Header RAPID structure</param>
        /// <param name="OpHeadVARName">Header RAPID structure name</param>
        public void ConnectParentController(RobotController ParentController, string OpManModule, string OpManVARName, string OpHeadModule, string OpHeadVARName)
        {
            _ParentController = ParentController;
            _OpManModule = OpManModule;
            _OpManVARName = OpManVARName;
            _OpHeadModule = OpManModule;
            _OpHeadVARName = OpHeadVARName;

            if (_currentJob)
            {
                // If controller is already specified
                _ParentController.ControllerConnectionChange += ConnectToController;
                if (_ParentController.ControllerConnected)
                {
                    ConnectToController(_ParentController, new ControllerConnectionEventArgs());
                }
            }
        }
        /// <summary>
        /// Sets this job as current Job, uploads all relevant data to robot, and makes all required connections
        /// </summary>
        /// <param name="Current"> Current Job Status</param>
        public void setCurrentJob(bool Current = true)
        {
            _currentJob = Current;
            if (_currentJob)
            {
                // If controller is already specified
                _ParentController.ControllerConnectionChange += ConnectToController;
                if (_ParentController.ControllerConnected)
                {
                    ConnectToController(_ParentController, new ControllerConnectionEventArgs());
                }
            }
        }
        /// <summary>
        /// Event to Connect to controller
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public void ConnectToController(object Sender, ControllerConnectionEventArgs args)
        {
            _ManBufferRD = _ParentController.tRob1.GetRapidData(_OpManModule, _OpManVARName);
            _HeadBufferRD = _ParentController.tRob1.GetRapidData(_OpHeadModule, _OpHeadVARName);
            _RobSystemData = _ParentController.tRob1.GetRapidData(_RobSysDataModule, _RobSysDataVARName);
            _OperationStarted = _ParentController.tRob1.GetRapidData(_OpStartedModule, _OpStartedVARName);
            _OperationCompleted = _ParentController.tRob1.GetRapidData(_OpCompletedModule, _OpCompletedVARName);
            _OperationWaitingForStart = _ParentController.tRob1.GetRapidData(_OperationWaitingForStartModule, _OperationWaitingForStartVARName);

            _RobSystemData.Subscribe(OnRobSystemDataUpdate, EventPriority.High);
            _OperationStarted.Subscribe(OnOperationStartedChange, EventPriority.High);
            _OperationCompleted.Subscribe(OnOperationCompletedChange, EventPriority.High);
            _OperationWaitingForStart.Subscribe(OnOperationWaitingForStartChange, EventPriority.High);

            _SizeOfManBuffer = _ManBufferRD.StringValue.Split(',').Count() / new OperationManoeuvre().ToString().Split(',').Count();
            _connected = true; // TODO: add this bool to other issues as a stop if required
            SetJobInProgress(true);
        }
        public void DisconnectFromController(object sender, ControllerConnectionEventArgs args)
        {
            SetJobInProgress(false);
            _connected = false;
            _ManBufferRD.Dispose();
            _HeadBufferRD.Dispose();
            _RobSystemData.Dispose();
            _OperationStarted.Dispose();
            _OperationCompleted.Dispose();
            _OperationWaitingForStart.Dispose();
        }
        /// <summary>
        /// removes all items contained in Operation List
        /// </summary>
        public void Clear()
        {
            _Operations.Clear();
        }
        /// <summary>
        /// Add Operations to list of operations
        /// </summary>
        /// <param name="Operations">register of operations to add</param>
        public void AddOperationRange(PC_RobotMove_Register Operations)
        {
            foreach (RobotComputedFeatures feature in Operations)
            {
                _Operations.Add(feature);
            }
            sort();
        }
        /// <summary>
        /// Add Operations to list of operations
        /// </summary>
        /// <param name="Operations">List of operations to add</param>
        public void AddOperationRange(List<RobotComputedFeatures> Operations)
        {
            foreach (RobotComputedFeatures feature in Operations)
            {
                _Operations.Add(feature);
            }
            sort();
        }
        /// <summary>
        /// Add Operation to list of operations
        /// </summary>
        /// <param name="Operation">Operation to add</param>
        public void AddOperation(RobotComputedFeatures Operation)
        {
            _Operations.Add(Operation);
            sort();
        }
        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="input"></param>
        public void AddFromString(string input)
        {
            throw new NotImplementedException();
            /*RobotComputedFeatures NewFeature = new RobotComputedFeatures();
            string[] inputArray = input.Trim('[', ']').Split(',');
            NewFeature.FeatureHeader.FromString(string.Join(",", inputArray[0..17]));

            inputArray[17] = inputArray[17].Trim('[');
            inputArray[inputArray.Length- 1] = inputArray[inputArray.Length - 1].Trim(']');
            for (int i = 0; i < (inputArray.Count() - 17 / 51); i++)
            {
                string InsertMe = string.Join(",", inputArray[(i * 51 + 17)..((i + 1) * 51 + 17)]);
                NewFeature.FeatureManoeuvres.Add(new OperationManoeuvre(InsertMe));
            }

            _Operations.Add(NewFeature);

            sort();*/

            // fill up empty buffers -- !! not required for PC storage !!
            //int length = _SizeOfManBuffer - NewFeature.FeatureManoeuvres.Count;
            //for (int i = 0; i < length; i++)
            //{
            //    NewFeature.FeatureManoeuvres.Add(new OperationManoeuvre());
            //}

        }
        /// <summary>
        /// Sort the Operations by operation sort preference
        /// </summary>
        public void sort()
        {
            if (_sortAscending)
            {
                _Operations.FromList(_Operations.ToList().OrderBy(op => op.FeatureHeader.LocationMin.X).ToList());
                int index = 1;
                foreach (var op in _Operations.ToList().OrderBy(op => op.FeatureHeader.LocationMin.X).ToList())
                {
                    op.FeatureHeader.FeatureNum = index;
                    index++;
                }
            }
            else
            {
                _Operations.FromList(_Operations.ToList().OrderByDescending(op => op.FeatureHeader.LocationMax.X).ToList());
                int index = 1;
                foreach (var op in _Operations.ToList().OrderByDescending(op => op.FeatureHeader.LocationMax.X).ToList())
                {
                    op.FeatureHeader.FeatureNum = index;
                    index++;
                }
            }

        }
        /// <summary>
        /// Sort the Operations by operation sort preference
        /// </summary>
        /// <param name="sortAscending">Set Sort Ascending or Descending</param>
        public void sort(bool sortAscending)
        {
            bool temp;
            temp = _sortAscending;
            _sortAscending = sortAscending;
            sort();
            _sortAscending = temp;
        }
        /// <summary>
        /// Outputs string representation of RAPID structure Feature
        /// </summary>
        /// <param name="feature">Feature Number to be output</param>
        /// <returns>string representation of RAPID structure Feature</returns>
        public string ToString(int feature)
        {
            string output = "[" + _Operations[feature].FeatureHeader.ToString() + ",[" + _Operations[feature].FeatureManoeuvres[0].ToString();
            for (int i = 1; i < _Operations[feature].FeatureManoeuvres.Count; i++)
            {
                output += "," + _Operations[feature].FeatureManoeuvres[i].ToString();
            }

            // fill in empty manoeuvres for RAPID data buffer
            string newOpManString = new OperationManoeuvre().ToString();
            for (int i = 0; i < _SizeOfManBuffer - _Operations[feature].FeatureManoeuvres.Count; i++)
            {
                output += "," + newOpManString;
            }
            return output + "]]";
        }
        /// <summary>
        /// Outputs string representation of RAPID structure Feature
        /// </summary>
        /// <param name="feature">Feature Number to be output</param>
        /// <returns>string representation of RAPID structure Feature</returns>
        public string OperationManoeuvresToString(int feature)
        {
            string output = "[" + _Operations[feature - 1].FeatureManoeuvres[0].ToString();
            for (int i = 1; i < _Operations[feature - 1].FeatureManoeuvres.Count; i++)
            {
                output += "," + _Operations[feature - 1].FeatureManoeuvres[i].ToString();
            }

            // fill in empty manoeuvres for RAPID data buffer
            string newOpManString = new OperationManoeuvre().ToString();
            for (int i = 0; i < _SizeOfManBuffer - _Operations[feature - 1].FeatureManoeuvres.Count; i++)
            {
                output += "," + newOpManString;
            }
            return output + "]";
        }
        /// <summary>
        /// Outputs string representation of Chunk of RAPID structure Features to fit into a carriage of data sent to network controller
        /// </summary>
        /// <param name="feature">Feature number to start chunk</param>
        /// <param name="Carriage">Carriage number to upload</param>
        /// <returns></returns>
        public string OperationManoeuvresChunkToString(int feature, int Carriage)
        {
            string output = "[" + _Operations[feature - 1].FeatureManoeuvres[(Carriage - 1) * _SizeOfManBuffer].ToString();

            int NumOfFeatures = _Operations[feature - 1].FeatureManoeuvres.Count - (Carriage - 1) * _SizeOfManBuffer;
            if (NumOfFeatures > _SizeOfManBuffer) NumOfFeatures = _SizeOfManBuffer;
            for (int i = (Carriage - 1) * _SizeOfManBuffer + 1; i < (Carriage - 1) * _SizeOfManBuffer + NumOfFeatures; i++)
            {
                output += "," + _Operations[feature - 1].FeatureManoeuvres[i].ToString();
            }

            if (NumOfFeatures != _SizeOfManBuffer)
            {
                // fill in empty manoeuvres for RAPID data buffer
                string newOpManString = new OperationManoeuvre().ToString();
                for (int i = 0; i < _SizeOfManBuffer - NumOfFeatures; i++)
                {
                    output += "," + newOpManString;
                }
            }

            return output + "]";
        }
        /// <summary>
        /// upload feature data to network Controller
        /// </summary>
        /// <param name="feature">Feature number to upload</param>
        /// <returns>Successful upload</returns>
        public bool UploadData(int feature)
        {
            bool complete = false;

            // check if feature exists
            if (_Operations.Count < feature) return false;

            string OpHeaderStringToUpload = _Operations[feature - 1].FeatureHeader.ToString();
            string OpManStringToUpload = OperationManoeuvresToString(feature);

            OpManStringToUpload = OpManStringToUpload.Replace("0.00000000", "0")
                .Replace(".0000000", "")
                .Replace(".000000", "")
                .Replace(".00000", "")
                .Replace(".0000", "")
                .Replace(".000", "")
                .Replace(".00 ", "")
                .Replace(".00,", ",")
                .Replace(".00]", "]")
                .Replace(".0 ", " ")
                .Replace(".0,", ",")
                .Replace(".0]", "]")
                .Replace("9E+09", "9E9")
                .Replace("-0,", "0,");

            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_ParentController.controller.Rapid))
                    {
                        _HeadBufferRD.StringValue = OpHeaderStringToUpload;
                        _ManBufferRD.StringValue = OpManStringToUpload;
                    }
                }
                catch
                {
                    complete = false;
                }
                finally
                {
                    if (_HeadBufferRD.StringValue == OpHeaderStringToUpload && _ManBufferRD.StringValue == OpManStringToUpload)
                    {
                        complete = true;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// upload Chunk of features data to network Controller starting from specified feature
        /// </summary>
        /// <param name="feature">Feature to start from</param>
        /// <param name="carriage">Carriage number to upload</param>
        /// <returns></returns>
        public bool UploadData(int feature, int carriage)
        {

            // check if feature exists
            if (_Operations.Count < feature) return false;

            string OpHeaderStringToUpload = _Operations[feature - 1].FeatureHeader.ToString();
            string OpManStringToUpload = OperationManoeuvresChunkToString(feature, carriage);

            OpManStringToUpload = OpManStringToUpload.Replace("0.00000000", "0")
                .Replace(".0000000", "")
                .Replace(".000000", "")
                .Replace(".00000", "")
                .Replace(".0000", "")
                .Replace(".000", "")
                .Replace(".00 ", "")
                .Replace(".00,", ",")
                .Replace(".00]", "]")
                .Replace(".0 ", " ")
                .Replace(".0,", ",")
                .Replace(".0]", "]")
                .Replace("9E+09", "9E9")
                .Replace("-0,", "0,");

            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_ParentController.controller.Rapid))
                    {
                        _HeadBufferRD.StringValue = OpHeaderStringToUpload;
                        _ManBufferRD.StringValue = OpManStringToUpload;

                        //m.Release(); // this may not work...
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

            // should check if the information has taken to the robot last - FOR DEBUG
            string check = _ManBufferRD.StringValue;
            if (OpManStringToUpload.ToLower() != check.ToLower()) // TODO: error if word not taken
            {
                bool damn = true;
            }
            return true;
        }
        /// <summary>
        /// If Operation waiting for start, this function will allow Robot to proceed
        /// </summary>
        public void AllowOperationStart(object sender = null, EventArgs args = null)
        {
            if (_RobotWaiting)
            {
                if (_OperationWaitingForStart.StringValue == "FALSE") Debug.Print("Robot WaitingForStart register indicates robot isn't waiting: ATTEMPT VOID");
                bool complete = false;

                while (!complete)
                {
                    try
                    {
                        using (Mastership m = Mastership.Request(_ParentController.controller))
                        {
                            _OperationWaitingForStart.StringValue = "FALSE";
                        }
                    }
                    catch
                    {
                        Debug.Print("mastership failed while attempting to update control register");
                        complete = false;
                    }
                    finally
                    {
                        Debug.Print("successfully set Robot WaitingForStart register to continue with execution");
                        complete = true;
                    }
                }
            }
        }
        

        /* this shouldnt work so easily... (basically)
        public void DownloadData()
        {
            string input = "[" + _HeadBufferRD.StringValue + "," + _ManBufferRD + "]";

            FromString(input);
        }
        */


        // --- INTERNAL EVENTS AND AUTOMATION ---
        /// <summary>
        /// checks if new feature has been selected and connects feature ability (event) to connect to robot start
        /// checks if new feature has been uploaded to robot and invokes event if the case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnRobSystemDataUpdate(object sender = null, EventArgs args = null)
        {
            bool NewCurrentFeature = false;
            bool NewuploadedFeature = false;
            string[] stringVals = _RobSystemData.StringValue.Trim('[', ']').Split(',');

            int CurrentOpCheck = int.Parse(stringVals[11]);
            while (Operation.Current.FeatureHeader.FeatureNum != CurrentOpCheck)
            {
                NewCurrentFeature = true;
                Operation.MoveNext();
            }

            // TODO: update processed Operations?
            int ProcessedOps = int.Parse(stringVals[4]);

            while (!Operation.Feature(ProcessedOps).UploadedToRobot)
            {
                NewuploadedFeature = true;
                Operation.Feature(ProcessedOps).UploadedToRobot = true;
                ProcessedOps--;
                if (ProcessedOps == 0) break;
            }

            if (NewCurrentFeature)
            {
                NewCurrentOperation?.Invoke(this, new EventArgs());
                Operation.Current.FeatureRequestRobotContinue += AllowOperationStart;
            }
            if (NewuploadedFeature) NewUploadedOperation?.Invoke(this, new EventArgs());
        }
        protected virtual void OnOperationStartedChange(object sender = null, EventArgs args = null)
        {
            int FeatureNum = int.Parse(_OperationStarted.StringValue);
            Operation.Feature(FeatureNum).InProgress = true;

            OperationStarted?.Invoke(Operation.Feature(FeatureNum), new EventArgs());
        }
        protected virtual void OnOperationCompletedChange(object sender = null, EventArgs args = null)
        {
            int FeatureNum = int.Parse(_OperationCompleted.StringValue);
            Operation.Feature(FeatureNum).InProgress = false;
            Operation.Feature(FeatureNum).CompletedByRobot = true;

            OperationCompleted?.Invoke(Operation.Feature(FeatureNum), new EventArgs());
        }
        /// <summary>
        /// Event triggered when robot changes register for start sequence
        /// sets feature to waiting for start (if feature started on pc, feature will automatically begin process)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnOperationWaitingForStartChange(object sender = null, EventArgs args = null)
        {
            int FeatureNum = int.Parse(_RobSystemData.StringValue.Trim('[', ']').Split(',')[11]);
            bool startReq = bool.Parse(_OperationWaitingForStart.StringValue);

            if (startReq)
            {
                Operation.Feature(FeatureNum).WaitingForStart = true;
                OperationWaitingForStart?.Invoke(Operation.Feature(FeatureNum), new EventArgs());
            }
        }
        protected virtual void SetJobInProgress(bool connected)
        {
            _ParentController.dataModel.Robot_Control.JobInProgress = connected;
        }

    }
}
