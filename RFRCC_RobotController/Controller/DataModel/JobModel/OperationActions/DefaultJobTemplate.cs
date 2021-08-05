using RFRCC_RobotController.Controller.DataModel.OperationData;
using System.Collections.Generic;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Default Job process Template without Infeed loading mechanism
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
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Infeed", "Drive until X Datum laser is cut");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Drive Stock onto Conveyor to X Datum",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 3,
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("MeasureLaser", "Read Stock length");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Measure stock length",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 4,
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Condition", "IF MinimumStockLength OK");
            NextAttributes.Add("Infeed", "Drive until Stock X @ ROBOTUPDATE:STOCK_X");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Drive stock into cell for checking",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 5,
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("InfeedVice", "Clamp Stock");
            NextAttributes.Add("InfeedViceEncoder", "Zero");
            TemplateListofOperations.Add(new OperationTemplateAction()
            {
                Name = "Close Infeed Clamp",
                ExpectedOperationType = typeof(OperationPLCProcess),
                ExpIndex = 6,
                Attributes = new Dictionary<string, string>(NextAttributes)
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
                Attributes = new Dictionary<string, string>(NextAttributes)
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
        /// <summary>
        /// Generates Job actions from JobModel Data
        /// </summary>
        /// <param name="jobData">Job Data</param>
        public override void GenerateOpActionsFromRobManoeuvres(JobModel jobData)
        {
            OperationActionList operationActions = jobData.operationActions;
            Dictionary<string, string> NextAttributes = new Dictionary<string, string>();

            NextAttributes.Clear();
            NextAttributes.Add("Description", "save time, date and job identification information to log for reference of machine activity");
            operationActions.Add(new OperationAction()
            {
                Name = "Start Job",
                Attributes = new Dictionary<string, string>(NextAttributes)
            }) ;

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Wait for Operator to place required stock on infeed conveyor and confirm ready to proceed");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Load Material onto Infeed",
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Drive stock up to X datum of cell entry to measure length of stock");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Drive Stock onto Conveyor to X Datum",
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Use displacement laser to measure end of stock");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable. If stock is not long enough to complete job, job will be ejected with stock not suitable error logged");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Measure stock length",
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Move stock into cell position to check dimensions");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            NextAttributes.Add("RequiredStockDX", (-jobData.HeaderInfo.SawLength + 1100).ToString());
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Drive stock into cell for checking",
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Infeed clamp will close on stock, and Encoder to zero");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Close Infeed Clamp",
                Attributes = new Dictionary<string, string>(NextAttributes)
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Robot will probe stock to ensure that stock is correct dimensions. If Stock dimensions are unacceptable, stock will be rejected, job ended, and stock not suitable error logged");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            NextAttributes.Add("RobotProcess", "VerifyStockDimensions");
            NextAttributes.Add("RequiredStockDX", (-jobData.HeaderInfo.SawLength + 1300).ToString());
            operationActions.Add(new OperationRobotProcess()
            {
                Name = "Measure stock dimensions",
                Attributes = new Dictionary<string, string>(NextAttributes)
                //TODO: add robot execution process
            });

            foreach (RobotComputedFeatures Feature in jobData.OperationRobotMoveData.Operation)
            {
                NextAttributes.Clear();
                NextAttributes.Add("Description", "Stock to move to required location");
                NextAttributes.Add("Note", "Robot will update required position of stock");
                NextAttributes.Add("RequiredStockDX", (-jobData.HeaderInfo.SawLength + 1300).ToString());
                operationActions.Add(new OperationPLCProcess()
                {
                    Name = "Drive Stock to next position",
                    Attributes = new Dictionary<string, string>(NextAttributes)
                });

                NextAttributes.Clear();
                NextAttributes.Add("Description", "Robot to cut feature");
                NextAttributes.Add("RequiredStockDX", (-jobData.HeaderInfo.SawLength + 1300).ToString());
                operationActions.Add(new OperationRobotManoeuvre(Feature)
                {
                    Name = "Cut Feature",
                    Attributes = new Dictionary<string, string>(NextAttributes)
                                                
                });
            }

            NextAttributes.Clear();
            NextAttributes.Add("Description", "Move finished piece onto outfeed for pickup");
            NextAttributes.Add("Note", "This process should be overridable, or operator contributable");
            operationActions.Add(new OperationPLCProcess()
            {
                Name = "Eject completed job",
                Attributes = new Dictionary<string, string>(NextAttributes)
                //TODO: add robot execution process
            });

            NextAttributes.Clear();
            NextAttributes.Add("Description", "save time, date and job identification information to log for reference of machine activity");
            operationActions.Add(new OperationAction()
            {
                Name = "End of job",
                Attributes = new Dictionary<string, string>(NextAttributes)
                //TODO: add robot execution process
            });
        }
    }
}
