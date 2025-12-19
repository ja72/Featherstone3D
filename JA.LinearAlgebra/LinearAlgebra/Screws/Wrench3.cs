using System.Runtime.CompilerServices;

using JA.LinearAlgebra.Spatial;

namespace JA.LinearAlgebra.Screws
{
    //using JA.LinearAlgebra.VectorCalculus;
    
    using Vector3 = Vector3;
    using Matrix3 = Matrix3;

    public static class Wrench3
    {
        public static ScrewLayout Layout {get; } = ScrewLayout.Ray;

        public static bool IsPureWrench(this Vector33 wrench) => wrench.linear.IsZero();

        #region Wrenches
        public static Vector33 At(Vector3 value, Vector3 position, double pitch = 0)
            => new Vector33(
                value,
                value*pitch+Vector3.Cross(position, value)
            );
        public static Vector33 Pure(Vector3 value)
            => new Vector33(
                Vector3.Zero,
                value
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix33 CrossOp(Vector33 wrench)
        {
            var wx = wrench.angular.CrossOp();
            var vx = wrench.linear.CrossOp();
            return new Matrix33(wx, Matrix3.Zero, vx, wx);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector33 Cross(Vector33 @this, Vector33 wrench)
        {
            //tex: Wrench Differential Operator $$\begin{bmatrix}v\\
            //\omega
            //\end{bmatrix}\times=\begin{bmatrix}\omega\times & 0\\
            //v\times & \omega\times
            //\end{bmatrix}$$
            return new Vector33(
                Vector3.Cross(@this.angular, wrench.linear),
                Vector3.Cross(@this.linear, wrench.linear)+Vector3.Cross(@this.angular, wrench.angular)
            );
        }
        /// <summary>
        /// By convention the angular vector is the line vector for wrenches.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LineVector(Vector33 wrench)
            => wrench.linear;
        /// <summary>
        /// By convention the linear vector is the moment vector for wrenches.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MomentVector(Vector33 wrench)
            => wrench.angular;
        /// <summary>
        /// The magnitude of a wrench.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Magnitude(Vector33 wrench)
            => wrench.linear.Magnitude;
        /// <summary>
        /// The direction of a wrench line.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Direction(Vector33 wrench)
            => wrench.linear.ToNormalized();
        /// <summary>
        /// The pitch of a wrench.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pitch(Vector33 wrench)
            //tex: $$h = \frac{ F \cdot \tau}{\|F\|^2}$$
            => Vector3.Dot(wrench.linear, wrench.angular)/wrench.linear.MagnitudeSquared;
        /// <summary>
        /// The point of a wrench line closest to the origin.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Position(Vector33 wrench)
            //tex: $$r = \frac{ F \times \tau}{\|F\|^2}$$
            => Vector3.Cross(wrench.linear, wrench.angular)/wrench.linear.MagnitudeSquared;
        #endregion

        public static Vector33 DoConvertWrench(this Vector33 wrench, UnitSystem from, UnitSystem to, Unit quantity)
        {
            float f_len = Units.GetFactor(UnitType.Length, from, to);
            float f_qty = quantity.Convert(from, to);
            var linear = wrench.linear * f_qty;
            var angular = wrench.angular * f_len * f_qty;
            return new Vector33(angular, linear);
        }
    }
}
