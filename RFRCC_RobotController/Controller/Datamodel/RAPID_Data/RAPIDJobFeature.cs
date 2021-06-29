using ABB.Robotics.Controllers.RapidDomain;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    /// <summary>
    /// Job Data relevant to DSTV import
    /// </summary>
    class RAPIDJobFeature
    {
        /// <summary>
        /// Piece Identification
        /// </summary>
        public RapidString JobID { get; set; }
        /// <summary>
        /// Index of feature in processing
        /// </summary>
        public Num FeatureNum { get; set; }
        /// <summary>
        /// Code for feature type
        /// </summary>
        public RapidString TaskCode { get; set; } // BO, SI, AK, IK
        /// <summary>
        /// If multiple features, this pertains to piece number
        /// </summary>
        public Num TaskIndex { get; set; } // Bn <- n
        /// <summary>
        /// Which face of the stock features is on
        public RapidString Face { get; set; }
        /// <summary>
        /// X1
        /// </summary>
        public Num Dim1 { get; set; } // X
        /// <summary>
        /// X1_Optimal, set by robot for PLC to achieve
        /// </summary>
        public Num Dim1Optimal { get; set; } // X set by Robot
        /// <summary>
        /// Y Reference edge
        /// </summary>
        public RapidString Dim2Ref { get; set; } //YRef
        /// <summary>
        /// Y from reference
        /// </summary>
        public Num Dim2 { get; set; } //Y
        /// <summary>
        /// Modifier 1
        /// </summary>
        public RapidString Mod1 { get; set; } // AK:IK:Toolnotch
        /// <summary>
        /// Diameter of hole
        /// </summary>
        public Num Diameter { get; set; }
        /// <summary>
        /// Radius of edge
        /// </summary>
        public Num Radius { get; set; }
        /// <summary>
        /// Depth of hole
        /// </summary>
        public Num Depth { get; set; }
        /// <summary>
        /// Modifier 2
        /// </summary>
        public RapidString Mod2 { get; set; }
        /// <summary>
        /// Width
        /// </summary>
        public Num Width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public Num Height { get; set; }
        /// <summary>
        /// Angle from edge
        /// </summary>
        public Num Angle { get; set; }
        /// <summary>
        /// Height of text
        /// </summary>
        public Num TextHeight { get; set; }
        /// <summary>
        /// Alpha numeric text to be etched
        /// </summary>
        public RapidString Text { get; set; }
        /// <summary>
        /// minimum tolerance
        /// maybe display to user or check?
        /// </summary>
        public Num ToleranceMin { get; set; }
        /// <summary>
        /// maximum tolerance
        /// maybe display to user or check?
        /// </summary>
        public Num ToleranceMax { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public Num WeldPrep1 { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public Num WeldPrep2 { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public Num WeldPrep3 { get; set; }
        /// <summary>
        /// Value for welding preparation
        /// </summary>
        public Num WeldPrep4 { get; set; }
        /// <summary>
        /// Populate from RAPIDData
        /// </summary>
        /// <param name="RAPIDData">RAPID Data Node</param>
        public void GetFromRapidData(RapidData RAPIDData)
        {
            DataNode[] RapidStruct = RAPIDData.Value.ToStructure().Children.ToArray();

            JobID = RapidString.Parse(RapidStruct[0].Value);
            FeatureNum = Num.Parse(RapidStruct[1].Value);
            TaskCode = RapidString.Parse(RapidStruct[2].Value);
            TaskIndex = Num.Parse(RapidStruct[3].Value);
            Face = RapidString.Parse(RapidStruct[4].Value);
            Dim1 = Num.Parse(RapidStruct[5].Value);
            Dim1Optimal = Num.Parse(RapidStruct[6].Value);
            Dim2Ref = RapidString.Parse(RapidStruct[7].Value);
            Dim2 = Num.Parse(RapidStruct[8].Value);
            Mod1 = RapidString.Parse(RapidStruct[9].Value);
            Diameter = Num.Parse(RapidStruct[10].Value);
            Radius = Num.Parse(RapidStruct[11].Value);
            Depth = Num.Parse(RapidStruct[12].Value);
            Mod2 = RapidString.Parse(RapidStruct[13].Value);
            Width = Num.Parse(RapidStruct[14].Value);
            Height = Num.Parse(RapidStruct[15].Value);
            Angle = Num.Parse(RapidStruct[16].Value);
            TextHeight = Num.Parse(RapidStruct[17].Value);
            Text = RapidString.Parse(RapidStruct[18].Value);
            ToleranceMin = Num.Parse(RapidStruct[19].Value);
            ToleranceMax = Num.Parse(RapidStruct[20].Value);
            WeldPrep1 = Num.Parse(RapidStruct[21].Value);
            WeldPrep2 = Num.Parse(RapidStruct[22].Value);
            WeldPrep3 = Num.Parse(RapidStruct[23].Value);
            WeldPrep4 = Num.Parse(RapidStruct[24].Value);
        }
        /// <summary>
        /// Populate from String representation of RAPID Structure
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            string output = "[" +
                JobID.ToString() + "," +
                FeatureNum.ToString() + "," +
                TaskCode.ToString() + "," +
                TaskIndex.ToString() + "," +
                Face.ToString() + "," +
                Dim1.ToString() + "," +
                Dim1Optimal.ToString() + "," +
                Dim2Ref.ToString() + "," +
                Dim2.ToString() + "," +
                Mod1.ToString() + "," +
                Diameter.ToString() + "," +
                Radius.ToString() + "," +
                Depth.ToString() + "," +
                Mod2.ToString() + "," +
                Width.ToString() + "," +
                Height.ToString() + "," +
                Angle.ToString() + "," +
                TextHeight.ToString() + "," +
                Text.ToString() + "," +
                ToleranceMin.ToString() + "," +
                ToleranceMax.ToString() + "," +
                WeldPrep1.ToString() + "," +
                WeldPrep2.ToString() + "," +
                WeldPrep3.ToString() + "," +
                WeldPrep4.ToString() +
                "]";
            return output;
        }
        /// <summary>
        /// Populate from JobFeature on SQL updated
        /// TO BE REMOVED
        /// </summary>
        /// <param name="SQLFeature">Job Feature pulled from SQL Table</param>
        public void UpdatedFromSQL(JobFeature SQLFeature)
        {
            JobID = RapidString.Parse(SQLFeature.JobID); //Not Nullable
            FeatureNum = Num.Parse(SQLFeature.FeatureNum.ToString());
            TaskCode = RapidString.Parse(SQLFeature.TaskCode); // Not Nullable
            TaskIndex = Num.Parse(SQLFeature.TaskIndex.ToString());
            if (SQLFeature.Face != null) { Face = RapidString.Parse(SQLFeature.Face); }
            Dim1 = Num.Parse(SQLFeature.Dim1.ToString());
            Dim1Optimal = Num.Parse(SQLFeature.Dim1Optimal.ToString());
            if (SQLFeature.Dim2Ref != null) { Dim2Ref = RapidString.Parse(SQLFeature.Dim2Ref); }
            Dim2 = Num.Parse(SQLFeature.Dim2.ToString());
            if (SQLFeature.Mod1 != null) { Mod1 = RapidString.Parse(SQLFeature.Mod1); }
            Diameter = Num.Parse(SQLFeature.Diameter.ToString());
            Radius = Num.Parse(SQLFeature.Radius.ToString());
            Depth = Num.Parse(SQLFeature.Depth.ToString());
            if (SQLFeature.Mod2 != null) { Mod2 = RapidString.Parse(SQLFeature.Mod2); }
            Width = Num.Parse(SQLFeature.Width.ToString());
            Height = Num.Parse(SQLFeature.Height.ToString());
            Angle = Num.Parse(SQLFeature.Angle.ToString());
            TextHeight = Num.Parse(SQLFeature.TextHeight.ToString());
            if (SQLFeature.Text != null) { Text = RapidString.Parse(SQLFeature.Text); }
            ToleranceMin = Num.Parse(SQLFeature.ToleranceMin.ToString());
            ToleranceMax = Num.Parse(SQLFeature.ToleranceMax.ToString());
            WeldPrep1 = Num.Parse(SQLFeature.WeldPrep1.ToString());
            WeldPrep2 = Num.Parse(SQLFeature.WeldPrep2.ToString());
            WeldPrep3 = Num.Parse(SQLFeature.WeldPrep3.ToString());
            WeldPrep4 = Num.Parse(SQLFeature.WeldPrep4.ToString());
        }

    }
}
