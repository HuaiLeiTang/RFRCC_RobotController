using System;

namespace RFRCC_RobotController.ABB_Data
{
    public class CS_confdata
    {
        public double cf1 { get; set; }
        public double cf4 { get; set; }
        public double cf6 { get; set; }
        public double cfx { get; set; }
        public CS_confdata()
        {
            cf1 = 0f;
            cf4 = 0f;
            cf6 = 0f;
            cfx = 0f;
        }
        public CS_confdata(float _cf1, float _cf4, float _cf6, float _cfx)
        {
            cf1 = _cf1;
            cf4 = _cf4;
            cf6 = _cf6;
            cfx = _cfx;
        }
        public CS_confdata(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            cf1 = Single.Parse(inputArray[0]);
            cf4 = Single.Parse(inputArray[1]);
            cf6 = Single.Parse(inputArray[2]);
            cfx = Single.Parse(inputArray[3]);
        }

        public override string ToString()
        {
            return ("[" + cf1.ToString("0.00000000") + "," + cf4.ToString("0.00000000") + "," + cf6.ToString("0.00000000") + "," + cfx.ToString("0.00000000") + "]");
        }

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
