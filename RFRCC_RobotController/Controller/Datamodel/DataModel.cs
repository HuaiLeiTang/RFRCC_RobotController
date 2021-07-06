using System;
using System.Collections.Generic;
using System.Text;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;
using RapidBool = ABB.Robotics.Controllers.RapidDomain.Bool;
using RFRCC_RobotController.Controller.DataModel.RAPID_Data;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Houses all project data and data connections to robot memory
    /// </summary>
    public class DataModel
    {
        private RobotController _parentController;

        // RAPID Data to be sorted
        internal RapidData SQLMessageRecieve;
        internal RapidData SQLMessageError;
        internal RapidData PCConnected;
        internal RapidData RapidJobData;
        internal RapidData RapidFeatureData;
        internal RapidData PCSDK_Complete;
        internal RapidData Robot_Status;
        /// <summary>
        /// Next job x from datum required for PLC to get job to
        /// </summary>
        public RapidData NextDX;
        /// <summary>
        /// list of job(s) to be completed
        /// </summary>
        public List<JobModel> Jobs = new List<JobModel>();
        /// <summary>
        /// Current Job loaded into memory and onboard connected network Controller
        /// </summary>
        public JobModel CurrentJob
        {
            get
            {
                if (Jobs.Count == 0)
                {
                    return null;
                }
                return Jobs[0];
            }
            set
            {
                if (Jobs.Count != 0)
                {
                    Jobs[0] = value;
                }
                // maybe a throw if no operations available
            }
        }
        /// <summary>
        /// Version of program loaded onto the controller connected
        /// </summary>
        public RobotProgramVersion ProgramVersion { get; }


        internal bool SaveJobDataOnComplete = false; // if true, save job information from robot into something...
        internal bool ClearJobDataOnComplete = true; // deletes operation information from operation list as soon as completed
        
        // TODO: update and move the below

        // Generated Path structures
        public PC_RobotMove_Register RobotInstuctionsRegister;
        public RAPID_OM_List OperationManeouvres;
        public RAPID_OH_List OperationHeaders;
        internal List<JobFeature> jobFeatureData = new List<JobFeature>();

        // Job Data Buffers
        // TODO: Update this and its use to RAPID connected register
        /// <summary>
        /// NOT FOR USE?
        /// To be updated to RAPID memory associated class
        /// </summary>
        public RAPIDJob_Header jobHeader { get; set; } // Move to operation (JobModel) 
        // TODO: Add Machine Settings
        public MachineProcessSettings ProcessSettings;
        internal JobHeader jobHeaderData = new JobHeader();
        /// <summary>
        /// RAPID Data entry on associated controller for Job Header Information
        /// </summary>
        internal RAPIDJobHeader Header_JobData_RapidBuffer { get; set; } = new RAPIDJobHeader();
        /// <summary>
        /// RAPID Data entry on associated controller for Featrue Data Information
        /// </summary>
        internal RAPIDJobFeature Header_FeatureData_RapidBuffer = new RAPIDJobFeature();
        /// <summary>
        /// NOT FOR USE?
        /// To be updated to RAPID memory associated class
        /// </summary>
        public List<JobHeader> jobHeaders = new List<JobHeader>();
        /// <summary>
        /// NOT FOR USE?
        /// To be updated to RAPID memory associated class
        /// </summary>
        public List<JobFeature> jobFeatures = new List<JobFeature>();
        /// <summary>
        /// NOT FOR USE?
        /// To be updated to RAPID memory associated class
        /// </summary>
        public Robot_ControlStruct Robot_Control = new Robot_ControlStruct();
        //public RAPID_OperationBuffer OperationBuffer; //  --> has been moved to JobModel
        public RAPID_CutChart TopCutChart = new RAPID_CutChart();
        public RAPID_CutChart BottomCutChart = new RAPID_CutChart();
        public RAPID_CutChart FrontCutChart = new RAPID_CutChart();
        public RAPID_CutChart BackCutChart = new RAPID_CutChart();

        /// <summary>
        /// Tool data relevant to plasma cutting torch
        /// </summary>
        public ReplaceRSConnection.Robotics.ToolInfo.ToolData ToolData;
        /// <summary>
        /// Data Model Constructor, securing parent controller and intitialising operations
        /// </summary>
        /// <param name="ParentController"></param>
        public DataModel(RobotController ParentController)
        {
            _parentController = ParentController;
            Jobs.Add( new JobModel(ParentController)); // add new job to job list ready for population
            CurrentJob.OperationRobotMoveData.ConnectParentController(_parentController, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
            CurrentJob.OperationRobotMoveData.CurrentJob = true;

            // Init variables
            ProgramVersion = new RobotProgramVersion(_parentController);
            ProcessSettings = new MachineProcessSettings(this);

        }
        // TODO: test that this function correctly identifies the robot program will work with this library
        /// <summary>
        /// Starts up data stream and checks that datamodel is in 
        /// </summary>
        public void InitDataStream()
        {
            bool complete = false;
            if (_parentController.tRob1 != null)
            {
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("Connecting to controller"));
                SQLMessageRecieve = _parentController.tRob1.GetRapidData("SQL_Comm", "SQLMessageRecieve");
                RapidJobData = _parentController.tRob1.GetRapidData("Module1", "Sys_JobData");
                RapidFeatureData = _parentController.tRob1.GetRapidData("Module1", "Sys_FeatureData");
                PCSDK_Complete = _parentController.tRob1.GetRapidData("SQL_Comm", "PCSDK_Complete");
                SQLMessageError = _parentController.tRob1.GetRapidData("SQL_Comm", "SQLMessageError");
                PCConnected = _parentController.tRob1.GetRapidData("SQL_Comm", "PCConnected");
                Robot_Status = _parentController.tRob1.GetRapidData("Module1", "Rob_Status");

                TopCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Top_CutChart");
                BottomCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Bottom_CutChart");
                FrontCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Front_CutChart");
                BackCutChart.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Back_CutChart");

                Robot_Control.ConnectToRAPID(_parentController.controller, _parentController.tRob1, "Module1", "Rob_Control");
                OperationManeouvres = new RAPID_OM_List(99, _parentController.controller, _parentController.tRob1, "Module1", "OperationManoeuvres");
                OperationHeaders = new RAPID_OH_List(20, _parentController.controller, _parentController.tRob1, "Module1", "OperationHeaders");
                jobHeader = new RAPIDJob_Header(_parentController.controller, _parentController.tRob1, "Module1", "Sys_JobData");

                Robot_Control.ValueUpdate += _parentController.OnControlValueUpdate; // Maybe update to enable Interrupts
                Robot_Control.PC_MessageUpdate += _parentController.RobotPC_MessageChanged;

                NextDX = _parentController.tRob1.GetRapidData("Module1", "NextDX");
                NextDX.ValueChanged += _parentController.NextDXChange;

                _parentController.ControllerConnectedEvent();

                while (!complete)
                {
                    try
                    {
                        using (Mastership m = Mastership.Request(_parentController.controller.Rapid))
                        {
                            PCConnected.Value = RapidBool.Parse("TRUE");
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
            else
            {
                _parentController.StatusMesssageChange(this, new RobotController.StatusMesssageEventArgs("'targets' data does not exist!"));
            }
        }
        // TODO: Implement this function "CheckProgramVersionSuitably()"
        /// <summary>
        /// Checks connected network controller if program version and features will work .
        /// PLACEHOLDER FUNCTION - TO BE IMPLEMENTED
        /// </summary>
        /// <returns>Version of connected network controller is acceptable</returns>
        public bool CheckProgramVersionSuitably()
        {
            // TODO: implement this function and update summary
            return true;
        }
        /// <summary>
        /// Entire Job feature list
        /// </summary>
        public List<JobFeature> FeatureDataList
        {
            get { return jobFeatureData; }
            set { jobFeatureData = value; }
        }
        /// <summary>
        /// Job feature data
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        public JobFeature FeatureData(int index)
        {
            return jobFeatureData[index];
        }
        /// <summary>
        /// Current Job Header Data
        /// </summary>
        public JobHeader HeaderData
        {
            get { return jobHeaderData; }
            set { jobHeaderData = value; }
        }
        /// <summary>
        /// Calls event to update Header or Manoeuvre information onto the robot
        /// </summary>
        /// <param name="Table">Table = "header"/"feature"/"manoeuvre"</param>
        /// <param name="FeatureNum">if Table != "header"</param>
        /// <param name="Carriage">Carriage index for chunking manoeuvre information</param>
        public void UpdateRobot(string Table, int FeatureNum = 0, int Carriage = 0)
        {
            bool complete = false;
            switch (Table.ToLower())
            {
                case "header":

                    // TODO: REMOVE SQL DEPENDENCIE
                    Header_JobData_RapidBuffer.UpdatedFromSQL(HeaderData);

                    // Rewrite data to robot memory
                    while (!complete)
                    {
                        try
                        {
                            using (Mastership m = Mastership.Request(_parentController.controller.Rapid))
                            {
                                // TODO: REMOVE SQL DEPENDENCIE
                                RapidJobData.StringValue = Header_JobData_RapidBuffer.ToString();
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


                    break;

                case "feature":

                    // Update Header Buffer from 1st Jobheader (assume first one is one we want)
                    foreach (JobFeature feature in FeatureDataList)
                    {
                        if (feature.FeatureNum == FeatureNum)
                        {
                            // TODO: change to internal memory?
                            Header_FeatureData_RapidBuffer.UpdatedFromSQL(feature);
                            break;
                        }
                    }

                    // Rewrite data to robot memory
                    while (!complete)
                    {
                        try
                        {
                            using (Mastership m = Mastership.Request(_parentController.controller.Rapid))
                            {
                                RapidFeatureData.StringValue = Header_FeatureData_RapidBuffer.ToString();
                            }
                        }
                        catch
                        {
                            _parentController.StatusMesssageChange(_parentController, new RobotController.StatusMesssageEventArgs("mastership failed while attempting to update Feature Data"));
                            complete = false;
                        }
                        finally
                        {
                            complete = true;
                        }
                    }

                    break;

                case "manoeuvre":

                    _parentController.StatusMesssageChange(_parentController, new RobotController.StatusMesssageEventArgs("Robot requested Manoeuvre " + FeatureNum.ToString() + ". Sending to Robot"));

                    //raise event to write manoeuvre 
                    if (_parentController.ManoeuvreUpdate(_parentController, new RobotController.ManoeuvreUpdateEventArgs(FeatureNum, Carriage, CurrentJob.OperationRobotMoveData)))
                    {
                        _parentController.StatusMesssageChange(_parentController, new RobotController.StatusMesssageEventArgs("Successfully transferred Manoeuvre to Robot"));
                    }
                    else
                    {
                        _parentController.StatusMesssageChange(_parentController, new RobotController.StatusMesssageEventArgs("ERROR in transfer Manoeuvre to Robot"));
                    }

                    break;

                default:
                    break;
            }


        }
        /// <summary>
        /// Calls event to update JobData on robot by JobID
        /// THIS MAY SOON BE REDUNDANT!
        /// </summary>
        /// <param name="JobID">JobID of job to update on robot</param>
        /// <returns></returns>
        public bool UpdateRCC(string JobID)
        {
            return _parentController.OnRequestUpdatedJobData(_parentController, new RobotController.RequestUpdatedJobDataEventArgs(JobID, false));
        }
        // TODO: finish load data onto robot controller
        /// <summary>
        /// imports ASCII contents of DSTV file ready for parseing and jobdata population
        /// </summary>
        /// <param name="filename">Name of file or job</param>
        /// <param name="ASCII_Content">ASCII contents of DSTV fileparam>
        /// <param name="Parse">if false, file will not be parsed immediately</param>
        /// <returns></returns>
        public int LoadJobFromASCII(string filename, string ASCII_Content, bool Parse = true)
        {
            Jobs.Add(new JobModel(_parentController));

            if (!CurrentJob.OperationRobotMoveData.CurrentJob)
            {
                if (ProcessSettings.AutoProgressJob) CurrentJob.JobCompleted += NextJob;
                CurrentJob.OperationRobotMoveData.ConnectParentController(_parentController, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
                CurrentJob.OperationRobotMoveData.CurrentJob = true;
                if (ProcessSettings.AutoProgressJob) CurrentJob.JobCompleted += NextJob;
            }

            if (!Jobs.Last().LoadJobFromASCII(filename, ASCII_Content, Parse)) return -1;

            return Jobs.Count - 1;
        }
        // TODO: Implement load additional Jobs onto 
        /// <summary>
        /// Load and parse data from file path in .nc1 format
        /// parsed automatically unless parse = false
        /// </summary>
        /// <param name="filepath">full filepath in storage</param>
        /// <param name="parse">if false, file will not be parsed immediately</param>
        /// <returns>index of file added, -1 if failed</returns>
        public int LoadJobFromFile(string filepath, bool Parse = true)
        {
            Jobs.Add(new JobModel(_parentController));
            
            if (!CurrentJob.OperationRobotMoveData.CurrentJob)
            {
                if (ProcessSettings.AutoProgressJob) CurrentJob.JobCompleted += NextJob;
                CurrentJob.OperationRobotMoveData.ConnectParentController(_parentController, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
                CurrentJob.OperationRobotMoveData.CurrentJob = true;
                if (ProcessSettings.AutoProgressJob) CurrentJob.JobCompleted += NextJob;
            }

            if (!Jobs.Last().LoadJobFromFile(filepath, Parse)) return -1;

            return Jobs.Count - 1;
        }
        /// <summary>
        /// NOT YET IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        public int LoadJobFromParser()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes all operations from Operation Queue
        /// </summary>
        public void ClearJobData()
        {
            if (CurrentJob != null) CurrentJob.DisconnectFromController();
            Jobs.Clear();
        }
        // TODO: add subscribe if auto pull next job
        /// <summary>
        /// Progresses Current Job out of memory and connects next job for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NextJob(object sender = null, EventArgs args = null)
        {
            CurrentJob.DisconnectFromController();
            CurrentJob.JobCompleted -= NextJob;
            Jobs.RemoveAt(0);
            if (CurrentJob != null)
            {
                CurrentJob.ConnectToController();
                if (ProcessSettings.AutoProgressJob) CurrentJob.JobCompleted += NextJob;
            }
        }
    }

}
