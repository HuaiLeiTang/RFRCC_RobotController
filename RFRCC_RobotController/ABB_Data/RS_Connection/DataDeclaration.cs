using ABB.Robotics.RobotStudio.Stations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.ABB_Data.RS_Connection
{
    public class DataDeclaration
    {
        public string DataType { get; set; }
        public short Dimension1 { get; set; }
        public short Dimension2 { get; set; }
        public short Dimension3 { get; set; }
        public double FrameSize { get; set; }
        public string InitialExpression { get; set; }
        public bool IsInline { get; set; }
        public bool Showname { get; set; }
        public RapidStorageType StorageType { get; set; }
        public string DisplayName { get; set; }
        public bool Local { get; set; }
        public string ModuleName { get; set; }
        public string Name { get; set; }

    }

    public class DataDeclarationCollection : ICollection, IEnumerable<DataDeclaration>, IEnumerable, IEnumerator
    {
        private Dictionary<string, DataDeclaration> _Collection = new Dictionary<string, DataDeclaration>();


        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }
        // Enum methods
        public DataDeclaration[] _Collectionlist;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public DataDeclarationCollection()
        {
        }

        public bool MoveNext()
        {
            position++;
            return (position < _Collectionlist.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public DataDeclaration Current
        {
            get
            {
                try
                {
                    return _Collectionlist[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }


        // end enum methods

        IEnumerator<DataDeclaration> IEnumerable<DataDeclaration>.GetEnumerator()
        {
            return (IEnumerator<DataDeclaration>)(IEnumerator) GetEnumerator();
        }

        public void Add(DataDeclaration dataDeclaration) // TODO: Update this bad boi
        {
            double itteration = 1;
            //bool saved = false;

            //try
            //{
            //    _Collection.Add(dataDeclaration.Name, dataDeclaration);
            //}
            //catch (Exception)
            //{
            //    while(!saved)
            //    {
            //        try
            //        {
                        
            //            saved = true;
            //        }
            //        catch (Exception)
            //        {
            //            itteration++;
            //        }
            //    }
            //}

            double step = 1;
            bool sw = false;
            double free;
            if (_Collection.Keys.Contains(dataDeclaration.Name))
            {
                free = itteration;
                if (_Collection.Keys.Contains(dataDeclaration.Name + "_" + itteration.ToString()))
                {
                    while (step >= 1 || step <= -1)
                    {
                        // this should double the check step until finding a possible answer, then honing in on the most accurate result, effictively reducing checking time
                        if (_Collection.Keys.Contains(dataDeclaration.Name + "_" + itteration.ToString()))
                        {
                            step *= step > 0 ? 1 : -1;
                            itteration += step;
                            step *= sw ? 0.5 : 2;
                        }
                        else
                        {
                            free = itteration;
                            sw = true;
                            step *= step > 0 ? 1 : -1;
                            itteration += step;
                            step *= 0.5;
                            
                        }
                    }
                }
                _Collection.Add(dataDeclaration.Name + "_" + free.ToString(), dataDeclaration);
            }
            else _Collection.Add(dataDeclaration.Name, dataDeclaration);


            _Collectionlist = _Collection.Values.ToArray();
        }

        public void AddRange(List<DataDeclaration> dataDeclarationList)
        {
            foreach (var dataDeclarationItem in dataDeclarationList)
            {
                _Collection.Add(dataDeclarationItem.Name, dataDeclarationItem);
            }
        }

        public Dictionary<string, DataDeclaration> ReturnDictionary() => _Collection;

    }

    public class GenericDataDeclaration : DataDeclaration
    {
        public string InitialExpression { get; set; }

        public GenericDataDeclaration(string name, string dataType)
        {
            Name = name;
            DataType = dataType;
        }

    }

}
