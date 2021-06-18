using System;
using System.Collections;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    public partial class PC_RobotMove_Register : IEnumerable, IEnumerator
    {
        private List<RobotComputedFeatures> _ComputedFeatures = new List<RobotComputedFeatures>();
        private int _current;
        public int NumberOfFeaturesComplete
        {
            get
            {
                return _ComputedFeatures.Count;
            }
        }

        public List<RobotComputedFeatures> ComputedFeatures => _ComputedFeatures;
        // TODO: make this function work
        public int Count => _ComputedFeatures.Count;

        public object Current => _ComputedFeatures[_current];

        public PC_RobotMove_Register(List<RobotComputedFeatures> computedFeatures)
        {
            _ComputedFeatures = computedFeatures;
        }
        public PC_RobotMove_Register()
        {
        }
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
        public void Add(RobotComputedFeatures item)
        {
            _ComputedFeatures.Add(item);
        }
        public void AddList(List<RobotComputedFeatures> List)
        {
            _ComputedFeatures.AddRange(List);
        }
        public void FromList(List<RobotComputedFeatures> List)
        {
            Clear();
            _ComputedFeatures.AddRange(List);
        }
        public void Clear()
        {
            _ComputedFeatures.Clear();
        }
        public RobotComputedFeatures this[int index]
        {
            get => _ComputedFeatures[index];
            set => _ComputedFeatures[index] = value;
        }
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
        public List<RobotComputedFeatures> ToList()
        {
            return _ComputedFeatures;
        }

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

        public void Reset()
        {
            _current = 0;
        }
    }
}
