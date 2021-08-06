﻿using ReplaceRSConnection.Robotics.ToolInfo;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using RFRCC_RobotController.Controller.DataModel.RAPID_Data;
using RFRCC_RobotController.Controller.Importers;
using System;
using System.Linq;
using System.Text;

namespace RFRCC_RobotController.Controller.DataModel
{
    /* TODO: list for operation models
     *      X add 'job header' to encapsulate job information
     *      o RE - add method to input data obtained from import process
     *      o add template of job execution (including PLC requirements and such)
     *      o 
     */
    /// <summary>
    /// Model of job operation file containing all processes and information on current job processing
    /// </summary>
    public class JobModel
    {
        // --- INTERNAL ---
        internal RobotController _parentController;
        internal RAPID_OperationBuffer OperationRobotMoveData { get; set; } = new RAPID_OperationBuffer();  // previously 'OperationBuffer'
        private bool _controllerPresent = false;
        private string _filepath;
        private string _filename;
        private int _NumFeatures;
        private bool _aborted = false;
        private bool _CurrentJob = false;
        

        // --- EVENTS ---
        public event EventHandler JobCompleted;
        public event EventHandler IMStopRequest;
        public event EventHandler JobParsed;
        public event EventHandler ActionCompleted;
        public event EventHandler ActionPaused;

        // --- PARAMETERS ---
        public IOperationAction CurrentAction
        {
            get
            {
                return operationActions.Current;
            }
        }
        public bool CurrentJob
        {
            get
            {
                return _CurrentJob;
            }
            set
            {
                _CurrentJob = value;
            }
        }
        /// <summary>
        /// Handles Status of the job
        /// </summary>
        public JobStatus Status { get; set; }
        /// <summary>
        /// Settings used during Job Processing through the machine
        /// </summary>
        public JobProcessSettings ProcessSettings;
        /// <summary>
        /// Template used om populating Job Action Items
        /// </summary>
        public JobModelTemplate Template { get; set; } = new DefaultJobTemplate();
        /// <summary>
        /// Name of job - generated by parse process unless named otherwise
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of Actions to be taken in processing the job
        /// </summary>
        public OperationActionList operationActions { get; set; }
        // TODO: UID generation for Job
        /// <summary>
        /// Job Header data
        /// </summary>
        public JobHeader HeaderInfo { get; set; } = new JobHeader();
        /// <summary>
        /// Cutting information for job
        /// </summary>
        public ToolData ToolData { get; set; } = new ToolData();

        // --- CONSTRUCTORS ---
        // TODO: Make a disposal method
        /// <summary>
        /// Initialised object with job process to follow
        /// if job process template not provided, default process will be used
        /// </summary>
        /// <param name="template">job process to follow</param>
        public JobModel(JobModelTemplate template = null)
        {
            if (template != null)
            {
                Template = template;
                Name = template.Name;
            }
            Status = new JobStatus(this);
            ProcessSettings = new JobProcessSettings(this);
            operationActions = new OperationActionList();
            ProcessSettings.StockDXPrecisionToleranceChanged += operationActions.OnProcessSettingsStockDXPrecisionsToleranceChange;
            operationActions.OperationActionRequestPause += OnOperationActionRequestPause;
            operationActions.OperationsAllComplete += OnJobCompleted;
            operationActions.OperationActionCompleted += OnOperationCompleted;
            operationActions.RobotProcessRequired += OnRobotProcessRequired;
            OperationRobotMoveData.OperationWaitingForStart += OnOperationManoeuvreWaitingForStart;
        }
        /// <summary>
        /// Initialise object with specified network controller, Job index on relevant network contoller, and template for job initialisation
        /// default to last job index and default job process
        /// </summary>
        /// <param name="ParentController">Network controller Job is associated with</param>
        /// <param name="index">index of job on Network controller list</param>
        /// <param name="template">Template of job process to be used</param>
        public JobModel(RobotController ParentController, JobModelTemplate template = null) : this(template)
        {
            _parentController = ParentController;
            _controllerPresent = true;
            ConnectAllJobEvents();
            OnControllerConnectionChange(); // run first time to check if can connect
        }

        // --- METHODS ---
        /// <summary>
        /// Job starts processing and autoprogressing procession actions
        /// </summary>
        /// <returns>if successfully started</returns>
        public bool Start()
        {
            if (!_CurrentJob) throw new Exception("This Job is not Current Job, therefore cannot be started");
            // check current status of job
            switch (Status.Progress)
            {
                case JobProgress.JobProcessing:
                    _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Robot already processing Job, if not correct Job, please cancel to proceed"));
                    return true;
                    break;
                case JobProgress.WaitingForFile:
                    throw new Exception("Robot waiting for file upload");
                    break;
                case JobProgress.WaitingToParse:
                    throw new Exception("Robot waiting to parse file");
                    break;
                case JobProgress.WaitingToPopulateJobData:
                    throw new Exception("Robot waiting to populate job data from parsed file");
                    break;
                case JobProgress.WaitingForRobotConnection:
                    throw new Exception("Robot Controller not connected");
                    break;
                case JobProgress.JobAborting:
                    throw new Exception("Robot Aborting job");
                    break;
                case JobProgress.JobCancelled:
                    throw new Exception("Job has been cancelled");
                    break;
                case JobProgress.JobFinished:
                    throw new Exception("Job completed");
                    break;
                default:
                    break;
            }

            // TODO: check process for getting data onto robot
            Status.Started();
            if (!_parentController.SetProgramPointerAndStartRobotTask()) new Exception("Robot failed to start");
            operationActions.Current.Start();
            return true;
        }
        // TODO: COmplete pause process
        /// <summary>
        /// Pauses current Job
        /// </summary>
        public void Pause()
        {
            operationActions.Current.Pause();
            Status.Paused();
            ActionPaused?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Un-Pauses current Job
        /// </summary>
        public void Continue()
        {
            // make error progress state
            if (Status.Progress != JobProgress.JobPaused) throw new Exception("Job not in correct state for continuation");
            // if action is a skip, skip it.
            if (CurrentAction.Skip) operationActions.MoveNext();
            // if action is not processing, start processing
            if (!CurrentAction.Processing)
            {
                CurrentAction.Start();
            }
            // if action requires continuation, do continuation to it
        }
        // TODO: Implement Abort process
        /// <summary>
        /// NOT YET IMPLEMENTED
        /// </summary>
        public void Abort()
        {
            if (_parentController.dataModel.ProcessSettings.ForcePauseBeforeAbort && Status.Progress != JobProgress.JobPaused) throw new Exception("Process Setting do not allow machine to abort without pause");
            _aborted = true;
            Status.Aborting();
            operationActions.Current.Complete(false);
            OnOperationAbort();
            // TODO: setup new Actions based on stock exit criteria 
        }
        /// <summary>
        /// Connects to a controller and insert Job in list of jobs to be completed
        /// </summary>
        /// <param name="ParentController">Controller to be connected to</param>
        /// <param name="index">Job index in list to be completed if known</param>
        /// <returns>Connection successfully established with network controller</returns>
        public bool ConnectParentController(RobotController ParentController, int index = -1)
        {
            if (_controllerPresent)
            {
                // TODO: check if need to do extra steps to change pointer
                _parentController = ParentController;
                // TODO: check connection active and associate this file with controller
            }
            else
            {
                _parentController = ParentController;
                _controllerPresent = true;
                // TODO: check connection active and associate this file with controller
            }

            if (index == -1) _parentController.dataModel.Jobs.Add(this);
            else _parentController.dataModel.Jobs.Insert(index, this);
            ConnectAllJobEvents();
            return true; // return false if failed to connect
        }
        // TODO: discriptive summary comment
        public bool ConnectAllJobEvents()
        {
            if (_controllerPresent)
            {
                IMStopRequest += _parentController.RobotProcess.ImmediateStop;
                _parentController.stream.ControllerConnectedChange += OnControllerConnectionChange;
                return true;
            }
            else
            {
                throw new Exception("No Controller present to connect all events to");
            }
        }
        // TODO: discriptive summary comment
        public bool DisconnectAllJobEvents()
        {
            if (_controllerPresent)
            {
                IMStopRequest -= _parentController.RobotProcess.ImmediateStop;
                _parentController.stream.ControllerConnectedChange -= OnControllerConnectionChange;
                return true;
            }
            else
            {
                throw new Exception("No Controller present to disconnect all events from");
            }
        }
        // TODO: discriptive summary comment
        public bool ConnectToController()
        {
            OperationRobotMoveData.ConnectParentController(_parentController, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
            OperationRobotMoveData.CurrentJob = true;
            return true;
        }
        // TODO: discriptive summary comment
        public void DisconnectFromController()
        {
            OperationRobotMoveData.DisconnectFromController(this, new ControllerConnectionEventArgs());
        }
        /// <summary>
        /// imports ASCII contents of DSTV file ready for parseing and jobdata population
        /// </summary>
        /// <param name="filename">Name of file or job</param>
        /// <param name="ASCII_Content">ASCII contents of DSTV file</param>
        /// <param name="Parse">If information is to be parsed immediately</param>
        /// <returns></returns>
        internal bool LoadJobFromASCII(string filename, string ASCII_Content , bool Parse)
        {
            _filepath = "Not Provided";
            _filename = filename;

            if (Parse)
            {
                FileImporter Parser = new FileImporter() { Job = this, FilePath = _filepath, FileName = _filename };
                Status.FileImported();

                if (!Parser.Parse())
                {
                    throw new NotImplementedException(); // TODO: turn this into an error exception
                    // or
                    return false;
                }
                // update infomation from Parser

                JobParsed?.Invoke(this, new EventArgs());
                return true;
            }
            else
            {
                JobParsed?.Invoke(this, new EventArgs());
                return true;
            }
        }
        /// <summary>
        /// Load and parse data from file path in .nc1 format
        /// </summary>
        /// <param name="filePath">full filepath in storage</param>
        /// <param name="Parse">if parse on load</param>
        /// <returns>Job Data successfully retrieved from filepath [and parsed]</returns>
        public bool LoadJobFromFile(string filePath, bool Parse)
        {
            _filepath = filePath;
            _filename = filePath.Split('\\').Last();

            if (Parse)
            {
                FileImporter Parser = new FileImporter() { Job = this, FilePath = _filepath, FileName = _filename };
                Status.FileImported();

                if (!Parser.Parse())
                {
                    throw new Exception("Parser failed to parse file"); // TODO: turn this into an error exception
                    // or
                    return false;
                }

                // update infomation from Parser

                JobParsed?.Invoke(this, new EventArgs());
                return true;
            }
            else
            {
                JobParsed?.Invoke(this, new EventArgs());
                return true;
            }
        }
        /// <summary>
        /// NOT YET IMPLEMENTED
        /// For loading job directly from a compatible job parsing class
        /// </summary>
        /// <param name="Parser"></param>
        /// <returns></returns>
        public bool LoadJobFromParser(FileImporter Parser)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Generate Operation Actions from template job
        /// </summary>
        public void GenerateOpActionsFromRobManoeuvres() 
        {
            this.Template.GenerateOpActionsFromRobManoeuvres(this);

            _NumFeatures = OperationRobotMoveData.Operation.Count;
            Status.JobDataPopulated(); 
        }

        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnJobCompleted(object sender, EventArgs args)
        {
            Status.JobEnd(_aborted);
            JobCompleted?.Invoke(this, args);
        }
        /// <summary>
        /// This is where pause and auto progress will be completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnOperationCompleted(object sender = null, EventArgs args = null)
        {
            // OperationActionList Automatically cycles to next operation and commences if autoprogress active
            if (ProcessSettings.MachineSettings.AutoProgressJob && !operationActions.Current.PauseOn)
            {
                operationActions.Current.Start();
            }
            ActionCompleted?.Invoke(this, new EventArgs());
        }
        protected virtual void OnOperationActionRequestPause(object sender = null, EventArgs args = null)
        {
            Pause();
            if (_parentController.dataModel.ProcessSettings.RobotIMStopOnPause)
            {
                if (IMStopRequest == null) throw new Exception("No subscribers to IM Stop request");
                IMStopRequest?.Invoke(sender, args);
            }
        }
        protected virtual void OnOperationAbort(object sender = null, EventArgs args = null)
        {
            // Immediately stop robot process
            if (IMStopRequest == null) throw new Exception("No subscribers to IM Stop request");
            IMStopRequest?.Invoke(sender, args);


        }
        protected virtual void OnControllerConnectionChange(object sender = null, EventArgs args = null)
        {
            if (_parentController.ControllerConnected && CurrentJob)
            {
                ConnectToController();
                Status.RobotConnected();
            }
            else
            {
                DisconnectAllJobEvents();
            }
        }
        protected virtual void OnRobotProcessRequired(object sender = null, EventArgs args = null)
        {
            if (CurrentAction is OperationRobotProcess)
            {
                ((OperationRobotProcess)CurrentAction).CallForRobotProcess += _parentController.RobotProcess.CallForProcessHandler;
            }
            else if (CurrentAction is OperationRobotManoeuvre)
            {
                ((OperationRobotManoeuvre)CurrentAction).CallForRobotProcess += _parentController.RobotProcess.CallForProcessHandler;
            }
        }
        protected virtual bool OnOperationManoeuvreWaitingForStart(IOperationAction sender, EventArgs args)
        {
            _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs(string.Format("OperationRobotMoveData.StartPermission = {0}", (sender == CurrentAction) && ((OperationRobotManoeuvre)CurrentAction).featureData.StartWhenReady)));
         
            if (sender != null && sender.Skip)
            {
                // TODO: handle skip
            }
            return OperationRobotMoveData.StartPermission = (sender == CurrentAction) && ((OperationRobotManoeuvre)CurrentAction).featureData.StartWhenReady;
        }

    }

    public delegate bool ActionRequestEventHandler(IOperationAction sender, EventArgs args); 

}
