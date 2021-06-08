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
        public bool recurring { get; set; } = false;
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
        public DefaultJobTemplate() : this("")
        {
        }

        public DefaultJobTemplate(string name)
        {
            Dictionary<string, string> NextAttributes = new Dictionary<string, string>();

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
                Name = "Measure stock length",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 5,
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("InfeedVice", "Clamp Stock");
            NextAttributes.Add("InfeedVice", "Clamp Stock");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Measure stock length",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 5,
                Attributes = NextAttributes // this might need to be cloned
            });


        }
    }
}
