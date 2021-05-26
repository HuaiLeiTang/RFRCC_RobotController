using ABB.Robotics.Controllers.RapidDomain;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using RapidString = ABB.Robotics.Controllers.RapidDomain.String;

namespace RFRCC_RobotController.Controller.DataModel.RAPID_Data
{
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
