namespace RFRCC_RobotController.Controller
{
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
}
