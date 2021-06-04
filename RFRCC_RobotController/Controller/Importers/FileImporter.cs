using ABB.Robotics.Math;
using CopingLineGenerators;
using CopingLineImporters.Models;
using CopingLineModels;
using RFRCC_RobotController.Controller.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFRCC_RobotController.Controller.Importers
{

    // TODO: setup importy to import file and load into operation model in order to pass to robot controller
    class FileImporter
    {
        // functional properties
        internal bool _parsed = false;

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

        public FileImporter()
        {
            
        }

        public FileImporter(string filePath, bool parse = false)
        {
            FilePath = filePath;
            FileName = filePath.Split('\\').Last(); // file name
            FileASCIIContent = System.IO.File.ReadAllText(filePath);

            if (parse)
            {
                // TODO: exception error if cannot parse file content provided
                if (!this.Parse()) throw new NotImplementedException();
            }
        }

        public bool Parse()
        {
            // TODO: Raise Exceptiong if importer filepath not present
            // TODO: 

            ImportEntityCollection filesToImport = new ImportEntityCollection();
            filesToImport.Add(new ImportEntity(FilePath, DataModel.Settings.Import.Qty, DataModel.Settings.Import.Offset, DataModel.Settings.Import.Flip_z, DataModel.Settings.Import.Rot_x));

            int number_of_faces_desired = 4; // default 4
            bool start_aligned_with_face = false; // default false
            double quadrant_angle = 2 * Math.PI / number_of_faces_desired;
            double start_angle;
            if (start_aligned_with_face) start_angle = 0;
            else start_angle = quadrant_angle / 2;

            for (int i = 0; i < number_of_faces_desired; i++)
            {
                double min = start_angle - (i + 1) * quadrant_angle;
                double max = start_angle - i * quadrant_angle;
                if (min < -start_angle) min += 2 * Math.PI;
                if (max <= -start_angle) max += 2 * Math.PI;
                int closest_face = (int)Math.Round((double)i * (8.0 / (double)number_of_faces_desired));
                DataModel.Settings.Import.FaceDefinitions.Add((PlateFace)closest_face, new FaceQuadrant(min, max));
            }

            if (Importer.ProcessFile(filesToImport, DataModel.Settings.Import))
            {
                Debug.Print("success in import");
                textBox3.Text = Importer.Operations.Count.ToString() + " manoeuvres";
            }
            else
            {
                Debug.Print("import failure");
            }


            // TODO: implement NC1_Importer.Parse
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
