using JA.LinearAlgebra.Geometry.Spatial;
using JA.LinearAlgebra.Screws;

namespace JA.Dynamics.Featherstone
{
    public class Articulated
    {
        internal readonly int count;
        internal readonly Matrix33[] I_A;
        internal readonly Vector33[] p_A;
        internal readonly Matrix33[] L_A;
        internal readonly Vector33[] b_A;
        public Articulated(int count)
        {
            this.count=count;
            this.I_A = new Matrix33[count];
            this.p_A = new Vector33[count];
            this.L_A = new Matrix33[count];
            this.b_A = new Vector33[count];
        }

        public void Calculate(State state, Kinematics kinematics)
        {
            var simulation = state.simulation;
            var joints = simulation.Joints;
            int[] parents = simulation.Parents;
            int[][] children = simulation.Children;
            int n = simulation.Dof;
            Vector3 gravity = simulation.Gravity;

            // Propagate Articulated Inertia Down The Chain
            for (int i_joint = n-1; i_joint>=0; i_joint--)
            {
                var joint = joints[i_joint];
                Matrix33 I_Ai = kinematics.I[i_joint];
                Vector33 I_pi = kinematics.p[i_joint] - kinematics.w[i_joint];
                var jointChildrenList = children[i_joint];
                for (int index = 0; index<jointChildrenList.Length; index++)
                {
                    int i_child = jointChildrenList[index];
                    var child = joints[i_child];
                    Matrix33 An = I_A[i_child];
                    Vector33 dn = p_A[i_child];
                    double Qn = state.tau[i_child];
                    Vector33 sn = kinematics.s[i_child];
                    Vector33 kn = kinematics.k[i_child];
                    Vector33 Ln = An*sn;
                    Vector33 Tn = Ln/Vector33.Dot(sn,Ln);
                    Matrix33 RUn = 1d - Vector33.Outer(Tn,sn);

                    //tex: $$\begin{aligned}{\bf I}_{i}^{A} & ={\bf I}_{i}+\sum_{n}^{{\rm children}}\left(1-\boldsymbol{T}_{n}\boldsymbol{s}_{n}^{\top}\right){\bf I}_{n}^{A}\\
                    //\boldsymbol{p}_{i}^{A} & =\boldsymbol{p}_{i}+\sum_{n}^{{\rm children}}\left(\boldsymbol{T}_{n}Q_{n}+\left(1-\boldsymbol{T}_{n}\boldsymbol{s}_{n}^{\top}\right)\left({\bf I}_{n}^{A}\boldsymbol{\kappa}_{n}+\boldsymbol{p}_{n}^{A}\right)\right)
                    //\end{aligned}$$

                    I_Ai+=RUn*An;
                    I_pi+=Tn*Qn+RUn*( An*kn+dn );
                }
                I_A[i_joint]=I_Ai;
                p_A[i_joint]=I_pi;
            }
            for (int i_joint = 0; i_joint<n; i_joint++)
            {
                var joint = joints[i_joint];
                int i_parent = parents[i_joint];
                var s_i = kinematics.s[i_joint];
                var I_i = kinematics.I[i_joint];
                var I_inv = I_i.Inverse();
                Matrix33 L_p   = Matrix33.Zero;
                Vector33 b_p   = Vector33.Zero;
                Matrix33 Φ_i   = Matrix33.Identity;
                Matrix33 Φ_inv = Matrix33.Identity;
                if (i_parent>=0)
                {
                    L_p = L_A[i_parent];
                    b_p = b_A[i_parent];

                    //tex: $$\Phi_{i}=1+I_{i}\Lambda_{i-1}^{-1}$$
                    Φ_i   = 1 + I_i*L_p;
                    Φ_inv = Φ_i.Inverse();
                }
                //tex: $$T_{i} =\Phi_{i}^{-1}I_{i}s_{i}\left(s_{i}^{\intercal}\Phi_{i}^{-1}I_{i}s_{i}\right)^{-1}$$
                Vector33 T_i = (Φ_inv*I_i*s_i)/(Vector33.Dot(s_i,Φ_inv*I_i*s_i));
                //tex: $$\Lambda_{i}^{-1}=\left(1+\Lambda_{i-1}^{-1}I_{i}\right)^{-1}\left(s_{i}T_{i}^{\intercal}I_{i}^{-1}+\Lambda_{i-1}^{-1}\right)$$
                L_A[i_joint] = Φ_inv*( Vector33.Outer(s_i,T_i)*I_inv + L_p );
                //tex: $$b_{i}=\Phi_{i}^{-1}\left(s_{i}\left(s_{i}^{\intercal}\Phi_{i}^{-1}I_{i}s_{i}\right)^{-1}Q_{i}+\left(1-s_{i}T_{i}^{\intercal}\right)\left(b_{i-1}+\kappa_{i}\right)\right)-\Lambda_{i}^{-1}p_{i}$$
                b_A[i_joint] = Φ_inv*( s_i*( state.tau[i_joint]/Vector33.Dot(s_i,Φ_inv*I_i*s_i) ) + ( 1 - Vector33.Outer(s_i,T_i) )*( b_p + kinematics.k[i_joint] ) ) - L_A[i_joint]*p_A[i_joint];
            }
        }
    }
}
