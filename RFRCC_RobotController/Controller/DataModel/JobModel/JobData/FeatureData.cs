using CopingLineModels;
using System.Numerics;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    /// <summary>
    /// Data of feature to be processed by machine
    /// </summary>
    public class FeatureData
    {
        /// <summary>
        /// Identifiable Name of features
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Feature Number or index of order features are to be processed
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// If feature information was collected from DSTV file (.NC1)
        /// </summary>
        public bool DSTV_Parsed { get; set; }
        /// <summary>
        /// If 'DSTV_Parsed', the line number from the DSTV (.NC1) document
        /// </summary>
        public int DSTV_LineNumber { get; set; }
        /// <summary>
        /// Minimum xyz coordinates of feature on relative geometry
        /// </summary>
        public Vector3 MinXYZ { get; set; }
        /// <summary>
        /// Maximum xyz coordinates of feature on relative geometry
        /// </summary>
        public Vector3 MaxXYZ { get; set; }
        /// <summary>
        /// Operation data on procees to succeed feature processing
        /// </summary>
        public Operation operation { get; set; }
    }
}
