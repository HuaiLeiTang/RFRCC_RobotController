using System;
using System.Collections.Generic;
using ABB.Robotics.Controllers.RapidDomain;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Program Version and support loaded onto robot
    /// </summary>
    public class RobotProgramVersion
    {
        private RobotController _ParentController;
        private RapidData _RobotProgramVer;
        private RapidData _RobotProgramSubCVer;
        private RapidData _RobotProgramRevision;
        private int _ProgVersion = 0;
        private int _ProgSubVersion = 0;
        private int _ProgRevision = 0;
        private bool _connected = false;
        private Dictionary<string, bool> _VersionFeatures = new Dictionary<string, bool>();

        /// <summary>
        /// Robot Program Version will provide the current firmware program version on the connected controller and determin what features are functional
        /// </summary>
        /// <param name="ParentController">Controller of whichthios object will check the functionality and version</param>
        public RobotProgramVersion(RobotController ParentController)
        {
            _ParentController = ParentController;
            _ParentController.ControllerConnectionChange += ConnectToController;

            if (_ParentController.ControllerConnected)
            {
                ConnectToController(_ParentController, new ControllerConnectionEventArgs());

                
            }
        }

        /// <summary>
        /// Returns bool if feature is supported by connected controller
        /// </summary>
        /// <param name="feature">Name of the feature to be checked</param>
        /// <returns></returns>
        public bool FeatureAvailable(string feature)
        {
            if (_VersionFeatures.ContainsKey(feature.ToLower()))
            {
                return _VersionFeatures[feature.ToLower()];
            }
            return false;
        }

        /// <summary>
        /// Program Version loaded onto Robot
        /// </summary>
        public string ProgramVersion 
        { get
            {
                if (!_ParentController._ControllerConnected || !_connected) throw new Exception("Invalid: Controller Not Connected");
                return _ProgVersion.ToString("N0") + "." + _ProgSubVersion.ToString("N0") + "." + _ProgRevision.ToString("N0");
            }
        }

        /// <summary>
        /// Connect This RAPIDData module to the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ConnectToController(object sender, ControllerConnectionEventArgs args)
        {
            _RobotProgramVer = _ParentController.tRob1.GetRapidData("CalibData", "RobotProgVer");
            _RobotProgramSubCVer = _ParentController.tRob1.GetRapidData("CalibData", "RobotProgSubVer");
            _RobotProgramRevision = _ParentController.tRob1.GetRapidData("CalibData", "RobotProgRevision");
            _connected = true;

            _ProgVersion = int.Parse(_RobotProgramVer.StringValue);
            _ProgSubVersion = int.Parse(_RobotProgramSubCVer.StringValue);
            _ProgRevision = int.Parse(_RobotProgramRevision.StringValue);
        }

        /// <summary>
        /// Will retrieve features the machine is capable of
        /// NOT YET IMPLEMENTED
        /// </summary>
        private void PopulateVersionFeatures()
        {
            //TODO: add feature in future if required
        }
        

    }
}
