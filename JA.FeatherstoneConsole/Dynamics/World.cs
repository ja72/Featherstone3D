using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace JA.Dynamics
{
    using JA.LinearAlgebra.Spatial;

    using Vector33 = JA.LinearAlgebra.Screws.Vector33;
    using Wrench3 = JA.LinearAlgebra.Screws.Wrench3;

    public class World : ICanChangeUnits
    {
        internal UnitSystem units;
        internal Vector3 gravity;
        internal readonly List<JointBody> rootJoints;
        internal readonly List<Contact> contacts;

        #region Factor
        public World(UnitSystem units)
            : this(units, -Vector3.UnitY*units.EarthGravity()) { }
        public World(UnitSystem units, Vector3 gravity)
        {
            this.units=units;
            this.gravity=gravity;
            this.rootJoints=new List<JointBody>();
            this.contacts = new List<Contact>();
        }
        #endregion

        #region Property
        public UnitSystem UnitSystem { get => units; }
        public Vector3 Gravity { get => gravity; set => gravity=value; }
        public IReadOnlyList<JointBody> RootJoints => rootJoints;

        public void DoConvert(UnitSystem target)
        {
            var f_acc = Units.Acceleration.Convert(units, target);
            gravity *= f_acc;

            foreach (var item in rootJoints)
            {
                item.DoConvert(target);
            }

            foreach (var contact in contacts)
            {
                contact.DoConvert(target);
            }

            this.units = target;
        }

        #endregion

        #region Structure
        public JointBody NewScrew(Pose3 localPosition, Vector3 localAxis, double pitch)
            => NewScrew(UnitSystem, localPosition, localAxis, pitch);
        public JointBody NewScrew(UnitSystem units, Pose3 localPosition, Vector3 localAxis, double pitch)
        {
            var joint = JointBody.NewScrew(units, localPosition, localAxis, pitch);
            rootJoints.Add(joint);
            return joint;
        }
        public JointBody NewRevolute(Pose3 localPosition, Vector3 localAxis)
            => NewRevolute(UnitSystem, localPosition, localAxis);
        public JointBody NewRevolute(UnitSystem units, Pose3 localPosition, Vector3 localAxis)
        {
            var joint = JointBody.NewRevolute(units, localPosition, localAxis);
            rootJoints.Add(joint);
            return joint;
        }
        public JointBody NewPrismatic(Pose3 localPosition, Vector3 localAxis)
            => NewPrismatic(UnitSystem, localPosition, localAxis);
        public JointBody NewPrismatic(UnitSystem units, Pose3 localPosition, Vector3 localAxis)
        {
            var joint = JointBody.NewPrismatic(units, localPosition, localAxis);
            rootJoints.Add(joint);
            return joint;
        }

        public Contact NewContact(JointBody action, Vector3 localPosition, Vector3 localDirection, double coefficientOfRestitution)
        {
            Vector33 normal = Wrench3.At(localDirection, localPosition);
            Contact contact = new Contact(action, normal, coefficientOfRestitution);
            contacts.Add(contact);
            return contact;
        }
        public Contact NewContact(JointBody action, JointBody reaction, Vector3 localPosition, Vector3 localDirection, double coefficientOfRestitution)
        {
            Vector33 normal = Wrench3.At(localDirection, localPosition);
            Contact contact = new Contact(action, reaction, normal, coefficientOfRestitution);
            contacts.Add(contact);
            return contact;
        }

        public void Transverse(Action<JointBody> action)
        {
            foreach (var item in rootJoints)
            {
                item.Traverse(action);
            }
        }
        public R Transverse<R>(R initialValue, Func<JointBody, R> operation)
        {
            R result = initialValue;
            foreach (var item in rootJoints)
            {
                result = item.Traverse(result, operation);
            }
            return result;
        }

        public JointBody[] GetAllJoints(UnitSystem units)
        {
            var list = new List<JointBody>();
            Transverse((jnt) =>
            {
                jnt.DoConvert(units);
                list.Add(jnt);
            });
            return list.ToArray();
        }
        public static World BuildSerialChain(int count, double deltaDistance, MassProperties massProperties)
            => BuildSerialChain(massProperties.UnitSystem, count, deltaDistance, massProperties);
        public static World BuildSerialChain(UnitSystem unit, int count, double deltaDistance, MassProperties massProperties)
        {
            var gravity = new Vector3(0, -unit.EarthGravity(),0);
            var world = new World(unit, gravity);
            JointBody parent = null;
            for (int i = 0; i < count; i++)
            {
                JointBody joint;
                if ( parent == null)
                {
                    joint = world.NewRevolute(Pose3.Origin, Vector3.UnitZ);
                }
                else
                {
                    joint = parent.AddRevolute(Pose3.Translation(deltaDistance, 0, 0), Vector3.UnitZ);
                }

                joint.AddMassProperties(massProperties);
                parent = joint;
            }
            return world;
        }

        #endregion

        #region Mechanics
        public Simulation ToSimulation()
        {
            return new Simulation(this);
        }


        #endregion

        #region Formatting
        public override string ToString()
        {
            return $"World: Units={UnitSystem}, Gravity={Gravity}, RootJoints={RootJoints.Count}";
        }
        #endregion
    }

}
