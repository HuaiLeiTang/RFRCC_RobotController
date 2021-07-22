using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;
using RapidBool = ABB.Robotics.Controllers.RapidDomain.Bool;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Messaging;
using ABB.Robotics.Controllers.RapidDomain;
using ABB.Robotics.Controllers.Discovery;
using System.Threading;
using System.Diagnostics;
using RFRCC_RobotController.Controller;
using RFRCC_RobotController.ABB_Data;
using RFRCC_RobotController.Controller.DataModel.RAPID_Data;
using RFRCC_RobotController.Controller.DataModel;
using System.Data.Entity.Core.Metadata.Edm;

namespace RFRCC_RobotController
{
    // TODO: Robot Controller class requirements:
    /*      - 
     * 
     * 
     */


    /// <summary>
    /// Robot controller Class for control of Robofab Australia Coping Cell
    /// </summary>
    public class RobotController
    {
        // --- INTERNAL FIELDS ---
        internal ABB.Robotics.Controllers.Controller controller = null;
        internal NetworkControllerInfo _controllerInfo;
        internal ABB.Robotics.Controllers.RapidDomain.Task tRob1;
        internal bool _ControllerConnected = false;
        private bool _ControllerTaskRunning;



        /// <summary>
        /// messaging function from controller to outside
        /// i.e. error / status messaging / etc
        /// </summary>
        public event EventHandler<StatusMesssageEventArgs> OnStatusMesssageChange;
        /// <summary>
        /// Event Arguments, holds status message update
        /// </summary>
        public class StatusMesssageEventArgs : EventArgs
        {
            public StatusMesssageEventArgs(string statusMesssage)
            {
                StatusMesssage = statusMesssage;
            }

            public string StatusMesssage { get; set; }
        }
        /// <summary>
        /// Internal OnStatusMessageChange event trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void StatusMesssageChange(object sender, StatusMesssageEventArgs e)
        {
            EventHandler<StatusMesssageEventArgs> handler = OnStatusMesssageChange;
            if (handler != null)
                handler(sender, e);
        }
        /// <summary>
        /// Returns ABB referenced controller
        /// </summary>
        ABB.Robotics.Controllers.Controller Controller
        {
            get
            {
                return controller;
            }
        }

        // --- EVENTS ---
        public event EventHandler RobotControllerRunningStatusChanged;
        /// <summary>
        /// Event triggered when Controller connection secured
        /// </summary>
        public event EventHandler<ControllerConnectionEventArgs> ControllerConnectionChange;
        public event EventHandler RobotOperatingModeChanged;
        public event EventHandler RobotStateChanged;
        public event EventHandler<ProgramStartEventArgs> RobotProgramStartResults;
        public event EventHandler<string> RobotRequiresOperatorIntervention; 


        // --- PROPERTIES ---
        /// <summary>
        /// Network and stream related methods and parameters
        /// </summary>
        public Stream stream;
        /// <summary>
        /// associated network controller memory and data relations, along with job and funcationality specific methods and parameters
        /// </summary>
        public DataModel dataModel;
        /// <summary>
        /// Controller information of the connected controller
        /// </summary>
        public NetworkControllerInfo ControllerInfo 
        {
            get => _controllerInfo;
        }
        public RobotProcesses RobotProcess;
        /// <summary>
        /// If controller object is associated with a network controller, and connected to
        /// </summary>
        public bool ControllerConnected
        {
            get
            {
                return _ControllerConnected;
            }
        }
        /// <summary>
        /// Flag for collecting data from robot
        /// </summary>
        internal bool FetchedData { get; set; } = false;
        /// <summary>
        /// Status of Task (program) running on Robot Controller, when connected and job loaded, this should be in running.
        /// If robot is not connected will return Unknown
        /// </summary>
        public RobotStatus TaskStatus
        {
            get 
            {
                if (controller != null) return (RobotStatus)tRob1.ExecutionStatus;
                else return RobotStatus.Unknown;
            }
        }
        /// <summary>
        /// Operating mode of the controller, this should be in auto, and can only be manually changed by the operator
        /// If robot is not connected will return NotApplicable
        /// </summary>
        public RobotControllerOperatingMode ControllerOperatingMode
        {
            get 
            {
                if (controller != null) return (RobotControllerOperatingMode)controller.OperatingMode;
                else return RobotControllerOperatingMode.NotApplicable;
            }
        }
        /// <summary>
        /// returns the control state of the robot controller, should return Motors On.
        /// If robot is not connected will return Unknown
        /// </summary>
        public RobotControllerState ControllerState
        {
            get
            {
                if (controller != null) return (RobotControllerState)controller.State;
                else return RobotControllerState.Unknown;
            }
        }
        public bool RobotProgramEnabled
        {
            get
            {
                if (controller != null) return tRob1.Enabled;
                else return false;
            }
        }

        // --- CONSTRUCTORS ---
        /// <summary>
        /// Robot controller Class for control of Robofab Australia Coping Cell
        /// </summary>
        public RobotController()
        {
            stream = new Stream(this);
            dataModel = new DataModel(this);
            RobotProcess = new RobotProcesses(this);
        }


        // --- METHODS ---
        
        /// <summary>
        /// For Communication to Robot that message is being parsed and actioned
        /// Soon to be outdated and unsupported from Controller Ver 2.0.0
        /// </summary>
        internal void MessageRecieved()
        {
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        dataModel.SQLMessageRecieve.Value = Bool.Parse("TRUE");
                    }
                    StatusMesssageChange(this, new StatusMesssageEventArgs("PC acknowledged message"));
                    Debug.WriteLine("Acknoledged Controller Message");
                }
                catch
                {
                    StatusMesssageChange(this, new StatusMesssageEventArgs("Internal Error: failed to get mastership while acknowleging reciept of message"));
                    complete = false;
                }
                finally
                {
                    complete = true;
                }
            }
        }
        /// <summary>
        /// To react to Network Controller messages, triggering related events 
        /// to be outdated and unsupported from Controller Ver 2.0.0
        /// </summary>
        /// <param name="MessageString"></param>
        /// <returns></returns>
        internal string ParseMessage(string MessageString)
        {
            if (MessageString.Length < 8)
            {
                PCSDK_Work_Complete();
                return MessageString + "ERROR : Message Not Recognised : \"" + MessageString + "\"";
            }
            MessageString = MessageString.Split("\"")[0];
            string MessageHeader = MessageString.Substring(0, 8); // breaks if less thab 8....
            switch (MessageHeader)
            {
                case "FEATBUFF":
                    string FeatBuff = MessageString.Split("<Feature>")[1].Split("</>")[0];
                    string Carriage = MessageString.Split("<Carriage>")[1].Split("</>")[0];
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to upload feature: " + FeatBuff));
                    dataModel.UpdateRobot("manoeuvre", int.Parse(FeatBuff), int.Parse(Carriage));
                    PCSDK_Work_Complete();
                    return "Updated Robot Manoruvre Buffer with Feature " + FeatBuff;

                case "FEATURE0":
                    throw new NotSupportedException();
                    string Feat = MessageString.Split("<Feature>")[1];
                    Feat = Feat.Split("</>")[0];
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to upload feature: " + Feat));
                    dataModel.UpdateRobot("feature", int.Parse(Feat));
                    PCSDK_Work_Complete();
                    return "Updated Robot Feature Register with Feature " + Feat;

                case "UPD_FEAT":
                    string[] FeatAndXOpt = MessageString.Split("</>");
                    if (FeatAndXOpt.Length != 3)
                    {
                        PCSDK_Work_Complete();
                        return "ERROR: UPD_FEAT TOO MANY VARIABLES";
                    }
                    FeatAndXOpt[0] = FeatAndXOpt[0].Split("UPD_FEAT <Feature>")[1];
                    FeatAndXOpt[1] = FeatAndXOpt[1].Split(" <X_Optimal>")[1];
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to update opt_x for feature: " + FeatAndXOpt[0]));

                    //OnUpdateFeatureOptimalX(this, new UpdateFeatureOptimalXEventArgs(Robot_Control.JobID, int.Parse(FeatAndXOpt[0]), decimal.Parse(FeatAndXOpt[1])));
                    dataModel.CurrentJob.OperationRobotMoveData.Operation[int.Parse(FeatAndXOpt[0]) - 1].FeatureHeader.IdealXDisplacement = double.Parse(FeatAndXOpt[1]);

                    PCSDK_Work_Complete();
                    return "Updated Feature " + FeatAndXOpt[0] + " X_Optimal to " + FeatAndXOpt[1];

                case "HEADER00":
                    throw new NotSupportedException();
                    string jobIDFromString = MessageString.Split("<JobID>")[1];
                    jobIDFromString = jobIDFromString.Split("</>")[0];
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to upload header for JobID: " + jobIDFromString));
                    if (FetchedData || (dataModel.HeaderData.JobID != jobIDFromString))
                    {
                        dataModel.UpdateRCC(jobIDFromString);
                        dataModel.UpdateRobot("header");
                        FetchedData = false; // reset fetched flag.
                        PCSDK_Work_Complete();
                        return "JobID didnt match: Updated Header from SQL & Updated Robot Header Register with JobID: " + jobIDFromString;
                    }
                    else
                    {
                        dataModel.UpdateRobot("header");
                        PCSDK_Work_Complete();
                        return "Updated Robot Header Register with JobID: " + jobIDFromString;
                    }
                case "JOBHEADR":
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to upload Job Header"));

                    dataModel.jobHeader.FeatureQuant = dataModel.CurrentJob.OperationRobotMoveData.Operation.Count;
                    dataModel.jobHeader.Upload();
                    PCSDK_Work_Complete();
                    return "Updated Robot Job Header Register";
                case "FRC_UPDT":
                    throw new NotSupportedException();
                    string jobIDUpdate = MessageString.Split("</>")[0];
                    jobIDUpdate = jobIDUpdate.Split("FRC_UPDT <JobID>")[0];
                    FetchedData = true;
                    dataModel.UpdateRCC(jobIDUpdate);
                    PCSDK_Work_Complete();
                    return "Updated SQL Server Integration Memory with JobID: " + jobIDUpdate;

                default:
                    // TODO error handle
                    return "Message Not Recognised";
            }


        }
        /// <summary>
        /// Internal function to update network controller that this object has completed parsed request
        /// </summary>
        private void PCSDK_Work_Complete()
        {
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        dataModel.PCSDK_Complete.Value = Bool.Parse("TRUE");
                    }
                }
                catch
                {
                    StatusMesssageChange(this, new StatusMesssageEventArgs("mastership failed while attempting to acknowledge process complete"));
                    complete = false;
                }
                finally
                {
                    complete = true;
                }
            }
        }
        internal bool SetProgramPointerAndStartRobotTask()
        {
            bool complete = false;
            int tries = 0;
            StartResult? startResult = null;

            if (!RobotProgramEnabled) throw new Exception("Robot not able to start");
            if (Controller.State != ABB.Robotics.Controllers.ControllerState.MotorsOn) Controller.State = ABB.Robotics.Controllers.ControllerState.MotorsOn;
            while (!complete && tRob1.ExecutionStatus != TaskExecutionStatus.Running)
            {
                if (tries > 99) return false;
                try
                {
                    using (Mastership m = Mastership.Request(controller))
                    {
                        tRob1.ResetProgramPointer();
                        startResult = tRob1.Start();
                    }
                }
                catch
                {
                    complete = false;
                    tries++;
                }
                finally
                {
                    complete = true;
                    RobotControllerRunningStatusChanged?.Invoke(this, new EventArgs());
                }
            }

            if (startResult == null)
            {
                RobotProgramStartResults?.Invoke(this, new ProgramStartEventArgs(RobotProgramStartResult.FailedToAttempt));
            }
            else RobotProgramStartResults?.Invoke(this, new ProgramStartEventArgs((RobotProgramStartResult)startResult));
            return true;
        }
        internal bool StopRobotTask()
        {
            bool complete = false;
            int tries = 0;

            if (!controller.IsMaster)
            {
                OnRobotRequiresOperatorIntervention("Another user has mastery of the controller, check flexpendant and retry resetting current task / program ");
                return false;
            }

            while (!complete && tRob1.ExecutionStatus != TaskExecutionStatus.Running)
            {
                if (tries > 99) return false;
                try
                {
                    using (Mastership m = Mastership.Request(controller))
                    {
                        tRob1.Stop(StopMode.Immediate);
                    }
                }
                catch
                {
                    complete = false;
                    tries++;
                }
                finally
                {
                    complete = true;
                    RobotControllerRunningStatusChanged?.Invoke(this, new EventArgs());
                }
            }
            return complete;
        }


        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnRobotOperatingModeChanged(object sender = null, EventArgs args = null)
        {
            StatusMesssageChange(this, new StatusMesssageEventArgs(controller.OperatingMode.ToString()));
            RobotOperatingModeChanged?.Invoke(this, new EventArgs());
        }
        protected virtual void OnRobotStateChanged(object sender = null, EventArgs args = null)
        {
            StatusMesssageChange(this, new StatusMesssageEventArgs(controller.State.ToString()));
            RobotStateChanged?.Invoke(this, new EventArgs());
        }
        internal void ConnectedToController()
        {
            _ControllerConnected = true;
            OnControllerConnectionChange();
        }
        internal void DisconnectedFromController()
        {
            _ControllerConnected = false;
            OnControllerConnectionChange();
        }
        protected virtual void OnControllerConnectionChange(object sender = null, EventArgs args = null)
        {
            if (controller != null)
            {
                StatusMesssageChange(this, new StatusMesssageEventArgs("Connected to controller"));
                controller.OperatingModeChanged += OnRobotOperatingModeChanged;
                controller.StateChanged += OnRobotStateChanged;
                // start running task
                if (!_ControllerTaskRunning && tRob1.ExecutionStatus != ABB.Robotics.Controllers.RapidDomain.TaskExecutionStatus.Running) _ControllerTaskRunning = SetProgramPointerAndStartRobotTask();
                else if (StopRobotTask()) _ControllerTaskRunning = SetProgramPointerAndStartRobotTask();

                if (!_ControllerTaskRunning) new Exception("failed to start robot automatically, user intervention required");
            }
            else
            {
                StatusMesssageChange(this, new StatusMesssageEventArgs("Connection to controller LOST"));
            }
            ControllerConnectionChange?.Invoke(this, new ControllerConnectionEventArgs(controller, tRob1));
        }
        protected virtual void OnRobotRequiresOperatorIntervention(string message)
        {
            RobotRequiresOperatorIntervention?.Invoke(this, message);
        }


        // ------------------------------------------------------------------------------------------------


        /// <summary>
        /// Controller Connection Event Invoke
        /// </summary>

        /// <summary>
        /// Delegte for when Control Memory struct Updated on network controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ControlValueUpdateEventHandler(RobotController sender, ControlStrucEventArgs e);
        /// <summary>
        /// Event triggered when Control Memory struct Updated on network controller
        /// </summary>
        public event ControlValueUpdateEventHandler ControlValueUpdate;
        /// <summary>
        /// Event for firing when Controller connection secured
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal protected virtual void OnControlValueUpdate(object sender, ControlStrucEventArgs e)
        {
            Debug.WriteLine("OnControlValueUpdate Recieved instruction");
            ControlValueUpdate?.Invoke(this, e);
        }
        /// <summary>
        /// Event for firing when the controller changes the PC_Message value in the control structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RobotPC_MessageChanged(object sender, ControlStrucEventArgs e)
        {
            Debug.WriteLine("RobotPC_MessageChanged Recieved instruction");

            if (e.ValueName != "<CLEAR>" && e.ValueName != "")
            {
                MessageRecieved();
                StatusMesssageChange(this, new StatusMesssageEventArgs("RAPID Data Change: PC Message"));
                StatusMesssageChange(this, new StatusMesssageEventArgs(ParseMessage(e.ValueName)));
            }
        }
        /// <summary>
        /// Custom arguments for feature optimal X updates delivered by network controller to PC
        /// used in updating memory structs in robot control library
        /// </summary>
        public class UpdateFeatureOptimalXEventArgs : EventArgs
        {
            public UpdateFeatureOptimalXEventArgs(string JobID, int FeatureNum, decimal OptimalX)
            {
                FeatureOptimalX_JobID = JobID;
                FeatureOptimalX_FeatureNum = FeatureNum;
                FeatureOptimalX_OptimalX = OptimalX;
            }

            public string FeatureOptimalX_JobID { get; set; }
            public int FeatureOptimalX_FeatureNum { get; set; }
            public decimal FeatureOptimalX_OptimalX { get; set; }
        }
        /// <summary>
        /// Custom Delegate for UpdateFeatureOptimalX so that the relevant network controller may be updated pased to event
        /// </summary>
        /// <param name="sender">relevant network controller to event</param>
        /// <param name="e"></param>
        /// <returns></returns>
        public delegate bool UpdateFeatureEventHandler(RobotController sender, UpdateFeatureOptimalXEventArgs e);
        /// <summary>
        /// Event triggered when feature optimal X is updated on the network controller
        /// </summary>
        public event UpdateFeatureEventHandler UpdateFeatureOptimalX;
        /// <summary>
        /// Triggering method to call event when network controller has updated optimal x for a feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        internal protected virtual bool OnUpdateFeatureOptimalX(object sender, UpdateFeatureOptimalXEventArgs e)
        {
            dataModel.FeatureDataList[e.FeatureOptimalX_FeatureNum].Dim1Optimal = (float)e.FeatureOptimalX_OptimalX;
            return (UpdateFeatureOptimalX != null) ? UpdateFeatureOptimalX(this, e)
                : throw new ArgumentException("No listeners logged to handle UpdateJobData event");
        }
        /// <summary>
        /// custom event args for network controller request for updated job data
        /// </summary>
        public class RequestUpdatedJobDataEventArgs : EventArgs
        {
            public RequestUpdatedJobDataEventArgs(string JobID, bool completeFlag)
            {
                FeatureOptimalX_JobID = JobID;
                completed = completeFlag;
            }
            public bool completed { get; set; }
            public string FeatureOptimalX_JobID { get; set; }
        }
        /// <summary>
        /// Custom delegate for event where network controller requestes updated job data, allowing check of successful completion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>if successfully updated network controller with required job data</returns>
        public delegate bool RequestUpdatedJobDataEventHandler(RobotController sender, RequestUpdatedJobDataEventArgs e);
        /// <summary>
        /// Event triggered when network controller requests updated Jobdata
        /// </summary>
        public event RequestUpdatedJobDataEventHandler RequestUpdatedJobData;
        /// <summary>
        /// triggering method to fire RequestUpdatedJobData event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>successfully updated relevant network controller with relevant jobdata</returns>
        internal protected virtual bool OnRequestUpdatedJobData(object sender, RequestUpdatedJobDataEventArgs e)
        {
            if (RequestUpdatedJobData != null)
            {
                bool completed = RequestUpdatedJobData(this, e);
                return completed;
            }
            else
            {
                throw new ArgumentException("No listeners logged to handle UpdateJobData event");
            }
        }
        /// <summary>
        /// Custom Delegate for event where network controller requests updated manoeuvre information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>If network controller has been successfully updated with requested manoeuvre information</returns>
        public delegate bool OnManoeuvreUpdateEventHandler(RobotController sender, ManoeuvreUpdateEventArgs e);
        /// <summary>
        /// Event triggered when network controller requests manoeuvre information update
        /// </summary>
        public event OnManoeuvreUpdateEventHandler OnManoeuvreUpdate;
        /// <summary>
        /// custom arguements for OnManoeuvreUpdate event
        /// </summary>
        public class ManoeuvreUpdateEventArgs : EventArgs
        {
            public int ManoeuvreNum { get; set; }
            public RAPID_OperationBuffer OperationBuffer { get; set; }
            public bool Complete { get; set; } = false;
            public int Carriage { get; set; }

            public ManoeuvreUpdateEventArgs(int manoeuvreNum, int carriage, RAPID_OperationBuffer operationBuffer)
            {
                ManoeuvreNum = manoeuvreNum;
                Carriage = carriage;
                OperationBuffer = operationBuffer;
            }
        }
        /// <summary>
        /// triggering method for OnManoeuvreUpdate event, when network contorller requests updated manoeuvre information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>successfully updated network controller with requested manoeuvre information</returns>
        internal protected virtual bool ManoeuvreUpdate(RobotController sender, ManoeuvreUpdateEventArgs e)
        {
            dataModel.jobHeader.FeatureQuant = dataModel.CurrentJob.OperationRobotMoveData.Operation.Count;
            bool ReturnVal = dataModel.CurrentJob.OperationRobotMoveData.UploadData(e.ManoeuvreNum, e.Carriage);

            if (OnManoeuvreUpdate != null)
            {
                return OnManoeuvreUpdate(sender, e);
            }
            return true;
        }

        public class ProgramStartEventArgs : EventArgs
        {
            public RobotProgramStartResult Result;

            public ProgramStartEventArgs(RobotProgramStartResult result)
            {
                Result = result;
            }
        }


    }

    public enum RobotStatus
    {
        Ready = 0,
        Stopping = 1,
        Running = 2,
        Uninitiated = 3,
        Unknown = 4
    }

    public enum RobotControllerOperatingMode
    {
        Auto = 0,
        Init = 1,
        ManualReducedSpeed = 2,
        ManualFullSpeed = 3,
        AutoChange = 4,
        ManualFullSpeedChange = 5,
        NotApplicable = 6
    }

    public enum RobotControllerState
    {
        Init = 0,
        MotorsOff = 1,
        MotorsOn = 2,
        GuardStop = 3,
        EmergencyStop = 4,
        EmergencyStopReset = 5,
        SystemFailure = 6,
        Unknown = 99
    }

    public enum RobotProgramStartResult
    {
        //
        // Summary:
        //     Start ok.
        Ok = 0,
        //
        // Summary:
        //     Task not started, regain to path request.
        RegainRequest = 1,
        //
        // Summary:
        //     Task not started, unable to clear path.
        RegainRequestNoClear = 2,
        //
        // Summary:
        //     Error.
        Error = 3,
        //
        // Summary:
        //     Task not started, previous path remains.
        PathRemain = 4,
        //
        // Summary:
        //     Task not started, unable to find entry point.
        IllegalEntryPoint = 5,
        //
        // Summary:
        //     Unable to take mastery in order to try
        FailedToAttempt = 6

    }
}
