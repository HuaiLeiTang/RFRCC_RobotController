using System;
using ABB.Robotics.Controllers.RapidDomain;

namespace RFRCC_RobotController.ABB_Data
{
    public class CS_zonedata
    {
        private bool _finep;
        private double _pzone_tcp;
        private double _pzone_ori;
        private double _pzone_eax;
        private double _zone_ori;
        private double _zone_leax;
        private double _zone_reax;

        public double zone_reax
        {
            get { return _zone_reax; }
            set { _zone_reax = value; }
        }
        public double zone_leax
        {
            get { return _zone_leax; }
            set { _zone_leax = value; }
        }
        public double zone_ori
        {
            get { return _zone_ori; }
            set { _zone_ori = value; }
        }
        public double pzone_eax
        {
            get { return _pzone_eax; }
            set { _pzone_eax = value; }
        }
        public double pzone_ori
        {
            get { return _pzone_ori; }
            set { _pzone_ori = value; }
        }
        public double pzone_tcp
        {
            get { return _pzone_tcp; }
            set { _pzone_tcp = value; }
        }
        public bool finep
        {
            get { return _finep; }
            set { _finep = value; }
        }

        public CS_zonedata()
        {
            _finep = false;
            _pzone_tcp = 0;
            _pzone_ori = 0;
            _pzone_eax = 0;
            _zone_ori = 0;
            _zone_leax = 0;
            _zone_reax = 0;
        }
        public CS_zonedata(bool finep, double pzone_tcp, double pzone_ori, double pzone_eax, double zone_ori, double zone_leax, double zone_reax)
        {
            _finep = finep;
            _pzone_tcp = pzone_tcp;
            _pzone_ori = pzone_ori;
            _pzone_eax = pzone_eax;
            _zone_ori = zone_ori;
            _zone_leax = zone_leax;
            _zone_reax = zone_reax;
        }
        public CS_zonedata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _finep = Bool.Parse(inputArray[0].ToLower());
            _pzone_tcp = Double.Parse(inputArray[1].ToLower());
            _pzone_ori = Double.Parse(inputArray[2].ToLower());
            _pzone_eax = Double.Parse(inputArray[3].ToLower());
            _zone_ori = Double.Parse(inputArray[4].ToLower());
            _zone_leax = Double.Parse(inputArray[5].ToLower());
            _zone_reax = Double.Parse(inputArray[6].ToLower());
        }
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            _finep = bool.Parse(InputArray[0]);
            _pzone_tcp = double.Parse(InputArray[1]);
            _pzone_ori = double.Parse(InputArray[2]);
            _pzone_eax = double.Parse(InputArray[3]);
            _zone_ori = double.Parse(InputArray[4]);
            _zone_leax = double.Parse(InputArray[5]);
            _zone_reax = double.Parse(InputArray[6]);
        }

        public override string ToString()
        {
            return ("[" + _finep.ToString() + "," + _pzone_tcp.ToString("0.00") + "," + _pzone_ori.ToString("0.00") + "," + _pzone_eax.ToString("0.00") + "," + _zone_ori.ToString("0.00") + "," + _zone_leax.ToString("0.00") + "," + _zone_reax.ToString("0.00") + "]");
        }
    }
}
