using RFRCC_RobotController.Controller.DataModel.OperationData;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Action step involving Robot cutting process
    /// </summary>
    public class OperationRobotManoeuvre : OperationAction
    {
        /// <summary>
        /// Data of feature to be cut during process
        /// </summary>
        public RobotComputedFeatures featureData { get; set; }
    }
}
