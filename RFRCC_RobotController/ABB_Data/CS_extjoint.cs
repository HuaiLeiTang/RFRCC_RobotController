using System;

namespace RFRCC_RobotController.ABB_Data
{
    public class CS_extjoint
    {
        double _eax_a;
        double _eax_b;
        double _eax_c;
        double _eax_d;
        double _eax_e;
        double _eax_f;
        public double eax_a
        {
            get
            {
                return _eax_a;
            }
            set
            {
                _eax_a = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[0] = true;
            }
        }
        public double eax_b
        {
            get
            {
                return _eax_b;
            }
            set
            {
                _eax_b = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[1] = true;
            }
        }
        public double eax_c
        {
            get
            {
                return _eax_c;
            }
            set
            {
                _eax_c = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[2] = true;
            }
        }
        public double eax_d
        {
            get
            {
                return _eax_d;
            }
            set
            {
                _eax_d = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[3] = true;
            }
        }
        public double eax_e
        {
            get
            {
                return _eax_e;
            }
            set
            {
                _eax_e = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[4] = true;
            }
        }
        public double eax_f
        {
            get
            {
                return _eax_f;
            }
            set
            {
                _eax_f = value;
                if (ConnectedAxis == null)
                {
                    ConnectedAxis = new bool[6];
                }
                ConnectedAxis[5] = true;
            }
        }
        private bool[] ConnectedAxis = new bool[6];

        public CS_extjoint()
        {
            eax_a = 0;
            eax_b = 0;
            eax_c = 0;
            eax_d = 0;
            eax_e = 0;
            eax_f = 0;
            ConnectedAxis = new bool[6];
            ConnectedAxis[0] = false;
            ConnectedAxis[1] = false;
            ConnectedAxis[2] = false;
            ConnectedAxis[3] = false;
            ConnectedAxis[4] = false;
            ConnectedAxis[5] = false;
        }
        public CS_extjoint(string _eax_a, string _eax_b, string _eax_c, string _eax_d, string _eax_e, string _eax_f)
        {
            eax_a = _eax_a == "9E+09" ? 0f : Double.Parse(_eax_a);
            eax_b = _eax_b == "9E+09" ? 0f : Double.Parse(_eax_b);
            eax_c = _eax_c == "9E+09" ? 0f : Double.Parse(_eax_c);
            eax_d = _eax_d == "9E+09" ? 0f : Double.Parse(_eax_d);
            eax_e = _eax_e == "9E+09" ? 0f : Double.Parse(_eax_e);
            eax_f = _eax_f == "9E+09" ? 0f : Double.Parse(_eax_f);
            ConnectedAxis = new bool[6];
            ConnectedAxis[0] = _eax_a == "9E+09" ? true : false;
            ConnectedAxis[1] = _eax_b == "9E+09" ? true : false;
            ConnectedAxis[2] = _eax_c == "9E+09" ? true : false;
            ConnectedAxis[3] = _eax_d == "9E+09" ? true : false;
            ConnectedAxis[4] = _eax_e == "9E+09" ? true : false;
            ConnectedAxis[5] = _eax_f == "9E+09" ? true : false;
        }
        public CS_extjoint(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(',');
            eax_a = inputArray[0] == "9E+09" ? 0 : Double.Parse(inputArray[0]);
            eax_b = inputArray[1] == "9E+09" ? 0 : Double.Parse(inputArray[1]);
            eax_c = inputArray[2] == "9E+09" ? 0 : Double.Parse(inputArray[2]);
            eax_d = inputArray[3] == "9E+09" ? 0 : Double.Parse(inputArray[3]);
            eax_e = inputArray[4] == "9E+09" ? 0 : Double.Parse(inputArray[4]);
            eax_f = inputArray[5] == "9E+09" ? 0 : Double.Parse(inputArray[5]);
            ConnectedAxis = new bool[6];
            ConnectedAxis[0] = inputArray[0] == "9E+09" ? false : true;
            ConnectedAxis[1] = inputArray[1] == "9E+09" ? false : true;
            ConnectedAxis[2] = inputArray[2] == "9E+09" ? false : true;
            ConnectedAxis[3] = inputArray[3] == "9E+09" ? false : true;
            ConnectedAxis[4] = inputArray[4] == "9E+09" ? false : true;
            ConnectedAxis[5] = inputArray[5] == "9E+09" ? false : true;
        }
        public CS_extjoint(bool[] BinaryInput)
        {
            int i = 0;

            foreach (bool check in BinaryInput)
            {
                ConnectedAxis[i] = check;
                i++;
                if (i > 6)
                {
                    break;
                }
            }
        }
        public void ConnectExtAxis(bool[] BinaryInput)
        {
            int i = 0;

            foreach (bool check in BinaryInput)
            {
                ConnectedAxis[i] = check;
                i++;
                if (i > 6)
                {
                    break;
                }
            }
        }

        public void ConnectExtAxis(string input)
        {
            ConnectedAxis[0] = input.Contains("eax_a") ? true : ConnectedAxis[0];
            ConnectedAxis[1] = input.Contains("eax_b") ? true : ConnectedAxis[1];
            ConnectedAxis[2] = input.Contains("eax_c") ? true : ConnectedAxis[2];
            ConnectedAxis[3] = input.Contains("eax_d") ? true : ConnectedAxis[3];
            ConnectedAxis[4] = input.Contains("eax_e") ? true : ConnectedAxis[4];
            ConnectedAxis[5] = input.Contains("eax_f") ? true : ConnectedAxis[5];
        }

        public override string ToString()
        {
            return ("[" + (ConnectedAxis[0] ? _eax_a.ToString() : "9E+09") + "," + (ConnectedAxis[1] ? _eax_b.ToString() : "9E+09") + "," + (ConnectedAxis[2] ? _eax_c.ToString() : "9E+09") + "," + (ConnectedAxis[3] ? _eax_d.ToString() : "9E+09") + "," + (ConnectedAxis[4] ? _eax_e.ToString() : "9E+09") + "," + (ConnectedAxis[5] ? _eax_f.ToString() : "9E+09") + "]");
        }

        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            _eax_a = double.Parse(InputArray[0]);
            _eax_b = double.Parse(InputArray[1]);
            _eax_c = double.Parse(InputArray[2]);
            _eax_d = double.Parse(InputArray[3]);
            _eax_e = double.Parse(InputArray[4]);
            _eax_f = double.Parse(InputArray[5]);
        }
    }
}
