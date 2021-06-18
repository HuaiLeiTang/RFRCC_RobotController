using ReplaceRSConnection.Robotics.ToolInfo;
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
     *      o add method to input data obtained from import process
     *      o add template of job execution (including PLC requirements and such)
     *      o 
     */
    /// <summary>
    /// Model of job operation file containing all processes and information on current job processing
    /// </summary>
    public class JobModel
    {
        private RobotController _parentController;
        private bool _controllerPresent = false;
        private string _filepath;
        private string _filename;
        private int _NumFeatures;
        private bool _StartedProcessing;
        private bool _FinishedProcessing;
        private bool _ReadyforProcessing;
        public JobModelTemplate Template { get; set; } = new DefaultJobTemplate();

        // Job specific Data
        public string Name { get; set; }
        internal RAPID_OperationBuffer OperationRobotMoveData { get; set; } = new RAPID_OperationBuffer();  // previously 'OperationBuffer'
        public OperationActionList operationActions { get; set; }  = new OperationActionList();
        // TODO: UID generation for Job

        /// <summary>
        /// String describing stage of job. e.g. 'complete'
        /// </summary>
        public string ProjectStatus { get; set; }
        public JobHeader HeaderInfo { get; set; }
        public ToolData ToolData { get; set; } = new ToolData();

        public JobModel(JobModelTemplate template = null)
        {
            if (template != null)
            {
                Template = template;
                Name = template.Name;
            }
                
        }
        public JobModel(RobotController ParentController, int index = -1, JobModelTemplate template = null) : this(template)
        {
            _parentController = ParentController;
            _controllerPresent = true;
        }
        /// <summary>
        /// Connects to a controller and insert Job in list of jobs to be completed
        /// </summary>
        /// <param name="ParentController">Controller to be connected to</param>
        /// <param name="index">Job index in list to be completed if known</param>
        /// <returns></returns>
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

            if (index == -1) _parentController.dataModel.Operations.Add(this);
            else _parentController.dataModel.Operations.Insert(index, this);
            return true; // return false if failed to connect
        }
        // TODO: setup Load Job information from file import

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="Parse"></param>
        /// <returns></returns>
        public bool LoadJobFromFile(string filePath, bool Parse)
        {
            _filepath = filePath;
            _filename = filePath.Split('\\').Last();

            if (Parse)
            {
                FileImporter Parser = new FileImporter() { Job = this, FilePath = _filepath, FileName = _filename };
                if (!Parser.Parse())
                {
                    throw new NotImplementedException(); // TODO: turn this into an error exception
                    // or
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        public bool LoadJobFromParser(FileImporter Parser)
        {
            throw new NotImplementedException();
        }
        public void GenerateOpActionsFromRobManoeuvres() 
        {
            this.Template.GenerateOpActionsFromRobManoeuvres(this);
        }


    }
}
