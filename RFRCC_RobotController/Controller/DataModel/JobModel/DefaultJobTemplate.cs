using RFRCC_RobotController.Controller.DataModel.OperationData;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
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

        public override void GenerateOpActionsFromRobManoeuvres(JobModel jobData)
        {
            OperationActionList operationActions = new OperationActionList();
            Dictionary<string, string> NextAttributes = new Dictionary<string, string>();

            NextAttributes.Clear();
            NextAttributes.Add("Description", "save time, date and job identification information to log for reference of machine activity");
            operationActions.Add(new OperationAction()
            {
                Name = "Start Job",
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Wait for Operator to place required stock on infeed conveyor and confirm ready to proceed");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Load Material onto Infeed",
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Drive stock up to X datum of cell entry to measure length of stock");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Drive Stock onto Conveyor to X Datum",
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Use displacement laser to measure end of stock");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable. If stock is not long enough to complete job, job will be ejected with stock not suitable error logged");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Measure stock length",
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Move stock into cell position to check dimensions");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Drive stock into cell for checking",
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Infeed clamp will close on stock, and Encoder to zero");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Close Infeed Clamp",
                Attributes = NextAttributes // this might need to be cloned
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Robot will probe stock to ensure that stock is correct dimensions. If Stock dimensions are unacceptable, stock will be rejected, job ended, and stock not suitable error logged");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationRobotProcess()
            {
                Name = "Measure stock dimensions",
                Attributes = NextAttributes // this might need to be cloned
                //TODO: add robot execution process
            });

            foreach (RobotComputedFeatures Feature in jobData.OperationRobotMoveData.Operation)
            {
                NextAttributes.Clear();
                NextAttributes.Add("Description", "Stock to move to required location");
                NextAttributes.Add("Note", "Robot will update required position of stock");
                operationActions.Add(new OperationPLCProcess()
                {
                    Name = "Drive Stock to next position",
                    Attributes = NextAttributes // this might need to be cloned
                });

                NextAttributes.Clear();
                NextAttributes.Add("Description", "Robot to cut feature");
                operationActions.Add(new OperationRobotManoeuvre()
                {
                    Name = "Cut Feature",
                    Attributes = NextAttributes,    // this might need to be cloned
                    featureData = Feature           // this needs to be a pointer 
                                                
                });
            }

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Move finished piece onto outfeed for pickup");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Eject completed job",
                Attributes = NextAttributes // this might need to be cloned
                //TODO: add robot execution process
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "save time, date and job identification information to log for reference of machine activity");
            operationActions.Add(new OperationAction()
            {
                Name = "End of job",
                Attributes = NextAttributes // this might need to be cloned
                //TODO: add robot execution process
            });
        }
    }
}
