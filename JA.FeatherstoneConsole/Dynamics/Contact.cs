using System;
using System.Collections.Generic;

using JA.LinearAlgebra.Screws;

namespace JA.Dynamics
{
    public class ContactInfo : ICanChangeUnits
    {
        public ContactInfo(UnitSystem units, Vector33 normal, double coefficientOfRestitution)
        {
            UnitSystem = units;
            Normal=normal;
            COR = coefficientOfRestitution;
        }
        public UnitSystem UnitSystem { get; private set; }
        public Vector33 Normal { get; }
        /// <summary>
        /// The coefficient of restitution.
        /// </summary>
        public double COR { get; }
        public double Impulse { get; set; }
        public void DoConvert(UnitSystem target)
        {
            Normal.DoConvertWrench(UnitSystem, target, UnitType.None);
        }
    }
    public class Contact : ContactInfo
    {
        public Contact(JointBody action, Vector33 normal, double coefficientOfRestitution)
            : this(action, null, normal, coefficientOfRestitution) { }
        public Contact(JointBody action, JointBody reaction, Vector33 normal, double coefficientOfRestitution)
            : base(action.UnitSystem, normal, coefficientOfRestitution) 
        {
            Action=action??throw new ArgumentException("Action Body Cannot be null");
            Reaction=reaction;
        }
        public JointBody Action { get; }
        public JointBody Reaction { get; }
        public bool IsWithGround => Reaction != null;
    }

}