using ABB.Robotics.RobotStudio.Stations;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics
{
    public partial class RobTarget : DataDeclaration, System.ICloneable
    {
        private ConfigurationData _ConfigurationData = new ConfigurationData();

        public RobTarget()
        {
            ConfigurationData = new ConfigurationData();
            ConfigurationStatus = new ConfigurationStatus();
            Frame = new FakeTransform();
            externalAxis = new ExternalAxisValues();
        }

        public RobTarget(RsRobTarget rsRobTarget)
        {
            RsRobTarget FromTarget = rsRobTarget.Copy() as RsRobTarget;
            _ConfigurationData = FromTarget.ConfigurationData;
            Frame = new FakeTransform(FromTarget.Frame);
            IsInline = FromTarget.IsInline;
            externalAxis = FromTarget.GetExternalAxes(true);
        }

        public ConfigurationData ConfigurationData
        {
            get => _ConfigurationData; set
            {
                _ConfigurationData = value;
            }
        }
        public ConfigurationStatus ConfigurationStatus { get; set; }
        public FakeTransform Frame { get; set; } = new FakeTransform();
        public bool IsInline { get; set; }
        public ExternalAxisValues externalAxis { get; set; }

        public object Clone()
        {
            return Copy();
        }

        public RobTarget Copy()
        {
            RobTarget output = new RobTarget()
            {
                DataType = DataType,
                Frame = new FakeTransform(Frame),
                IsInline = IsInline,
            };

            output.SetConfiguration(ConfigurationData.Cf1, ConfigurationData.Cf4, ConfigurationData.Cf6, ConfigurationData.Cfx);
            output.SetExternalAxes(externalAxis, false);
            return output;
        }
        
        public void SetConfiguration(int cf1, int cf4, int cf6, int cfx)
        {
            _ConfigurationData.Cf1 = cf1;
            _ConfigurationData.Cf4 = cf4;
            _ConfigurationData.Cf6 = cf6;
            _ConfigurationData.Cfx = cfx;
        }

        public void SetExternalAxes(ExternalAxisValues value, bool convertFromSIToRapidData)
        {
            externalAxis = value;
        }
    }

}
