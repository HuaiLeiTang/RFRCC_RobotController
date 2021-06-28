using System;

namespace RFRCC_RobotController.ABB_Data
{
    /// <summary>
    /// usedfor orientations(suchas the orientationof a tool) and rotations(suchas the rotationof a coordinatesystem) on robot
    /// Identical to a quaternion
    /// </summary>
    public class CS_orient
    {
        /// <summary>
        /// Quaternion value 1, colloquial W
        /// </summary>
        public double q1 { get; set; } // W
        /// <summary>
        /// Quaternion value 2, colloquial X
        /// </summary>
        public double q2 { get; set; } // X
        /// <summary>
        /// Quaternion value 3, colloquial Y
        /// </summary>
        public double q3 { get; set; } // Y
        /// <summary>
        /// Quaternion value 4, colloquial Z
        /// </summary>
        public double q4 { get; set; } // Z
        /// <summary>
        /// Normalised to 0
        /// </summary>
        public CS_orient()
        {
            q1 = 1;
            q2 = 0;
            q3 = 0;
            q4 = 0;
        }
        /// <summary>
        /// Input all values of Quaternion
        /// </summary>
        /// <param name="_q1">q1,w</param>
        /// <param name="_q2">q2,x</param>
        /// <param name="_q3">q3,y</param>
        /// <param name="_q4">q4,z</param>
        public CS_orient(double _q1, double _q2, double _q3, double _q4)
        {
            q1 = _q1;
            q2 = _q2;
            q3 = _q3;
            q4 = _q4;
        }
        /// <summary>
        /// Locationa Quaternion with no rotational factor, i.e. q1/w = 0
        /// </summary>
        /// <param name="from_pos"></param>
        public CS_orient(CS_pos from_pos)
        {
            q1 = 0.00f;
            q2 = from_pos.X;
            q3 = from_pos.Y;
            q4 = from_pos.Z;
        }
        /// <summary>
        /// Quaternion generated from Vectro and rotational angle
        /// </summary>
        /// <param name="Vector">Vector</param>
        /// <param name="Angle">Angle</param>
        public CS_orient(CS_pos Vector, double Angle)
        {
            q1 = (float)Math.Cos(Angle / 2);
            q2 = (float)(Vector.X * Math.Sin(Angle / 2));
            q3 = (float)(Vector.Y * Math.Sin(Angle / 2));
            q4 = (float)(Vector.Z * Math.Sin(Angle / 2));
        }
        /// <summary>
        /// Generated from string of format "[q1,q2,q3,q4]"
        /// </summary>
        /// <param name="input">"[q1,q2,q3,q4]"</param>
        public CS_orient(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            q1 = double.Parse(inputArray[0]);
            q2 = double.Parse(inputArray[1]);
            q3 = double.Parse(inputArray[2]);
            q4 = double.Parse(inputArray[3]);
        }
        /// <summary>
        /// Populated from string of format "[q1,q2,q3,q4]"
        /// </summary>
        /// <param name="input">"[q1,q2,q3,q4]"</param>
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            q1 = double.Parse(InputArray[0]);
            q2 = double.Parse(InputArray[1]);
            q3 = double.Parse(InputArray[2]);
            q4 = double.Parse(InputArray[3]);
        }
        /// <summary>
        /// Generate string representation of orient of format "[q1,q2,q3,q4]"
        /// </summary>
        /// <returns>"[q1,q2,q3,q4]"</returns>
        public override string ToString()
        {
            return ("[" + q1.ToString("0.0000") + "," + q2.ToString("0.0000") + "," + q3.ToString("0.0000") + "," + q4.ToString("0.0000") + "]");
        }

    }

}
