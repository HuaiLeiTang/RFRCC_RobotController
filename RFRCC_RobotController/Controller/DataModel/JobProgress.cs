namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// indicator of current job status
    /// </summary>
    public enum JobProgress
    {
        WaitingForFile = 0,
        WaitingToParse = 1,
        WaitingToPopulateJobData = 2,
        WaitingForRobotConnection = 3,
        WaitingForJobStart = 4,
        JobProcessing = 5,
        JobPaused = 6,
        JobAborting = 7,
        JobCancelled = 8,
        JobFinished = 9,
    }
}
