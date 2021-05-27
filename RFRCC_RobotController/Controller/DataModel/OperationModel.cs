using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RFRCC_RobotController.Controller.DataModel
{
    /* TODO:
     *      - add 'job header' to encapsulate job information
     *      - add method to input data obtained from import process
     *      - add template of job execution (including PLC requirements and such)
     *      - 
     */
    public class OperationModel
    {
        private RobotController _parentController;
        private bool _controllerPresent = false;
        private string filename;
        private string filepath;
        private OperationActionList _operationActions = new OperationActionList();
        private int _NumFeatures;
        private bool _StartedProcessing;
        private bool _FinishedProcessing;
        private bool _ReadyforProcessing;




        public string ProjectStatus { get; set; }

        public OperationModel()
        {

        }
        public OperationModel(RobotController ParentController, int index = -1)
        {
            _parentController = ParentController;
            _controllerPresent = true;
            // TODO: check connection active and associate this file with controller

            if (index == -1) _parentController.dataModel.Operations.Add(this);
            else _parentController.dataModel.Operations.Insert(index, this);
        }
        public bool ConnectParentController(RobotController ParentController, int index = -1)
        {
            if (_controllerPresent)
            {
                // TODO: check if need to do extra steps to change pointer
                _parentController = ParentController;
                // TODO: check connection active and associate this file with controller
            }
            else
            {
                _parentController = ParentController;
                _controllerPresent = true;
                // TODO: check connection active and associate this file with controller
            }

            if (index == -1) _parentController.dataModel.Operations.Add(this);
            else _parentController.dataModel.Operations.Insert(index, this);
            return true; // return false if failed to connect
        }
    }

    /* TODO:
     *      - add 'attributes' dict or key list (refer operations produced)
     *      -
     */
    public class OperationAction : ICloneable
    {
        public string Name { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

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

    public class OperationRobotManoeuvre : OperationAction
    {
        
    }

    public class OperationPLCProcess : OperationAction
    {
        
    }

    // This class will be used as a placehold in a job execution templating method. This will house attributes and expected values in order to structure the population of a 'job'
    public class OperationTemplateAction : OperationAction
    {
        public object ExpectedOperationType { get; set; } //maybe enum? instead
    }
}
