using System;
using System.Collections.Generic;
using System.ComponentModel;

using JA.LinearAlgebra.Geometry.Spatial;
using JA.LinearAlgebra.Screws;

namespace JA.Dynamics
{
    public enum JointType
    {
        /// <summary>
        /// Specifies a joint that allows translational and rotational movement about a single axis.
        /// </summary>
        [Description("Combined rotation and parallel translation")]
        Screw,
        /// <summary>
        /// Specifies a joint that allows pure rotational movement about a single axis.
        /// </summary>
        [Description("Pure rotation about an axis")]
        Revolute,
        /// <summary>
        /// Specifies a joint that allows pure translational movement along a single axis.
        /// </summary>
        [Description("Pure translation along an axis")]
        Prismatic,
    }

    /// <summary>
    /// Class to contain information about a joint and body in a mechanical system.
    /// </summary>
    public class JointBodyInfo : ICanChangeUnits<JointBodyInfo>, ICanChangeUnits
    {
        internal readonly JointType type;
        internal Pose3 localPosition;
        internal Vector3 localAxis;
        internal double pitch;
        internal UnitSystem units;
        internal Motor motor;
        internal MassProperties massProperties;
        internal (double q, double qp) initialConditions;

        public JointBodyInfo(UnitSystem units, JointType type, Pose3 localPosition, Vector3 localAxis, double pitch)
            : this(units, type, localPosition, localAxis, pitch, MassProperties.Zero) { }
        public JointBodyInfo(UnitSystem units, JointType type, Pose3 localPosition, Vector3 localAxis, double pitch, MassProperties massProperties)
        {
            switch (type)
            {
                case JointType.Revolute:
                {   
                    pitch=0;
                    break;
                }
                case JointType.Prismatic:
                {
                    pitch=double.PositiveInfinity;
                    break;
                }
            }
            this.units=units;
            this.type=type;
            this.localPosition=localPosition;
            this.localAxis=localAxis;
            this.pitch=pitch;
            this.massProperties = massProperties.ToConverted(units);
            this.initialConditions=(0, 0);
            this.motor=Motor.ConstForcing(0);
        }

        #region Properties
        public JointType Type => type;

        public UnitSystem UnitSystem
        {
            get => units;
            private set => units=value;
        }
        public Pose3 LocalPosition
        {
            get => localPosition;
            set => localPosition=value;
        }
        public Vector3 LocalAxis
        {
            get => localAxis;
            set => localAxis=value;
        }
        public double Pitch
        {
            get => pitch;
            set => pitch=value;
        }
        public MassProperties MassProperties
        {
            get => massProperties;
            private set => massProperties=value;
        }

        public (double q, double qp) InitialConditions
        {
            get => initialConditions;
            set => initialConditions=value;
        }

        public Motor Motor
        {
            get => motor;
            set => motor=value;
        } 
        #endregion

        #region Mass Properties
        public void AddMassProperties(MassProperties additionalMassProperties)
        {
            massProperties+=additionalMassProperties.ToConverted(units);
        }
        public void SubMassProperties(MassProperties additionalMassProperties)
        {
            massProperties-=additionalMassProperties.ToConverted(units);
        }
        public void ZeroMassProperties() => MassProperties=MassProperties.Zero; 
        #endregion

        #region Mechanics
        public Vector33 GetJointAxis(Pose3 top)
        {
            Vector3 axis = top.Orientation.RotateVector(localAxis);
            switch (type)
            {
                case JointType.Screw:
                {
                    return Twist3.At(axis, top.Position, pitch);
                }
                case JointType.Revolute:
                {
                    return Twist3.At(axis, top.Position, 0.0);
                }
                case JointType.Prismatic:
                {
                    return Twist3.Pure(axis);
                }
                default:
                throw new NotSupportedException("Unknown joint type.");
            }
        }

        public Pose3 GetLocalJointStep(double q)
        {
            Vector3 stepPos;
            Quaternion3 stepOri;
            switch (type)
            {
                case JointType.Screw:
                {
                    stepPos=Vector3.Scale(localAxis*pitch, q);
                    stepOri=Quaternion3.FromAxisAngle(localAxis, q);
                    return localPosition.TranslateRotate(stepPos, stepOri);
                }
                case JointType.Revolute:
                {
                    stepOri=Quaternion3.FromAxisAngle(localAxis, q);
                    return localPosition.Rotate(stepOri);
                }
                case JointType.Prismatic:
                {
                    stepPos=Vector3.Scale(localAxis, q);
                    return localPosition.Translate(stepPos);
                }
                default:
                throw new NotSupportedException("Unknown joint type.");
            }
        }


        #endregion

        #region Units
        public JointBodyInfo ToConverted(UnitSystem target)
        {
            var fl = Units.Length.Convert(UnitSystem, target);
            var newpitch= pitch * fl;
            var newlocalPosition=localPosition.ToConvertedFrom(units, target);
            var newMassProperties=MassProperties.ToConverted(target);
            var newUnits=target;

            var info = new JointBodyInfo(target, type, newlocalPosition, localAxis, newpitch, newMassProperties);
            // TODO: Convert the motor, for both q,qp (prismatic) and result (frc/trq)
            return info;
        }

        public virtual void DoConvert(UnitSystem target)
        {
            if (units==target) return;

            var fl = Units.Length.Convert(UnitSystem, target);
            this.pitch*=fl;
            this.localPosition=localPosition.ToConvertedFrom(units, target);
            this.MassProperties=this.MassProperties.ToConverted(target);
            this.UnitSystem=target;            
        }

        #endregion

        #region Formatting
        public override string ToString()
        {
            switch (type)
            {
                case JointType.Screw:
                return $"Screw(Units={UnitSystem}, LocalPosition={localPosition}, LocalAxis={localAxis}, Pitch={pitch})";
                case JointType.Revolute:
                return $"Revolute(Units={UnitSystem}, LocalPosition={localPosition}, LocalAxis={localAxis})";
                case JointType.Prismatic:
                return $"Prismatic(Units={UnitSystem}, LocalPosition={localPosition}, LocalAxis={localAxis})";
                default:
                return $"{type}(Units={UnitSystem}, LocalPosition={localPosition}, LocalAxis={localAxis}, Pitch={pitch})";
            }
        }
        #endregion

    }

    /// <summary>
    /// Class to describe the structure of a mechanical system
    /// </summary>
    public class JointBody : JointBodyInfo, ITree<JointBody>, ICanChangeUnits
    {
        internal JointBody parent;
        internal readonly List<JointBody> children;

        #region Factory
        JointBody(UnitSystem units, JointType type, Pose3 localPosition, Vector3 localAxis, double pitch)
            : this(units, type, localPosition, localAxis, pitch, MassProperties.Zero)
        { }
        JointBody(JointBody parent, JointType type, Pose3 localPosition, Vector3 localAxis, double pitch)
            : this(parent, type, localPosition, localAxis, pitch, MassProperties.Zero)
        { }
        JointBody(UnitSystem units, JointType type, Pose3 localPosition, Vector3 localAxis, double pitch, MassProperties massProperties)
            : base(units, type, localPosition, localAxis, pitch, massProperties)
        {
            this.parent=null;
            this.children=new List<JointBody>();
        }
        JointBody(JointBody parent, JointType type, Pose3 localPosition, Vector3 localAxis, double pitch, MassProperties massProperties)
            : base(parent?.units??UnitSystem.MKS, type, localPosition, localAxis, pitch, massProperties)
        {
            this.parent=parent;
            this.children=new List<JointBody>();
            if (parent!=null)
            {
                parent.children.Add(this);
            }
        }

        public JointBody AddScrew(Pose3 localPosition, Vector3 localAxis, double pitch)
            => new JointBody(this, JointType.Screw, localPosition, localAxis, pitch);
        public JointBody AddRevolute(Pose3 localPosition, Vector3 localAxis)
            => new JointBody(this, JointType.Revolute, localPosition, localAxis, 0);
        public JointBody AddPrismatic(Pose3 localPosition, Vector3 localAxis)
            => new JointBody(this, JointType.Prismatic, localPosition, localAxis, double.PositiveInfinity);
        public static JointBody NewScrew(UnitSystem units, Pose3 localPosition, Vector3 localAxis, double pitch)
            => new JointBody(units, JointType.Screw, localPosition, localAxis, pitch);
        public static JointBody NewRevolute(UnitSystem units, Pose3 localPosition, Vector3 localAxis)
            => new JointBody(units, JointType.Revolute, localPosition, localAxis, 0);
        public static JointBody NewPrismatic(UnitSystem units, Pose3 localPosition, Vector3 localAxis)
            => new JointBody(units, JointType.Prismatic, localPosition, localAxis, double.PositiveInfinity);

        #endregion

        #region Properties

        public JointBody Parent
        {
            get => parent;
        }
        public IReadOnlyList<JointBody> Children => children;
        public bool IsRoot => parent==null;
        public bool IsLeaf => children.Count==0;

        public void AttachTo(JointBody parent)
        {
            this.parent?.children.Remove(this);
            this.parent=parent;
            this.parent?.children.Add(this);
        }

        public R Traverse<R>(R initialValue, Func<JointBody, R> operation)
        {
            R result = operation(this);
            foreach (var child in children)
            {
                result=child.Traverse(result, operation);
            }
            return result;
        }
        public void Traverse(Action<JointBody> operation)
        {
            operation(this);
            foreach (var child in children)
            {
                child.Traverse(operation);
            }
        }

        #endregion

        #region Units
        public override void DoConvert(UnitSystem target)
        {
            base.DoConvert(target);
            foreach (var item in children)
            {
                item.DoConvert(target);
            }
        }

        #endregion

        #region Formatting
        public override string ToString()
        {
            var info = base.ToString();

            return $"{info} #parents={(IsRoot?0:1)}, #children={children.Count}";
        }
        #endregion

    }
}
