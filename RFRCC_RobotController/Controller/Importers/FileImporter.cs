using ABB.Robotics.Math;
using CopingLineGenerators;
using CopingLineImporters.Models;
using CopingLineModels;
using ReplaceRSConnection.Robotics;
using ReplaceRSConnection.Robotics.ToolInfo;
using RFRCC_RobotController.Controller.DataModel;
using RFRCC_RobotController.Controller.DataModel.OperationData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RFRCC_RobotController.Controller.Importers
{

    // TODO: setup importy to import file and load into operation model in order to pass to robot controller
    public class FileImporter
    {
        // functional properties
        internal bool _parsed = false;
        internal bool _JobReady = false;

        // file properties
        public string FileASCIIContent;
        public string FileName;
        public string FilePath;

        private ParserDataModel DataModel = new ParserDataModel();
        public ImporterSettings Settings => DataModel.Settings;
        private ImportModel Importer = new ImportModel();
        private GeneratePresenter PathGenerator = new GeneratePresenter();
        private Matrix4 MatrixBase = new Matrix4(); // not sure what for...
        

        // Operation Data Loaded
        public JobModel Job { get; set; }
        /// <summary>
        /// Status if parsed file
        /// </summary>
        public bool Parsed { get => _parsed; }
        public bool JobReady { get => _JobReady; }

        // TODO: raise events on parse complete and JobReady complete

        public FileImporter()
        {
            PathGenerator.GenerateComplete += PopulateJobData;
        }

        public FileImporter(string filePath, bool parse = false)
        {
            FilePath = filePath;
            FileName = filePath.Split('\\').Last(); // file name
            FileASCIIContent = System.IO.File.ReadAllText(filePath);
            PathGenerator.GenerateComplete += PopulateJobData;

            if (parse)
            {
                // TODO: exception error if cannot parse file content provided
                if (!this.Parse()) throw new NotImplementedException();
            }
        }

        public bool Parse()
        {
            // TODO: Raise Exceptiong if importer filepath not present
            // TODO: complete job 

            ImportEntityCollection filesToImport = new ImportEntityCollection();
            filesToImport.Add(new ImportEntity(FilePath, DataModel.Settings.Import.Qty, DataModel.Settings.Import.Offset, DataModel.Settings.Import.Flip_z, DataModel.Settings.Import.Rot_x));

            int number_of_faces_desired = 4; // default 4
            bool start_aligned_with_face = false; // default false
            double quadrant_angle = 2 * Math.PI / number_of_faces_desired;
            double start_angle;
            if (start_aligned_with_face) start_angle = 0;
            else start_angle = quadrant_angle / 2;


            // this should be better something
            // TODO: condense all parameters into parse settings somewhere
            for (int i = 0; i < number_of_faces_desired; i++)
            {
                double min = start_angle - (i + 1) * quadrant_angle;
                double max = start_angle - i * quadrant_angle;
                if (min < -start_angle) min += 2 * Math.PI;
                if (max <= -start_angle) max += 2 * Math.PI;
                int closest_face = (int)Math.Round((double)i * (8.0 / (double)number_of_faces_desired));
                DataModel.Settings.Import.FaceDefinitions.Add((PlateFace)closest_face, new FaceQuadrant(min, max));
            }

            PathGenerator.View_Generate(this, new GenerateArgs(Importer.Operations, MatrixBase, "GeometryName", DataModel.Settings.Generation));

            if (Importer.ProcessFile(filesToImport, DataModel.Settings.Import))
            {
                Debug.Print("success in import");
                string NumRobotManoeuvres = Importer.Operations.Count.ToString() + " manoeuvres"; //might be used later for something TODO: remove

                return true;
            }
            else
            {
                Debug.Print("import failure");
                return false;
            }
        }

        internal void PopulateJobData(object sender, GenerateCompleteArgs args)
        {
            // Fetch tool data for job
            // TODO: review and refine Tool Data Collection Process and bring in line with requirements and practices of program
            ToolDataCollection tdc = ToolDataCollection.Load();
            Job.ToolData = tdc.GetSelectedToolData();

            List<KeyValuePair<double, PathOperation>> Operations = new List<KeyValuePair<double, PathOperation>>();
            List<RobotComputedFeatures> RobotManouvers = new List<RobotComputedFeatures>();
            List<OperationManoeuvre> manoeuvres = new List<OperationManoeuvre>();
            RobTarget toPoint;
            RobTarget cirPoint;
            int TargetVoltage;
            int Speed;


            foreach (var Feature in args.Manoeuvres)
            {
                // refresh for each feature 
                manoeuvres.Clear();

                if (args.PathOperations.Any(op => Feature.Name.Contains(op.Name)))
                {
                    // compile all move instruction into one list
                    foreach (MoveInstruction moveInstruction in Feature.Instructions.OfType<MoveInstruction>())
                    {
                        toPoint = Feature.Targets.Find(robtarget => robtarget.Name.Contains(moveInstruction.ToPointName)).RobTarget.Copy();
                        cirPoint = moveInstruction.CirPointName != null && moveInstruction.CirPointName != "" ?
                            Feature.Targets.Find(robtarget => robtarget.Name.Contains(moveInstruction.CirPointName)).RobTarget.Copy() :
                            new RobTarget().Copy();
                        Speed = moveInstruction.ToPointName.Contains("Target") ?
                            int.Parse(moveInstruction.InstructionArguments["speed"].Value.Split("_")[0].Split("cv")[1]) :
                            401;
                        if (moveInstruction.ToPointName.Contains("Target"))
                        {
                            CutEntry targetHigh = Job.ToolData.CutCharts.Where(chart => chart.Speed >= Speed * 60).OrderBy(chart => chart.Thickness).First();
                            CutEntry targetLow = Job.ToolData.CutCharts.Where(chart => chart.Speed <= Speed * 60).OrderByDescending(chart => chart.Thickness).First();
                            TargetVoltage = (int)(targetLow.ArcVoltage + (targetHigh.ArcVoltage - targetLow.ArcVoltage) * (0.1 + ((Speed * 60 - targetLow.Speed) / (targetHigh.Speed - targetLow.Speed)))); // lerp with a 10% positive bias
                        }
                        else
                        {
                            TargetVoltage = 131;
                        }
                        manoeuvres.Add(new OperationManoeuvre(moveInstruction, toPoint, cirPoint, TargetVoltage, Speed));
                    }

                    bool cutting = false;
                    OperationManoeuvre previous = manoeuvres.FirstOrDefault();

                    foreach (var item in manoeuvres)
                    {
                        cutting = cutting || previous.Name.Contains("Pierce");
                        item.ManRobT.trans *= 1000;
                        item.ManEndRobT.trans *= 1000;
                        previous.EndCut = item.Name.Contains("Safe") && cutting;
                        cutting = previous.EndCut ? false : cutting;
                        if (item.Name.Contains("Taget") && previous.TargetVoltage < item.TargetVoltage) previous.TargetVoltage = item.TargetVoltage;
                        if (item.Name.Contains("Taget") && previous.ManSpeed.v_tcp > item.ManSpeed.v_tcp) previous.ManSpeed.v_tcp = item.ManSpeed.v_tcp;
                        previous = item;
                    }

                    // add to RobotManouvre 
                    RobotManouvers.Add(new RobotComputedFeatures(new OperationHeader(Feature, manoeuvres), manoeuvres));
                }
            }

            Job.OperationRobotMoveData.AddOperationRange(RobotManouvers);

            // upload header information

            //TODO: Throw a JobData not  loaded exception on failure
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class structure holding all information required for CopingLine Parsing method
        /// </summary>
        private class ParserDataModel
        {
            //public Part BeamGeometry;
            public List<Operation> Operations = new List<Operation>();
            public List<PathOperation> PathOperations = new List<PathOperation>();
            public ProfileSectionType SectionType;
            public Dictionary<double, List<PathOperation>> PathMap = new Dictionary<double, List<PathOperation>>();
            public List<double> BeamPositions = new List<double>();
            //public FakeWorkObject WorkObject;
            public double WorkZoneLocation;
            public double BeamLoadLocation;
            public List<Manoeuvre> Manoeuvres = new List<Manoeuvre>();
            public ImporterSettings Settings = new ImporterSettings();
        }


    }

}
