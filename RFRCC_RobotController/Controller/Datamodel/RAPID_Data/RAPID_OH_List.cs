using ABB.Robotics.Controllers;
using System;
using ABB.Robotics.Controllers.RapidDomain;
using System.Diagnostics;
using RFRCC_RobotController.Controller.DataModel.OperationData;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    public class RAPID_OH_List
    {
        private OperationHeader[] _OpHeadArrayBuffer;
        private int _BufferSize;
        private RapidData _RAPIDdata;
        private ABB.Robotics.Controllers.Controller _Controller;
        public event EventHandler<EventArgs> StructChange;
        public int SizeOfBuffer
        {
            get
            {
                return _BufferSize;
            }
        }
        public OperationHeader Data(int index, bool update = true)
        {
            if (update)
                DownloadData();
            return _OpHeadArrayBuffer[index];
        }
        public void Data(int index, OperationHeader Append)
        {
            DownloadData();
            _OpHeadArrayBuffer[index] = Append;
            UploadData();
        }
        protected virtual void OnStructChange(object source, DataValueChangedEventArgs e)
        {
            StructChange?.Invoke(this, e);
        }
        public RAPID_OH_List(int SizeOfArray, ABB.Robotics.Controllers.Controller controller, Task task, string tool, string workobject)
        {
            _BufferSize = SizeOfArray;
            _RAPIDdata = task.GetRapidData(tool, workobject);
            _Controller = controller;
            _RAPIDdata.ValueChanged += OnStructChange;
            _OpHeadArrayBuffer = new OperationHeader[SizeOfArray];

            // check
            DownloadData();

        }
        public void FromString(string String)
        {

            string[] inputArray = String.Trim('[', ']').Split(',');
            for (int i = 0; i < _BufferSize; i++)
            {
                if (_OpHeadArrayBuffer[i] == null)
                    _OpHeadArrayBuffer[i] = new OperationHeader();
                _OpHeadArrayBuffer[i].FromString(string.Join(",", inputArray[(i * 17)..((i + 1) * 17)]).Trim('[', ']'));
            }
        }
        public override string ToString()
        {
            string output = "[";

            for (int i = 0; i < _BufferSize; i++)
            {
                output += _OpHeadArrayBuffer[i].ToString() + ",";
            }

            return output.Trim(',') + "]";

        }
        // upload function takes 511ms
        public void UploadData()
        {
            bool complete = false;
            string sendme = ToString();
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller.Rapid))
                    {

                        _RAPIDdata.StringValue = sendme;
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
        }
        // download function takes 3708ms
        public void DownloadData()
        {
            FromString(_RAPIDdata.StringValue);
        }
    }
}
