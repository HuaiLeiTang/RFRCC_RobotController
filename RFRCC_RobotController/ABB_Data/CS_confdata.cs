using System;

namespace RFRCC_RobotController.ABB_Data
{
    /// <summary>
    /// Configuration of the Robot axis
    /// </summary>
    public class CS_confdata
    {
        /// <summary>
        /// Quadrant number for axis 1
        /// </summary>
        public double cf1 { get; set; }
        /// <summary>
        /// Quadrant number for axis 4
        /// </summary>
        public double cf4 { get; set; }
        /// <summary>
        /// Quadrand number for axis 6
        /// </summary>
        public double cf6 { get; set; }
        /// <summary>
        /// used to configure the robot in one of 8 possible configureations detailed in Technical Reference 
        /// </summary>
        public double cfx { get; set; }
        /// <summary>
        /// 0'd configuration
        /// </summary>
        public CS_confdata()
        {
            cf1 = 0f;
            cf4 = 0f;
            cf6 = 0f;
            cfx = 0f;
        }
        /// <summary>
        /// Set configuration of all parameters
        /// </summary>
        /// <param name="_cf1"></param>
        /// <param name="_cf4"></param>
        /// <param name="_cf6"></param>
        /// <param name="_cfx"></param>
        public CS_confdata(float _cf1, float _cf4, float _cf6, float _cfx)
        {
            cf1 = _cf1;
            cf4 = _cf4;
            cf6 = _cf6;
            cfx = _cfx;
        }
        /// <summary>
        /// Configuration from string
        /// </summary>
        /// <param name="input">Formated "[cf1,cf4,cf6,cfx]"</param>
        public CS_confdata(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            cf1 = Single.Parse(inputArray[0]);
            cf4 = Single.Parse(inputArray[1]);
            cf6 = Single.Parse(inputArray[2]);
            cfx = Single.Parse(inputArray[3]);
        }
        /// <summary>
        /// returns string formated "[cf1,cf4,cf6,cfx]"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ("[" + cf1.ToString("0") + "," + cf4.ToString("0") + "," + cf6.ToString("0") + "," + cfx.ToString("0") + "]");
        }
        /// <summary>
        /// Configuration from string
        /// </summary>
        /// <param name="Input">Formated "[cf1,cf4,cf6,cfx]"</param>
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            cf1 = double.Parse(InputArray[0]);
            cf4 = double.Parse(InputArray[1]);
            cf6 = double.Parse(InputArray[2]);
            cfx = double.Parse(InputArray[3]);
        }
    }

}
