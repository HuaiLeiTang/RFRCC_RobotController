using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Template of job Data to be generated, passed to the parsing function in a new job in order to populate job as expected.
    /// </summary>
    public class JobModelTemplate
    {
        /// <summary>
        /// Constructor to generate Job Template with no name
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Constructor to generate Job Template
        /// </summary>
        /// <param name="name">Name of process for ID</param>
        public OperationActionList TemplateListofOperations { get; set; } = new OperationActionList();
        /// <summary>
        /// Generates Job actions from JobModel Data
        /// </summary>
        /// <param name="jobData">Job Data</param>
        public virtual void GenerateOpActionsFromRobManoeuvres(JobModel jobData)
        {
            throw new NotImplementedException();
        }
    }
}
