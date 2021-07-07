using RFRCC_RobotController.ABB_Data;
using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class JobProcessSettings
    {
        //TODO: load default speeds
        internal JobModel _parentJob;
        internal CS_speeddata _MoveSpeed;
        internal CS_speeddata _SafeMoveSpeed;
        internal CS_speeddata _TopMoveSpeed;
        internal CS_speeddata _BottomMoveSpeed;
        internal CS_speeddata _FrontMoveSpeed;
        internal CS_speeddata _BackMoveSpeed;

        internal bool _takeMachineSettings = true;

        /// <summary>
        /// Event when MoveSpeed has been changed
        /// </summary>
        public event EventHandler MoveSpeedChanged;
        /// <summary>
        /// Event when SafeMoveSpeed has been changed
        /// </summary>
        public event EventHandler SafeMoveSpeedChanged;
        /// <summary>
        /// Event when TopMoveSpeed has been changed
        /// </summary>
        public event EventHandler TopMoveSpeedChanged;
        /// <summary>
        /// Event when BottomMoveSpeed has been changed
        /// </summary>
        public event EventHandler BottomMoveSpeedChanged;
        /// <summary>
        /// Event when FrontMoveSpeed has been changed
        /// </summary>
        public event EventHandler FrontMoveSpeedChanged;
        /// <summary>
        /// Event when BackMoveSpeed has been changed
        /// </summary>
        public event EventHandler BackMoveSpeedChanged;
        /// <summary>
        /// Event when TakeMachineSettings has been changed
        /// </summary>
        public event EventHandler TakeMachineSettingsChanged;

        /// <summary>
        /// Construct object with link to parent JobModel to be housed in
        /// </summary>
        /// <param name="ParentJob">parent JobModel to be housed in</param>
        public JobProcessSettings(JobModel ParentJob)
        {
            _parentJob = ParentJob;
        }

        /// <summary>
        /// General Move speed of robot between points
        /// </summary>
        public CS_speeddata MoveSpeed 
        {
            get 
            {
                if (_takeMachineSettings) return _parentJob._parentController.dataModel.ProcessSettings._MoveSpeed;
                return _MoveSpeed; 
            }
            set 
            {
                _MoveSpeed = value;
                if (_takeMachineSettings) _parentJob._parentController.dataModel.ProcessSettings.MoveSpeedChanged -= UpdateMoveSpeed;
                OnMoveSpeedChange();
                if (_takeMachineSettings) _parentJob._parentController.dataModel.ProcessSettings.MoveSpeedChanged += UpdateMoveSpeed;
            }
        }
        /// <summary>
        /// Movement speed to final positions at safe speed
        /// </summary>
        public CS_speeddata SafeMoveSpeed
        {
            get
            {
                return _SafeMoveSpeed;
            }
            set
            {
                _SafeMoveSpeed = value;
                if (_takeMachineSettings) _parentJob._parentController.dataModel.ProcessSettings.SafeMoveSpeedChanged -= UpdateSafeMoveSpeed;
                OnSafeMoveSpeedChange();
                if (_takeMachineSettings) _parentJob._parentController.dataModel.ProcessSettings.SafeMoveSpeedChanged += UpdateSafeMoveSpeed;
            }
        }
        /// <summary>
        /// General Cutting speed on top surface of stock based on top surface thickness and tooling
        /// </summary>
        public CS_speeddata TopMoveSpeed
        {
            get
            {
                return _TopMoveSpeed;
            }
            set
            {
                _TopMoveSpeed = value;
                OnTopMoveSpeedChange();
            }
        }
        /// <summary>
        /// General Cutting speed on bottom surface of stock based on bottom surface thickness and tooling
        /// </summary>
        public CS_speeddata BottomMoveSpeed
        {
            get
            {
                return _BottomMoveSpeed;
            }
            set
            {
                _BottomMoveSpeed = value;
                OnBottomMoveSpeedChange();
            }
        }
        /// <summary>
        /// General Cutting speed on front surface of stock based on front surface thickness and tooling
        /// </summary>
        public CS_speeddata FrontMoveSpeed
        {
            get
            {
                return _FrontMoveSpeed;
            }
            set
            {
                _FrontMoveSpeed = value;
                OnFrontMoveSpeedChange();
            }
        }
        /// <summary>
        /// General Cutting speed on back surface of stock based on back surface thickness and tooling
        /// </summary>
        public CS_speeddata BackMoveSpeed
        {
            get
            {
                return _BackMoveSpeed;
            }
            set
            {
                _BackMoveSpeed = value;
                OnBackMoveSpeedChange();
            }
        }
        /// <summary>
        /// setting to enable connection to enable connection of Move and Safemove to Machine held values
        /// </summary>
        public bool TakeMachineSettings 
        {
            get 
            {
                return _takeMachineSettings; 
            }
            set 
            {
                _takeMachineSettings = value;
                _parentJob._parentController.dataModel.ProcessSettings._updatedJobProcessSettings = value;
                if (value)
                {
                    _parentJob._parentController.dataModel.ProcessSettings.MoveSpeedChanged += UpdateMoveSpeed;
                    _parentJob._parentController.dataModel.ProcessSettings.SafeMoveSpeedChanged += UpdateSafeMoveSpeed;
                    this.MoveSpeedChanged += _parentJob._parentController.dataModel.ProcessSettings.UpdateMoveSpeed;
                    this.SafeMoveSpeedChanged += _parentJob._parentController.dataModel.ProcessSettings.UpdateSafeMoveSpeed;
                }
                else
                {
                    _parentJob._parentController.dataModel.ProcessSettings.MoveSpeedChanged -= UpdateMoveSpeed;
                    _parentJob._parentController.dataModel.ProcessSettings.SafeMoveSpeedChanged -= UpdateSafeMoveSpeed;
                    this.MoveSpeedChanged += _parentJob._parentController.dataModel.ProcessSettings.UpdateMoveSpeed;
                    this.SafeMoveSpeedChanged += _parentJob._parentController.dataModel.ProcessSettings.UpdateSafeMoveSpeed;
                }
                OnTakeMachineSettingsChange();
            } 
        }

        // internal events
        virtual protected void OnMoveSpeedChange()
        {
            MoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnSafeMoveSpeedChange()
        {
            SafeMoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnBottomMoveSpeedChange()
        {
            BottomMoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnTopMoveSpeedChange()
        {
            TopMoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnFrontMoveSpeedChange()
        {
            FrontMoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnBackMoveSpeedChange()
        {
            BackMoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnTakeMachineSettingsChange()
        {
            TakeMachineSettingsChanged?.Invoke(this, new EventArgs());
        }

        // xlink methods
        internal void UpdateMoveSpeed(object sender, EventArgs args)
        {
            if (sender is MachineProcessSettings)
            {
                _MoveSpeed = ((MachineProcessSettings)sender)._MoveSpeed;
                this.MoveSpeedChanged -= _parentJob._parentController.dataModel.ProcessSettings.UpdateMoveSpeed;
                OnMoveSpeedChange();
                this.MoveSpeedChanged += _parentJob._parentController.dataModel.ProcessSettings.UpdateMoveSpeed;
            }
            else
            {
                throw new Exception("sender is not of type MachineProcessSettings");
            }
        }
        internal void UpdateSafeMoveSpeed(object sender, EventArgs args)
        {
            if (sender is MachineProcessSettings)
            {
                _SafeMoveSpeed = ((MachineProcessSettings)sender)._SafeMoveSpeed;
                this.SafeMoveSpeedChanged -= _parentJob._parentController.dataModel.ProcessSettings.UpdateSafeMoveSpeed;
                OnSafeMoveSpeedChange();
                this.SafeMoveSpeedChanged += _parentJob._parentController.dataModel.ProcessSettings.UpdateSafeMoveSpeed;
            }
            else
            {
                throw new Exception("sender is not of type MachineProcessSettings");
            }
        }
    }
}
