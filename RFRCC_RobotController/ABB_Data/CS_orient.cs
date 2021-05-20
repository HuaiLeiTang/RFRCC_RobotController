using System;

namespace RFRCC_RobotController.ABB_Data
{
    public class CS_orient
    {

        public double q1 { get; set; } // W
        public double q2 { get; set; } // X
        public double q3 { get; set; } // Y
        public double q4 { get; set; } // Z

        public CS_orient()
        {
            q1 = 1;
            q2 = 0;
            q3 = 0;
            q4 = 0;
        }

        public CS_orient(double _q1, double _q2, double _q3, double _q4)
        {
            q1 = _q1;
            q2 = _q2;
            q3 = _q3;
            q4 = _q4;
        }

        public CS_orient(CS_pos from_pos)
        {
            q1 = 0.00f;
            q2 = from_pos.X;
            q3 = from_pos.Y;
            q4 = from_pos.Z;
        }

        public CS_orient(CS_pos Vector, double Angle)
        {
            q1 = (float)Math.Cos(Angle / 2);
            q2 = (float)(Vector.X * Math.Sin(Angle / 2));
            q3 = (float)(Vector.Y * Math.Sin(Angle / 2));
            q4 = (float)(Vector.Z * Math.Sin(Angle / 2));
        }
        public CS_orient(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            q1 = double.Parse(inputArray[0]);
            q2 = double.Parse(inputArray[1]);
            q3 = double.Parse(inputArray[2]);
            q4 = double.Parse(inputArray[3]);
        }
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            q1 = double.Parse(InputArray[0]);
            q2 = double.Parse(InputArray[1]);
            q3 = double.Parse(InputArray[2]);
            q4 = double.Parse(InputArray[3]);
        }

        public override string ToString()
        {
            return ("[" + q1.ToString("0.0000") + "," + q2.ToString("0.0000") + "," + q3.ToString("0.0000") + "," + q4.ToString("0.0000") + "]");
        }

    }

}
