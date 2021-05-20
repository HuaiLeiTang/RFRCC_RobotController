// Microsoft classes
using ABB.Robotics.Controllers.ConfigurationDomain;
using System.Drawing;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics
{
    public class Target //: ABB.Robotics.RobotStudio.ProjectObject
    {
        //public ABB.Robotics.RobotStudio.Stations.AxisDirection ApproachVector { get; set; }
        public Color Color { get; set; }
        public string DisplayName { get; }
        public double FrameSize { get; set; }
        public string Name { get; set; }
        public FakeTransform ReferenceFrame { get; }
        public RobTarget RobTarget { get; set; }
        public bool ShowName { get; set; }
        public bool ShowReferenceFrame { get; set; }
        public FakeTransform Transform { get; }
        public bool Visible { get; set; }
        public WorkObject WorkObject { get; set; }
        public AttributeCollection Attributes { get; }


        public Target(WorkObject workObject, RobTarget robTarget)
        {
            WorkObject = workObject;
            RobTarget = robTarget;
        }

        public object Copy()
        {
            return MemberwiseClone();
        }
    }
}