using System;
using RFRCC_RobotController;
using RFRCC_RobotController.ABB_Data;

namespace Example
{
    class ConnectAndLoadFile
    {
        RobotController Robot = new RobotController();
        System.Windows.Forms.ComboBox ListofControllers;

        /// <summary>
        /// Search Network for controllers and update ComboBox
        /// </summary>
        void SearchForController()
        {
            ControllerCollection AvailalbeControllerList = Robot.stream.AvailableControllers;
            ListboxofControllers.DisplayMember = "SystemName";
            ListboxofControllers.DataSource = AvailalbeControllerList.ToList();
        }

        /// <summary>
        /// Connect to selected controller in ComboBox
        /// </summary>
        void ConnectToController()
        {
            Robot.stream.ConnectToController((NetworkControllerInfo)ListboxofControllers.SelectedItem);
        }

        /// <summary>
        /// Use File dialog to get DSTV (.nc1) file and parse for information
        /// </summary>
        void LoadFile()
        {
            string filePath;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\Users\\samue\\OneDrive\\RoboFab\\Robofab Coping Cell\\Software\\DSTV Files\\";
                openFileDialog.Filter = "DSTV files (*.nc1)|*.nc1";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    Robot.dataModel.CurrentJob.LoadJobFromFile(filePath, true);
                }
            }
        }
        
    }
}