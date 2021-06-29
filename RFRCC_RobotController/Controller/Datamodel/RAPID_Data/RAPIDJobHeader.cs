using ABB.Robotics.Controllers.RapidDomain;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    /// <summary>
    /// Job Header information from DSTV import
    /// </summary>
    class RAPIDJobHeader
    {
        /// <summary>
        /// File name and job allocated title
        /// </summary>
        public RapidString JobID { get; set; }
        /// <summary>
        /// Order Identification
        /// </summary>
        public RapidString OrderID { get; set; }
        /// <summary>
        /// Drawing Idenidification
        /// </summary>
        public RapidString DwgID { get; set; } //DrawingID
        /// <summary>
        /// Phase Identification
        /// </summary>
        public RapidString PhaseID { get; set; }
        /// <summary>
        /// Piece Identifcaiotn
        /// </summary>
        public RapidString PieceID { get; set; }
        /// <summary>
        /// Steel Qaulity
        /// </summary>
        public RapidString SteelQual { get; set; }
        /// <summary>
        /// Piece Quantity
        /// </summary>
        public Num PieceQty { get; set; }
        /// <summary>
        /// Steel Profile
        /// </summary>
        public RapidString Profile { get; set; }
        /// <summary>
        /// Steel Code Profile?
        /// </summary>
        public RapidString CodeProfile { get; set; }
        /// <summary>
        /// Length of final piece
        /// </summary>
        public Num Length { get; set; }
        /// <summary>
        /// Minimum saw cut stock length required
        /// </summary>
        public Num SawLength { get; set; }
        /// <summary>
        /// Profile height
        /// </summary>
        public Num Height { get; set; } //ProfileHeight
        /// <summary>
        /// Flange Width
        /// </summary>
        public Num FlangeWidth { get; set; }
        /// <summary>
        /// Flange thickness
        /// </summary>
        public Num FlangeThick { get; set; } //FlangeThickness
        /// <summary>
        /// Web Thickness
        /// </summary>
        public Num WebThick { get; set; } //WebThickness
        /// <summary>
        /// Inner web radius
        /// </summary>
        public Num Radius { get; set; }
        /// <summary>
        /// Weight of profile per mtr
        /// </summary>
        public Num Weight { get; set; } //WeightByMtr
        /// <summary>
        /// Paint surface area per mtr
        /// </summary>
        public Num PaintSurf { get; set; } //PaintSurfaceByMtr
        /// <summary>
        /// Web start cut angle
        /// </summary>
        public Num WebStartCut { get; set; }
        /// <summary>
        /// Web end cut angle
        /// </summary>
        public Num WebEndCut { get; set; }
        /// <summary>
        /// Flange start cut angle
        /// </summary>
        public Num FlangeStartCut { get; set; }
        /// <summary>
        /// flange end cut angle
        /// </summary>
        public Num FlangeEndCut { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public RapidString TextInfo1 { get; set; } // suggest only one
        /// <summary>
        /// comment
        /// </summary>
        public RapidString TextInfo2 { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public RapidString TextInfo3 { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public RapidString TextInfo4 { get; set; }
        /// <summary>
        /// Number of features within piece
        /// </summary>
        public Num FeatureQuant { get; set; }
        /// <summary>
        /// Populate from RAPIDData
        /// </summary>
        /// <param name="RAPIDData">RAPID Data Node</param>
        public void GetFromRapidData(RapidData RAPIDData)
        {
            DataNode[] RapidStruct = RAPIDData.Value.ToStructure().Children.ToArray();

            JobID = RapidString.Parse(RapidStruct[0].Value);
            OrderID = RapidString.Parse(RapidStruct[1].Value);
            DwgID = RapidString.Parse(RapidStruct[2].Value);
            PhaseID = RapidString.Parse(RapidStruct[3].Value);
            PieceID = RapidString.Parse(RapidStruct[4].Value);
            SteelQual = RapidString.Parse(RapidStruct[5].Value);
            PieceQty = Num.Parse(RapidStruct[6].Value);
            Profile = RapidString.Parse(RapidStruct[7].Value);
            CodeProfile = RapidString.Parse(RapidStruct[8].Value);
            Length = Num.Parse(RapidStruct[9].Value);
            SawLength = Num.Parse(RapidStruct[10].Value);
            Height = Num.Parse(RapidStruct[11].Value);
            FlangeWidth = Num.Parse(RapidStruct[12].Value);
            FlangeThick = Num.Parse(RapidStruct[13].Value);
            WebThick = Num.Parse(RapidStruct[14].Value);
            Radius = Num.Parse(RapidStruct[15].Value);
            Weight = Num.Parse(RapidStruct[16].Value);
            PaintSurf = Num.Parse(RapidStruct[17].Value);
            WebStartCut = Num.Parse(RapidStruct[18].Value);
            WebEndCut = Num.Parse(RapidStruct[19].Value);
            FlangeStartCut = Num.Parse(RapidStruct[20].Value);
            FlangeEndCut = Num.Parse(RapidStruct[21].Value);
            TextInfo1 = RapidString.Parse(RapidStruct[22].Value);
            TextInfo2 = RapidString.Parse(RapidStruct[23].Value);
            TextInfo3 = RapidString.Parse(RapidStruct[24].Value);
            TextInfo4 = RapidString.Parse(RapidStruct[25].Value);
            FeatureQuant = Num.Parse(RapidStruct[26].Value);
        }
        /// <summary>
        /// Populate from String representation of RAPID Structure
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            string output = "[" +
                JobID.ToString() + "," +
                OrderID.ToString() + "," +
                DwgID.ToString() + "," +
                PhaseID.ToString() + "," +
                PieceID.ToString() + "," +
                SteelQual.ToString() + "," +
                PieceQty.ToString() + "," +
                Profile.ToString() + "," +
                CodeProfile.ToString() + "," +
                Length.ToString() + "," +
                SawLength.ToString() + "," +
                Height.ToString() + "," +
                FlangeWidth.ToString() + "," +
                FlangeThick.ToString() + "," +
                WebThick.ToString() + "," +
                Radius.ToString() + "," +
                Weight.ToString() + "," +
                PaintSurf.ToString() + "," +
                WebStartCut.ToString() + "," +
                WebEndCut.ToString() + "," +
                FlangeStartCut.ToString() + "," +
                FlangeEndCut.ToString() + "," +
                TextInfo1.ToString() + "," +
                TextInfo2.ToString() + "," +
                TextInfo3.ToString() + "," +
                TextInfo4.ToString() + "," +
                FeatureQuant.ToString() +
                "]";
            return output;
        }
        /// <summary>
        /// Populate from JobFeature on SQL updated
        /// TO BE REMOVED
        /// </summary>
        /// <param name="SQLFeature">Job Feature pulled from SQL Table</param>
        public void UpdatedFromSQL(JobHeader SQLHeader)
        {
            JobID = RapidString.Parse(SQLHeader.JobID);
            OrderID = RapidString.Parse(SQLHeader.OrderID);
            DwgID = RapidString.Parse(SQLHeader.DwgID);
            PhaseID = RapidString.Parse(SQLHeader.PhaseID);
            PieceID = RapidString.Parse(SQLHeader.PieceID);
            SteelQual = RapidString.Parse(SQLHeader.SteelQual);
            PieceQty = Num.Parse(SQLHeader.PieceQty.ToString());
            Profile = RapidString.Parse(SQLHeader.Profile);
            CodeProfile = RapidString.Parse(SQLHeader.CodeProfile);
            Length = Num.Parse(SQLHeader.Length.ToString());
            SawLength = Num.Parse(SQLHeader.SawLength.ToString());
            Height = Num.Parse(SQLHeader.Height.ToString());
            FlangeWidth = Num.Parse(SQLHeader.FlangeWidth.ToString());
            FlangeThick = Num.Parse(SQLHeader.FlangeThick.ToString());
            WebThick = Num.Parse(SQLHeader.WebThick.ToString());
            Radius = Num.Parse(SQLHeader.Radius.ToString());
            Weight = Num.Parse(SQLHeader.Weight.ToString());
            PaintSurf = Num.Parse(SQLHeader.PaintSurf.ToString());
            WebStartCut = Num.Parse(SQLHeader.WebStartCut.ToString());
            WebEndCut = Num.Parse(SQLHeader.WebEndCut.ToString());
            FlangeStartCut = Num.Parse(SQLHeader.FlangeStartCut.ToString());
            FlangeEndCut = Num.Parse(SQLHeader.FlangeEndCut.ToString());
            if (SQLHeader.TextInfo1 != null) { TextInfo1 = RapidString.Parse(SQLHeader.TextInfo1); }
            if (SQLHeader.TextInfo2 != null) { TextInfo2 = RapidString.Parse(SQLHeader.TextInfo2); }
            if (SQLHeader.TextInfo3 != null) { TextInfo3 = RapidString.Parse(SQLHeader.TextInfo3); }
            if (SQLHeader.TextInfo4 != null) { TextInfo4 = RapidString.Parse(SQLHeader.TextInfo4); }
            FeatureQuant = Num.Parse(SQLHeader.FeatureQuant.ToString());
        }
    }
}
