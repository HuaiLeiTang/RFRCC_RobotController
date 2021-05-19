using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;

namespace RFARCC_RobotController.RCC_RobotController
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
    public class JobFeature
    {
        public string JobID { get; set; }
        public int FeatureNum { get; set; }
        public string TaskCode { get; set; } // BO, SI, AK, IK
        public int TaskIndex { get; set; } // Bn <- n
        public string Face { get; set; }
        public float Dim1 { get; set; } // X
        public float Dim1Optimal { get; set; } // X set by Robot
        public string Dim2Ref { get; set; } //YRef
        public float Dim2 { get; set; } //Y
        public string Mod1 { get; set; } // AK:IK:Toolnotch
        public float Diameter { get; set; }
        public float Radius { get; set; }
        public float Depth { get; set; }
        public string Mod2 { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Angle { get; set; }
        public int TextHeight { get; set; }
        public string Text { get; set; }
        public float ToleranceMin { get; set; }
        public float ToleranceMax { get; set; }

        // other stuff
        public float WeldPrep1 { get; set; }
        public float WeldPrep2 { get; set; }
        public float WeldPrep3 { get; set; }
        public float WeldPrep4 { get; set; }

        public string DisplayInfo
        {
            get { return $"{ JobID }, { Face }, { Dim1 }, { Dim2Ref }, { Dim2 }, {Mod1 }, { Radius }, { WeldPrep1 }, { WeldPrep2 }, { WeldPrep3 }, { WeldPrep4 }"; }


        }
    }
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

            this.JobID = RapidString.Parse(RapidStruct[0].Value);
            this.OrderID = RapidString.Parse(RapidStruct[1].Value);
            this.DwgID = RapidString.Parse(RapidStruct[2].Value);
            this.PhaseID = RapidString.Parse(RapidStruct[3].Value);
            this.PieceID = RapidString.Parse(RapidStruct[4].Value);
            this.SteelQual = RapidString.Parse(RapidStruct[5].Value);
            this.PieceQty = Num.Parse(RapidStruct[6].Value);
            this.Profile = RapidString.Parse(RapidStruct[7].Value);
            this.CodeProfile = RapidString.Parse(RapidStruct[8].Value);
            this.Length = Num.Parse(RapidStruct[9].Value);
            this.SawLength = Num.Parse(RapidStruct[10].Value);
            this.Height = Num.Parse(RapidStruct[11].Value);
            this.FlangeWidth = Num.Parse(RapidStruct[12].Value);
            this.FlangeThick = Num.Parse(RapidStruct[13].Value);
            this.WebThick = Num.Parse(RapidStruct[14].Value);
            this.Radius = Num.Parse(RapidStruct[15].Value);
            this.Weight = Num.Parse(RapidStruct[16].Value);
            this.PaintSurf = Num.Parse(RapidStruct[17].Value);
            this.WebStartCut = Num.Parse(RapidStruct[18].Value);
            this.WebEndCut = Num.Parse(RapidStruct[19].Value);
            this.FlangeStartCut = Num.Parse(RapidStruct[20].Value);
            this.FlangeEndCut = Num.Parse(RapidStruct[21].Value);
            this.TextInfo1 = RapidString.Parse(RapidStruct[22].Value);
            this.TextInfo2 = RapidString.Parse(RapidStruct[23].Value);
            this.TextInfo3 = RapidString.Parse(RapidStruct[24].Value);
            this.TextInfo4 = RapidString.Parse(RapidStruct[25].Value);
            this.FeatureQuant = Num.Parse(RapidStruct[26].Value);
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
            this.JobID = RapidString.Parse(SQLHeader.JobID);
            this.OrderID = RapidString.Parse(SQLHeader.OrderID);
            this.DwgID = RapidString.Parse(SQLHeader.DwgID);
            this.PhaseID = RapidString.Parse(SQLHeader.PhaseID);
            this.PieceID = RapidString.Parse(SQLHeader.PieceID);
            this.SteelQual = RapidString.Parse(SQLHeader.SteelQual);
            this.PieceQty = Num.Parse(SQLHeader.PieceQty.ToString());
            this.Profile = RapidString.Parse(SQLHeader.Profile);
            this.CodeProfile = RapidString.Parse(SQLHeader.CodeProfile);
            this.Length = Num.Parse(SQLHeader.Length.ToString());
            this.SawLength = Num.Parse(SQLHeader.SawLength.ToString());
            this.Height = Num.Parse(SQLHeader.Height.ToString());
            this.FlangeWidth = Num.Parse(SQLHeader.FlangeWidth.ToString());
            this.FlangeThick = Num.Parse(SQLHeader.FlangeThick.ToString());
            this.WebThick = Num.Parse(SQLHeader.WebThick.ToString());
            this.Radius = Num.Parse(SQLHeader.Radius.ToString());
            this.Weight = Num.Parse(SQLHeader.Weight.ToString());
            this.PaintSurf = Num.Parse(SQLHeader.PaintSurf.ToString());
            this.WebStartCut = Num.Parse(SQLHeader.WebStartCut.ToString());
            this.WebEndCut = Num.Parse(SQLHeader.WebEndCut.ToString());
            this.FlangeStartCut = Num.Parse(SQLHeader.FlangeStartCut.ToString());
            this.FlangeEndCut = Num.Parse(SQLHeader.FlangeEndCut.ToString());
            if (SQLHeader.TextInfo1 != null) { this.TextInfo1 = RapidString.Parse(SQLHeader.TextInfo1); }
            if (SQLHeader.TextInfo2 != null) { this.TextInfo2 = RapidString.Parse(SQLHeader.TextInfo2); }
            if (SQLHeader.TextInfo3 != null) { this.TextInfo3 = RapidString.Parse(SQLHeader.TextInfo3); }
            if (SQLHeader.TextInfo4 != null) { this.TextInfo4 = RapidString.Parse(SQLHeader.TextInfo4); }
            this.FeatureQuant = Num.Parse(SQLHeader.FeatureQuant.ToString());
        }
    }
    class RAPIDJobFeature
    {
        public RapidString JobID { get; set; }
        public Num FeatureNum { get; set; }
        public RapidString TaskCode { get; set; } // BO, SI, AK, IK
        public Num TaskIndex { get; set; } // Bn <- n
        public RapidString Face { get; set; }
        public Num Dim1 { get; set; } // X
        public Num Dim1Optimal { get; set; } // X set by Robot
        public RapidString Dim2Ref { get; set; } //YRef
        public Num Dim2 { get; set; } //Y
        public RapidString Mod1 { get; set; } // AK:IK:Toolnotch
        public Num Diameter { get; set; }
        public Num Radius { get; set; }
        public Num Depth { get; set; }
        public RapidString Mod2 { get; set; }
        public Num Width { get; set; }
        public Num Height { get; set; }
        public Num Angle { get; set; }
        public Num TextHeight { get; set; }
        public RapidString Text { get; set; }
        public Num ToleranceMin { get; set; }
        public Num ToleranceMax { get; set; }
        public Num WeldPrep1 { get; set; }
        public Num WeldPrep2 { get; set; }
        public Num WeldPrep3 { get; set; }
        public Num WeldPrep4 { get; set; }

        public void GetFromRapidData(RapidData RAPIDData)
        {
            DataNode[] RapidStruct = RAPIDData.Value.ToStructure().Children.ToArray();

            this.JobID = RapidString.Parse(RapidStruct[0].Value);
            this.FeatureNum = Num.Parse(RapidStruct[1].Value);
            this.TaskCode = RapidString.Parse(RapidStruct[2].Value);
            this.TaskIndex = Num.Parse(RapidStruct[3].Value);
            this.Face = RapidString.Parse(RapidStruct[4].Value);
            this.Dim1 = Num.Parse(RapidStruct[5].Value);
            this.Dim1Optimal = Num.Parse(RapidStruct[6].Value);
            this.Dim2Ref = RapidString.Parse(RapidStruct[7].Value);
            this.Dim2 = Num.Parse(RapidStruct[8].Value);
            this.Mod1 = RapidString.Parse(RapidStruct[9].Value);
            this.Diameter = Num.Parse(RapidStruct[10].Value);
            this.Radius = Num.Parse(RapidStruct[11].Value);
            this.Depth = Num.Parse(RapidStruct[12].Value);
            this.Mod2 = RapidString.Parse(RapidStruct[13].Value);
            this.Width = Num.Parse(RapidStruct[14].Value);
            this.Height = Num.Parse(RapidStruct[15].Value);
            this.Angle = Num.Parse(RapidStruct[16].Value);
            this.TextHeight = Num.Parse(RapidStruct[17].Value);
            this.Text = RapidString.Parse(RapidStruct[18].Value);
            this.ToleranceMin = Num.Parse(RapidStruct[19].Value);
            this.ToleranceMax = Num.Parse(RapidStruct[20].Value);
            this.WeldPrep1 = Num.Parse(RapidStruct[21].Value);
            this.WeldPrep2 = Num.Parse(RapidStruct[22].Value);
            this.WeldPrep3 = Num.Parse(RapidStruct[23].Value);
            this.WeldPrep4 = Num.Parse(RapidStruct[24].Value);
        }

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

        public void UpdatedFromSQL(JobFeature SQLFeature)
        {
            this.JobID = RapidString.Parse(SQLFeature.JobID); //Not Nullable
            this.FeatureNum = Num.Parse(SQLFeature.FeatureNum.ToString());
            this.TaskCode = RapidString.Parse(SQLFeature.TaskCode); // Not Nullable
            this.TaskIndex = Num.Parse(SQLFeature.TaskIndex.ToString());
            if (SQLFeature.Face != null) { this.Face = RapidString.Parse(SQLFeature.Face); }
            this.Dim1 = Num.Parse(SQLFeature.Dim1.ToString());
            this.Dim1Optimal = Num.Parse(SQLFeature.Dim1Optimal.ToString());
            if (SQLFeature.Dim2Ref != null) { this.Dim2Ref = RapidString.Parse(SQLFeature.Dim2Ref); }
            this.Dim2 = Num.Parse(SQLFeature.Dim2.ToString());
            if (SQLFeature.Mod1 != null) { this.Mod1 = RapidString.Parse(SQLFeature.Mod1); }
            this.Diameter = Num.Parse(SQLFeature.Diameter.ToString());
            this.Radius = Num.Parse(SQLFeature.Radius.ToString());
            this.Depth = Num.Parse(SQLFeature.Depth.ToString());
            if (SQLFeature.Mod2 != null) { this.Mod2 = RapidString.Parse(SQLFeature.Mod2); }
            this.Width = Num.Parse(SQLFeature.Width.ToString());
            this.Height = Num.Parse(SQLFeature.Height.ToString());
            this.Angle = Num.Parse(SQLFeature.Angle.ToString());
            this.TextHeight = Num.Parse(SQLFeature.TextHeight.ToString());
            if (SQLFeature.Text != null) { this.Text = RapidString.Parse(SQLFeature.Text); }
            this.ToleranceMin = Num.Parse(SQLFeature.ToleranceMin.ToString());
            this.ToleranceMax = Num.Parse(SQLFeature.ToleranceMax.ToString());
            this.WeldPrep1 = Num.Parse(SQLFeature.WeldPrep1.ToString());
            this.WeldPrep2 = Num.Parse(SQLFeature.WeldPrep2.ToString());
            this.WeldPrep3 = Num.Parse(SQLFeature.WeldPrep3.ToString());
            this.WeldPrep4 = Num.Parse(SQLFeature.WeldPrep4.ToString());
        }

    }

    public class RAPIDJob_Header
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

        private RapidData _RAPIDData;
        private Controller _controller;

        public RAPIDJob_Header(Controller controller, Task task, string tool, string workobject)
        {
            _RAPIDData = task.GetRapidData(tool, workobject);
            _controller = controller;
        }
        public void Upload()
        {
            bool complete = false;
            string sendme = ToString();
            while (!complete)
            {
                try
                {
                    using (Mastership m = Mastership.Request(_controller.Rapid))
                    {

                        _RAPIDData.StringValue = sendme;
                    }
                }
                catch
                {
                    complete = false;
                }
                finally
                {
                    complete = true;
                    string review = _RAPIDData.StringValue;
                }
            }
        }
        public string DisplayInfo
        {
            get { return $"{JobID}, { OrderID }, { DwgID }, { PhaseID }, { PieceID }, { SteelQual }, {PieceQty}, { Profile }, { CodeProfile }, { Length }, { Height }, { FlangeWidth }, { FlangeThick }, { WebThick }, { Radius }, { Weight }, { PaintSurf }, { WebStartCut }, { WebEndCut }, { FlangeStartCut }, { FlangeEndCut }, { TextInfo1 }, { TextInfo2 }, { TextInfo3 }, { TextInfo4 }, {FeatureQuant} "; }


        }
        public override string ToString()
        {
            string output = "[\"" + JobID + "\",";
            output += "\"" + OrderID + "\",";
            output += "\"" + DwgID + "\",";
            output += "\"" + PhaseID + "\",";
            output += "\"" + PieceID + "\",";
            output += "\"" + SteelQual + "\",";
            output += "" + PieceQty.ToString() + ",";
            output += "\"" + Profile + "\",";
            output += "\"" + CodeProfile + "\",";
            output += "" + Length.ToString() + ",";
            output += "" + SawLength.ToString() + ",";
            output += "" + Height.ToString() + ",";
            output += "" + FlangeWidth.ToString() + ",";
            output += "" + FlangeThick.ToString() + ",";
            output += "" + WebThick.ToString() + ",";
            output += "" + Radius.ToString() + ",";
            output += "" + Weight.ToString() + ",";
            output += "" + PaintSurf.ToString() + ",";
            output += "" + WebStartCut.ToString() + ",";
            output += "" + WebEndCut.ToString() + ",";
            output += "" + FlangeStartCut.ToString() + ",";
            output += "" + FlangeEndCut.ToString() + ",";
            output += "\"" + TextInfo1 + "\",";
            output += "\"" + TextInfo2 + "\",";
            output += "\"" + TextInfo3 + "\",";
            output += "\"" + TextInfo4 + "\",";
            return output += "" + FeatureQuant.ToString() + "]";
        }
    }
}
