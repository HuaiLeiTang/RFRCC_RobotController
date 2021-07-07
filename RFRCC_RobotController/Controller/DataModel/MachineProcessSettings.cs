using System;
using RFRCC_RobotController.ABB_Data;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class MachineProcessSettings
    {
        //TODO: load default speeds
        internal DataModel _parentMachine;
        internal bool _autoProgressJob;
        internal bool _forcePauseBeforeAbort;
        internal bool _robotIMStopOnPause;
        internal CS_speeddata _MoveSpeed;
        internal CS_speeddata _SafeMoveSpeed;

        internal bool _updatedJobProcessSettings = true;

        /// <summary>
        /// Event when AutoProgressJob has been changed
        /// </summary>
        public event EventHandler AutoProgressJobChanged;
        /// <summary>
        /// Event when ForcePauseBeforeAbort has been changed
        /// </summary>
        public event EventHandler ForcePauseBeforeAbortChanged;
        /// <summary>
        /// Event when RobotIMStopOnPause has been changed
        /// </summary>
        public event EventHandler RobotIMStopOnPauseChanged;
        /// <summary>
        /// Event when MoveSpeed has been changed
        /// </summary>
        public event EventHandler MoveSpeedChanged;
        /// <summary>
        /// Event when SafeMoveSpeed has been changed
        /// </summary>
        public event EventHandler SafeMoveSpeedChanged;


        /// <summary>
        /// construct object with link to parent data model to be housed in
        /// </summary>
        /// <param name="ParentDataModel">parent data model to be housed in</param>
        public MachineProcessSettings(DataModel ParentDataModel)
        {
            _parentMachine = ParentDataModel;
            AutoProgressJob = false;
            ForcePauseBeforeAbort = false;
            RobotIMStopOnPause = true;
        }

        /// <summary>
        /// Status if Object will automatically dispose of current job on completion and upload following job data if available
        /// If false, on completion of job object will wait for manual disposal and loading next job data
        /// </summary>
        public bool AutoProgressJob 
        { 
            get
            {
                return _autoProgressJob;
            }
            set
            {
                _autoProgressJob = value;
                if(value && _parentMachine.CurrentJob != null)
                {
                    _parentMachine.CurrentJob.JobCompleted += _parentMachine.NextJob;
                }
                else if (!value && _parentMachine.CurrentJob != null)
                {
                    _parentMachine.CurrentJob.JobCompleted -= _parentMachine.NextJob;
                }
                OnAutoProgressJobChange();
            } 
        }
        /// <summary>
        /// Status if robotcontroller requires job to be paused before allowing abort process to begin
        /// </summary>
        public bool ForcePauseBeforeAbort
        {
            get
            {
                return _forcePauseBeforeAbort;
            }
            set
            {
                _forcePauseBeforeAbort = value;
                OnForcePauseBeforeAbortChanged();
            }
        }
        /// <summary>
        /// Status if robot is the perform an Immediate Stop if job is paused
        /// If false, robot will complete its current manoeuvre before halting
        /// </summary>
        public bool RobotIMStopOnPause
        {
            get
            {
                return _robotIMStopOnPause;
            }
            set
            {
                _robotIMStopOnPause = value;
                OnRobotIMStopOnPauseChanged();
            }
        }
        /// <summary>
        /// General Move speed of robot between points
        /// </summary>
        public CS_speeddata MoveSpeed
        {
            get
            {
                return _MoveSpeed;

            }
            set
            {
                _MoveSpeed = value;
                OnMoveSpeedChange();
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
                OnSafeMoveSpeedChange();
            }
        }


        // internal methods and events
        virtual protected void OnMoveSpeedChange()
        {
            MoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnSafeMoveSpeedChange()
        {
            SafeMoveSpeedChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnAutoProgressJobChange()
        {
            AutoProgressJobChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnForcePauseBeforeAbortChanged()
        {
            ForcePauseBeforeAbortChanged?.Invoke(this, new EventArgs());
        }
        virtual protected void OnRobotIMStopOnPauseChanged()
        {
            RobotIMStopOnPauseChanged?.Invoke(this, new EventArgs());
        }

        // xlink methods
        internal void UpdateMoveSpeed(object sender, EventArgs args)
        {
            if (sender is JobProcessSettings)
            {
                _MoveSpeed = ((JobProcessSettings)sender)._MoveSpeed;
                OnMoveSpeedChange();
            }
            else
            {
                throw new Exception("sender is not of type JobProcessSettings");
            }
        }
        internal void UpdateSafeMoveSpeed(object sender, EventArgs args)
        {
            if (sender is JobProcessSettings)
            {
                _SafeMoveSpeed = ((JobProcessSettings)sender)._SafeMoveSpeed;
                OnSafeMoveSpeedChange();
            }
            else
            {
                throw new Exception("sender is not of type JobProcessSettings");
            }
        }
    }

}
