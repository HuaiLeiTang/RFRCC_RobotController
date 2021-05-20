using ABB.Robotics.Controllers;
using System.Collections.Generic;
using ABB.Robotics.Controllers.RapidDomain;
using System.Linq;
using System.Diagnostics;
using RFRCC_RobotController.ABB_Data;
using RFRCC_RobotController.ABB_Data.RS_Connection.Robotics.ToolInfo;

namespace RFRCC_RobotController.RAPID_Data
{
    public class RAPID_CutChart
    {
        private RapidData _RobotData;
        private ABB.Robotics.Controllers.Controller _Controller;
        public bool _AutoUpdate = false;


        private float _Thickness;
        private float _ArcVoltage;
        private string _CutGas;
        private string _ShieldGas;
        private float _CutHeight;
        private float _PierceHeight;
        private float _PierceDelay;
        private float _PlasmaGasCutFlow;
        private float _PlasmaGasPreFlow;
        private float _ShieldGasCutFlow;
        private float _ShieldGasPreFlow;
        private CS_speeddata _RAPIDSpeed;
        private float _KerfWidth;
        private float _MinimumClearance;

        public float Thickness
        {
            get
            {
                return _Thickness;
            }

            set
            {
                _Thickness = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float ArcVoltage
        {
            get
            {
                return _ArcVoltage;
            }

            set
            {
                _ArcVoltage = value;
                if (_AutoUpdate) Update_Robot();
}
        }
        public string CutGas
        {
            get
            {
                return _CutGas;
            }

            set
            {
                _CutGas = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public string ShieldGas
        {
            get
            {
                return _ShieldGas;
            }

            set
            {
                _ShieldGas = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float CutHeight
        {
            get
            {
                return _CutHeight;
            }

            set
            {
                _CutHeight = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float PierceHeight
        {
            get
            {
                return _PierceHeight;
            }

            set
            {
                _PierceHeight = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float PierceDelay
        {
            get
            {
                return _PierceDelay;
            }

            set
            {
                _PierceDelay = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float PlasmaGasCutFlow
        {
            get
            {
                return _PlasmaGasCutFlow;
            }

            set
            {
                _PlasmaGasCutFlow = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float PlasmaGasPreFlow
        {
            get
            {
                return _PlasmaGasPreFlow;
            }

            set
            {
                _PlasmaGasPreFlow = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float ShieldGasCutFlow
        {
            get
            {
                return _ShieldGasCutFlow;
            }

            set
            {
                _ShieldGasCutFlow = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float ShieldGasPreFlow
        {
            get
            {
                return _ShieldGasPreFlow;
            }

            set
            {
                _ShieldGasPreFlow = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float Speed
        {
            get
            {
                return float.Parse(_RAPIDSpeed.v_tcp.ToString());
            }

            set
            {
                _RAPIDSpeed.v_tcp = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public CS_speeddata RAPIDSpeed
        {
            get
            {
                return _RAPIDSpeed;
            }

            set
            {
                _RAPIDSpeed = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float KerfWidth
        {
            get
            {
                return _KerfWidth;
            }

            set
            {
                _KerfWidth = value;
                if (_AutoUpdate) Update_Robot();
            }
        }
        public float MinimumClearance
        {
            get
            {
                return _MinimumClearance;
            }

            set
            {
                _MinimumClearance = value;
                if (_AutoUpdate) Update_Robot();
            }
        }

        public RAPID_CutChart()
        {
            _Thickness = 0;
            _ArcVoltage = 0;
            _CutGas = "";
            _ShieldGas = "";
            _CutHeight = 0;
            _PierceHeight = 0;
            _PierceDelay = 0;
            _PlasmaGasCutFlow = 0;
            _PlasmaGasPreFlow = 0;
            _ShieldGasCutFlow = 0;
            _ShieldGasPreFlow = 0;
            _RAPIDSpeed = new CS_speeddata();
            _KerfWidth = 0;
            _MinimumClearance = 0;
        }
        public RAPID_CutChart(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _Thickness = float.Parse(inputArray[0].ToLower());
            _ArcVoltage = float.Parse(inputArray[1].ToLower());
            _CutGas = inputArray[2].Trim('"');
            _ShieldGas = inputArray[3].Trim('"');
            _CutHeight = float.Parse(inputArray[4].ToLower());
            _PierceHeight = float.Parse(inputArray[5].ToLower());
            _PierceDelay = float.Parse(inputArray[6].ToLower());
            _PlasmaGasCutFlow = float.Parse(inputArray[7].ToLower());
            _PlasmaGasPreFlow = float.Parse(inputArray[8].ToLower());
            _ShieldGasCutFlow = float.Parse(inputArray[9].ToLower());
            _ShieldGasPreFlow = float.Parse(inputArray[10].ToLower());
            _RAPIDSpeed = new CS_speeddata("["+string.Join(',', inputArray.Skip(10).Take(4).ToArray())+"]");
            _KerfWidth = float.Parse(inputArray[15].ToLower());
            _MinimumClearance = float.Parse(inputArray[16].ToLower());
        }
        public void ConnectToRAPID(ABB.Robotics.Controllers.Controller controller, Task RobotTask, string Module, string RAPID_Name)
        {
            this._Controller = controller;
            this._RobotData = RobotTask.GetRapidData(Module, RAPID_Name);

        }
        public override string ToString()
        {
            string[] OutArray = 
            {
                _Thickness.ToString("0.00"),
                _ArcVoltage.ToString("0.00"),
                "\"" + _CutGas + "\"",
                "\"" + _ShieldGas + "\"",
                _CutHeight.ToString("0.00"),
                _PierceHeight.ToString("0.00"),
                _PierceDelay.ToString("0.00"),
                _PlasmaGasCutFlow.ToString("0.00"),
                _PlasmaGasPreFlow.ToString("0.00"),
                _ShieldGasCutFlow.ToString("0.00"),
                _ShieldGasPreFlow.ToString("0.00"),
                _RAPIDSpeed.ToString(),
                _KerfWidth.ToString("0.00"),
                _MinimumClearance.ToString("0.00")
            };

            return "[" + string.Join(',', OutArray) + "]";

        }
        public void Update_Robot()
        {
            bool complete = false;
            Debug.Print(this.ToString());

            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(this._Controller.Rapid))
                    {
                        this._RobotData.StringValue = this.ToString();
                    }
                }
                catch
                {
                    Debug.Print("mastership failed while attempting to update CutData register");
                    complete = false;
                }
                finally
                {
                    complete = true;
                }
            }
            complete = false;
        }
        public void Update_Local()
        {
            DataNode[] RapidStruct = this._RobotData.Value.ToStructure().Children.ToArray();

            if (this._Thickness != float.Parse(RapidStruct[0].Value)) this._Thickness = float.Parse(RapidStruct[0].Value);
            if (this._ArcVoltage != float.Parse(RapidStruct[1].Value)) this._ArcVoltage = float.Parse(RapidStruct[1].Value);
            if (this._CutGas != "\"" + RapidStruct[2].Value + "\"") this._CutGas = RapidStruct[2].Value[1..^1];
            if (this._ShieldGas != "\"" + RapidStruct[3].Value + "\"") this._ShieldGas = RapidStruct[3].Value[1..^1];
            if (this._CutHeight != float.Parse(RapidStruct[4].Value)) this._CutHeight = float.Parse(RapidStruct[4].Value);
            if (this._PierceHeight != float.Parse(RapidStruct[5].Value)) this._PierceHeight = float.Parse(RapidStruct[5].Value);
            if (this._PierceDelay != float.Parse(RapidStruct[6].Value)) this._PierceDelay = float.Parse(RapidStruct[6].Value);
            if (this._PlasmaGasCutFlow != float.Parse(RapidStruct[7].Value)) this._PlasmaGasCutFlow = float.Parse(RapidStruct[7].Value);
            if (this._PlasmaGasPreFlow != float.Parse(RapidStruct[8].Value)) this._PlasmaGasPreFlow = float.Parse(RapidStruct[8].Value);
            if (this._ShieldGasCutFlow != float.Parse(RapidStruct[9].Value)) this._ShieldGasCutFlow = float.Parse(RapidStruct[9].Value);
            if (this._ShieldGasPreFlow != float.Parse(RapidStruct[10].Value)) this._ShieldGasPreFlow = float.Parse(RapidStruct[10].Value);
            _RAPIDSpeed.FromString(RapidStruct[11].Value); 
            if (this._KerfWidth != float.Parse(RapidStruct[12].Value)) this._KerfWidth = float.Parse(RapidStruct[12].Value);
            if (this._MinimumClearance != float.Parse(RapidStruct[13].Value)) this._MinimumClearance = float.Parse(RapidStruct[13].Value);

    }
        public void FromCutEntry(CutEntry FromCutEntry)
        {
            if (FromCutEntry == null) return;
            _Thickness = float.Parse(FromCutEntry.Thickness.ToString());
            _ArcVoltage = float.Parse(FromCutEntry.ArcVoltage.ToString());
            _CutGas = FromCutEntry.CutGas.ToString();
            _ShieldGas = FromCutEntry.ShieldGas.ToString();
            _CutHeight = float.Parse(FromCutEntry.CutHeight.ToString());
            _PierceHeight = float.Parse(FromCutEntry.PierceHeight.ToString());
            _PierceDelay = float.Parse(FromCutEntry.PierceDelay.ToString());
            _PlasmaGasCutFlow = float.Parse(FromCutEntry.PlasmaGasCutflow.ToString());
            _PlasmaGasPreFlow = float.Parse(FromCutEntry.PlasmaGasPreflow.ToString());
            _ShieldGasCutFlow = float.Parse(FromCutEntry.ShieldGasCutflow.ToString());
            _ShieldGasPreFlow = float.Parse(FromCutEntry.ShieldGasPreflow.ToString());
            _RAPIDSpeed.v_tcp = float.Parse(FromCutEntry.Speed.ToString());
            _KerfWidth = float.Parse(FromCutEntry.KerfWidth.ToString());
            _MinimumClearance = float.Parse(FromCutEntry.MinimumClearance.ToString());
        }

        public void FromString(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            _Thickness = float.Parse(inputArray[0].ToLower());
            _ArcVoltage = float.Parse(inputArray[1].ToLower());
            _CutGas = inputArray[2].Trim('"');
            _ShieldGas = inputArray[3].Trim('"');
            _CutHeight = float.Parse(inputArray[4].ToLower());
            _PierceHeight = float.Parse(inputArray[5].ToLower());
            _PierceDelay = float.Parse(inputArray[6].ToLower());
            _PlasmaGasCutFlow = float.Parse(inputArray[7].ToLower());
            _PlasmaGasPreFlow = float.Parse(inputArray[8].ToLower());
            _ShieldGasCutFlow = float.Parse(inputArray[9].ToLower());
            _ShieldGasPreFlow = float.Parse(inputArray[10].ToLower());
            _RAPIDSpeed = new CS_speeddata(string.Join(',', inputArray.Skip(11).Take(4).ToArray()));
            _KerfWidth = float.Parse(inputArray[15].ToLower());
            _MinimumClearance = float.Parse(inputArray[16].ToLower());
        }

    }
}
