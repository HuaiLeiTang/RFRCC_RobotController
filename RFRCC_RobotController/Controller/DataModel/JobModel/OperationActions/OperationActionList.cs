using RFRCC_RobotController.Controller.DataModel.OperationData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    // TODO: Current action is action worked on by robot

    // List of Operation Actions taken during a Job Processing
    public class OperationActionList : IEnumerable<IOperationAction>, IEnumerator<IOperationAction>, IList<IOperationAction>
    {
        // --- INTERNAL FIELDS ---
        private List<IOperationAction> _operationActions;
        private bool _ReadOnly;
        private int _index;
        private double _rollingRequiredStockDX = 0;

        internal event EventHandler ProcessSettingsStockDXPrecisionsToleranceChange;

        // --- EVENTS ---

        public event EventHandler OperationActionRequestPause;
        /// <summary>
        /// The List of OperationActions has changed
        /// </summary>
        public event EventHandler OperationActionsListChanged;
        /// <summary>
        /// an Operation action has been completed
        /// </summary>
        public event EventHandler OperationActionCompleted;
        /// <summary>
        /// An operations skip status has been changed
        /// </summary>
        public event EventHandler OperationSkipUpdated;
        /// <summary>
        /// Current operation has been started
        /// </summary>
        public event EventHandler OperationStarted;
        /// <summary>
        /// Current Process is a PLC enabled process
        /// </summary>
        public event EventHandler PLCProcessRequired;
        /// <summary>
        /// Current Process is a Robot enabled process
        /// </summary>
        public event EventHandler RobotProcessRequired;
        /// <summary>
        /// Call to initiate a robot process
        /// </summary>
        public event CallForProcessEventHandler CallForRobotProcess;
        /// <summary>
        /// Final Operation has been completed or is skipped
        /// </summary>
        public event EventHandler OperationsAllComplete;
        /// <summary>
        /// RequiredStockDX has changed from previous value 
        /// </summary>
        public event EventHandler OperationRequiredStockDXChanged;

        // --- PROPERTIES ---
        public IOperationAction this[int index] => _operationActions[index];
        /// <summary>
        /// Current set Operation Action 
        /// </summary>
        public IOperationAction Current => _operationActions[_index];
        /// <summary>
        /// Number of operation actions
        /// </summary>
        public int Count => _operationActions.Count;
        /// <summary>
        /// if list is editable
        /// </summary>
        public bool IsReadOnly
        {
            get { return _ReadOnly; }
        }

        // --- CONSTRUCTORS ---

        /// <summary>
        /// Initialise object with empty list
        /// </summary>
        public OperationActionList()
        {
            _ReadOnly = false;
            _index = 0;
            _operationActions = new List<IOperationAction>();
        }
        /// <summary>
        /// Initialise object with a list of Operation Actions
        /// </summary>
        /// <param name="operationActions">list of Operation Actions</param>
        public OperationActionList(List<IOperationAction> operationActions) : this()
        {
            _operationActions.AddRange(operationActions);
            foreach (OperationAction Action in _operationActions)
            {
                Action.ActionCompleted += OnOperationActionCompleted;
                Action.ActionSkipUpdated += OnOperationSkipUpdated;
                Action.ActionStarted += OnOperationStarted;
                if (Action is OperationRobotManoeuvre) ((OperationRobotManoeuvre)Action).RequiredStockDXUpdate += OnRobotManoeuvreIdealDXUpdate;
                if (Action is OperationPLCProcess && Action.Attributes.ContainsKey("StockDXPrecisionTolerance_Positive")) ProcessSettingsStockDXPrecisionsToleranceChange += ((OperationPLCProcess)Action).UpdateStockDXPrecisionsTolerance;
            }
        }
        /// <summary>
        /// indexed operation action from list
        /// </summary>
        /// <param name="index">index of operation action</param>
        /// <returns>operation action</returns>
        
        // --- METHODS ---
        // TODO: Implement ReadOnly properly
        /// <summary>
        /// Edit read only status
        /// NOT PROPERLY IMPLEMENTED
        /// </summary>
        /// <param name="ReadOnly">Read Only</param>
        public void SetReadOnly(bool ReadOnly)
        {
            _ReadOnly = ReadOnly;
        }
        /// <summary>
        /// Cuurent indexed Operation Action
        /// </summary>
        object IEnumerator.Current => _operationActions[_index];
        /// <summary>
        /// Operation Action from List
        /// </summary>
        /// <param name="index">index location in list</param>
        /// <returns>Operation Action</returns>
        IOperationAction IList<IOperationAction>.this[int index] 
        { 
            get => _operationActions[index]; 
            set
            {
                _operationActions[index] = value;
            }
        }
        /// <summary>
        /// Add item to list of Operation Actions
        /// </summary>
        /// <param name="item"></param>
        public void Add(IOperationAction item)
        {
            _operationActions.Add(item);
            _operationActions.Last().ActionCompleted += OnOperationActionCompleted;
            _operationActions.Last().ActionSkipUpdated += OnOperationSkipUpdated;
            _operationActions.Last().ActionStarted += OnOperationStarted;
            if (_operationActions.Last() is OperationRobotManoeuvre) ((OperationRobotManoeuvre)_operationActions.Last()).RequiredStockDXUpdate += OnRobotManoeuvreIdealDXUpdate;
            if (_operationActions.Last() is OperationPLCProcess && _operationActions.Last().Attributes.ContainsKey("StockDXPrecisionTolerance_Positive")) ProcessSettingsStockDXPrecisionsToleranceChange += ((OperationPLCProcess)_operationActions.Last()).UpdateStockDXPrecisionsTolerance;
            OnOperationActionsListChanged(this, new EventArgs());
        }
        /// <summary>
        /// Add multiple items to list of operation actions
        /// </summary>
        /// <param name="items">List of items to be added</param>
        public void Add(List<IOperationAction> items)
        {
            foreach (OperationAction item in items)
            {
                _operationActions.Add(item);
                _operationActions.Last().ActionCompleted += OnOperationActionCompleted;
                _operationActions.Last().ActionSkipUpdated += OnOperationSkipUpdated;
                _operationActions.Last().ActionStarted += OnOperationStarted;
                if (_operationActions.Last() is OperationRobotManoeuvre) ((OperationRobotManoeuvre)_operationActions.Last()).RequiredStockDXUpdate += OnRobotManoeuvreIdealDXUpdate;
                if (_operationActions.Last() is OperationPLCProcess && _operationActions.Last().Attributes.ContainsKey("StockDXPrecisionTolerance_Positive")) ProcessSettingsStockDXPrecisionsToleranceChange += ((OperationPLCProcess)_operationActions.Last()).UpdateStockDXPrecisionsTolerance;
            }
            OnOperationActionsListChanged(this, new EventArgs());
        }
        /// <summary>
        /// Clear list of all operation actions
        /// </summary>
        public void Clear()
        {
            foreach (OperationAction Op in _operationActions)
            {
                Op.DisposeEvents();
            }
            _operationActions.Clear();
            OnOperationActionsListChanged(this, new EventArgs());
        }
        /// <summary>
        /// check if operation action is contained in list
        /// </summary>
        /// <param name="item">Operation actiong to check</param>
        /// <returns>If Contains Item checked</returns>
        public bool Contains(IOperationAction item)
        {
            return _operationActions.Contains(item);
        }
        /// <summary>
        /// Get enumerator of operation actions list
        /// </summary>
        /// <returns>Enumorator</returns>
        public IEnumerator<IOperationAction> GetEnumerator()
        {
            return _operationActions.GetEnumerator();
        }
        /// <summary>
        /// index to next operation action in list
        /// </summary>
        /// <returns>if successfull incremented item</returns>
        public bool MoveNext()
        {
            if (Current == _operationActions.Last())
            {
                Current.CurrentAction = false;
                OnOperationsAllComplete();
                return false;
            }

            Current.DisposeEvents();
            _index++;
            while (Current.Skip)
            {
                if (Current is OperationRobotManoeuvre)
                {
                    CallForRobotProcess?.Invoke((OperationRobotManoeuvre)Current, new CallForProcessEventArgs("SkipManoeuvre", true, ((OperationRobotManoeuvre)Current).featureData.FeatureHeader.FeatureNum));
                }
                if (Current == _operationActions.Last())
                {
                    OnOperationsAllComplete();
                    return false;
                }
                Current.DisposeEvents();
                _index++;
            }

            Current.CurrentAction = true;

            if (Current.PauseOn)
            {
                //machine pause
                OnOperationActionRequestPause(this, new EventArgs());
            }
            
            if (Current.GetType().GetProperty("RequiredStockDX") != null && _rollingRequiredStockDX != double.Parse((Current.GetType().GetProperty("RequiredStockDX").GetValue(Current, null)).ToString()))
            {
                double check = double.Parse((Current.GetType().GetProperty("RequiredStockDX").GetValue(Current, null)).ToString());
                if (check != 99999)
                {
                    _rollingRequiredStockDX = check;
                    OperationRequiredStockDXChanged?.Invoke(Current, new EventArgs());
                }
            }
            if (Current is OperationPLCProcess)
            {
                OnPLCProcessRequired(Current, new EventArgs());
            } 
            else if (Current is OperationRobotManoeuvre || Current is OperationRobotProcess)
            {
                OnRobotProcessRequired(Current, new EventArgs());
            }
            return true;
        }
        /// <summary>
        /// NOT IMPLEMENTED YET
        /// move action to index of actionlist
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetCurrentActionTo(int index)
        {
            throw new NotImplementedException();
            // if machine pause, pause action before set to current
        }

        /// <summary>
        /// Reset index lacation back to 0
        /// </summary>
        public void Reset()
        {
            _index = 0;
        }
        /// <summary>
        /// realease all memory allocated to Operation Actions held within
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Find the index of an Operation action if housed in object Operation Actions list
        /// </summary>
        /// <param name="item">Item to be indexed</param>
        /// <returns>index of item</returns>
        public int IndexOf(IOperationAction item)
        {
            return _operationActions.IndexOf(item);
        }
        /// <summary>
        /// Inset item at indexed location
        /// </summary>
        /// <param name="index">Index at which to be inserted</param>
        /// <param name="item">Operation Action to be inserted</param>
        public void Insert(int index, IOperationAction item)
        {
            _operationActions.Insert(index, item);
            _operationActions[index].ActionCompleted += OnOperationActionCompleted;
            _operationActions[index].ActionSkipUpdated += OnOperationSkipUpdated;
            _operationActions[index].ActionStarted += OnOperationStarted;
            if (_operationActions[index] is OperationRobotManoeuvre) ((OperationRobotManoeuvre)_operationActions[index]).RequiredStockDXUpdate += OnRobotManoeuvreIdealDXUpdate;
            if (_operationActions[index] is OperationPLCProcess && _operationActions[index].Attributes.ContainsKey("StockDXPrecisionTolerance_Positive")) ProcessSettingsStockDXPrecisionsToleranceChange += ((OperationPLCProcess)_operationActions[index]).UpdateStockDXPrecisionsTolerance;

            OnOperationActionsListChanged(this, new EventArgs());

        }
        /// <summary>
        /// Remove item from list at index location
        /// </summary>
        /// <param name="index">index of item to be removed</param>
        public void RemoveAt(int index)
        {
            _operationActions[index].ActionCompleted -= OnOperationActionCompleted;
            _operationActions[index].ActionSkipUpdated -= OnOperationSkipUpdated;
            _operationActions[index].ActionStarted -= OnOperationStarted;
            if (_operationActions[index] is OperationRobotManoeuvre) ((OperationRobotManoeuvre)_operationActions[index]).RequiredStockDXUpdate -= OnRobotManoeuvreIdealDXUpdate;
            if (_operationActions[index] is OperationPLCProcess && _operationActions[index].Attributes.ContainsKey("StockDXPrecisionTolerance_Positive")) ProcessSettingsStockDXPrecisionsToleranceChange -= ((OperationPLCProcess)_operationActions[index]).UpdateStockDXPrecisionsTolerance;
            _operationActions.RemoveAt(index);
            OnOperationActionsListChanged(this, new EventArgs());
        }
        /// <summary>
        /// Copy indexed item from list to the end of an array
        /// </summary>
        /// <param name="array">Array for item to be added to</param>
        /// <param name="arrayIndex">Index of item to be copied</param>
        public void CopyTo(IOperationAction[] array, int arrayIndex)
        {
            _operationActions.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Remove item from list if it is found
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>if found and removed</returns>
        public bool Remove(IOperationAction item)
        {
            _operationActions[_operationActions.IndexOf(item)].ActionCompleted += OnOperationActionCompleted;
            _operationActions[_operationActions.IndexOf(item)].ActionSkipUpdated += OnOperationSkipUpdated;
            _operationActions[_operationActions.IndexOf(item)].ActionStarted += OnOperationStarted;
            var ans = _operationActions.Remove(item);
            OnOperationActionsListChanged(this, new EventArgs());
            return ans;

        }
        /// <summary>
        /// Enumerator of OperationActions list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_operationActions).GetEnumerator();
        }

        public void DisposeEvents()
        {
            ProcessSettingsStockDXPrecisionsToleranceChange = null;
            OperationActionRequestPause = null;
            OperationActionsListChanged = null;
            OperationActionCompleted = null;
            OperationSkipUpdated = null;
            OperationStarted = null;
            PLCProcessRequired = null;
            RobotProcessRequired = null;
            CallForRobotProcess = null;
            OperationsAllComplete = null;
            OperationRequiredStockDXChanged = null;
        }

        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void OnOperationActionRequestPause(object sender, EventArgs args)
        {
            OperationActionRequestPause?.Invoke(sender, args);
        }
        protected virtual void OnOperationActionsListChanged(object sender, EventArgs args)
        {
            OperationActionsListChanged?.Invoke(sender, args);
        }
        protected virtual void OnOperationActionCompleted(object sender, EventArgs args)
        {
            if (sender == Current)
            {
                this.MoveNext();
            }
            OperationActionCompleted?.Invoke(sender, args);
        }
        protected virtual void OnOperationSkipUpdated(object sender, EventArgs args)
        {
            OperationSkipUpdated?.Invoke(sender, args);
        }
        protected virtual void OnOperationStarted(object sender, EventArgs args)
        {
            OperationStarted?.Invoke(sender, args);
        }
        protected virtual void OnPLCProcessRequired(object sender, EventArgs args)
        {
            PLCProcessRequired?.Invoke(sender, args);
        }
        protected virtual void OnRobotProcessRequired(object sender, EventArgs args)
        {
            RobotProcessRequired?.Invoke(sender, args);
        }
        protected virtual void OnOperationsAllComplete()
        {
            OperationsAllComplete?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// This will fire when an OperationRobotManoeuvre updates the required DX, and thus, the previous PLC process must be updated to reflect this requirement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnRobotManoeuvreIdealDXUpdate(OperationRobotManoeuvre sender, EventArgs args)
        {
            int i = 0;
            if (_operationActions[_operationActions.IndexOf(sender) - 1] is OperationPLCProcess && _operationActions[_operationActions.IndexOf(sender) - 1].Name == "Drive Stock to next position")
            {
                _operationActions[_operationActions.IndexOf(sender) - 1].Attributes["RequiredStockDX"] = sender.RequiredStockDX.ToString();
            }
        }
        internal virtual void OnProcessSettingsStockDXPrecisionsToleranceChange(object sender, EventArgs args)
        {
            if (sender is JobProcessSettings)
            {
                // send min and max values to the update task
                double max = ((JobProcessSettings)sender)._StockDXPrecisionTolerance_Positive;
                double min = ((JobProcessSettings)sender)._StockDXPrecisionTolerance_Negative;
                ProcessSettingsStockDXPrecisionsToleranceChange?.Invoke(new Tuple<Double, Double>(min, max), new EventArgs());
            }
            else
            {
                // TODO: handle is sender isnt process settings
            }
        }
    }
}
