using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization;
using System.IO;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics.ToolInfo
{
    public enum MaterialType { MildSteel, StainlessSteel, Aluminium };

    public class ToolData
    {
        public string ImageResourceName { get; set; }
        public bool Bevel { get; set; }
        public MaterialType Material { get; set; }
        public int Current { get; set; }
        public BindingList<CutEntry> CutCharts = new BindingList<CutEntry>();
        public BindingList<MarkEntry> MarkingCharts = new BindingList<MarkEntry>();
        //[IgnoreDataMember]
        public string Name { get; set; }

        public ToolData()
        {
            //Name = GenerateName();
            //ImageResourceName = Name;
        }

        static public double linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        static public double calc_pc(double x, double x0, double x1)
        {
            if ((x1 - x0) == 0)
            {
                return 0.5;
            }
            return (x - x0) / (x1 - x0);
        }

        static public double linear_pc(double pc, double y0, double y1)
        {
            return y0 + pc * (y1 - y0);
        }

        static public double lagrange(double x, double[] xd, double[] yd)
        {
            if (xd.Length != yd.Length)
            {
                throw new ArgumentException("Arrays must be of equal length."); //$NON-NLS-1$
            }
            double sum = 0;
            for (int i = 0, n = xd.Length; i < n; i++)
            {
                if (x - xd[i] == 0)
                {
                    return yd[i];
                }
                double product = yd[i];
                for (int j = 0; j < n; j++)
                {
                    if ((i == j) || (xd[i] - xd[j] == 0))
                    {
                        continue;
                    }
                    product *= (x - xd[i]) / (xd[i] - xd[j]);
                }
                sum += product;
            }
            return sum;
        }

        public MarkEntry GetMarkingDataFromGasType(GasType type)
        {
            List<MarkEntry> matches = MarkingCharts.Where(q => q.CutGas == type).ToList();

            if (matches.Count == 0) return null;
            else return matches.First();
            
        }

        public CutEntry GetCutDataFromThickness(double thickness)
        {

            if (CutCharts.Count < 2) return null;

            // more than likely our thicknesses from the cad model won't fit our cut charts.
            // so we will perform linear interpolation between the points either side of the thickness to approximate.
            CutEntry low, high;
            var cutdatalistlow = CutCharts.Where(q => q.Thickness <= thickness).OrderByDescending(t=>t.Thickness);
            var cutdatalisthigh = CutCharts.Where(q => q.Thickness >= thickness).OrderBy(t => t.Thickness);

            if (cutdatalistlow.Count() == 0) return cutdatalisthigh.First();
            if (cutdatalisthigh.Count() == 0) return cutdatalistlow.First();
            
            low = cutdatalistlow.First();
            high = cutdatalisthigh.First();
            // copy from low initially.
            CutEntry cuttingdata = low.ShallowCopy();

            double pc = calc_pc(thickness, low.Thickness, high.Thickness);

            // go through each property and interpolate as required.
            cuttingdata.ArcVoltage = linear_pc(pc, low.ArcVoltage, high.ArcVoltage);
            cuttingdata.CutHeight = linear_pc(pc, low.CutHeight, high.CutHeight);
            cuttingdata.KerfWidth = linear_pc(pc, low.KerfWidth, high.KerfWidth);
            cuttingdata.MinimumClearance = linear_pc(pc, low.MinimumClearance, high.MinimumClearance);
            cuttingdata.PierceDelay = linear_pc(pc, low.PierceDelay, high.PierceDelay);
            cuttingdata.PierceHeight = linear_pc(pc, low.PierceHeight, high.PierceHeight);
            cuttingdata.PlasmaGasCutflow = linear_pc(pc, low.PlasmaGasCutflow, high.PlasmaGasCutflow);
            cuttingdata.PlasmaGasPreflow = linear_pc(pc, low.PlasmaGasPreflow, high.PlasmaGasPreflow);
            cuttingdata.ShieldGasCutflow = linear_pc(pc, low.ShieldGasCutflow, high.ShieldGasCutflow);
            cuttingdata.ShieldGasPreflow = linear_pc(pc, low.ShieldGasPreflow, high.ShieldGasPreflow);
            cuttingdata.Speed = linear_pc(pc, low.Speed, high.Speed);

            return cuttingdata;
        }

        public string GenerateName()
        {
            string name = "";
            switch (Material)
            {
                case MaterialType.MildSteel:
                    name += "MS";
                    break;
                case MaterialType.StainlessSteel:
                    name += "SS";
                    break;
                case MaterialType.Aluminium:
                    name += "AL";
                    break;
            }
            name += "_";
            if (Bevel) name += "B";
            else name += "NB";
            name += "_" + Current.ToString();

            return name;
        }

        /*private static List<string> GetAvailableResourceNames()
        {
            List<string> resource_names = new List<string>();
            ResourceManager rm = Properties.Resources.ResourceManager;
            ResourceSet set = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            resource_names.Clear();
            foreach (DictionaryEntry o in set)
            {
                resource_names.Add((string)o.Key);
            }
            return resource_names;
        }*/

        /*public Bitmap GetImage()
        {
            string imageName = Name;//GenerateName();
            List<string> resource_names = GetAvailableResourceNames();

            if(resource_names.Contains(imageName))
            {
                ResourceManager rm = Properties.Resources.ResourceManager;
                return (Bitmap)rm.GetObject(imageName);
            }
            
            return Properties.Resources.NoImageFound;
        }*/

    }
}
