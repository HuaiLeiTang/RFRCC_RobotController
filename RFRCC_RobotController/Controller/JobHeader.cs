namespace RFRCC_RobotController.Controller
{
    public class JobHeader
    {
        public string JobID { get; set; }
        public string OrderID { get; set; }
        public string DwgID { get; set; } //DrawingID
        public string PhaseID { get; set; }
        public string PieceID { get; set; }
        public string SteelQual { get; set; }
        public int PieceQty { get; set; }
        public string Profile { get; set; }
        public string CodeProfile { get; set; }
        public float Length { get; set; }
        public float SawLength { get; set; }
        public float Height { get; set; } //ProfileHeight
        public float FlangeWidth { get; set; }
        public float FlangeThick { get; set; } //FlangeThickness
        public float WebThick { get; set; } //WebThickness
        public float Radius { get; set; }
        public float Weight { get; set; } //WeightByMtr
        public float PaintSurf { get; set; } //PaintSurfaceByMtr
        public float WebStartCut { get; set; }
        public float WebEndCut { get; set; }
        public float FlangeStartCut { get; set; }
        public float FlangeEndCut { get; set; }
        public string TextInfo1 { get; set; } // suggest only one
        public string TextInfo2 { get; set; }
        public string TextInfo3 { get; set; }
        public string TextInfo4 { get; set; }
        public int FeatureQuant { get; set; }


        public string DisplayInfo
        {
            get { return $"{JobID}, { OrderID }, { DwgID }, { PhaseID }, { PieceID }, { SteelQual }, {PieceQty}, { Profile }, { CodeProfile }, { Length }, { Height }, { FlangeWidth }, { FlangeThick }, { WebThick }, { Radius }, { Weight }, { PaintSurf }, { WebStartCut }, { WebEndCut }, { FlangeStartCut }, { FlangeEndCut }, { TextInfo1 }, { TextInfo2 }, { TextInfo3 }, { TextInfo4 }, {FeatureQuant} "; }


        }

    }
}
