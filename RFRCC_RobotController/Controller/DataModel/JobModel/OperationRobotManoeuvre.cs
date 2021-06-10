using RFRCC_RobotController.Controller.DataModel.OperationData;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class OperationRobotManoeuvre : OperationAction
    {
        public RobotComputedFeatures featureData { get; set; }
    }
}
