using System;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    // TODO: make a class of UI Action :: may include an event execution
    // TODO: Action; skippable, stop_alert

    /// <summary>
    /// Action step during processing
    /// </summary>
    public class OperationAction : ICloneable
    {
        /// <summary>
        /// Key text of the operation action
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Attributes of the Operation sorted by key and string value
        /// </summary>
        public Dictionary<string,string> Attributes { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
