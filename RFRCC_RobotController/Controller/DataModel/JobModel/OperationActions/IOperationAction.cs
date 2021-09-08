using System;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    public interface IOperationAction
    {

        public event EventHandler InternalAbortEvent;

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


        public string Name { get; set; }
        public bool CurrentAction { get; set; }
        /// <summary>
        /// Status if Action has been completed
        /// </summary>
        public bool Completed { get; }
        /// <summary>
        /// Status of Action is to be skipped
        /// </summary>
        public bool Skip { get; set; }
        /// <summary>
        /// Action Will force a system pause when it becomes the Current Action
        /// </summary>
        public bool PauseOn { get; set; }
        /// <summary>
        /// Status of Actions execution, if process has been started successfully
        /// </summary>
        public bool Processing { get; }
        /// <summary>
        /// If Current Action is paused in processing
        /// </summary>
        public bool Paused { get; }
        /// <summary>
        /// Attributes of the Operation sorted by key and string value
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        // --- CONSTRUCTORS ---


        // --- METHODS ---
        /// <summary>
        /// Begins Actions Processing and raises start event
        /// </summary>
        public void Start();
        /// <summary>
        /// Action Paused and all subscribed objects notified
        /// </summary>
        public void Pause();
        /// <summary>
        /// Action Continued from pause and all subscribed objects notified
        /// </summary>
        public void Continue();
        /// <summary>
        /// Stops processing of Action marks complete if action was successful
        /// </summary>
        public void Complete(bool success = true);
        public object Clone();

        public void DisposeEvents();

    }
}