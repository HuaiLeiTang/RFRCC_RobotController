namespace RFRCC_RobotController.Controller.DataModel
{
    public interface ICallForProcess
    {
        /// <summary>
        /// Event calling to processing object for handling, to be connected to a handling object and raised when required
        /// </summary>
        public event CallForProcessEventHandler CallForRobotProcess;

        /// <summary>
        /// Response of Call for Process Method
        /// </summary>
        /// <param name="success"></param>
        public void CallForProcessResponse(bool success, object process = null, object response = null);

    }
}