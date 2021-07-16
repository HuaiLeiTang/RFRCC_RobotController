using ABB.Robotics.Controllers;
using System;
using System.IO;
using System.Net;

namespace RFRCC_RobotController.ABB_Data
{
    /// <summary>
    /// Controller information of online machine
    /// </summary>
    public class NetworkControllerInfo
    {
        /// <summary>
        /// Construct object from ABB ControllerInfo class
        /// </summary>
        /// <param name="ABBControllerInfo">ABB ControllerInfo class to be populated from</param>
        public NetworkControllerInfo(ControllerInfo ABBControllerInfo)
        {
            SystemId = ABBControllerInfo.SystemId;
            IsVirtual = ABBControllerInfo.IsVirtual;
            IPAddress = ABBControllerInfo.IPAddress;
            VersionName = ABBControllerInfo.VersionName;
            Version = ABBControllerInfo.Version;
            BaseDirectory = ABBControllerInfo.BaseDirectory;
            HostName = ABBControllerInfo.HostName;
            NetscanId = ABBControllerInfo.NetscanId;
            MacAddress = ABBControllerInfo.MacAddress;
            SystemName = ABBControllerInfo.SystemName;
            RobApiPort = ABBControllerInfo.RobApiPort;
            RunLevel = ABBControllerInfo.RunLevel;
            Id = ABBControllerInfo.Id;
            ControllerName = ABBControllerInfo.ControllerName;
            Availability = ABBControllerInfo.Availability;
            WebServicesPort = ABBControllerInfo.WebServicesPort;
            _ABBControllerInfo = ABBControllerInfo;
        }
        public NetworkControllerInfo(ABB.Robotics.Controllers.Controller ABBController)
        {
            SystemId = ABBController.SystemId;
            IsVirtual = ABBController.IsVirtual;
            IPAddress = ABBController.IPAddress;
            VersionName = ABBController.RobotWare.Name;
            Version = ABBController.RobotWareVersion;
            BaseDirectory = new DirectoryInfo(ABBController.FileSystem.LocalDirectory);
            HostName = ABBController.Name;
            MacAddress = ABBController.MacAddress;
            SystemName = ABBController.SystemName;
            RunLevel = ABBController.RunLevel;
            Id = ABBController.Name;
            ControllerName = ABBController.Name;
        }

        internal ControllerInfo _ABBControllerInfo;

        /// <summary>
        /// Gets the system id of the controller.
        /// </summary>
        public Guid SystemId { get; }

        /// <summary>
        /// Gets a flag to indicate if the controller is virtual or real.
        /// </summary>
        public bool IsVirtual { get; }

        /// <summary>
        /// Gets the IP address of the controller.
        /// </summary>
        public IPAddress IPAddress { get; }

        /// <summary>
        /// Gets the RobotWare version as a string
        /// </summary>
        /// <remarks>
        /// Supported from RW 6.03
        /// </remarks>
        public string VersionName { get; }

        /// <summary>
        /// Gets the version of the system on the controller.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets the base directory of a virtual controller.
        /// </summary>
        public DirectoryInfo BaseDirectory { get; }

        /// <summary>
        /// Gets the host name of the controller.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// Gets the netscan id of the object. Not for public use.
        /// </summary>
        public int NetscanId { get; }

        /// <summary>
        /// Gets the mac address of the controller.
        /// </summary>
        public string MacAddress { get; }

        /// <summary>
        /// Gets the system name of the controller.
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// ABB internal use only
        /// </summary>
        public int RobApiPort { get; }

        /// <summary>
        /// Get the RunLevel of the controller.
        /// </summary>
        public Level RunLevel { get; }

        /// <summary>
        /// Get the Id of the controller.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the controller name of the controller.
        /// </summary>
        public string ControllerName { get; }

        /// <summary>
        /// Gets the Availability of the controller.
        /// </summary>
        public Availability Availability { get; }

        /// <summary>
        /// Returns the TCP port number for a virtual controller when using Robot Web Services.
        /// </summary>
        /// <remarks>
        /// Supported from RobotWare 6.03.01.
        /// </remarks>
        public int WebServicesPort { get; }
        /// <summary>
        /// Combines the two other equals operations.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>True if the system ids are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType().GetProperty("SystemId") != null)
            {
                return this.SystemId.ToString() == obj.GetType().GetProperty("SystemId").ToString();
            }
            else
            {
                return false;
            }
            
        } 
        /// <summary>
        /// Checks if this object has same systemId as the provided info object.
        /// </summary>
        /// <param name="info">ControllerInfo object to compare two.</param>
        /// <returns>True if the systemId is same on both objects.</returns>
        public bool Equals(ControllerInfo info)
        {
            return this.SystemId== info.SystemId;
        }
        /// <summary>
        /// Checks if this object system id is equal to the provided Guid.
        /// </summary>
        /// <param name="systemId">Guid to compare two.</param>
        /// <returns>True if this objects SystemId is equal as systemId.</returns>
        public bool Equals(Guid systemId)
        {
            return this.SystemId == systemId;
        }
        /// <summary>
        /// Gets the hash code for this object. Based on the system id.
        /// </summary>
        /// <returns>hash code for this object</returns>
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
        /// <summary>
        /// Compares two ControllerInfo objects for equility.
        /// </summary>
        /// <param name="co1">Object 1.</param>
        /// <param name="co2">Object 2.</param>
        /// <returns>True if both objects have the same system id.</returns>
        public static bool operator ==(NetworkControllerInfo co1, NetworkControllerInfo co2)
        {
            return co1.SystemId == co2.SystemId;
        }
        /// <summary>
        /// Compares two ControllerInfo objects for in equility.
        /// </summary>
        /// <param name="co1">Object 1.</param>
        /// <param name="co2">Object 2.</param>
        /// <returns>True if both doesn't have the same system id.</returns>
        public static bool operator !=(NetworkControllerInfo co1, NetworkControllerInfo co2)
        {
            return co1.SystemId != co2.SystemId;
        }
    }

}
