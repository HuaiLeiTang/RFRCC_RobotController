using ABB.Robotics.Controllers.RapidDomain;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    class RAPIDJobHeader
    {

        public RapidString JobID { get; set; }
        public RapidString OrderID { get; set; }
        public RapidString DwgID { get; set; } //DrawingID
        public RapidString PhaseID { get; set; }
        public RapidString PieceID { get; set; }
        public RapidString SteelQual { get; set; }
        public Num PieceQty { get; set; }
        public RapidString Profile { get; set; }
        public RapidString CodeProfile { get; set; }
        public Num Length { get; set; }
        public Num SawLength { get; set; }
        public Num Height { get; set; } //ProfileHeight
        public Num FlangeWidth { get; set; }
        public Num FlangeThick { get; set; } //FlangeThickness
        public Num WebThick { get; set; } //WebThickness
        public Num Radius { get; set; }
        public Num Weight { get; set; } //WeightByMtr
        public Num PaintSurf { get; set; } //PaintSurfaceByMtr
        public Num WebStartCut { get; set; }
        public Num WebEndCut { get; set; }
        public Num FlangeStartCut { get; set; }
        public Num FlangeEndCut { get; set; }
        public RapidString TextInfo1 { get; set; } // suggest only one
        public RapidString TextInfo2 { get; set; }
        public RapidString TextInfo3 { get; set; }
        public RapidString TextInfo4 { get; set; }
        public Num FeatureQuant { get; set; }

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
