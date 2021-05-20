using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Stations;
using System;

namespace RFRCC_RobotController.ABB_Data.RS_Connection.Robotics
{
    public class FakeTransform : ICloneable
    {
        private Matrix4 _GlobalMatrix = new Matrix4();
        public ProjectObject Parent { get; }
        public double X
        {
            get => _GlobalMatrix.Translation.x;
            set
            {
                var vector = _GlobalMatrix.Translation;
                vector.x = value;
                _GlobalMatrix.Translation = vector;
            }
        }
        public double Y
        {
            get => _GlobalMatrix.Translation.y;
            set
            {
                var vector = _GlobalMatrix.Translation;
                vector.y = value;
                _GlobalMatrix.Translation = vector;
            }
        }
        public double Z
        {
            get => _GlobalMatrix.Translation.z;
            set
            {
                var vector = _GlobalMatrix.Translation;
                vector.z = value;
                _GlobalMatrix.Translation = vector;
            }
        }
        public Vector3 Translation
        {
            get
            {
                return _GlobalMatrix.Translation;
            }
            set
            {
                _GlobalMatrix.Translation = value;
            }
        }
        public double RX => _GlobalMatrix.EulerZYX.x;
        public double RY => _GlobalMatrix.EulerZYX.y;
        public double RZ => _GlobalMatrix.EulerZYX.z;
        public Matrix4 Matrix 
        { 
            get => _GlobalMatrix;
            set
            {
                _GlobalMatrix = value;
            }
        }
        public Matrix4 GlobalMatrix
        {
            get => _GlobalMatrix;
            set
            {
                _GlobalMatrix = value;
            }
        }

        public FakeTransform(Transform transform)
        {
            _GlobalMatrix = new Matrix4()
            {
                AxisAngle = new Vector4(
                    transform.GlobalMatrix.AxisAngle.x,
                    transform.GlobalMatrix.AxisAngle.y,
                    transform.GlobalMatrix.AxisAngle.z,
                    transform.GlobalMatrix.AxisAngle.w),
                Translation = new Vector3(
                    transform.GlobalMatrix.Translation.x,
                    transform.GlobalMatrix.Translation.y,
                    transform.GlobalMatrix.Translation.z),
                x = transform.GlobalMatrix.x,
                y = transform.GlobalMatrix.y,
                z = transform.GlobalMatrix.z,
                t = transform.GlobalMatrix.t,
                Quaternion = new Quaternion(
                    transform.GlobalMatrix.Quaternion.q1,
                    transform.GlobalMatrix.Quaternion.q2,
                    transform.GlobalMatrix.Quaternion.q3,
                    transform.GlobalMatrix.Quaternion.q4),
                EulerZYX = new Vector3(
                    transform.GlobalMatrix.EulerZYX.x,
                    transform.GlobalMatrix.EulerZYX.y,
                    transform.GlobalMatrix.EulerZYX.z)
            };
        }

        public FakeTransform(FakeTransform transform)
        {
            _GlobalMatrix = new Matrix4()
            {
                AxisAngle = new Vector4(
                    transform.GlobalMatrix.AxisAngle.x,
                    transform.GlobalMatrix.AxisAngle.y,
                    transform.GlobalMatrix.AxisAngle.z,
                    transform.GlobalMatrix.AxisAngle.w),
                Translation = new Vector3(
                    transform.GlobalMatrix.Translation.x,
                    transform.GlobalMatrix.Translation.y,
                    transform.GlobalMatrix.Translation.z),
                x = transform.GlobalMatrix.x,
                y = transform.GlobalMatrix.y,
                z = transform.GlobalMatrix.z,
                t = transform.GlobalMatrix.t,
                Quaternion = new Quaternion(
                    transform.GlobalMatrix.Quaternion.q1,
                    transform.GlobalMatrix.Quaternion.q2,
                    transform.GlobalMatrix.Quaternion.q3,
                    transform.GlobalMatrix.Quaternion.q4),
                EulerZYX = new Vector3(
                    transform.GlobalMatrix.EulerZYX.x,
                    transform.GlobalMatrix.EulerZYX.y,
                    transform.GlobalMatrix.EulerZYX.z)
            };
        }

        public FakeTransform()
        {
            _GlobalMatrix = new Matrix4();
            Translation = new Vector3();
            Matrix = new Matrix4();
            GlobalMatrix = new Matrix4();
        }
        //
        // Summary:
        //     Gets a ABB.Robotics.Math.Matrix4 that converts from this ABB.Robotics.RobotStudio.Stations.Transform
        //     to a reference ABB.Robotics.RobotStudio.Stations.Transform.
        //
        // Parameters:
        //   relativeTo:
        //     Reference ABB.Robotics.RobotStudio.Stations.Transform.
        //
        // Returns:
        //     A ABB.Robotics.Math.Matrix4 that converts from this coordinate system to the
        //     coordinate system of the reference object.
        public Matrix4 GetRelativeTransform(FakeTransform relativeTo)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Gets a ABB.Robotics.Math.Matrix4 that converts from this ABB.Robotics.RobotStudio.Stations.Transform
        //     to a reference object.
        //
        // Parameters:
        //   relativeTo:
        //     Reference object.
        //
        // Returns:
        //     A ABB.Robotics.Math.Matrix4 that converts from this coordinate system to the
        //     coordinate system of the reference object.
        public Matrix4 GetRelativeTransform(IHasTransform relativeTo)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Sets the position and orientation relative to a reference frame.
        //
        // Parameters:
        //   relativeToGlobal:
        //     Reference frame in global coordinates.
        //
        //   matrix:
        //     Position and orientation relative to the reference frame.
        public void SetRelativeTransform(Matrix4 relativeToGlobal, Matrix4 matrix)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Sets the position and orientation relative to another object.
        //
        // Parameters:
        //   relativeTo:
        //     Reference object.
        //
        //   matrix:
        //     Position and orientation relative to the reference object.
        public void SetRelativeTransform(IHasTransform relativeTo, Matrix4 matrix)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new FakeTransform(this); 
        }
    }

}
