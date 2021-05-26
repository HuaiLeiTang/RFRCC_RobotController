﻿namespace RFRCC_RobotController.ABB_Data
{
    // C# libraries of RobotStudio Classes
    public class CS_pos
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public override string ToString()
        {
            return ("[" + X.ToString("0.00") + "," + Y.ToString("0.00") + "," + Z.ToString("0.00") + "]");
        }

        public CS_pos()
        {
            X = 0.00f;
            Y = 0.00f;
            Z = 0.00f;
        }
        public CS_pos(CS_orient PosFromQuat)
        {
            X = PosFromQuat.q2;
            Y = PosFromQuat.q3;
            Z = PosFromQuat.q4;
        }

        public CS_pos(double _X, double _Y, double _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
        public CS_pos(string input)
        {
            new CS_pos().FromString(input);
        }

        public void FromString(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            this.X = double.Parse(inputArray[0]);
            this.Y = double.Parse(inputArray[1]);
            this.Z = double.Parse(inputArray[2]);
        }


    }

}