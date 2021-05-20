using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Runtime.Serialization;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics.ToolInfo
{

    public enum GasType { O2, N2, AR, AIR, Unknown };

    public class Entry
    {
        public double ArcVoltage { get; set; }
        public double CutHeight { get; set; }

        public double PlasmaGasPreflow { get; set; }
        public double PlasmaGasCutflow { get; set; }
        public double ShieldGasPreflow { get; set; }
        public double ShieldGasCutflow { get; set; }

        public double PierceHeight { get; set; }
        public double PierceDelay { get; set; }

        public GasType CutGas { get; set; }
        public GasType ShieldGas { get; set; }
        public double Speed { get; set; }

        [IgnoreDataMember]
        [Browsable(false)]
        public virtual ToolData ToolData { get; set; }
    }

    public class CutEntry : Entry
    {
        public double Thickness { get; set; }
        public double KerfWidth { get; set; }
        public double MinimumClearance { get; set; }

        public CutEntry ShallowCopy()
        {
            CutEntry clone = (CutEntry)this.MemberwiseClone();
            clone.ToolData = this.ToolData;
            return clone;
        }
    }

    public class MarkEntry : Entry
    {
        public int CurrentOverride { get; set; }
    }
    
}
