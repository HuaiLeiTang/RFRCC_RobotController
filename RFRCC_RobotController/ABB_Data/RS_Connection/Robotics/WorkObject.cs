// Microsoft classes
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Stations;
using RobotStudio.API.Core;
using RobotStudio.API.Internal;
using RobotStudio.API.Persistence;
using System;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics
{
    public class WorkObject : DataDeclaration
    {
        public WorkObject()
        {
        }

        public FakeTransform ObjectFrame { get; }
        //
        // Summary:
        //     Gets a ABB.Robotics.RobotStudio.Stations.Transform that corresponds to the user
        //     frame of the work object.
        public FakeTransform UserFrame { get; }
        //
        // Summary:
        //     Gets or sets a value that specifies whether or not the ABB.Robotics.RobotStudio.Stations.WorkObject.UserFrame
        //     is a fixed coordinate system or that it is moveable, i.e. by a coordinated external
        //     axis. True means that the ABB.Robotics.RobotStudio.Stations.WorkObject.UserFrame
        //     is fixed and False that its is moveable.
        public bool UserFrameProgrammed { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the mechanical unit with which the robot movements are
        //     coordinated. Only specified in the case of movable user coordinate systems (ABB.Robotics.RobotStudio.Stations.WorkObject.UserFrameProgrammed
        //     is False).
        public string UserFrameMechanicalUnit { get; set; }
        //
        // Summary:
        //     Gets or sets a values specifying whether or not the robot is holding the work
        //     object: True if the robot is holding the workobject, false otherwise.
        public bool RobotHold { get; set; }
        //
        // Summary:
        //     Gets or sets whether the user and object frames should be displayed in the graphics.
        public bool Visible { get; set; }
        //
        // Summary:
        //     Gets or set the size of the user and object frames in the graphics.
        public double FrameSize { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the object
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets whether the name of the workobject will be displayed in the graphics
        public bool ShowName { get; set; }

        public ProjectObject Parent { get; set; }

        public string DisplayName { get; set; }

        public string ModuleName { get; set; }
        public string InitialExpressionInternal { get; set; }

    }
}