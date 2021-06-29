namespace RFRCC_RobotController.Controller.DataModel.OperationData
{
    public class JobHeader
    {
        /// <summary>
        /// File name and job allocated title
        /// </summary>
        public string JobID { get; set; }
        /// <summary>
        /// Order Identification
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// Drawing Idenidification
        /// </summary>
        public string DwgID { get; set; } //DrawingID
        /// <summary>
        /// Phase Identification
        /// </summary>
        public string PhaseID { get; set; }
        /// <summary>
        /// Piece Identifcaiotn
        /// </summary>
        public string PieceID { get; set; }
        /// <summary>
        /// Steel Qaulity
        /// </summary>
        public string SteelQual { get; set; }
        /// <summary>
        /// Piece Quantity
        /// </summary>
        public int PieceQty { get; set; }
        /// <summary>
        /// Steel Profile
        /// </summary>
        public string Profile { get; set; }
        /// <summary>
        /// Steel Code Profile?
        /// </summary>
        public string CodeProfile { get; set; }
        /// <summary>
        /// Length of final piece
        /// </summary>
        public float Length { get; set; }
        /// <summary>
        /// Minimum saw cut stock length required
        /// </summary>
        public float SawLength { get; set; }
        /// <summary>
        /// Profile height
        /// </summary>
        public float Height { get; set; } //ProfileHeight
        /// <summary>
        /// Flange Width
        /// </summary>
        public float FlangeWidth { get; set; }
        /// <summary>
        /// Flange thickness
        /// </summary>
        public float FlangeThick { get; set; } //FlangeThickness
        /// <summary>
        /// Web Thickness
        /// </summary>
        public float WebThick { get; set; } //WebThickness
        /// <summary>
        /// Inner web radius
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// Weight of profile per mtr
        /// </summary>
        public float Weight { get; set; } //WeightByMtr
        /// <summary>
        /// Paint surface area per mtr
        /// </summary>
        public float PaintSurf { get; set; } //PaintSurfaceByMtr
        /// <summary>
        /// Web start cut angle
        /// </summary>
        public float WebStartCut { get; set; }
        /// <summary>
        /// Web end cut angle
        /// </summary>
        public float WebEndCut { get; set; }
        /// <summary>
        /// Flange start cut angle
        /// </summary>
        public float FlangeStartCut { get; set; }
        /// <summary>
        /// flange end cut angle
        /// </summary>
        public float FlangeEndCut { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public string TextInfo1 { get; set; } // suggest only one
        /// <summary>
        /// comment
        /// </summary>
        public string TextInfo2 { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public string TextInfo3 { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public string TextInfo4 { get; set; }
        /// <summary>
        /// Number of features within piece
        /// </summary>
        public int FeatureQuant { get; set; }
        /// <summary>
        /// Display info prepared in displaying
        /// </summary>
        public string DisplayInfo
        {
            get { return $"{JobID}, { OrderID }, { DwgID }, { PhaseID }, { PieceID }, { SteelQual }, {PieceQty}, { Profile }, { CodeProfile }, { Length }, { Height }, { FlangeWidth }, { FlangeThick }, { WebThick }, { Radius }, { Weight }, { PaintSurf }, { WebStartCut }, { WebEndCut }, { FlangeStartCut }, { FlangeEndCut }, { TextInfo1 }, { TextInfo2 }, { TextInfo3 }, { TextInfo4 }, {FeatureQuant} "; }


        }

    }
}
