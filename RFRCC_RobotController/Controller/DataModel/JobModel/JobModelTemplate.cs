namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Template of job Data to be generated, passed to the parsing function in a new job in order to populate job as expected.
    /// </summary>
    public class JobModelTemplate
    {
        public string Name { get; set; } = "";
        public OperationActionList TemplateListofOperations { get; set; }
    }
}
