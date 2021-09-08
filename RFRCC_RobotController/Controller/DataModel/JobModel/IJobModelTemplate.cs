using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Template of job Data to be generated, passed to the parsing function in a new job in order to populate job as expected.
    /// </summary>
    public interface IJobModelTemplate
    {
        /// <summary>
        /// Constructor to generate Job Template with no name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Constructor to generate Job Template
        /// </summary>
        public OperationActionList TemplateListofOperations { get; set; }
        /// <summary>
        /// Generates Job actions from JobModel Data
        /// </summary>
        /// <param name="jobData">Job Data</param>
        public void GenerateOpActionsFromRobManoeuvres(JobModel jobData);
        public void AbortJob(JobModel jobData);
    }
}
