using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics.ToolInfo
{

    [CollectionDataContract(ItemName = "ToolDataDictionary", KeyName = "ToolName", ValueName = "ToolData")]
    public class ToolDataDictionary : Dictionary<string, ToolData> { }

    public enum ToolMode {Cutting, Marking };

    public class ToolDataCollection
    {
        public string LastSelectedTool;
        public ToolMode LastSelectedMode;
        public DateTime LastSaved;
        public int eon;
        public string Path;
        public ToolDataDictionary Tools = new ToolDataDictionary();

        public ToolData GetSelectedToolData()
        {
            if (LastSelectedTool == null) return new ToolData();
            return Tools[LastSelectedTool];
        }

        public ToolMode GetLastSelectedToolMode()
        {
            return LastSelectedMode;
        }

        #region Serialisation Routines
        public static ToolDataCollection Load()
        {
            string tool_data_key = "CopingLinePlugin_ToolDataSet";
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + tool_data_key + ".xml";

            ToolDataCollection pdb;
            try
            {
                DataContractSerializer s = new DataContractSerializer(typeof(ToolDataCollection));
                using (FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    pdb = (ToolDataCollection)s.ReadObject(reader);
                }
            }
            catch
            {
                pdb = new ToolDataCollection();
            }
            pdb.Path = path;

            return pdb;
        }

        public void Save()
        {
            LastSaved = System.DateTime.Now;
            eon++;
            var s = new DataContractSerializer(typeof(ToolDataCollection));
            var settings = new XmlWriterSettings { Indent = true };
            using (var w = XmlWriter.Create(Path, settings))
                s.WriteObject(w, this);

        }
        #endregion

    }
}
