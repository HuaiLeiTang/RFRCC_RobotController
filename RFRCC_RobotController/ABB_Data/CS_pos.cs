namespace RFRCC_RobotController.ABB_Data
{
    // C# libraries of RobotStudio Classes
    /// <summary>
    /// 3D vector with RAPID Pos analog functionality in order to interface with ABB RAPID easily
    /// </summary>
    public class CS_pos
    {
        /// <summary>
        /// X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Z
        /// </summary>
        public double Z { get; set; }
        /// <summary>
        /// In string format "[X,Y,Z]"
        /// </summary>
        /// <returns>"[X,Y,Z]"</returns>
        public override string ToString()
        {
            return ("[" + X.ToString("0.00") + "," + Y.ToString("0.00") + "," + Z.ToString("0.00") + "]");
        }
        /// <summary>
        /// normalised to 0,0,0
        /// </summary>
        public CS_pos()
        {
            X = 0.00f;
            Y = 0.00f;
            Z = 0.00f;
        }
        /// <summary>
        /// Taken XYZ from Q2, Q3, Q4 respectively
        /// </summary>
        /// <param name="PosFromQuat"></param>
        public CS_pos(CS_orient PosFromQuat)
        {
            X = PosFromQuat.q2;
            Y = PosFromQuat.q3;
            Z = PosFromQuat.q4;
        }
        /// <summary>
        /// Set X Y Z directly
        /// </summary>
        /// <param name="_X">X</param>
        /// <param name="_Y">Y</param>
        /// <param name="_Z">Z</param>
        public CS_pos(double _X, double _Y, double _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
        /// <summary>
        /// set from string of format "[X,Y,Z]"
        /// </summary>
        /// <param name="input">"[X,Y,Z]"</param>
        public CS_pos(string input)
        {
            new CS_pos().FromString(input);
        }
        /// <summary>
        /// Adds X1+X2, Y1+Y2, Z1+Z2
        /// </summary>
        /// <param name="p1">xyz1</param>
        /// <param name="p2">xyz2</param>
        /// <returns></returns>
        public static CS_pos operator +(CS_pos p1, CS_pos p2)
        {
            return new CS_pos(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }
        /// <summary>
        /// mutliplies s*X, s*Y, s*Z
        /// </summary>
        /// <param name="p1">XYZ</param>
        /// <param name="i">s</param>
        /// <returns></returns>
        public static CS_pos operator *(CS_pos p1, int i)
        {
            return new CS_pos(p1.X * i, p1.Y * i, p1.Z * i);
        }
        /// <summary>
        /// mutliplies s*X, s*Y, s*Z
        /// </summary>
        /// <param name="p1">XYZ</param>
        /// <param name="i">s</param>
        /// <returns></returns>
        public static CS_pos operator *(CS_pos p1, double i)
        {
            return new CS_pos(p1.X * i, p1.Y * i, p1.Z * i);
        }
        /// <summary>
        /// set from string of format "[X,Y,Z]"
        /// </summary>
        /// <param name="input">"[X,Y,Z]"</param>
        public void FromString(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            this.X = double.Parse(inputArray[0]);
            this.Y = double.Parse(inputArray[1]);
            this.Z = double.Parse(inputArray[2]);
        }


    }

}
