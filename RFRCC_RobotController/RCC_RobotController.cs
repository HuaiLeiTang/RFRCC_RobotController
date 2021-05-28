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

namespace RFRCC_RobotController
{
    /// <summary>
    /// Robot controller Class for control of Robofab Australia Coping Cell
    /// </summary>
    public class RobotController
    {
        public Stream stream;
        public DataModel dataModel;

        internal ABB.Robotics.Controllers.Controller controller = null;
        internal ABB.Robotics.Controllers.RapidDomain.Task tRob1;
        internal bool _ControllerConnected = false;

        // Housekeeping and networking  
        //private bool FetchedData;

        //TODO make into an event? or understand exactly what it does
        public bool FetchedData { get; set; }

        public event EventHandler<EventArgs> OnNextDXChange;
        internal protected virtual void NextDXChange(object sender, EventArgs e)
        {
            OnNextDXChange?.Invoke(sender, e);
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
        protected internal virtual void StatusMesssageChange(object sender, StatusMesssageEventArgs e)
        {
            EventHandler<StatusMesssageEventArgs> handler = OnStatusMesssageChange;
            if (handler != null)
                handler(sender, e);
        }

        public RobotController()
        {
            stream = new Stream(this);
            dataModel = new DataModel(this);
        }
        ABB.Robotics.Controllers.Controller Controller
        {
            get
            {
                return controller;
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

        // Control Message from robot
        public delegate void ControlValueUpdateEventHandler(RobotController sender, ControlStrucEventArgs e);
        public event ControlValueUpdateEventHandler ControlValueUpdate;
        internal protected virtual void OnControlValueUpdate(object sender, ControlStrucEventArgs e)
        {
            Debug.WriteLine("OnControlValueUpdate Recieved instruction");
            ControlValueUpdate?.Invoke(this, e);
        }

        // This will fire when the controller changes the PC_Message value in the control structure - TODO: setup message reading from this.
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
                        dataModel.SQLMessageRecieve.Value = Bool.Parse("TRUE");
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
                    dataModel.OperationBuffer.Operation[int.Parse(FeatAndXOpt[0]) - 1].FeatureHeader.IdealXDisplacement = double.Parse(FeatAndXOpt[1]);

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

                    dataModel.jobHeader.FeatureQuant = dataModel.OperationBuffer.Operation.Count;
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
        internal protected virtual bool OnUpdateFeatureOptimalX(object sender, UpdateFeatureOptimalXEventArgs e)
        {
            dataModel.FeatureDataList[e.FeatureOptimalX_FeatureNum].Dim1Optimal = (float)e.FeatureOptimalX_OptimalX;
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


        // Event to request update of Manoeuvre from PC
        public delegate bool OnManoeuvreUpdateEventHandler(RobotController sender, ManoeuvreUpdateEventArgs e);
        public event OnManoeuvreUpdateEventHandler OnManoeuvreUpdate;
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
        internal protected virtual bool ManoeuvreUpdate(RobotController sender, ManoeuvreUpdateEventArgs e)
        {
            dataModel.jobHeader.FeatureQuant = dataModel.OperationBuffer.Operation.Count;
            bool ReturnVal = dataModel.OperationBuffer.UploadData(e.ManoeuvreNum, e.Carriage);

            if (OnManoeuvreUpdate != null)
            {
                return OnManoeuvreUpdate(sender, e);
            }
            return true;
        }





        



    }
}
