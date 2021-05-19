using System;
using System.Collections;
using System.Collections.Generic;

namespace RFARCC_RobotController.RCC_RobotController
{
    public partial class PC_RobotMove_Register : IEnumerable
    {
        private List<RobotComputedFeatures> _ComputedFeatures = new List<RobotComputedFeatures>();
        public List<RobotComputedFeatures> ComputedFeatures => _ComputedFeatures;

        // TODO: make this function work
        public int NumberOfFeaturesComplete
        {
            get
            {
                return _ComputedFeatures.Count;
            }
        }
        public int Count => _ComputedFeatures.Count;
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


        private void UpdateComputedFeatureHeader(OperationHeader Input)
        {
            
        }
        // checks if FeatureNum already contained within ComputedFeatures
        private bool FeatureNumberInComputedFeatures(int FeatureNum)
        {
            for (int i = 0; i < _ComputedFeatures.Count; i++)
                if (_ComputedFeatures[i].FeatureHeader.FeatureNum == FeatureNum)
                    return true;
            return false;
        }
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)_ComputedFeatures;
        }
        public List<RobotComputedFeatures> ToList()
        {
            return _ComputedFeatures;
        }
    }
}
