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

namespace RFRCC_RobotController
{
    public class RobotController
    {
        /*// ------------------------------------------------------------------------------------------------
                                                VARIABLES AND PROPERTIES
        */// ------------------------------------------------------------------------------------------------
        private Controller controller = null;
        private ABB.Robotics.Controllers.RapidDomain.Task tRob1;
        private bool _ControllerConnected = false;

        // Housekeeping and networking  
        private NetworkScanner scanner = null;
        private NetworkWatcher networkwatcher = null;
        private ControllerInfoCollection _AvailableControllers = null;
        private bool FetchedData;

        // Generated Path structures
        public PC_RobotMove_Register RobotInstuctionsRegister;
        public RAPID_OM_List OperationManeouvres;
        public RAPID_OH_List OperationHeaders;
        private List<JobFeature> jobFeatureData = new List<JobFeature>();

        // Job Data Buffers
        private JobHeader jobHeaderData = new JobHeader();
        private RAPIDJobHeader Header_JobData_RapidBuffer = new RAPIDJobHeader();
        private RAPIDJobFeature Header_FeatureData_RapidBuffer = new RAPIDJobFeature();
        public List<JobHeader> jobHeaders = new List<JobHeader>();
        public List<JobFeature> jobFeatures = new List<JobFeature>();
        public Robot_ControlStruct Robot_Control = new Robot_ControlStruct();

        // RAPID Data to be sorted
        private RapidData SQLMessageRecieve;
        private RapidData SQLMessageError;
        private RapidData PCConnected;
        private RapidData RapidJobData;
        private RapidData RapidFeatureData;
        private RapidData PCSDK_Complete;
        private RapidData Robot_Status;

        /*// ------------------------------------------------------------------------------------------------
                                     CONTROLLER HANDELING, CREATION AND DISPOSAL  
        */// ------------------------------------------------------------------------------------------------

        public event EventHandler<AvailableControllersEventArgs> OnAvailableControllersChange;
        public class AvailableControllersEventArgs : EventArgs
        {
            public AvailableControllersEventArgs(ControllerInfoCollection AvalableList)
            {
                AvailableControllers = AvalableList;
            }

            public ControllerInfoCollection AvailableControllers { get; set; }
        }
        protected virtual void AvailableControllersChange(object sender, AvailableControllersEventArgs e)
        {
            EventHandler<AvailableControllersEventArgs> handler = OnAvailableControllersChange;
            if (handler != null)
                handler(sender, e);
        }
        void HandleNWCChangeEvent(object sender, NetworkWatcherEventArgs e)
        {
            scanner.Scan();
            _AvailableControllers = scanner.Controllers;
            OnAvailableControllersChange(this, new AvailableControllersEventArgs(_AvailableControllers));
        }
        public void ConnectToController(Controller controller)
        {
            if (_ControllerConnected)
                Dispose();
            this.controller = controller;
            this.controller.Logon(UserInfo.DefaultUser);
            tRob1 = controller.Rapid.GetTask("T_ROB1");
            InitDataStream();
            _ControllerConnected = true;
            ControllerConnectedChange(this, new ControllerConnectedEventArgs(_ControllerConnected));
            StatusMesssageChange(this, new StatusMesssageEventArgs("Connected to controller"));
        }
        public void ConnectToController(ControllerInfo controllerInfo)
        {
            if (_ControllerConnected)
                Dispose();
            this.controller = Controller.Connect(controllerInfo, ConnectionType.Standalone);
            this.controller.Logon(UserInfo.DefaultUser);
            tRob1 = controller.Rapid.GetTask("T_ROB1");
            InitDataStream();
            _ControllerConnected = true;
            ControllerConnectedChange(this, new ControllerConnectedEventArgs(_ControllerConnected));
            StatusMesssageChange(this, new StatusMesssageEventArgs("Connected to controller"));
        }
        public event EventHandler<ControllerConnectedEventArgs> OnControllerConnectedChange;
        public class ControllerConnectedEventArgs : EventArgs
        {
            public ControllerConnectedEventArgs(bool Connected)
            {
                ControllerConnected = Connected;
            }

            public bool ControllerConnected { get; set; }
        }
        protected virtual void ControllerConnectedChange(object sender, ControllerConnectedEventArgs e)
        {
            EventHandler<ControllerConnectedEventArgs> handler = OnControllerConnectedChange;
            if (handler != null)
                handler(sender, e);
        }


        // Initialise Controller with all Variables & Check correct controller
        public void InitDataStream()
        {
            bool complete;
            if (tRob1 != null)
            {
                StatusMesssageChange(this, new StatusMesssageEventArgs("Connecting to controller"));
                SQLMessageRecieve = tRob1.GetRapidData("SQL_Comm", "SQLMessageRecieve");
                RapidJobData = tRob1.GetRapidData("Module1", "Sys_JobData");
                RapidFeatureData = tRob1.GetRapidData("Module1", "Sys_FeatureData");
                PCSDK_Complete = tRob1.GetRapidData("SQL_Comm", "PCSDK_Complete");
                SQLMessageError = tRob1.GetRapidData("SQL_Comm", "SQLMessageError");
                PCConnected = tRob1.GetRapidData("SQL_Comm", "PCConnected");
                Robot_Status = tRob1.GetRapidData("Module1", "Rob_Status");

                Robot_Control.ConnectToRAPID(controller, tRob1, "Module1", "Rob_Control");
                OperationManeouvres = new RAPID_OM_List(99, controller, tRob1, "Module1", "OperationManoeuvres");
                OperationHeaders = new RAPID_OH_List(20, controller, tRob1, "Module1", "OperationHeaders");
                RobotInstuctionsRegister = new PC_RobotMove_Register(OperationManeouvres, OperationHeaders);


                Robot_Control.ValueUpdate += OnControlValueUpdate; // Maybe update to enable Interrupts
                Robot_Control.PC_MessageUpdate += RobotPC_MessageChanged;

                complete = false;
                while (!complete)
                {
                    try
                    {
                        using (Mastership m = Mastership.Request(controller.Rapid))
                        {
                            PCConnected.Value = Bool.Parse("TRUE");
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
                StatusMesssageChange(this,new StatusMesssageEventArgs("'targets' data does not exist!"));
            }


        }
        public void Dispose()
        {
            // TODO Stop current task on disconnect?
            this.controller.Logoff();
            this.controller.Dispose();
            this.controller = null;
            _ControllerConnected = false;
            ControllerConnectedChange(this, new ControllerConnectedEventArgs(_ControllerConnected));
            StatusMesssageChange(this, new StatusMesssageEventArgs("Disconnected from controller"));
        }

        // messaging function from controller to outside
        // i.e. error / status messaging / etc
        public event EventHandler<StatusMesssageEventArgs> OnStatusMesssageChange;
        public class StatusMesssageEventArgs : EventArgs
        {
            public StatusMesssageEventArgs(string statusMesssage)
            {
                StatusMesssage = statusMesssage;
            }

            public string StatusMesssage { get; set; }
        }
        protected virtual void StatusMesssageChange(object sender, StatusMesssageEventArgs e)
        {
            EventHandler<StatusMesssageEventArgs> handler = OnStatusMesssageChange;
            if (handler != null)
                handler(sender, e);
        }

        public RobotController()
        {
            scanner = new NetworkScanner();
            scanner.Scan();
            _AvailableControllers = scanner.Controllers;
            _ControllerConnected = false;

            // NetworkWatcher setup
            networkwatcher = new NetworkWatcher(scanner.Controllers);
            networkwatcher.Found += new EventHandler<NetworkWatcherEventArgs>(HandleNWCChangeEvent);
            networkwatcher.Lost += new EventHandler<NetworkWatcherEventArgs>(HandleNWCChangeEvent);
            networkwatcher.EnableRaisingEvents = true;
        }
        public Controller Controller
        {
            get
            {
                return controller;
            }
        }
        public ControllerInfoCollection AvailableControllers
        {
            get
            {
                return _AvailableControllers;
            }
        }
        public bool ControllerConnected
        {
            get
            {
                return _ControllerConnected;
            }
        }


        /*// ------------------------------------------------------------------------------------------------
                                            ROBOT FUNCTIONALITY AND CONTROL
        */// ------------------------------------------------------------------------------------------------

        // Control secondry

        // Control Message from robot
        public delegate void ControlValueUpdateEventHandler(RobotController sender, ControlStrucEventArgs e);
        public event ControlValueUpdateEventHandler ControlValueUpdate;
        protected virtual void OnControlValueUpdate(object sender, ControlStrucEventArgs e)
        {
            Debug.WriteLine("OnControlValueUpdate Recieved instruction");
            ControlValueUpdate?.Invoke(this, e);
        }

        // This will fire when the controller changes the PC_Message value in the control structure - TODO: setup message reading from this.
        void RobotPC_MessageChanged(object sender, ControlStrucEventArgs e)
        {
            Debug.WriteLine("RobotPC_MessageChanged Recieved instruction");

            if (e.ValueName != "<CLEAR>" && e.ValueName != "")
            {
                MessageRecieved();
                Debug.WriteLine("About to parse PC_Message: " + e.ValueName);
                StatusMesssageChange(this, new StatusMesssageEventArgs("RAPID Data Change: PC Message"));
                var parsemessagetime = new Stopwatch();
                parsemessagetime.Start();
                StatusMesssageChange(this, new StatusMesssageEventArgs(ParseMessage(e.ValueName)));
                parsemessagetime.Stop();
                Debug.WriteLine("Parse complete | dT = " + parsemessagetime.ElapsedMilliseconds.ToString() + "ms");
            }
        }

        // This will fire when the controller changes a value in the control structure - TODO: setup message reading from this.
        void Robot_Control_ValueChanged(object sender, ControlStrucEventArgs e)
        {
            Debug.WriteLine("Robot_Control_ValueChanged Recieved instruction");
            switch (e.ValueName)
            {
                case "PC_Message":
                    if (Robot_Control.PC_Message != "<CLEAR>" && Robot_Control.PC_Message != "")
                    {
                        MessageRecieved();
                        Debug.WriteLine("About to parse PC_Message: " + Robot_Control.PC_Message);
                        StatusMesssageChange(this, new StatusMesssageEventArgs("RAPID Data Change: PC Message"));
                        var parsemessagetime = new Stopwatch();
                        parsemessagetime.Start();
                        StatusMesssageChange(this, new StatusMesssageEventArgs(ParseMessage(Robot_Control.PC_Message)));
                        parsemessagetime.Stop();
                        Debug.WriteLine("Parse complete | dT = " + parsemessagetime.ElapsedMilliseconds.ToString() + "ms");
                    }
                    break;
                default:
                    StatusMesssageChange(this, new StatusMesssageEventArgs("RAPID Data Change: " + e.ValueName));
                    break;
            }
        }
        // For Communication to Robot that message is being parsed and actioned
        public void MessageRecieved()
        {
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        SQLMessageRecieve.Value = Bool.Parse("TRUE");
                    }
                    StatusMesssageChange(this, new StatusMesssageEventArgs("PC acknowledged message"));
                    Debug.WriteLine("Acknoledged Controller SQL Message");
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
        public string ParseMessage(string MessageString)
        {
            if (MessageString.Length < 8)
            {
                PCSDK_Work_Complete();
                return MessageString + "ERROR : Message Not Recognised : \""+ MessageString + "\"";
            }
            MessageString = MessageString.Split("\"")[0];
            string MessageHeader = MessageString.Substring(0, 8); // breaks if less thab 8....
            switch (MessageHeader)
            {
                case "FEATURE0":
                    string Feat = MessageString.Split("<Feature>")[1];
                    Feat = Feat.Split("</>")[0];
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to upload feature: " + Feat));
                    UpdateRobot("feature", int.Parse(Feat));
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
                    OnUpdateFeatureOptimalX(this, new UpdateFeatureOptimalXEventArgs(Robot_Control.JobID, int.Parse(FeatAndXOpt[0]), decimal.Parse(FeatAndXOpt[1])));
                    PCSDK_Work_Complete();
                    return "Updated Feature " + FeatAndXOpt[0] + " X_Optimal to " + FeatAndXOpt[1];

                case "HEADER00":
                    string jobIDFromString = MessageString.Split("<JobID>")[1];
                    jobIDFromString = jobIDFromString.Split("</>")[0];
                    StatusMesssageChange(this, new StatusMesssageEventArgs("About to upload header for JobID: " + jobIDFromString));
                    if (FetchedData || (this.HeaderData.JobID != jobIDFromString))
                    {
                        UpdateRCC(jobIDFromString);
                        UpdateRobot("header");
                        FetchedData = false; // reset fetched flag.
                        PCSDK_Work_Complete();
                        return "JobID didnt match: Updated Header from SQL & Updated Robot Header Register with JobID: " + jobIDFromString;
                    }
                    else
                    {
                        UpdateRobot("header");
                        PCSDK_Work_Complete();
                        return "Updated Robot Header Register with JobID: " + jobIDFromString;
                    }
                case "FRC_UPDT":
                    string jobIDUpdate = MessageString.Split("</>")[0];
                    jobIDUpdate = jobIDUpdate.Split("FRC_UPDT <JobID>")[0];
                    FetchedData = true;
                    UpdateRCC(jobIDUpdate);
                    PCSDK_Work_Complete();
                    return "Updated SQL Server Integration Memory with JobID: " + jobIDUpdate;

                default:
                    // TODO error handle
                    return "Message Not Recognised";
            }

            
        }
        private void PCSDK_Work_Complete()
        {
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        PCSDK_Complete.Value = Bool.Parse("TRUE");
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
        public delegate bool UpdateFeatureEventHandler(RobotController sender, UpdateFeatureOptimalXEventArgs e);
        public event UpdateFeatureEventHandler UpdateFeatureOptimalX;
        protected virtual bool OnUpdateFeatureOptimalX(object sender, UpdateFeatureOptimalXEventArgs e)
        {
            FeatureDataList[e.FeatureOptimalX_FeatureNum].Dim1Optimal = (float)e.FeatureOptimalX_OptimalX;
            return (UpdateFeatureOptimalX != null) ? UpdateFeatureOptimalX(this, e)
                : throw new ArgumentException("No listeners logged to handle UpdateJobData event");
        }

        // custom event args
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
        // custom event delegate !! WITH RESPONSE !!
        public delegate bool RequestUpdatedJobDataEventHandler(RobotController sender, RequestUpdatedJobDataEventArgs e);
        // eventHandler
        public event RequestUpdatedJobDataEventHandler RequestUpdatedJobData;
        protected virtual bool OnRequestUpdatedJobData(object sender, RequestUpdatedJobDataEventArgs e)
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












        public void UpdateRobot(string Table, int FeatureNum = 0)
        {
            bool complete = false;
            switch (Table.ToLower())
            {
                case "header":
                    

                    Header_JobData_RapidBuffer.UpdatedFromSQL(this.HeaderData);

                    // Rewrite data to robot memory
                    while (!complete)
                    {
                        try
                        {
                            using (Mastership m = Mastership.Request(controller.Rapid))
                            {
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
                    foreach (JobFeature feature in this.FeatureDataList)
                    {
                        if (feature.FeatureNum == FeatureNum)
                        {
                            Header_FeatureData_RapidBuffer.UpdatedFromSQL(feature);
                            break;
                        }
                    }

                    // Rewrite data to robot memory
                    while (!complete)
                    {
                        try
                        {
                            using (Mastership m = Mastership.Request(controller.Rapid))
                            {
                                RapidFeatureData.StringValue = Header_FeatureData_RapidBuffer.ToString();
                            }
                        }
                        catch
                        {
                            StatusMesssageChange(this, new StatusMesssageEventArgs("mastership failed while attempting to update Feature Data"));
                            complete = false;
                        }
                        finally
                        {
                            complete = true;
                        }
                    }

                    break;

                default:
                    break;
            }


        }
        public bool UpdateRCC(string JobID)
        {
            return OnRequestUpdatedJobData(this, new RequestUpdatedJobDataEventArgs(JobID, false));
        }
        

        public List<JobFeature> FeatureDataList
        {
            get { return jobFeatureData; }
            set { jobFeatureData = value; }
        }
        public JobFeature FeatureData(int index)
        {
            return jobFeatureData[index];
        }
        public JobHeader HeaderData
        {
            get { return jobHeaderData; }
            set { jobHeaderData = value; }
        }

    }
}
