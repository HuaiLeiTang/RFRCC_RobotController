using RFRCC_RobotController.RAPID_Data;
using RFRCC_RobotController.ABB_Data;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller
{

    public class RobotComputedFeatures
    {
        public string Name { get; set; } = "";
        private OperationHeader _FeatureHeader = new OperationHeader();
        private List<OperationManoeuvre> _FeatureManoeuvres = new List<OperationManoeuvre>();
        public RobotComputedFeatures()
        {
        }
        public RobotComputedFeatures(OperationHeader NewHeader)
        {
            _FeatureHeader = NewHeader;
        }
        public RobotComputedFeatures(OperationHeader NewHeader, List<OperationManoeuvre> NewManoeuvres)
        {
            Name = "";
            _FeatureHeader = NewHeader;
            _FeatureManoeuvres.AddRange(NewManoeuvres);
        }
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
