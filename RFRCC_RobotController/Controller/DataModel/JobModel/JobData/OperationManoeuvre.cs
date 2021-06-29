using System;
using CopingLineModels;
using ReplaceRSConnection.Robotics;
using RFRCC_RobotController.ABB_Data;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{

    public class OperationManoeuvre
    {
        private string _Movement = "";
        private string _Type = "";
        private string _Dim2Ref = "";
        private bool _StartCut;
        private bool _EndCut;
        private bool _WristFirst;
        private int _TargetVoltage;
        private CS_RobTarget _ManRobT;
        private CS_RobTarget _ManEndRobT;
        private CS_speeddata _ManSpeed = new CS_speeddata(1000, 500, 5000, 1000);
        private CS_zonedata _ManZone = new CS_zonedata(false, 100, 150, 150, 15, 150, 15);
        /// <summary>
        /// Name of Operation Manoeuvre
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// ZoneData from Robot relevant to manoeuvre
        /// </summary>
        public CS_zonedata ManZone
        {
            get
            {
                return _ManZone;
            }
            set
            {
                _ManZone = value;
            }
        }
        /// <summary>
        /// Speed Data from Robot relevant to manoeuvre
        /// </summary>
        public CS_speeddata ManSpeed
        {
            get
            {
                return _ManSpeed;
            }
            set
            {
                _ManSpeed = value;
            }
        }
        /// <summary>
        /// End Robtarget from Robot relevant to manoeuvre
        /// </summary>
        public CS_RobTarget ManEndRobT
        {
            get
            {
                return _ManEndRobT;
            }
            set
            {
                _ManEndRobT = value;
            }
        }
        /// <summary>
        /// RobTarget from Robot relevant to manoeuvre
        /// </summary>
        public CS_RobTarget ManRobT
        {
            get
            {
                return _ManRobT;
            }
            set
            {
                _ManRobT = value;
            }
        }
        /// <summary>
        /// Indication if wrist will be moved first in move
        /// </summary>
        public bool WristFirst
        {
            get
            {
                return _WristFirst;
            }
            set
            {
                _WristFirst = value;
            }
        }
        /// <summary>
        /// indication if position is final position in cutting move
        /// </summary>
        public bool EndCut
        {
            get
            {
                return _EndCut;
            }
            set
            {
                _EndCut = value;
            }
        }
        /// <summary>
        /// indication if position is starting position in cutting move
        /// </summary>
        public bool StartCut
        {
            get
            {
                return _StartCut;
            }
            set
            {
                _StartCut = value;
            }
        }
        /// <summary>
        /// target voltage of plasma cutter during cutting
        /// </summary>
        public int TargetVoltage
        {
            get
            {
                return _TargetVoltage;
            }

            set
            {
                _TargetVoltage = value;
            }
        }
        /// <summary>
        /// Reference edge of dimension 2
        /// </summary>
        public string Dim2Ref
        {
            get
            {
                return _Dim2Ref;
            }
            set
            {
                _Dim2Ref = value;
            }
        }
        /// <summary>
        /// Type of Operation to be executed?
        /// </summary>
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        /// <summary>
        /// Type of movement to be executed
        /// </summary>
        public string Movement
        {
            get
            {
                return _Movement;
            }
            set
            {
                _Movement = value;
            }
        }
        /// <summary>
        /// initialise empty object
        /// </summary>
        public OperationManoeuvre()
        {
            _Movement = "";
            _Type = "";
            _Dim2Ref = "";
            _StartCut = false;
            _EndCut = false;
            _WristFirst = false;
            _TargetVoltage = 0;
            _ManRobT = new CS_RobTarget();
            _ManEndRobT = new CS_RobTarget();
            _ManSpeed = new CS_speeddata();
            _ManZone = new CS_zonedata();
        }
        /// <summary>
        /// Initialise object from string representation of memory array
        /// format: "[Movement, Type, Dim2Ref, StartCut, EndCut, WristFirst, TargetVoltage, ManoeuvreRobT, ManoeuvreEndRobT, ManoeuvreSpeed, ManoeuvreZone]"
        /// </summary>
        /// <param name="Input">String representation of memory array</param>
        public OperationManoeuvre(string Input)
        {
            new OperationManoeuvre().FromString(Input);
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="target"></param>
        public OperationManoeuvre(Target target)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initialise object with direct infomration
        /// </summary>
        /// <param name="instruction">Move Instruction</param>
        /// <param name="toPoint">to Robtarget</param>
        /// <param name="cirPoint">mid Robtarget if circular move</param>
        /// <param name="TargetVoltage">Target voltatge of plasma cutter</param>
        /// <param name="Speed">robot speed</param>
        public OperationManoeuvre(MoveInstruction instruction, RobTarget toPoint, RobTarget cirPoint = null, int TargetVoltage = 0, double Speed = 0)
        {
            Name = instruction.ToPointName;
            _Movement = instruction.MotionType.ToString(); // this might need better clarification from "Joint" or "Linear"
            _Movement = _Movement == "Joint" && (cirPoint == null || cirPoint == (new RobTarget())) ? "Circle" : _Movement;
            _Type = Name.Contains("Safe") ? "Reposition" : "Cut";
            _StartCut = false;
            _EndCut = false;
            _TargetVoltage = TargetVoltage;
            _WristFirst = instruction.InstructionArguments["wristfirst"] != null && instruction.InstructionArguments["wristfirst"].Enabled;
            _ManRobT = new CS_RobTarget(toPoint);
            _ManSpeed.v_tcp = Speed > 0 ? Speed : _ManSpeed.v_tcp;
            _ManEndRobT = new CS_RobTarget(cirPoint.Frame != new RobTarget().Frame ? cirPoint : null);
        }
        /// <summary>
        /// update object from string representation of memory array
        /// format: "[Movement, Type, Dim2Ref, StartCut, EndCut, WristFirst, TargetVoltage, ManoeuvreRobT, ManoeuvreEndRobT, ManoeuvreSpeed, ManoeuvreZone]"
        /// </summary>
        /// <param name="String"></param>
        public void FromString(string String)
        {
            string[] inputArray = String.Trim('[', ']').Split(',');
            _Movement = inputArray[0].ToLower().Trim('\"');
            _Type = inputArray[1].ToLower().Trim('\"');
            _Dim2Ref = inputArray[2].ToLower().Trim('\"');
            _StartCut = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[3].ToLower());
            _EndCut = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[4].ToLower());
            _WristFirst = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[5].ToLower());
            _TargetVoltage = int.Parse(inputArray[6].ToLower());
            _ManRobT.FromString(string.Join(",", inputArray[7..24]).ToLower()); // 17 variables
            _ManEndRobT.FromString(string.Join(",", inputArray[24..41]).ToLower()); // 17 variables
            _ManSpeed.FromString(string.Join(",", inputArray[41..45]).ToLower()); // 4 variables
            _ManZone.FromString(string.Join(",", inputArray[45..52]).ToLower()); // 7 varables
        }
        /// <summary>
        /// provides string representation of memory array
        /// format: "[Movement, Type, Dim2Ref, StartCut, EndCut, WristFirst, TargetVoltage, ManoeuvreRobT, ManoeuvreEndRobT, ManoeuvreSpeed, ManoeuvreZone]"
        /// </summary>
        /// <returns>string representation of memory array</returns>
        public override string ToString()
        {
            return "[\"" +
               _Movement + "\",\"" +
               _Type + "\",\"" +
               _Dim2Ref + "\"," +
               _StartCut.ToString() + "," +
               _EndCut.ToString() + "," +
               _WristFirst.ToString() + "," +
               _TargetVoltage.ToString() + "," +
               _ManRobT.ToString() + "," +
               _ManEndRobT.ToString() + "," +
               _ManSpeed.ToString() + "," +
               _ManZone.ToString() + "]";
        }
        /// <summary>
        /// Elementwise clone of object
        /// </summary>
        /// <returns>new object clone of existing object</returns>
        public OperationManoeuvre Clone()
        {
            return (OperationManoeuvre)this.MemberwiseClone();
        }
    }
}
