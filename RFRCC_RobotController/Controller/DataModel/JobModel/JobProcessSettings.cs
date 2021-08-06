using RFRCC_RobotController.ABB_Data;
using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class JobProcessSettings
    {
        // --- INTERNAL FIELDS ---
        //TODO: load default speeds
        internal JobModel _parentJob;
        internal CS_speeddata _MoveSpeed;
        internal CS_speeddata _SafeMoveSpeed;
        internal CS_speeddata _TopMoveSpeed;
        internal CS_speeddata _BottomMoveSpeed;
        internal CS_speeddata _FrontMoveSpeed;
        internal CS_speeddata _BackMoveSpeed;
        internal double _StockDXPrecisionTolerance_Positive;
        internal double _StockDXPrecisionTolerance_Negative;

        internal bool _takeMachineSettings = true;

        // --- EVENTS ---
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
        /// If the stock required precision is to change, this event will fire
        /// </summary>
        public event EventHandler StockDXPrecisionToleranceChanged;

        // --- CONSTRUCTORS ---
        /// <summary>
        /// Construct object with link to parent JobModel to be housed in
        /// </summary>
        /// <param name="ParentJob">parent JobModel to be housed in</param>
        public JobProcessSettings(JobModel ParentJob)
        {
            _parentJob = ParentJob;
            _StockDXPrecisionTolerance_Positive = 100;
            _StockDXPrecisionTolerance_Negative = 100;
        }


        // --- PROPERTIES ---
        public MachineProcessSettings MachineSettings 
        {
            get 
            {
                if (_parentJob._parentController != null)
                {
                    return _parentJob._parentController.dataModel.ProcessSettings;
                }
                else throw new Exception("No machine associated with Job");
                
            }
            set
            {
                if (_parentJob._parentController != null)
                {
                    _parentJob._parentController.dataModel.ProcessSettings = value;
                }
                else throw new Exception("No machine associated with Job");
            } 
        }
        /// <summary>
        /// General Move speed of robot between points
        /// </summary>
        public CS_speeddata MoveSpeed 
        {
            get 
            {
                if (_takeMachineSettings) return MachineSettings._MoveSpeed;
                return _MoveSpeed; 
            }
            set 
            {
                _MoveSpeed = value;
                if (_takeMachineSettings) MachineSettings.MoveSpeedChanged -= UpdateMoveSpeed;
                OnMoveSpeedChange();
                if (_takeMachineSettings) MachineSettings.MoveSpeedChanged += UpdateMoveSpeed;
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
                if (_takeMachineSettings) MachineSettings.SafeMoveSpeedChanged -= UpdateSafeMoveSpeed;
                OnSafeMoveSpeedChange();
                if (_takeMachineSettings) MachineSettings.SafeMoveSpeedChanged += UpdateSafeMoveSpeed;
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
        /// Positive tolerance in requested position from Robot for robot manoeuvre
        /// </summary>
        public double StockDXPrecisionTolerance_Positive 
        {
            get
            {
                return _StockDXPrecisionTolerance_Positive;
            }
            set
            {
                if (_StockDXPrecisionTolerance_Positive != value)
                {
                    _StockDXPrecisionTolerance_Positive = value;
                    StockDXPrecisionToleranceChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// Negative tolerance in requested position from Robot for robot manoeuvre
        /// </summary>
        public double StockDXPrecisionTolerance_Negative 
        {
            get
            {
                return _StockDXPrecisionTolerance_Negative;
            }
            set
            {
                if (_StockDXPrecisionTolerance_Negative != value)
                {
                    _StockDXPrecisionTolerance_Negative = value;
                    StockDXPrecisionToleranceChanged?.Invoke(this, new EventArgs());
                }
            } 
        }

        // --- METHODS ---
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
                MachineSettings._updatedJobProcessSettings = value;
                if (value)
                {
                    MachineSettings.MoveSpeedChanged += UpdateMoveSpeed;
                    MachineSettings.SafeMoveSpeedChanged += UpdateSafeMoveSpeed;
                    this.MoveSpeedChanged += MachineSettings.UpdateMoveSpeed;
                    this.SafeMoveSpeedChanged += MachineSettings.UpdateSafeMoveSpeed;
                }
                else
                {
                    MachineSettings.MoveSpeedChanged -= UpdateMoveSpeed;
                    MachineSettings.SafeMoveSpeedChanged -= UpdateSafeMoveSpeed;
                    this.MoveSpeedChanged += MachineSettings.UpdateMoveSpeed;
                    this.SafeMoveSpeedChanged += MachineSettings.UpdateSafeMoveSpeed;
                }
                OnTakeMachineSettingsChange();
            } 
        }

        // --- INTERNAL EVENTS AND AUTOMATION ---
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
                this.MoveSpeedChanged -= MachineSettings.UpdateMoveSpeed;
                OnMoveSpeedChange();
                this.MoveSpeedChanged += MachineSettings.UpdateMoveSpeed;
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
                this.SafeMoveSpeedChanged -= MachineSettings.UpdateSafeMoveSpeed;
                OnSafeMoveSpeedChange();
                this.SafeMoveSpeedChanged += MachineSettings.UpdateSafeMoveSpeed;
            }
            else
            {
                throw new Exception("sender is not of type MachineProcessSettings");
            }
        }
    }
}
