namespace RFRCC_RobotController.ABB_Data
{
    /// <summary>
    /// used to describe the work object that the robot welds, processes, moves within, etc.
    /// </summary>
    public class CS_wobjdata
    {
        /// <summary>
        /// whether or not the robot in the actual program task is holding the work object
        /// </summary>
        public bool robhold { get; set; }
        /// <summary>
        /// Defines whether or not a fixed user coordinate system is used
        /// </summary>
        public bool ufprog { get; set; }
        /// <summary>
        /// The mechanical unit with which the robot movements are coordinated.
        /// Only specified in the case of movable user coordinate systems (ufprog is FALSE).
        /// Specify the mechanical unit name defined in system parameters, e.g. orbit_a
        /// </summary>
        public string ufmec { get; set; }
        /// <summary>
        /// The user coordinate system, i.e. the position of the current work surface or fixture:
        /// </summary>
        public CS_pos uframe { get; set; }
        /// <summary>
        /// The object coordinate system, i.e. the position of the current work object
        /// </summary>
        public CS_pos oframe { get; set; }
        /// <summary>
        /// generate work object with all normalised components
        /// </summary>
        public CS_wobjdata()
        {
            robhold = false;
            ufprog = false;
            ufmec = "";
            uframe = new CS_pos();
            oframe = new CS_pos();
        }
        /// <summary>
        /// generate work object from string formated "[robhold,ufprog,ufmec,ufram,oframe]"
        /// </summary>
        /// <param name="FromString">"[robhold,ufprog,ufmec,ufram,oframe]"</param>
        public CS_wobjdata(string FromString)
        {
            string[] inputArray = FromString.Trim('[', ']').Split(',');
            robhold = inputArray[0].ToLower() == "true" ? true : false;
            ufprog = inputArray[1].ToLower() == "true" ? true : false;
            ufmec = inputArray[2].Trim('"');
            uframe = new CS_pos(inputArray[3]);
            oframe = new CS_pos(inputArray[4]);
        }
        public CS_wobjdata(bool _robhold, bool _ufprog, string _ufmec, CS_pos _uframe, CS_pos _oframe)
        {
            robhold = _robhold;
            ufprog = _ufprog;
            ufmec = _ufmec;
            uframe = _uframe;
            oframe = _oframe;
        }



    }
}
