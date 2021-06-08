using System;
using System.Collections;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    public class OperationActionList : IEnumerable<OperationAction>, IEnumerator<OperationAction>, IList<OperationAction>
    {
        private List<OperationAction> _operationActions;
        private bool _ReadOnly;
        private int _index;

        public OperationActionList()
        {
            _ReadOnly = false;
            _index = 0;
            _operationActions = new List<OperationAction>();
        }
        public OperationActionList(List<OperationAction> operationActions)
        {
            _ReadOnly = false;
            _index = 0;
            _operationActions = new List<OperationAction>();
            _operationActions.AddRange(operationActions);
        }
     
        public OperationAction this[int index] => _operationActions[index];

        public OperationAction Current => _operationActions[_index];

        public int Count => _operationActions.Count;

        public bool IsReadOnly 
        {
            get { return _ReadOnly; }
        }

        public void SetReadOnly(bool ReadOnly)
        {
            _ReadOnly = ReadOnly;
        }

        object IEnumerator.Current => _operationActions[_index];

        OperationAction IList<OperationAction>.this[int index] 
        { 
            get => _operationActions[index]; 
            set
            {
                _operationActions[index] = value;
            }
        }

        public void Add(OperationAction item)
        {
            _operationActions.Add(item);
        }
        public void Add(List<OperationAction> items)
        {
            foreach (OperationAction item in items)
            {
                _operationActions.Add(item);
            }
        }

        public void Clear()
        {
            _operationActions.Clear();
        }

        public bool Contains(OperationAction item)
        {
            return _operationActions.Contains(item);
        }

        public IEnumerator<OperationAction> GetEnumerator()
        {
            return _operationActions.GetEnumerator();
        }

        public bool MoveNext()
        {
            if (_index + 1 == _operationActions.Count) return false;
            _index++;
            return true;
        }

        public void Reset()
        {
            _index = 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(OperationAction item)
        {
            return _operationActions.IndexOf(item);
        }

        public void Insert(int index, OperationAction item)
        {
            _operationActions.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _operationActions.RemoveAt(index);
        }

        public void CopyTo(OperationAction[] array, int arrayIndex)
        {
            _operationActions.CopyTo(array, arrayIndex);
        }

        public bool Remove(OperationAction item)
        {
            return _operationActions.Remove(item);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_operationActions).GetEnumerator();
        }
    }
}
