namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    /// <summary>
    /// Job Data relevant to DSTV import
    /// </summary>
    public class JobFeature
    {
        /// <summary>
        /// Piece Identification
        /// </summary>
        public string JobID { get; set; }
        /// <summary>
        /// Index of feature in processing
        /// </summary>
        public int FeatureNum { get; set; }
        /// <summary>
        /// Code for feature type
        /// </summary>
        public string TaskCode { get; set; } // BO, SI, AK, IK
        /// <summary>
        /// If multiple features, this pertains to piece number
        /// </summary>
        public int TaskIndex { get; set; } // Bn <- n
        /// <summary>
        /// Which face of the stock features is on
        /// </summary>
        public string Face { get; set; }
        /// <summary>
        /// X1
        /// </summary>
        public float Dim1 { get; set; } // X
        /// <summary>
        /// X1_Optimal, set by robot for PLC to achieve
        /// </summary>
        public float Dim1Optimal { get; set; } // X set by Robot
        /// <summary>
        /// Y Reference edge
        /// </summary>
        public string Dim2Ref { get; set; } //YRef
        /// <summary>
        /// Y from reference
        /// </summary>
        public float Dim2 { get; set; } //Y
        /// <summary>
        /// Modifier 1
        /// </summary>
        public string Mod1 { get; set; } // AK:IK:Toolnotch
        /// <summary>
        /// Diameter of hole
        /// </summary>
        public float Diameter { get; set; }
        /// <summary>
        /// Radius of edge
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// Depth of hole
        /// </summary>
        public float Depth { get; set; }
        /// <summary>
        /// Modifier 2
        /// </summary>
        public string Mod2 { get; set; }
        /// <summary>
        /// Width
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// Angle from edge
        /// </summary>
        public float Angle { get; set; }
        /// <summary>
        /// Height of text
        /// </summary>
        public int TextHeight { get; set; }
        /// <summary>
        /// Alpha numeric text to be etched
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// minimum tolerance
        /// maybe display to user or check?
        /// </summary>
        public float ToleranceMin { get; set; }
        /// <summary>
        /// maximum tolerance
        /// maybe display to user or check?
        /// </summary>
        public float ToleranceMax { get; set; }

        // other stuff
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public float WeldPrep1 { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public float WeldPrep2 { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public float WeldPrep3 { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public float WeldPrep4 { get; set; }

        /// <summary>
        /// Display info if showing info in list
        /// </summary>
        public string DisplayInfo
        {
            get { return $"{ JobID }, { Face }, { Dim1 }, { Dim2Ref }, { Dim2 }, {Mod1 }, { Radius }, { WeldPrep1 }, { WeldPrep2 }, { WeldPrep3 }, { WeldPrep4 }"; }


        }
    }
}
