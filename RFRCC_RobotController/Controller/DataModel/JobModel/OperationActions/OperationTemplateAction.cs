using System;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    // This class will be used as a placehold in a job execution templating method. This will house attributes and expected values in order to structure the population of a 'job'
    /// <summary>
    /// Template of Action to be taken during job processing
    /// A placeholder for actions to be populated upon "GenerateOpActionsFromRobManoeuvres()"
    /// </summary>
    public class OperationTemplateAction : OperationAction
    {
        /// <summary>
        /// Type of action to occupy this placeholder after generation
        /// </summary>
        public Type ExpectedOperationType { get; set; }
        /// <summary>
        /// Expected Index of generated action to occupy after generation
        /// </summary>
        public int ExpIndex { get; set; }
        /// <summary>
        /// Modifier to Expected Index used during generation
        /// </summary>
        public int ExpIndexModifier { get; set; } = 0;
        /// <summary>
        /// If this step is a recurring step
        /// </summary>
        public bool Recurring { get; set; } = false;
    }
}
