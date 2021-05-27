using RFRCC_RobotController.ABB_Data.RS_Connection.Robotics;
using RFRCC_RobotController.ABB_Data.RS_Connection.Robotics.ToolInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.ABB_Data.RS_Connection
{
    // to replay RsTask
    public class AppMemory
    {
        DataDeclarationCollection _DataDeclarations = new DataDeclarationCollection();
        public ToolData ActiveTool { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public DataDeclarationCollection DataDeclarations { get => _DataDeclarations; }


        public string GetValidRapidName(string name, string seperator, int increment)
        {
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();
            double output = increment;

            Dictionary<string, DataDeclaration> dataDeclarations = _DataDeclarations.ReturnDictionary();
            List<string> keysList = dataDeclarations.Keys.Where(key => key.Contains(name)).ToList();


            //if (!(keysList.Count > 0)) 
            //    return name + seperator + increment;

            //foreach (var Keystring in keysList)
            //{
            //    if (int.Parse(Keystring.Split(seperator.ToArray(), StringSplitOptions.None)[1]) >= output) 
            //        output = int.Parse(Keystring.Split(seperator.ToArray(), StringSplitOptions.None)[1]) + increment;
            //}

            //int i = 1;
            //while (keysList.Contains(name + seperator + increment*i))
            //{
            //    i++;
            //}

            double step = 1;
            bool sw = false;
            double check = output;
            if (keysList.Contains(name + seperator + check.ToString()))
            {
                while (step >= 1 || step <= -1)
                {
                    // this should double the check step until finding a possible answer, then honing in on the most accurate result, effictively reducing checking time
                    if (keysList.Contains(name + seperator + check.ToString()))
                    {
                        step *= step > 0 ? 1 : -1;
                        check += step;
                        step *= sw ? 0.5 : 2;
                    }
                    else
                    {
                        output = check;
                        sw = true;
                        step *= step > 0 ? 1 : -1;
                        check += step;
                        step *= 0.5;
                    }
                }
            }


            ////Do things
            //watch.Stop();
            //System.Diagnostics.Debug.WriteLine("Process took: "+watch.Elapsed.Milliseconds.ToString()+"ms");
            //times.Add(new Tuple<int, string>(watch.Elapsed.Milliseconds+1, name + seperator + output.ToString()));

            return name + seperator + output.ToString();
        }



    }
}
