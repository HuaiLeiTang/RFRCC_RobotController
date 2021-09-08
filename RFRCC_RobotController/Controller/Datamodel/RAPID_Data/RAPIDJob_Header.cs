using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
    /// <summary>
    /// TO BE REMOVED -  this or other in jobdata
    /// </summary>
    public class RAPIDJob_Header
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
        private RapidData _RAPIDData;
        private ABB.Robotics.Controllers.Controller _controller;
        // TODO: is tool / workobject meant to be module and object name?
        /// <summary>
        /// Initialise Object
        /// </summary>
        /// <param name="controller">Connected Network Controller</param>
        /// <param name="task">Controller task</param>
        /// <param name="tool">Tool</param>
        /// <param name="workobject">Workobject</param>
        public RAPIDJob_Header(ABB.Robotics.Controllers.Controller controller, Task task, string tool, string workobject)
        {
            _RAPIDData = task.GetRapidData(tool, workobject);
            _controller = controller;
        }
        /// <summary>
        /// Upload Data to connected Network Controller
        /// </summary>
        public void Upload()
        {
            bool complete = false;
            string sendme = ToString();
            while (!complete) 
            {
                try
                {
                    using (Mastership m = Mastership.Request(_controller))
                    {

                        _RAPIDData.StringValue = sendme;
                        m.Release();
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
        /// <summary>
        /// Display info prepared in displaying
        /// </summary>
        public string DisplayInfo
        {
            get { return $"{JobID}, { OrderID }, { DwgID }, { PhaseID }, { PieceID }, { SteelQual }, {PieceQty}, { Profile }, { CodeProfile }, { Length }, { Height }, { FlangeWidth }, { FlangeThick }, { WebThick }, { Radius }, { Weight }, { PaintSurf }, { WebStartCut }, { WebEndCut }, { FlangeStartCut }, { FlangeEndCut }, { TextInfo1 }, { TextInfo2 }, { TextInfo3 }, { TextInfo4 }, {FeatureQuant} "; }


        }
        /// <summary>
        /// Output string representation of RAPID Data Structure
        /// </summary>
        /// <returns></returns>
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
