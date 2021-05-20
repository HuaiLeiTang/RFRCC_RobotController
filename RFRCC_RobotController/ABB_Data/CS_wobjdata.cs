namespace RFRCC_RobotController.ABB_Data
{
    public class CS_wobjdata
    {
        public bool robhold { get; set; }
        public bool ufprog { get; set; }
        public string ufmec { get; set; }
        public CS_pos uframe { get; set; }
        public CS_pos oframe { get; set; }

        public CS_wobjdata()
        {
            robhold = false;
            ufprog = false;
            ufmec = "";
            uframe = new CS_pos();
            oframe = new CS_pos();
        }
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
