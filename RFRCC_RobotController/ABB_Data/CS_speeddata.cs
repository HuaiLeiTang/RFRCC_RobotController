using System;

namespace RFRCC_RobotController.ABB_Data
{
    public class CS_speeddata
    {
        private double _v_tcp;
        private double _v_ori;
        private double _v_leax;
        private double _v_reax;

        public double v_reax
        {
            get { return _v_reax; }
            set { _v_reax = value; }
        }

        public double v_leax
        {
            get { return _v_leax; }
            set { _v_leax = value; }
        }

        public double v_ori
        {
            get { return _v_ori; }
            set { _v_ori = value; }
        }


        public double v_tcp
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }

        public CS_speeddata()
        {
            _v_tcp = 0;
            _v_ori = 0;
            _v_leax = 0;
            _v_reax = 0;
        }
        public CS_speeddata(double v_tcp, double v_ori, double v_leax, double v_reax)
        {
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
        }
        public CS_speeddata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _v_tcp = Double.Parse(inputArray[0].ToLower());
            _v_ori = Double.Parse(inputArray[1].ToLower());
            _v_leax = Double.Parse(inputArray[2].ToLower());
            _v_reax = Double.Parse(inputArray[3].ToLower());
        }

        public override string ToString()
        {
            return ("[" + _v_tcp.ToString("0.00") + "," + _v_ori.ToString("0.00") + "," + _v_leax.ToString("0.00") + "," + _v_reax.ToString("0.00") + "]");
        }
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
