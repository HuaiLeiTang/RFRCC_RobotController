using RFRCC_RobotController.Controller.DataModel.RAPID_Data;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    /// <summary>
    /// Computed feature and relevant feature data for Robot
    /// </summary>
    public class RobotComputedFeatures
    {
        /// <summary>
        /// Feature name
        /// </summary>
        public string Name { get; set; } = "";
        private OperationHeader _FeatureHeader = new OperationHeader();
        private List<OperationManoeuvre> _FeatureManoeuvres = new List<OperationManoeuvre>();
        /// <summary>
        /// Feature specific data
        /// </summary>
        public FeatureData featureData { get; set; } = new FeatureData();
        /// <summary>
        /// Initialise as empty object
        /// </summary>
        public RobotComputedFeatures()
        {
        }
        /// <summary>
        /// Implement with only header information on feature
        /// </summary>
        /// <param name="NewHeader">Header Information</param>
        public RobotComputedFeatures(OperationHeader NewHeader)
        {
            _FeatureHeader = NewHeader;
        }
        /// <summary>
        /// Implement new object with header information and list on Manoeuvre information
        /// </summary>
        /// <param name="NewHeader">Feature header information</param>
        /// <param name="NewManoeuvres">Feature maoeuvres</param>
        public RobotComputedFeatures(OperationHeader NewHeader, List<OperationManoeuvre> NewManoeuvres)
        {
            Name = "";
            _FeatureHeader = NewHeader;
            _FeatureManoeuvres.AddRange(NewManoeuvres);
        }
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
    }
}
