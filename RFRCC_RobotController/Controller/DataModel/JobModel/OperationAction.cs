using System;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    // TODO: make a class of UI Action :: may include an event execution
    // TODO: Action; skippable, stop_alert

    /// <summary>
    /// Action step during processing
    /// </summary>
    public class OperationAction : ICloneable
    {
        private bool _complete = false;
        private bool _skip = false;
        private bool _processing = false;
        
        
        /// <summary>
        /// Key text of the operation action
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Status if Action has been completed
        /// </summary>
        public bool Completed 
        { 
            get => _complete;
            set 
            {
                _complete = value;
                if (_complete)
                {
                    _processing = false;
                    OnActionCompleted();
                }
            } 
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
        /// Status of Actions execution
        /// </summary>
        public bool Processing 
        { 
            get => _processing; 
        }
        /// <summary>
        /// Attributes of the Operation sorted by key and string value
        /// </summary>
        public Dictionary<string,string> Attributes { get; set; }
        /// <summary>
        /// Event when Action is completed
        /// </summary>
        public EventHandler ActionCompleted;
        /// <summary>
        /// Event when Action is started
        /// </summary>
        public EventHandler ActionStarted;
        /// <summary>
        /// Event when skip status of this action is changed
        /// </summary>
        public EventHandler ActionSkipUpdated;
        /// <summary>
        /// Begins Actions Processing and raises start event
        /// </summary>
        public void Start()
        {
            _processing = true;
            OnActionStart();
        }
        public void Pause()
        {
            _processing = false;
            throw new NotImplementedException();
        }
        public void Continue()
        {
            _processing = true;
            throw new NotImplementedException();
        }
        /// <summary>
        /// Stops processing of Action marks complete if action was successful
        /// </summary>
        public void Complete(bool success = true)
        {
            _processing = false;
            Completed = success;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }



        // Internal Event Handling
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
            ActionCompleted?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Raise event ActionSkipUpdated
        /// </summary>
        protected virtual void OnActionSkipUpdated()
        {
            ActionSkipUpdated?.Invoke(this, new EventArgs());
        }

    }
}
