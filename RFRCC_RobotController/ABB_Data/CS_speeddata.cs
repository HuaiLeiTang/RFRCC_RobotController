using System;

namespace RFRCC_RobotController.ABB_Data
{
    /// <summary>
    /// used to specify the velocity at which both the robot and the external axes move.
    /// </summary>
    public class CS_speeddata
    {
        private double _v_tcp;
        private double _v_ori;
        private double _v_leax;
        private double _v_reax;
        /// <summary>
        /// The velocity of rotating external axes in degrees/s.
        /// </summary>
        public double v_reax
        {
            get { return _v_reax; }
            set { _v_reax = value; }
        }
        /// <summary>
        /// The velocity of linear external axes in mm/s
        /// </summary>
        public double v_leax
        {
            get { return _v_leax; }
            set { _v_leax = value; }
        }
        /// <summary>
        /// The reorientation velocity of the TCP expressed in degrees/s.
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the workobject.
        /// </summary>
        public double v_ori
        {
            get { return _v_ori; }
            set { _v_ori = value; }
        }
        /// <summary>
        /// The velocity of the tool centerpoint (TCP) in mm/s.
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the workobject.
        /// </summary>
        public double v_tcp
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }
        /// <summary>
        /// speeddata normalised to 0,0,0,0
        /// </summary>
        public CS_speeddata()
        {
            _v_tcp = 0;
            _v_ori = 0;
            _v_leax = 0;
            _v_reax = 0;
        }
        /// <summary>
        /// Speed data setting all parameters otherwise normalised to 0
        /// </summary>
        /// <param name="v_tcp">velocity tcp</param>
        /// <param name="v_ori">velocity orientation</param>
        /// <param name="v_leax">volicity linear external axes</param>
        /// <param name="v_reax">velocity rotational external axes</param>
        public CS_speeddata(double v_tcp = 0, double v_ori = 0, double v_leax = 0, double v_reax = 0)
        {
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
        }
        /// <summary>
        /// speeddata set from string of format "[v_tcp,v_ori,v_leax,v_reax]"
        /// </summary>
        /// <param name="FromString">"[v_tcp,v_ori,v_leax,v_reax]"</param>
        public CS_speeddata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _v_tcp = Double.Parse(inputArray[0].ToLower());
            _v_ori = Double.Parse(inputArray[1].ToLower());
            _v_leax = Double.Parse(inputArray[2].ToLower());
            _v_reax = Double.Parse(inputArray[3].ToLower());
        }
        /// <summary>
        /// Output variable as string of format "[v_tcp,v_ori,v_leax,v_reax]"
        /// </summary>
        /// <returns>"[v_tcp,v_ori,v_leax,v_reax]"</returns>
        public override string ToString()
        {
            return ("[" + _v_tcp.ToString("0.00") + "," + _v_ori.ToString("0.00") + "," + _v_leax.ToString("0.00") + "," + _v_reax.ToString("0.00") + "]");
        }
        /// <summary>
        /// set from string of format "[v_tcp,v_ori,v_leax,v_reax]"
        /// </summary>
        /// <param name="Input">"[v_tcp,v_ori,v_leax,v_reax]"</param>
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            _v_tcp = double.Parse(InputArray[0]);
            _v_ori = double.Parse(InputArray[1]);
            _v_leax = double.Parse(InputArray[2]);
            _v_reax = double.Parse(InputArray[3]);
        }

    }
}
