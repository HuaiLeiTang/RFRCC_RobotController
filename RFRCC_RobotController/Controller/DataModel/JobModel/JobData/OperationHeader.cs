using ABB.Robotics.Math;
using System;
using System.Text;
using System.Net;
using RFRCC_RobotController.ABB_Data;
using CopingLineModels;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    /// <summary>
    /// Header information for operation to be performed by robot
    /// </summary>
    public class OperationHeader
    {
        // --- INTERNAL FIELDS ---
        private int _FeatureNum;
        private double _IdealXDisplacement;
        private string _TaskCode;
        private string _Face;
        private CS_pos _LocationMin;
        private CS_pos _LocationMax;
        private int _NumInstructions;
        private int _NumManoeuvres;
        private int _ManoeuvreIndex;
        private bool _PathComplete;
        private bool _Ready;
        private bool _LeadInstruction;
        private bool _Complete;

        // --- EVENTS ---
        public event EventHandler IdealXDisplacementUpdate;

        // --- PROPERTIES ---
        /// <summary>
        /// name of operation
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// status if complete
        /// </summary>
        public bool Complete
        {
            get
            {
                return _Complete;
            }
            set
            {
                _Complete = value;
            }
        }
        /// <summary>
        /// Status if this is the first instruction to be completed at a new X loaction
        /// </summary>
        public bool LeadInstruction
        {
            get
            {
                return _LeadInstruction;
            }
            set
            {
                _LeadInstruction = value;
            }
        }
        /// <summary>
        /// status if this instruction is ready to be executed on the robot
        /// </summary>
        public bool Ready
        {
            get
            {
                return _Ready;
            }
            set
            {
                _Ready = value;
            }
        }
        /// <summary>
        /// status if the path for the operation has been completed 
        /// </summary>
        public bool PathComplete
        {
            get
            {
                return _PathComplete;
            }
            set
            {
                _PathComplete = value;
            }
        }
        /// <summary>
        /// index location of movement data in robot memory
        /// </summary>
        public int ManoeuvreIndex
        {
            get
            {
                return _ManoeuvreIndex;
            }
            set
            {
                _ManoeuvreIndex = value;
            }
        }
        /// <summary>
        /// number of manoeuvres required to complete feature
        /// </summary>
        public int NumManoeuvres
        {
            get
            {
                return _NumManoeuvres;
            }
            set
            {
                _NumManoeuvres = value;
            }
        }
        /// <summary>
        /// Number of instructions associated with complete feature
        /// </summary>
        public int NumInstructions
        {
            get
            {
                return _NumInstructions;
            }
            set
            {
                _NumInstructions = value;
            }
        }
        /// <summary>
        /// minimum XYZ Location 
        /// </summary>
        public CS_pos LocationMax
        {
            get
            {
                return _LocationMax;
            }
            set
            {
                _LocationMax = value;
            }
        }
        /// <summary>
        /// maximum XYZ location
        /// </summary>
        public CS_pos LocationMin
        {
            get
            {
                return _LocationMin;
            }
            set
            {
                _LocationMin = value;
            }
        }
        /// <summary>
        /// Face of stock profile that this feature is on
        /// </summary>
        public string Face
        {
            get
            {
                return _Face;
            }
            set
            {
                _Face = value;
            }
        }
        /// <summary>
        /// DSTV task code
        /// </summary>
        public string TaskCode
        {
            get
            {
                return _TaskCode;
            }
            set
            {
                _TaskCode = value;
            }
        }
        /// <summary>
        /// Ideal X of stock in machine for processing
        /// set by robot
        /// </summary>
        public double IdealXDisplacement
        {
            get
            {
                return _IdealXDisplacement;
            }
            set
            {
                _IdealXDisplacement = value;
                IdealXDisplacementUpdate?.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// index number of feature in processesing timeline
        /// </summary>
        public int FeatureNum
        {
            get
            {
                return _FeatureNum;
            }
            set
            {
                _FeatureNum = value;
            }
        }

        // --- CONSTRUCTORS ---
        /// <summary>
        /// intiialise feature with 0's
        /// </summary>
        public OperationHeader()
        {
            _FeatureNum = 0;
            _IdealXDisplacement = 0;
            _TaskCode = "";
            _Face = "";
            _LocationMin = new CS_pos();
            _LocationMax = new CS_pos();
            _NumInstructions = 0;
            _NumManoeuvres = 0;
            _ManoeuvreIndex = 0;
            _PathComplete = false;
            _Ready = false;
            _LeadInstruction = false;
            _Complete = false;
        }
        /// <summary>
        /// Initialise feature with from string representing array of information
        /// format: "[featurenum, idealXDisplacement, TaskCode, Face, LocationMin, LocationMax, NumInstructions, NumManoeuvres, MonoeuvreIndex, PathComplete, Ready, LeadInstruction, Complete]"
        /// </summary>
        /// <param name="input"></param>
        public OperationHeader(string input)
        {
            FromString(input);
        }
        /// <summary>
        /// Operation header populated from Manouvre header and list of manoeuvres
        /// </summary>
        /// <param name="manoeuvreHeader">Manoeuvre header information</param>
        /// <param name="movesList">List of Operation Manoeuvres that are covered by the Operation</param>
        public OperationHeader(Manoeuvre manoeuvreHeader, List<OperationManoeuvre> movesList)
        {
            Name = manoeuvreHeader.Name;
            _FeatureNum = 0;
            _IdealXDisplacement = 0;
            _TaskCode = "";
            _Face = manoeuvreHeader.Face.ToString();
            _LocationMin = movesList.Where(move => move.Type == "Cut").OrderBy(move => move.ManRobT.trans.X).First().ManRobT.trans;
            _LocationMax = movesList.Where(move => move.Type == "Cut").OrderByDescending(move => move.ManRobT.trans.X).First().ManRobT.trans;
            // might consider cheching ManEndRobT in future for min and max?
            _NumInstructions = 1;
            _NumManoeuvres = movesList.Count;
            _ManoeuvreIndex = 0;
            _PathComplete = true;
            _Ready = false;
            _LeadInstruction = false;
            _Complete = false;
        }

        // --- METHODS ---
        /// <summary>
        /// update object from information in string representation of robot data
        /// string format: "[featurenum, idealXDisplacement, TaskCode, Face, LocationMin, LocationMax, NumInstructions, NumManoeuvres, MonoeuvreIndex, PathComplete, Ready, LeadInstruction, Complete]
        /// </summary>
        /// <param name="String">string representation of rapid data</param>
        public void FromString(string String)
        {
            string[] inputArray = String.Trim('[', ']').Split(',');
            _FeatureNum = int.Parse(inputArray[0].ToLower());
            _IdealXDisplacement = double.Parse(inputArray[1].ToLower());
            _TaskCode = inputArray[2].ToLower().Trim('\"');
            _Face = inputArray[3].ToLower().Trim('\"');
            _LocationMin.FromString(string.Join(",", inputArray[4..7]).ToLower());
            _LocationMax = new CS_pos(string.Join(",", inputArray[7..10]).ToLower());
            _NumInstructions = int.Parse(inputArray[10].ToLower());
            _NumManoeuvres = int.Parse(inputArray[11].ToLower());
            _ManoeuvreIndex = int.Parse(inputArray[12].ToLower());
            _PathComplete = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[13].ToLower());
            _Ready = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[14].ToLower());
            _LeadInstruction = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[15].ToLower());
            _Complete = ABB.Robotics.Controllers.RapidDomain.Bool.Parse(inputArray[16].ToLower());
        }
        /// <summary>
        /// returns string representation of robot data
        /// format: "[featurenum, idealXDisplacement, TaskCode, Face, LocationMin, LocationMax, NumInstructions, NumManoeuvres, MonoeuvreIndex, PathComplete, Ready, LeadInstruction, Complete]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" +
               _FeatureNum.ToString() + "," +
               _IdealXDisplacement.ToString() + ",\"" +
               _TaskCode + "\",\"" +
               _Face + "\"," +
               _LocationMin.ToString() + "," +
               _LocationMax.ToString() + "," +
               _NumInstructions.ToString() + "," +
               _NumManoeuvres.ToString() + "," +
               _ManoeuvreIndex.ToString() + "," +
               _PathComplete.ToString() + "," +
               _Ready.ToString() + "," +
               _LeadInstruction.ToString() + "," +
               _Complete.ToString() + "]";
        }
        /// <summary>
        /// Clones object elementwise
        /// </summary>
        /// <returns></returns>
        public OperationHeader Clone()
        {
            return (OperationHeader)MemberwiseClone();
        }

    }
}
