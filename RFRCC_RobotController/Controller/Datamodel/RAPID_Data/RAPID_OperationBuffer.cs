using ABB.Robotics.Controllers;
using System.Collections.Generic;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using static RFRCC_RobotController.Controller.DataModel.OperationData.PC_RobotMove_Register;
using RFRCC_RobotController.Controller;
using RFRCC_RobotController.Controller.DataModel.OperationData;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    // RAPID Data connection to Operation Buffer for PC generation of paths for manoeuvres
    public class RAPID_OperationBuffer
    {
        private int _SizeOfManBuffer;
        private RapidData _ManBufferRD;
        private RapidData _HeadBufferRD;
        private ABB.Robotics.Controllers.Controller _Controller;
        private PC_RobotMove_Register _Operations = new PC_RobotMove_Register();
        private bool _sortAscending = true;

        public PC_RobotMove_Register Operation => _Operations;
        public int SizeOfManBuffer
        {
            get
            {
                return _SizeOfManBuffer;
            }
        }
        public bool AscendingOrder
        {
            get
            {
                return _sortAscending;
            }
            set
            {
                _sortAscending = value;
            }
        }
        public bool DescendingOrder
        {
            get
            {
                return !_sortAscending;
            }
            set
            {
                _sortAscending = !value;
            }
        }
        public RAPID_OperationBuffer(ABB.Robotics.Controllers.Controller controller, Task task, string OpManModule, string OpManVARName, string OpHeadModule, string OpHeadVARName)
        {
            _Controller = controller;
            _ManBufferRD = task.GetRapidData(OpManModule, OpManVARName);
            _HeadBufferRD = task.GetRapidData(OpHeadModule, OpHeadVARName);
            _SizeOfManBuffer = _ManBufferRD.StringValue.Split(',').Count() / new OperationManoeuvre().ToString().Split(',').Count();
        }
        public void Clear()
        {
            _Operations.Clear();
        }
        public void AddOperationRange(PC_RobotMove_Register Operations)
        {
            foreach (RobotComputedFeatures feature in Operations)
            {
                _Operations.Add(feature);
            }
            sort();
        }
        public void AddOperationRange(List<RobotComputedFeatures> Operations)
        {
            foreach (RobotComputedFeatures feature in Operations)
            {
                _Operations.Add(feature);
            }
            sort();
        }
        public void AddOperation(RobotComputedFeatures Operation)
        {
            _Operations.Add(Operation);
            sort();
        }
        public void AddFromString(string input)
        {
            /*RobotComputedFeatures NewFeature = new RobotComputedFeatures();
            string[] inputArray = input.Trim('[', ']').Split(',');
            NewFeature.FeatureHeader.FromString(string.Join(",", inputArray[0..17]));

            inputArray[17] = inputArray[17].Trim('[');
            inputArray[inputArray.Length- 1] = inputArray[inputArray.Length - 1].Trim(']');
            for (int i = 0; i < (inputArray.Count() - 17 / 51); i++)
            {
                string InsertMe = string.Join(",", inputArray[(i * 51 + 17)..((i + 1) * 51 + 17)]);
                NewFeature.FeatureManoeuvres.Add(new OperationManoeuvre(InsertMe));
            }

            _Operations.Add(NewFeature);

            sort();*/

            // fill up empty buffers -- !! not required for PC storage !!
            //int length = _SizeOfManBuffer - NewFeature.FeatureManoeuvres.Count;
            //for (int i = 0; i < length; i++)
            //{
            //    NewFeature.FeatureManoeuvres.Add(new OperationManoeuvre());
            //}

        }
        public void sort()
        {
            if (_sortAscending)
            {
                _Operations.FromList(_Operations.ToList().OrderBy(op => op.FeatureHeader.LocationMin.X).ToList());
                int index = 1;
                foreach (var op in _Operations.ToList().OrderBy(op => op.FeatureHeader.LocationMin.X).ToList())
                {
                    op.FeatureHeader.FeatureNum = index;
                    index++;
                }
            }
            else
            {
                _Operations.FromList(_Operations.ToList().OrderByDescending(op => op.FeatureHeader.LocationMax.X).ToList());
                int index = 1;
                foreach (var op in _Operations.ToList().OrderByDescending(op => op.FeatureHeader.LocationMax.X).ToList())
                {
                    op.FeatureHeader.FeatureNum = index;
                    index++;
                }
            }

        }
        public void sort(bool sortAscending)
        {
            bool temp;
            temp = _sortAscending;
            _sortAscending = sortAscending;
            sort();
            _sortAscending = temp;
        }
        public string ToString(int feature)
        {
            string output = "[" + _Operations[feature].FeatureHeader.ToString() + ",[" + _Operations[feature].FeatureManoeuvres[0].ToString();
            for (int i = 1; i < _Operations[feature].FeatureManoeuvres.Count; i++)
            {
                output += "," + _Operations[feature].FeatureManoeuvres[i].ToString();
            }

            // fill in empty manoeuvres for RAPID data buffer
            string newOpManString = new OperationManoeuvre().ToString();
            for (int i = 0; i < _SizeOfManBuffer - _Operations[feature].FeatureManoeuvres.Count; i++)
            {
                output += "," + newOpManString;
            }
            return output + "]]";
        }
        public string OperationManoeuvresToString(int feature)
        {
            string output = "[" + _Operations[feature - 1].FeatureManoeuvres[0].ToString();
            for (int i = 1; i < _Operations[feature - 1].FeatureManoeuvres.Count; i++)
            {
                output += "," + _Operations[feature - 1].FeatureManoeuvres[i].ToString();
            }

            // fill in empty manoeuvres for RAPID data buffer
            string newOpManString = new OperationManoeuvre().ToString();
            for (int i = 0; i < _SizeOfManBuffer - _Operations[feature - 1].FeatureManoeuvres.Count; i++)
            {
                output += "," + newOpManString;
            }
            return output + "]";
        }
        public string OperationManoeuvresChunkToString(int feature, int Carriage)
        {
            string output = "[" + _Operations[feature - 1].FeatureManoeuvres[(Carriage - 1) * _SizeOfManBuffer].ToString();

            int NumOfFeatures = _Operations[feature - 1].FeatureManoeuvres.Count - (Carriage - 1) * _SizeOfManBuffer;
            if (NumOfFeatures > _SizeOfManBuffer) NumOfFeatures = _SizeOfManBuffer;
            for (int i = (Carriage - 1) * _SizeOfManBuffer + 1; i < (Carriage - 1) * _SizeOfManBuffer + NumOfFeatures; i++)
            {
                output += "," + _Operations[feature - 1].FeatureManoeuvres[i].ToString();
            }

            if (NumOfFeatures != _SizeOfManBuffer)
            {
                // fill in empty manoeuvres for RAPID data buffer
                string newOpManString = new OperationManoeuvre().ToString();
                for (int i = 0; i < _SizeOfManBuffer - NumOfFeatures; i++)
                {
                    output += "," + newOpManString;
                }
            }

            return output + "]";
        }
        public bool UploadData(int feature)
        {
            bool complete = false;

            // check if feature exists
            if (_Operations.Count < feature) return false;

            string OpHeaderStringToUpload = _Operations[feature - 1].FeatureHeader.ToString();
            string OpManStringToUpload = OperationManoeuvresToString(feature);

            OpManStringToUpload = OpManStringToUpload.Replace("0.00000000", "0")
                .Replace(".0000000", "")
                .Replace(".000000", "")
                .Replace(".00000", "")
                .Replace(".0000", "")
                .Replace(".000", "")
                .Replace(".00 ", "")
                .Replace(".00,", ",")
                .Replace(".00]", "]")
                .Replace(".0 ", " ")
                .Replace(".0,", ",")
                .Replace(".0]", "]")
                .Replace("9E+09", "9E9")
                .Replace("-0,", "0,");

            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller.Rapid))
                    {
                        _HeadBufferRD.StringValue = OpHeaderStringToUpload;
                        _ManBufferRD.StringValue = OpManStringToUpload;
                    }
                }
                catch
                {
                    complete = false;
                }
                finally
                {
                    if (_HeadBufferRD.StringValue == OpHeaderStringToUpload && _ManBufferRD.StringValue == OpManStringToUpload)
                    {
                        complete = true;
                    }
                }
            }
            return true;
        }
        public bool UploadData(int feature, int carriage)
        {

            // check if feature exists
            if (_Operations.Count < feature) return false;

            string OpHeaderStringToUpload = _Operations[feature - 1].FeatureHeader.ToString();
            string OpManStringToUpload = OperationManoeuvresChunkToString(feature, carriage);

            OpManStringToUpload = OpManStringToUpload.Replace("0.00000000", "0")
                .Replace(".0000000", "")
                .Replace(".000000", "")
                .Replace(".00000", "")
                .Replace(".0000", "")
                .Replace(".000", "")
                .Replace(".00 ", "")
                .Replace(".00,", ",")
                .Replace(".00]", "]")
                .Replace(".0 ", " ")
                .Replace(".0,", ",")
                .Replace(".0]", "]")
                .Replace("9E+09", "9E9")
                .Replace("-0,", "0,");

            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller.Rapid))
                    {
                        _HeadBufferRD.StringValue = OpHeaderStringToUpload;
                        _ManBufferRD.StringValue = OpManStringToUpload;

                        //m.Release(); // this may not work...
                    }
                }
                catch
                {
                    complete = false;
                }
                finally
                {
                    complete = true;

                }
            }

            // should check if the information has taken to the robot last - FOR DEBUG
            string check = _ManBufferRD.StringValue;
            if (OpManStringToUpload.ToLower() != check.ToLower()) // TODO: error if word not taken
            {
                bool damn = true;
            }
            return true;
        }

        /* this shouldnt work so easily... (basically)
        public void DownloadData()
        {
            string input = "[" + _HeadBufferRD.StringValue + "," + _ManBufferRD + "]";

            FromString(input);
        }
        */

    }
}
