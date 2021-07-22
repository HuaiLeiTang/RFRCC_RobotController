namespace RFRCC_RobotController.Controller.DataModel
{
    public interface ICallForProcess
    {
        /// <summary>
        /// Response of Call for Process Method
        /// </summary>
        /// <param name="success"></param>
        public void CallForProcessResponse(bool success, object response = null);

    }
}