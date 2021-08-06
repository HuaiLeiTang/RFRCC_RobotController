using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Action step involving PLC process
    /// </summary>
    public class OperationPLCProcess : OperationAction
    {
        internal void UpdateStockDXPrecisionsTolerance(object MinMax, EventArgs args)
        {
            if (MinMax is Tuple<double, double>)
            {
                if (Attributes.ContainsKey("StockDXPrecisionTolerance_Positive") && Attributes.ContainsKey("StockDXPrecisionTolerance_Negative"))
                {
                    Attributes["StockDXPrecisionTolerance_Negative"] = ((Tuple<double, double>)MinMax).Item1.ToString();
                    Attributes["StockDXPrecisionTolerance_Positive"] = ((Tuple<double, double>)MinMax).Item2.ToString();
                }
                else
                {
                    // TODO: handle if receiving does not have StockDXPrecisionTolerance_Negative or StockDXPrecisionTolerance_Positive
                }
            }
            else
            {
                // TODO: handle if sender is not the values required for Min and Max
            }
        }
    }
}
