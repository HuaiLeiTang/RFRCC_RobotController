using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Collection of JobModels to be stored and accessed 
    /// </summary>
    public class JobModelCollection : IOrderedEnumerable<JobModel>, IEnumerable<JobModel>, IEnumerator<JobModel>, IList<JobModel>, ICollection<JobModel>
    {
        // --- INTERNAL FIELDS ---
        internal RobotController _parentController;
        internal List<JobModel> _InternalCollection = new List<JobModel>();
        internal bool _isReadOnly = false;

        // --- EVENTS ---

        public event EventHandler JobAdded;
        public event EventHandler CurrentJobUpdated;

        // --- PROPERTIES ---
        public JobModel this[int index]
        {
            get => _InternalCollection[index];
            set
            {
                if (!_isReadOnly) _InternalCollection[index] = value;
                else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
            }
        }
        public JobModel Current
        {
            get => _InternalCollection[0];
            set
            {
                if (!_isReadOnly) _InternalCollection[0] = value;
                else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
            }
        }
        public int Count
        {
            get => _InternalCollection.Count;
        }
        public bool IsReadOnly
        {
            get => _isReadOnly;
        }
        object IEnumerator.Current => Current;

        // --- CONSTRUCTORS ---
        public JobModelCollection(RobotController ParentController)
        {
            _parentController = ParentController;
            JobAdded += CheckCurrentJob;
            _InternalCollection.Add(null);
        }
        public JobModelCollection(RobotController ParentController, List<JobModel> Jobs) : this(ParentController)
        {
            _InternalCollection.AddRange(Jobs);
        }


        // --- METHODS ---
        /// <summary>
        /// Adds an object to the end of the lsit
        /// </summary>
        /// <param name="item">The object to be added to the end of the list<T>. Object can be null for reference types</param>
        public void Add(JobModel item)
        {
            if (!_isReadOnly)
            {
                _InternalCollection.Add(item);
                if (Current == null) RemoveAt(0);
                JobAdded?.Invoke(_InternalCollection.Last(), new EventArgs());
            }
            else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
        }
        public void AddRange(IEnumerable<JobModel> items)
        {
            _InternalCollection.AddRange(items);
            if (Current == null) _InternalCollection.RemoveAt(0);
            JobAdded?.Invoke(items, new EventArgs());
        }
        /// <summary>
        /// Removes all elements from the list
        /// </summary>
        public void Clear()
        {
            if (!_isReadOnly)
            {
                Current.DisconnectFromController();
                _InternalCollection.Clear();
                _InternalCollection.Add(null);
            }
            else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
        }
        public bool Contains(JobModel item) => _InternalCollection.Contains(item);
        public void CopyTo(JobModel[] array, int arrayIndex) => _InternalCollection.CopyTo(array, arrayIndex);
        public IOrderedEnumerable<JobModel> CreateOrderedEnumerable<TKey>(Func<JobModel, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            return descending ?
                _InternalCollection.OrderByDescending(keySelector, comparer)
                : _InternalCollection.OrderBy(keySelector, comparer);
        }
        public void Dispose()
        {
            Current.DisconnectFromController();
            _InternalCollection.Clear();
            // Disponse Managed Resources

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        public IEnumerator<JobModel> GetEnumerator() => _InternalCollection.GetEnumerator();
        public int IndexOf(JobModel item) => _InternalCollection.IndexOf(item);
        public void Insert(int index, JobModel item)
        {
            if (!_isReadOnly) _InternalCollection.Insert(index, item);
            else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
        }
        public bool MoveNext()
        {
            if (Current != null) Current.DisconnectFromController();
            _InternalCollection.RemoveAt(0);

            if (Current != null)
            {
                Current.OperationRobotMoveData.ConnectParentController(_parentController, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
                Current.ConnectToController();
                CurrentJobUpdated?.Invoke(Current, new EventArgs());
                return true;
            }
            else if (_InternalCollection.Count < 1)
            {
                _InternalCollection.Add(null);
            }

            return false; // no more jobs left in register
        }
        public bool Remove(JobModel item)
        {
            if (!_isReadOnly)
            {
                if (_InternalCollection.IndexOf(item) == 0)
                {
                    MoveNext();
                    return true;
                }
                else return _InternalCollection.Remove(item);
            }
            else
            {
                new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
                return false;
            }
        }
        public void RemoveAt(int index)
        {
            if (!_isReadOnly)
            {
                if (index == 0) MoveNext();
                else _InternalCollection.RemoveAt(index);
            }   
            else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
        }
        /// <summary>
        /// Function only placeholder, as current is always the leading job, there is no reset function
        /// </summary>
        public void Reset()
        {
            
        }
        IEnumerator IEnumerable.GetEnumerator() => _InternalCollection.GetEnumerator();

        // --- INTERNAL EVENTS AND AUTOMATION ---
        protected virtual void CheckCurrentJob(object sender = null, EventArgs args = null)
        {
            if (Current != null && !(Current.CurrentJob))
            {
                Current.OperationRobotMoveData.ConnectParentController(_parentController, "PC_Manoeuvre_Register", "OpManPCBuffer", "PC_Manoeuvre_Register", "OpHeadPCBuffer");
                Current.ConnectToController();

                CurrentJobUpdated?.Invoke(Current, new EventArgs());
            }
        }

    }

}
