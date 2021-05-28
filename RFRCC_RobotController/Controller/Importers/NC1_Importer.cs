using System;
using System.Collections.Generic;
using System.Text;

namespace RFRCC_RobotController.Controller.Importers
{
    class NC1_Importer
    {
        // functional properties
        internal bool _parsed = false;

        // file properties
        public string FileASCIIContent;
        public string FileName;
        public string FilePath;

        // Operation Data Loaded

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


    }
}
