using System;
using System.Collections.Generic;
using System.Linq;

namespace RFRCC_RobotController.Controller.DataModel
{
    // This class will be used as a placehold in a job execution templating method. This will house attributes and expected values in order to structure the population of a 'job'
    public class OperationTemplateAction : OperationAction
    {
        public Type ExpectedOperationType { get; set; } //maybe enum? instead
        public int ExpIndex { get; set; }
        public int ExpIndexModifier { get; set; } = 0;
        public bool Recurring { get; set; } = false;
    }

    public class JobModelTemplate
    {
        public string Name { get; set; } = "";
        public OperationActionList TemplateListofOperations { get; set; }
    }

    /// <summary>
    /// Default Job Template without Infeed loading mechanism
    /// </summary>
    public class DefaultJobTemplate : JobModelTemplate
    {
        /// <summary>
        /// Constructor to generate Job Template with no name
        /// </summary>
        public DefaultJobTemplate() : this("")
        {
        }

        /// <summary>
        /// Constructor to generate Job Template
        /// </summary>
        /// <param name="name">Name of process for ID</param>
        public DefaultJobTemplate(string name)
        {
            Dictionary<string, string> NextAttributes = new Dictionary<string, string>();

            /* 
             * 1. start job - capture time and date
             * 2. PLC - Load Material
             * 3. PLC - Drive stock into cell
             * 4. PLC - measure stock lenght
             * 5. PLC - drive stock for probe
             * 6. PLC - Close Vice & zero
             * 7. Robot - probe stock for sizes
             * (R) 8. PLC - drive stock to required delta-x
             * (R) 8+1. Robot - Make Cut
             * 9. PLC - Drive stock out of cell
             */

            Name = name;
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Start Job",
                ExpectedOperationType = typeof(OperationAction),
                ExpIndex = 1

            });

            NextAttributes.Clear();
            NextAttributes.Add("MaterialName", "NULL");
            NextAttributes.Add("MinimumStockLength", "NULL");
            NextAttributes.Add("Condition", "Wait for Operator OK");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Load Material onto Infeed",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 2,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Infeed", "Drive until X Datum laser is cut");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Drive Stock onto Conveyor to X Datum",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 3,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("MeasureLaser", "Read Stock length");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Measure stock length",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 4,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Condition", "IF MinimumStockLength OK");
            NextAttributes.Add("Infeed", "Drive until Stock X @ ROBOTUPDATE:STOCK_X");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Drive stock into cell for checking",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 5,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("InfeedVice", "Clamp Stock");
            NextAttributes.Add("InfeedViceEncoder", "Zero");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Close Infeed Clamp",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 6,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Measure stock dimensions",
                ExpectedOperationType = typeof(OperationRobotProcess),
                ExpIndex = 7
            });

            NextAttributes.Clear();
            NextAttributes.Add("Condition", "IF stock DX < NextDX");
            NextAttributes.Add("Infeed", "Drive until Stock DX @ NextDX");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Drive Stock to next position",
                ExpectedOperationType = typeof(OperationPLCProcess),
                Recurring = true,
                ExpIndex = 8,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Cut Feature",
                ExpectedOperationType = typeof(OperationRobotManoeuvre),
                Recurring = true,
                ExpIndex = 8,
                ExpIndexModifier = 1
            });
        }
    }
}
