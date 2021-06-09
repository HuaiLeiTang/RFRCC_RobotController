using System;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    // This class will be used as a placehold in a job execution templating method. This will house attributes and expected values in order to structure the population of a 'job'
    /// <summary>
    /// 
    /// </summary>
    public class OperationTemplateAction : OperationAction
    {
        public Type ExpectedOperationType { get; set; }
        public int ExpIndex { get; set; }
        public int ExpIndexModifier { get; set; } = 0;
        public bool Recurring { get; set; } = false;
    }
}
