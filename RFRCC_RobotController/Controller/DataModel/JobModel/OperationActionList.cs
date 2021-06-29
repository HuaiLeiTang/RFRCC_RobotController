using System;
using System.Collections;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    // TODO: Current action is action worked on by robot

    // List of Operation Actions taken during a Job Processing
    public class OperationActionList : IEnumerable<OperationAction>, IEnumerator<OperationAction>, IList<OperationAction>
    {
        private List<OperationAction> _operationActions;
        private bool _ReadOnly;
        private int _index;

        /// <summary>
        /// Initialise object with empty list
        /// </summary>
        public OperationActionList()
        {
            _ReadOnly = false;
            _index = 0;
            _operationActions = new List<OperationAction>();
        }
        /// <summary>
        /// Initialise object with a list of Operation Actions
        /// </summary>
        /// <param name="operationActions">list of Operation Actions</param>
        public OperationActionList(List<OperationAction> operationActions)
        {
            _ReadOnly = false;
            _index = 0;
            _operationActions = new List<OperationAction>();
            _operationActions.AddRange(operationActions);
        }
        /// <summary>
        /// indexed operation action from list
        /// </summary>
        /// <param name="index">index of operation action</param>
        /// <returns>operation action</returns>
        public OperationAction this[int index] => _operationActions[index];
        /// <summary>
        /// Current set Operation Action 
        /// </summary>
        public OperationAction Current => _operationActions[_index];
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
        OperationAction IList<OperationAction>.this[int index] 
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
        public void Add(OperationAction item)
        {
            _operationActions.Add(item);
        }
        /// <summary>
        /// Add multiple items to list of operation actions
        /// </summary>
        /// <param name="items">List of items to be added</param>
        public void Add(List<OperationAction> items)
        {
            foreach (OperationAction item in items)
            {
                _operationActions.Add(item);
            }
        }
        /// <summary>
        /// Clear list of all operation actions
        /// </summary>
        public void Clear()
        {
            _operationActions.Clear();
        }
        /// <summary>
        /// check if operation action is contained in list
        /// </summary>
        /// <param name="item">Operation actiong to check</param>
        /// <returns>If Contains Item checked</returns>
        public bool Contains(OperationAction item)
        {
            return _operationActions.Contains(item);
        }
        /// <summary>
        /// Get enumerator of operation actions list
        /// </summary>
        /// <returns>Enumorator</returns>
        public IEnumerator<OperationAction> GetEnumerator()
        {
            return _operationActions.GetEnumerator();
        }
        /// <summary>
        /// index to next operation action in list
        /// </summary>
        /// <returns>if successfull incremented item</returns>
        public bool MoveNext()
        {
            if (_index + 1 == _operationActions.Count) return false;
            _index++;
            return true;
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
        public int IndexOf(OperationAction item)
        {
            return _operationActions.IndexOf(item);
        }
        /// <summary>
        /// Inset item at indexed location
        /// </summary>
        /// <param name="index">Index at which to be inserted</param>
        /// <param name="item">Operation Action to be inserted</param>
        public void Insert(int index, OperationAction item)
        {
            _operationActions.Insert(index, item);
        }
        /// <summary>
        /// Remove item from list at index location
        /// </summary>
        /// <param name="index">index of item to be removed</param>
        public void RemoveAt(int index)
        {
            _operationActions.RemoveAt(index);
        }
        /// <summary>
        /// Copy indexed item from list to the end of an array
        /// </summary>
        /// <param name="array">Array for item to be added to</param>
        /// <param name="arrayIndex">Index of item to be copied</param>
        public void CopyTo(OperationAction[] array, int arrayIndex)
        {
            _operationActions.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Remove item from list if it is found
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>if found and removed</returns>
        public bool Remove(OperationAction item)
        {
            return _operationActions.Remove(item);
        }
        /// <summary>
        /// Enumerator of OperationActions list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_operationActions).GetEnumerator();
        }
    }
}
