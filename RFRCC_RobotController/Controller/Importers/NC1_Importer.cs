using ABB.Robotics.Math;
using CopingLineGenerators;
using CopingLineImporters.Models;
using CopingLineModels;
using RFRCC_RobotController.Controller.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RFRCC_RobotController.Controller.Importers
{

    // TODO: setup importy to import file and load into operation model in order to pass to robot controller
    class NC1_Importer
    {
        // functional properties
        internal bool _parsed = false;

        // file properties
        public string FileASCIIContent;
        public string FileName;
        public string FilePath;
        public ImporterSettings Settings => DataModel.Settings;
        private ImportModel Importer = new ImportModel();
        private ParserDataModel DataModel = new ParserDataModel();
        private GeneratePresenter PathGenerator = new GeneratePresenter();
        private Matrix4 MatrixBase = new Matrix4();
        

        // Operation Data Loaded
        public OperationModel operationModel { get; set; }

        public NC1_Importer()
        {

        }

        public NC1_Importer(string filePath, string fileName, string fileASCIIContent, bool parse = false)
        {
            FileASCIIContent = fileASCIIContent;
            FilePath = filePath;
            FileName = fileName;

            if (parse)
            {
                // TODO: exception error if cannot parse file content provided
                if (!this.Parse()) throw new NotImplementedException();
            }
        }

        public bool Parse()
        {
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
