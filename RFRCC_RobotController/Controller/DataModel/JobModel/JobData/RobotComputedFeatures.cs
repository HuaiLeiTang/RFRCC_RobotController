using RFRCC_RobotController.Controller.DataModel.RAPID_Data;
using System;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    /// <summary>
    /// Computed feature and relevant feature data for Robot
    /// </summary>
    public class RobotComputedFeatures
    {
        // --- PRIVATE PROPERTIES ---
        private OperationHeader _FeatureHeader = new OperationHeader();
        private List<OperationManoeuvre> _FeatureManoeuvres = new List<OperationManoeuvre>();
        private bool _StartWhenReady = false;
        private bool _WaitingForStart = false;

        // --- EVENTS ---
        public event EventHandler FeatureRequestRobotContinue;


        // --- PUBLIC PROPERTIES ---
        /// <summary>
        /// Feature name
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Feature specific data
        /// </summary>
        public FeatureData featureData { get; set; } = new FeatureData();
        /// <summary>
        /// Feature Header Information
        /// </summary>
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
        /// <summary>
        /// Feature Manoeuvres 
        /// </summary>
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
        /// <summary>
        /// If robot is waiting for permission to start
        /// </summary>
        public bool WaitingForStart 
        { 
            get { return _WaitingForStart; } 
            set 
            { 
                _WaitingForStart = value; 
                if (value && _StartWhenReady) AllowRobotToContinue();
            } 
        }
        /// <summary>
        /// if the job data has been uploadeded to robot for parsing yet
        /// </summary>
        public bool UploadedToRobot { get; set; } = false;
        /// <summary>
        /// if the robot has indicated it has completed this operation
        /// </summary>
        public bool CompletedByRobot { get; set; } = false;
        /// <summary>
        /// If robot is currently processing this operation
        /// </summary>
        public bool InProgress { get; set; } = false;
        /// <summary>
        ///  status of autocontinue
        /// </summary>
        public bool StartWhenReady 
        {
            get { return _StartWhenReady; } 
            set 
            {
                _StartWhenReady = value;
                if (_StartWhenReady && WaitingForStart) AllowRobotToContinue();
            } 
        }
        /// <summary>
        /// X displacement of stock required for completion of operation
        /// </summary>
        public double IdealXDisplacement
        {
            get
            {
                return _FeatureHeader.IdealXDisplacement;
            }
        }

        // --- CONSTRUCTORS
        /// <summary>
        /// Initialise as empty object
        /// </summary>
        public RobotComputedFeatures()
        {
            Name = "";
        }
        /// <summary>
        /// Implement with only header information on feature
        /// </summary>
        /// <param name="NewHeader">Header Information</param>
        public RobotComputedFeatures(OperationHeader NewHeader) : this()
        {
            Name = NewHeader.Name;
            _FeatureHeader = NewHeader;
        }
        /// <summary>
        /// Implement new object with header information and list on Manoeuvre information
        /// </summary>
        /// <param name="NewHeader">Feature header information</param>
        /// <param name="NewManoeuvres">Feature maoeuvres</param>
        public RobotComputedFeatures(OperationHeader NewHeader, List<OperationManoeuvre> NewManoeuvres) : this()
        {
            Name = NewHeader.Name;
            _FeatureHeader = NewHeader;
            _FeatureManoeuvres.AddRange(NewManoeuvres);
        }

        // --- METHODS ---
        public void AllowRobotToContinue()
        {
            if (FeatureRequestRobotContinue != null) throw new Exception("no subscriber to Robot start");
            FeatureRequestRobotContinue?.Invoke(this, new EventArgs());
        }


        // --- INTERNAL EVENTS & AUTOMATION ---
    }
}
