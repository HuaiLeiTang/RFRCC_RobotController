using ABB.Robotics.Controllers;
using System;
using ABB.Robotics.Controllers.RapidDomain;
using System.Diagnostics;
using RFRCC_RobotController.ABB_Data;

namespace RFRCC_RobotController.RAPID_Data
{
    public class RAPID_OM_List
    {
        private OperationManoeuvre[] _OpManArrayBuffer;
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
        public OperationManoeuvre Data(int index, bool update = true)
        {
            if (update)
                DownloadData();
            return _OpManArrayBuffer[index];
        }
        public void Data(int index, OperationManoeuvre Append)
        {
            DownloadData();
            _OpManArrayBuffer[index] = Append;
            UploadData();
        }
        protected virtual void OnStructChange(object source, DataValueChangedEventArgs e)
        {
            StructChange?.Invoke(this, e);
        }
        public RAPID_OM_List(int SizeOfArray, ABB.Robotics.Controllers.Controller controller, Task task, string tool, string workobject)
        {
            _BufferSize = SizeOfArray;
            _RAPIDdata = task.GetRapidData(tool, workobject);
            _Controller = controller;
            _RAPIDdata.ValueChanged += OnStructChange;
            _OpManArrayBuffer = new OperationManoeuvre[SizeOfArray];

            // check
            DownloadData();

        }
        public void FromString(string String)
        {

            string[] inputArray = String.Trim('[', ']').Split(',');
            for (int i = 0; i < _BufferSize; i++)
            {
                if (_OpManArrayBuffer[i] == null)
                    _OpManArrayBuffer[i] = new OperationManoeuvre();
                string InsertMe = string.Join(",", inputArray[(i * 51)..((i + 1) * 51)]);
                _OpManArrayBuffer[i].FromString(InsertMe);
            }
        }
        public override string ToString()
        {
            string output = "[";

            for (int i = 0; i < _BufferSize; i++)
            {
                output += _OpManArrayBuffer[i].ToString() + ",";
            }

            return output.Trim(',') + "]";

        }
        public void UploadData()
        {
            bool complete = false;
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller.Rapid))
                    {
                        _RAPIDdata.StringValue = ToString();
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
        public void DownloadData()
        {


            FromString(_RAPIDdata.StringValue);
        }

    }
}
