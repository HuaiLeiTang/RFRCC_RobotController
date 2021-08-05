using ABB.Robotics.Controllers;
using System;
using ABB.Robotics.Controllers.RapidDomain;
using System.Diagnostics;
using RFRCC_RobotController.Controller.DataModel.OperationData;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    /// <summary>
    /// List of Operation Headers
    /// May be removed
    /// </summary>
    public class RAPID_OH_List
    {
        private OperationHeader[] _OpHeadArrayBuffer;
        private int _BufferSize;
        private RapidData _RAPIDdata;
        private ABB.Robotics.Controllers.Controller _Controller;
        /// <summary>
        /// Event triggered on change of struct on Robot
        /// </summary>
        public event EventHandler<EventArgs> StructChange;
        /// <summary>
        /// Size of array in struct on robot
        /// </summary>
        public int SizeOfBuffer
        {
            get
            {
                return _BufferSize;
            }
        }
        /// <summary>
        /// Return Operation Header from robot at index
        /// </summary>
        /// <param name="index">index of robot array</param>
        /// <param name="update">if value should be updated from robot before return</param>
        /// <returns>Retrieved Operation Header</returns>
        public OperationHeader Data(int index, bool update = true)
        {
            if (update)
                DownloadData();
            return _OpHeadArrayBuffer[index];
        }
        /// <summary>
        /// Apend data in memory structure 
        /// </summary>
        /// <param name="index">index at which item is replaced with</param>
        /// <param name="Append">item to replace indexed item</param>
        public void Data(int index, OperationHeader Append)
        {
            DownloadData();
            _OpHeadArrayBuffer[index] = Append;
            UploadData();
        }
        /// <summary>
        /// Event to raise StructChange
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void OnStructChange(object source, DataValueChangedEventArgs e)
        {
            StructChange?.Invoke(this, e);
        }
        /// <summary>
        /// Initialise object
        /// </summary>
        /// <param name="SizeOfArray">Size of array on robot</param>
        /// <param name="controller">relevant Network controller</param>
        /// <param name="task">Network Controller Task</param>
        /// <param name="tool">Tool</param>
        /// <param name="workobject">Workobject</param>
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
        /// <summary>
        /// Populate object from string representation of RAPID structure of array of OperationHeader
        /// format: "[not defined]"
        /// </summary>
        /// <param name="String">string representation of RAPID structure of array of OperationHeader</param>
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
        /// <summary>
        /// output string reprentation of RAPID structure of array of OperationHeader
        /// </summary>
        /// <returns>string reprentation of RAPID structure of array of OperationHeader</returns>
        public override string ToString()
        {
            string output = "[";

            for (int i = 0; i < _BufferSize; i++)
            {
                output += _OpHeadArrayBuffer[i].ToString() + ",";
            }

            return output.Trim(',') + "]";

        }
        /// <summary>
        /// Upload Data in object to Network Controller
        /// </summary>
        public void UploadData()
        {
            bool complete = false;
            string sendme = ToString();
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_Controller))
                    {

                        _RAPIDdata.StringValue = sendme;
                        m.Release();
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
        /// <summary>
        /// download and sync object data from network Controller
        /// </summary>
        public void DownloadData()
        {
            FromString(_RAPIDdata.StringValue);
        }
    }
}
