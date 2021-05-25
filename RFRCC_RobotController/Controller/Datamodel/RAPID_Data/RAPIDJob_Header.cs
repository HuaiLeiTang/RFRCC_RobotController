using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{

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
        private ABB.Robotics.Controllers.Controller _controller;

        public RAPIDJob_Header(ABB.Robotics.Controllers.Controller controller, Task task, string tool, string workobject)
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
