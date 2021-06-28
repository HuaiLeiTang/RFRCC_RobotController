using ReplaceRSConnection.Robotics;
using System;

namespace RFRCC_RobotController.ABB_Data
{
    /// <summary>
    /// used to define the position of the robot axes and additional axes.
    /// </summary>
    public class CS_RobTarget
    {
        /// <summary>
        /// Position Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Translation of TCP location
        /// </summary>
        public CS_pos trans { get; set; }
        /// <summary>
        /// Rotation of TCP
        /// </summary>
        public CS_orient rot { get; set; }
        /// <summary>
        /// Configuration of robot at location
        /// </summary>
        public CS_confdata robconf { get; set; }
        /// <summary>
        /// Configuration of external axes at TCP location
        /// </summary>
        public CS_extjoint extax { get; set; }
        /// <summary>
        /// Robtarget nomalised all variables normalised at 0
        /// </summary>
        public CS_RobTarget()
        {
            trans = new CS_pos();
            rot = new CS_orient();
            robconf = new CS_confdata();
            extax = new CS_extjoint();
        }
        /// <summary>
        /// Robtarget nomalised all variables normalised at 0
        /// External axes connected set by bool array
        /// Bool Array may be of any size, connecteding and normalising all indicated axes from eax_a for each array index recieved
        /// </summary>
        /// <param name="ExtAx"></param>
        public CS_RobTarget(bool[] ExtAx)
        {
            trans = new CS_pos();
            rot = new CS_orient();
            robconf = new CS_confdata();
            extax = new CS_extjoint(ExtAx);
        }
        /// <summary>
        /// Set robtarget from string with following format
        /// "[[X,Y,Z],[q1,q2,q3,q4],[cf1,cf4,cf6,cfx],[eax_a,eax_b,eax_c,eax_d,eax_e,eax_f]]"
        /// </summary>
        /// <param name="input">"[[X,Y,Z],[q1,q2,q3,q4],[cf1,cf4,cf6,cfx],[eax_a,eax_b,eax_c,eax_d,eax_e,eax_f]]"</param>
        public CS_RobTarget(string input)
        {
            string[] inputArray = input.Trim('[', ']').Split(new string[] { "],[" }, StringSplitOptions.None);
            trans = new CS_pos(inputArray[0]);
            rot = new CS_orient(inputArray[1]);
            robconf = new CS_confdata(inputArray[2]);
            extax = new CS_extjoint(inputArray[3]);
        }
        /// <summary>
        /// Robtraget set from DataNode Array
        /// </summary>
        /// <param name="input">DataNode Array of parameters pos, orient, confdata, extjoint</param>
        public CS_RobTarget(ABB.Robotics.Controllers.RapidDomain.DataNode[] input)
        {
            trans = new CS_pos(Single.Parse(input[0].Children[0].Value), Single.Parse(input[0].Children[1].Value), Single.Parse(input[0].Children[2].Value));
            rot = new CS_orient(Single.Parse(input[1].Children[0].Value), Single.Parse(input[1].Children[1].Value), Single.Parse(input[1].Children[2].Value), Single.Parse(input[1].Children[3].Value));
            robconf = new CS_confdata(Single.Parse(input[2].Children[0].Value), Single.Parse(input[2].Children[1].Value), Single.Parse(input[2].Children[2].Value), Single.Parse(input[2].Children[3].Value));
            extax = new CS_extjoint(input[3].Children[0].Value, input[3].Children[1].Value, input[3].Children[2].Value, input[3].Children[3].Value, input[3].Children[4].Value, input[3].Children[5].Value);
        }
        /// <summary>
        /// Robtarget cloned from ABB RAPID Robtarget
        /// </summary>
        /// <param name="robTarget">ABB RAPID Robtarget</param>
        public CS_RobTarget(RobTarget robTarget)
        {
            if (robTarget != null)
            {
                Name = robTarget.Name;
                trans = new CS_pos(robTarget.Frame.X, robTarget.Frame.Y, robTarget.Frame.Z);
                rot = new CS_orient(robTarget.Frame.GlobalMatrix.Quaternion.q1, robTarget.Frame.GlobalMatrix.Quaternion.q2, robTarget.Frame.GlobalMatrix.Quaternion.q3, robTarget.Frame.GlobalMatrix.Quaternion.q4);
                robconf = new CS_confdata(robTarget.ConfigurationData.Cf1, robTarget.ConfigurationData.Cf4, robTarget.ConfigurationData.Cf6, robTarget.ConfigurationData.Cfx);
                extax = new CS_extjoint(robTarget.externalAxis.Eax_a.ToString(), robTarget.externalAxis.Eax_b.ToString(), robTarget.externalAxis.Eax_c.ToString(), robTarget.externalAxis.Eax_d.ToString(), robTarget.externalAxis.Eax_e.ToString(), robTarget.externalAxis.Eax_f.ToString());
            }
            else
            {
                trans = new CS_pos();
                rot = new CS_orient();
                robconf = new CS_confdata();
                extax = new CS_extjoint();
            }
        }
        /// <summary>
        /// out string formated "[[X,Y,Z],[q1,q2,q3,q4],[cf1,cf4,cf6,cfx],[eax_a,eax_b,eax_c,eax_d,eax_e,eax_f]]"
        /// </summary>
        /// <returns>"[[X,Y,Z],[q1,q2,q3,q4],[cf1,cf4,cf6,cfx],[eax_a,eax_b,eax_c,eax_d,eax_e,eax_f]]"</returns>
        public override string ToString()
        {
            return ("[" + trans.ToString() + "," + rot.ToString() + "," + robconf.ToString() + "," + extax.ToString() + "]");
        }
        /// <summary>
        /// Robtarget populated from string formatted "[[X,Y,Z],[q1,q2,q3,q4],[cf1,cf4,cf6,cfx],[eax_a,eax_b,eax_c,eax_d,eax_e,eax_f]]"
        /// </summary>
        /// <param name="Input">"[[X,Y,Z],[q1,q2,q3,q4],[cf1,cf4,cf6,cfx],[eax_a,eax_b,eax_c,eax_d,eax_e,eax_f]]"</param>
        public void FromString(string Input)
        {
            string[] InputArray = Input.Trim('[', ']').Split(',');
            trans.FromString(string.Join(",", InputArray[0..3]));
            rot.FromString(string.Join(",", InputArray[3..7]));
            robconf.FromString(string.Join(",", InputArray[7..11]));
            extax.FromString(string.Join(",", InputArray[11..17]));
        }

    }
}
