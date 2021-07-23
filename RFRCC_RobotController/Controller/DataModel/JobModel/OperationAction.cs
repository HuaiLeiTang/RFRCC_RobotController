using System;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    // TODO: make a class of UI Action :: may include an event execution
    // TODO: Action; skippable, stop_alert

    /// <summary>
    /// Action step during processing
    /// </summary>
    public class OperationAction : ICloneable, IOperationAction
    {

        // --- PRIVATE PROPERTIES ---
        private bool _complete = false;
        private bool _skip = false;
        private bool _processing = false;
        private bool _paused = false;
        private bool _pauseOn = false;
        private bool _currentAction = false;


        // --- EVENTS ---
        /// <summary>
        /// Event when Action is completed
        /// </summary>
        public event EventHandler ActionCompleted;
        /// <summary>
        /// Event when Action is started
        /// </summary>
        public event EventHandler ActionStarted;
        /// <summary>
        /// Event when skip status of this action is changed
        /// </summary>
        public event EventHandler ActionSkipUpdated;
        /// <summary>
        /// Event when PauseOn status of this action is changed
        /// </summary>
        public event EventHandler ActionPauseOnUpdated;
        /// <summary>
        /// Event when Action is Paused
        /// </summary>
        public event EventHandler ActionPaused;
        /// <summary>
        /// Event when Action is Coninued
        /// </summary>
        public event EventHandler ActionContinued;
        /// <summary>
        /// Event when Action is Canceled
        /// </summary>
        public event EventHandler ActionCanceled;



        // --- PUBLIC PROPERTIES ---
        /// <summary>
        /// Key text of the operation action
        /// </summary>
        public string Name { get; set; }
        public bool CurrentAction
        {
            get
            {
                return _currentAction;
            }
            set
            {
                _currentAction = value;
            }
        }
        /// <summary>
        /// Status if Action has been completed
        /// </summary>
        public bool Completed 
        { 
            get => _complete;
        }
        /// <summary>
        /// Status of Action is to be skipped
        /// </summary>
        public bool Skip 
        {
            get => _skip;
            set 
            {
                _skip = value;
                OnActionSkipUpdated();
            } 
        }
        /// <summary>
        /// Action Will force a system pause when it becomes the Current Action
        /// </summary>
        public bool PauseOn
        {
            get
            {
                return _pauseOn;
            }
            set
            {
                _pauseOn = value;
            }
        }
        /// <summary>
        /// Status of Actions execution, if process has been started successfully
        /// </summary>
        public bool Processing 
        { 
            get => _processing; 
        }
        /// <summary>
        /// Attributes of the Operation sorted by key and string value
        /// </summary>
        public Dictionary<string,string> Attributes { get; set; }

        // --- CONSTRUCTORS ---


        // --- METHODS ---
        /// <summary>
        /// Begins Actions Processing and raises start event
        /// </summary>
        public void Start()
        {
            if (_skip) throw new Exception("Current Action has been skipped, please change status of skip on current action, or move to next action");
            if (_paused)
            {
                OnActionPaused();
                throw new Exception("Process cannont be restarted with Start(), please use Continue()");
            }
            if (_pauseOn)
            {
                Pause();
                return;
            }

            _processing = true;
            OnActionStart();
        }
        /// <summary>
        /// Action Paused and all subscribed objects notified
        /// </summary>
        public void Pause()
        {
            if (!_paused)
            {
                _paused = true;
                OnActionPaused();
            }
        }
        /// <summary>
        /// Action Continued from pause and all subscribed objects notified
        /// </summary>
        public void Continue()
        {
            if (!_paused) throw new Exception("Process cannont be started with Continue(), please use Start()");
            _paused = false;
            OnActionContinued();
        }
        /// <summary>
        /// Stops processing of Action marks complete if action was successful
        /// </summary>
        public void Complete(bool success = true)
        {
            _processing = false;
            _complete = success;
            OnActionCompleted();
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }



        // --- Internal Event Handling ---
        /// <summary>
        /// raise event ActionStarted if Action.Processing == true
        /// </summary>
        protected virtual void OnActionStart()
        {
            if (_processing)
            {
                ActionStarted?.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// Raise event ActionFinished
        /// </summary>
        protected virtual void OnActionCompleted()
        {
            if (Completed) ActionCompleted?.Invoke(this, new EventArgs());
            else ActionCanceled?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Raise event ActionSkipUpdated
        /// </summary>
        protected virtual void OnActionSkipUpdated()
        {
            ActionSkipUpdated?.Invoke(this, new EventArgs());
        }
        protected virtual void OnActionPauseOnUpdated()
        {
            ActionPauseOnUpdated?.Invoke(this, new EventArgs());
        }
        protected virtual void OnActionPaused()
        {
            ActionPaused?.Invoke(this, new EventArgs());
        }
        protected virtual void OnActionContinued()
        {
            ActionContinued?.Invoke(this, new EventArgs());
        }


    }
}
