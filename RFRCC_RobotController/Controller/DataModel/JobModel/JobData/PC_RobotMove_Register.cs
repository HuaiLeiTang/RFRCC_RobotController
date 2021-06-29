using System;
using System.Collections;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    /// <summary>
    /// Register of robot moves relating to a feature to be cut
    /// </summary>
    public partial class PC_RobotMove_Register : IEnumerable, IEnumerator
    {
        private List<RobotComputedFeatures> _ComputedFeatures = new List<RobotComputedFeatures>();
        private int _current;
        /// <summary>
        /// Number of completed moves done by the robot
        /// </summary>
        public int NumberOfFeaturesComplete
        {
            get
            {
                return _ComputedFeatures.Count;
            }
        }
        /// <summary>
        /// List of computed features
        /// </summary>
        public List<RobotComputedFeatures> ComputedFeatures => _ComputedFeatures;
        // TODO: make this function work
        /// <summary>
        /// Number of comupted features
        /// </summary>
        public int Count => _ComputedFeatures.Count;
        /// <summary>
        /// current computed feature indicated
        /// </summary>
        public object Current => _ComputedFeatures[_current];
        /// <summary>
        /// intialise object with list of computed features
        /// </summary>
        /// <param name="computedFeatures"></param>
        public PC_RobotMove_Register(List<RobotComputedFeatures> computedFeatures)
        {
            _ComputedFeatures = computedFeatures;
        }
        /// <summary>
        /// initialise empty listed object
        /// </summary>
        public PC_RobotMove_Register()
        {
        }
        /// <summary>
        /// retrieve object by index number
        /// </summary>
        /// <param name="FeatureNum">index location</param>
        /// <returns>Computed feature at index indicated</returns>
        public RobotComputedFeatures Feature(int FeatureNum)
        {
            if (FeatureNumberInComputedFeatures(FeatureNum))
            {
                for (int i = 0; i < _ComputedFeatures.Count; i++)
                {
                    if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                        return _ComputedFeatures[i];
                }
                return new RobotComputedFeatures();
            }
            else
                return new RobotComputedFeatures();
        }
        /// <summary>
        /// Add item to end of Computed feature list
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void Add(RobotComputedFeatures item)
        {
            _ComputedFeatures.Add(item);
        }
        /// <summary>
        /// Add multiple items to list of Computed Deatures
        /// </summary>
        /// <param name="List">List of items to be added</param>
        public void AddList(List<RobotComputedFeatures> List)
        {
            _ComputedFeatures.AddRange(List);
        }
        /// <summary>
        /// removes all items and updates list with provided items
        /// </summary>
        /// <param name="List">List of items to replace current list</param>
        public void FromList(List<RobotComputedFeatures> List)
        {
            Clear();
            _ComputedFeatures.AddRange(List);
        }
        /// <summary>
        /// remove all items from list
        /// </summary>
        public void Clear()
        {
            _ComputedFeatures.Clear();
        }
        /// <summary>
        /// Returns Computed feature at indexed location
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>Computer Feature</returns>
        public RobotComputedFeatures this[int index]
        {
            get => _ComputedFeatures[index];
            set => _ComputedFeatures[index] = value;
        }
        /// <summary>
        /// Find index number of the feature with the feature number provided
        /// </summary>
        /// <param name="FeatureNum">feature number required</param>
        /// <returns>index location of feature with feature number</returns>
        private int FeatureIndex(int FeatureNum)
        {
            if (FeatureNumberInComputedFeatures(FeatureNum))
            {
                for (int i = 0; i < _ComputedFeatures.Count; i++)
                {
                    if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                        return i;
                }
                return -1;
            }
            else
                return -1;
        }
        /// <summary>
        /// If list contains feature with feature number
        /// </summary>
        /// <param name="FeatureNum">Feature Number to be checked for</param>
        /// <returns>If Feature with feature number is present</returns>
        private bool FeatureNumberInComputedFeatures(int FeatureNum)
        {
            for (int i = 0; i < _ComputedFeatures.Count; i++)
                if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                    return true;
            return false;
        }
        public IEnumerator GetEnumerator()
        {
            return this;
        }
        /// <summary>
        /// returns list Robot Computed features
        /// </summary>
        /// <returns></returns>
        public List<RobotComputedFeatures> ToList()
        {
            return _ComputedFeatures;
        }
        /// <summary>
        /// Increment to next robot computed features
        /// </summary>
        /// <returns>successfully incremented to next feature</returns>
        public bool MoveNext()
        {
            _current++;
            if (_current == _ComputedFeatures.Count)
            {
                _current--;
                return false;
            }
            return true;
            
        }
        /// <summary>
        /// Set current feature to index 0
        /// </summary>
        public void Reset()
        {
            _current = 0;
        }
    }
}
