using CopingLineModels;
using System.Numerics;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    public class FeatureData
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public bool DSTV_Parsed { get; set; }
        public int DSTV_LineNumber { get; set; }
        public Vector3 MinXYZ { get; set; }
        public Vector3 MaxXYZ { get; set; }
        public Operation operation { get; set; }
    }
}
