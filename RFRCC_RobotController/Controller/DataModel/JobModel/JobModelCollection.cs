using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Collection of JobModels to be stored and accessed 
    /// </summary>
    public class JobModelCollection : IOrderedEnumerable<JobModel>, IEnumerable<JobModel>, IEnumerator<JobModel>, IList<JobModel>
    {
        // --- INTERNAL FIELDS ---
        List<JobModel> _InternalCollection = new List<JobModel>();
        int _current = 0;
        bool _isReadOnly = false;

        // --- EVENTS ---

        // TODO: Event of Job about to be unloaded from Robot
        // TODO: Event on Job Being ready for Load to Robot

        // --- PROPERTIES ---
        public JobModel this[int index]
        {
            get => _InternalCollection[index];
            set
            {
                if (!_isReadOnly) _InternalCollection[_current] = value;
                else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
            }
        }
        public JobModel Current
        {
            get => _InternalCollection[_current];
            set
            {
                if (!_isReadOnly) _InternalCollection[_current] = value;
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



        // --- METHODS ---
        /// <summary>
        /// Adds an object to the end of the lsit
        /// </summary>
        /// <param name="item">The object to be added to the end of the list<T>. Object can be null for reference types</param>
        public void Add(JobModel item)
        {
            if (!_isReadOnly) _InternalCollection.Add(item);
            else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
        }
        /// <summary>
        /// Removes all elements from the list
        /// </summary>
        public void Clear()
        {
            if (!_isReadOnly) _InternalCollection.Clear();
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
            _current++;
            if (_current == _InternalCollection.Count)
            {
                _current--;
                return false;
            }
            else return true;
        }
        public bool Remove(JobModel item)
        {
            if (!_isReadOnly) return _InternalCollection.Remove(item);
            else
            {
                new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
                return false;
            }
        }
        public void RemoveAt(int index)
        {
            if (!_isReadOnly) _InternalCollection.RemoveAt(index);
            else new Exception("JobModelCollection is set to readonly; edditting of parameters is not allowed");
        }
        public void Reset()
        {
            _current = 0;
        }
        IEnumerator IEnumerable.GetEnumerator() => _InternalCollection.GetEnumerator();

        // --- INTERNAL EVENTS AND AUTOMATION ---















    }

}
