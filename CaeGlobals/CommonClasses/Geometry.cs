using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaeGlobals
{
    public static class Geometry
    {
        public static double PointToSegmentDistance(double[] x, double[] a1, double[] a2)
        {
            Vec3D v0 = new Vec3D(x);
            Vec3D v1 = new Vec3D(a1);
            Vec3D v2 = new Vec3D(a2);

            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            double t = -Vec3D.DotProduct(v1 - v0, v2 - v1) / (v2 - v1).Len2;

            if (t <= 0)
                return (v0 - v1).Len;
            else if (t >= 1)
                return (v0 - v2).Len;
            else
                return Vec3D.CrossProduct(v0 - v1, v0 - v2).Len / (v2 - v1).Len;
        }
        public static double SegmentToSegmentDistance(double[] a1, double[] a2, double[] b1, double[] b2)
        {
            Vec3D p1 = new Vec3D(a1);
            Vec3D d1 = new Vec3D(a2) - p1;          // Direction
            Vec3D p2 = new Vec3D(b1);
            Vec3D d2 = new Vec3D(b2) - p2;          // Direction
            // https://en.wikipedia.org/wiki/Skew_lines#Distance
            // v1 = p1 + t1*d1
            // v2 = p2 + t2*d2
            // Min distance line normal
            Vec3D n = Vec3D.CrossProduct(d1, d2);
            // The plane formed by the translations of Line 1 along n 
            Vec3D n1 = Vec3D.CrossProduct(d1, Vec3D.CrossProduct(d2, d1));
            // The plane formed by the translations of Line 2 along n 
            Vec3D n2 = Vec3D.CrossProduct(d2, Vec3D.CrossProduct(d1, d2));
            //
            double div1 = Vec3D.DotProduct(d1, n2);
            double div2 = Vec3D.DotProduct(d2, n1);
            // Unparallel lines
            if (div1 != 0 && div2 != 0)
            {
                double t1 = Vec3D.DotProduct(p2 - p1, n2) / div1;
                double t2 = Vec3D.DotProduct(p1 - p2, n1) / div2;
                //
                Vec3D c1;
                if (t1 <= 0) c1 = p1;
                else if (t1 >= 1) c1 = new Vec3D(a2);
                else c1 = p1 + t1 * d1;
                Vec3D c2;
                if (t2 <= 0) c2 = p2;
                else if (t2 >= 1) c2 = new Vec3D(b2);
                else c2 = p2 + t2 * d2;
                //
                return (c1 - c2).Len;
            }
            // Parellel or coincident lines
            else
            {
                double dist = Vec3D.CrossProduct(p1 - p2, p1 - (p2 + d2)).Len / d2.Len;
                // Coincident lines - two poit to segment possibilities
                if (dist == 0)
                {
                    return Math.Min(Math.Min((p1 - p2).Len, (p1 - (p2 + d2)).Len),
                                    Math.Min(((p1 + d1) - p2).Len, ((p1 + d1) - (p2 + d2)).Len));
                }
                // Parellel lines
                else
                {
                    return dist;
                }
            }
        }
        public static double PointToTriangleDistance(double[] point, double[][] triangle)
        {
            // ChatGPT using UnityEngine;
            double[] tp1 = triangle[0];
            double[] tp2 = triangle[1];
            double[] tp3 = triangle[2];
            //
            double[] edge1 = new double[3];
            double[] edge2 = new double[3];
            double[] edge3 = new double[3];
            double[] edge3inv = new double[3];
            VmV(ref edge1, tp2, tp1);
            VmV(ref edge2, tp3, tp2);
            VmV(ref edge3, tp1, tp3);
            VmV(ref edge3inv, tp3, tp1);
            //
            double[] normal = new double[3];
            VcrossV(ref normal, edge1, edge3inv);
            Vnorm(ref normal, normal);
            //
            double[] tmp = new double[3];
            VmV(ref tmp, tp1, point);
            double distance = VdotV(normal, tmp);
            //
            double[] projection = new double[3];
            VpVxS(ref projection, point, normal, distance);
            //
            double a = VdotV(edge1, edge1);
            double b = VdotV(edge1, edge2);
            double c = VdotV(edge2, edge2);
            VmV(ref tmp, projection, tp1);
            double d = VdotV(edge1, tmp);
            VmV(ref tmp, projection, tp2);
            double e = VdotV(edge2, tmp);
            VmV(ref tmp, projection, tp3);
            //double f = VdotV(tmp, tmp);
            //
            double det = a * c - b * b;
            double s = b * e - c * d;
            double t = b * d - a * e;
            //
            if (s + t < det)
            {
                if (s < 0)
                {
                    if (t < 0)
                    {
                        if (d < 0)
                        {
                            s = Clamp(s, 0, det);
                            t = 0;
                        }
                        else
                        {
                            s = 0;
                            t = Clamp(t, 0, det);
                        }
                    }
                    else
                    {
                        s = 0;
                        t = Clamp(t, 0, det);
                    }
                }
                else if (t < 0)
                {
                    s = Clamp(s, 0, det);
                    t = 0;
                }
                else
                {
                    double invDet = 1 / det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else
            {
                if (s < 0)
                {
                    double tmp0 = b + d;
                    double tmp1 = c + e;
                    if (tmp1 > tmp0)
                    {
                        double numer = tmp1 - tmp0;
                        double denom = a - 2 * b + c;
                        s = Clamp(numer / denom, 0, 1);
                        t = 1 - s;
                    }
                    else
                    {
                        s = 0;
                        t = Clamp(-e / c, 0, 1);
                    }
                }
                else if (t < 0)
                {
                    if (a + d > b + e)
                    {
                        double numer = c + e - b - d;
                        double denom = a - 2 * b + c;
                        s = Clamp(numer / denom, 0, 1);
                        t = 1 - s;
                    }
                    else

                    {
                        s = Clamp(-e / c, 0, 1);
                        t = 0;
                    }
                }
                else
                {
                    double numer = c + e - b - d;
                    double denom = a - 2 * b + c;
                    s = Clamp(numer / denom, 0, 1);
                    t = 1 - s;
                }
            }
            //
            double[] closestPoint = new double[3];
            VxS(ref tmp, edge1, s);
            VpV(ref closestPoint, tp1, tmp);
            VxS(ref tmp, edge2, t);
            VpV(ref closestPoint, closestPoint, tmp);
            //
            return Math.Sqrt(VdistV2(point, closestPoint));
        }
        public static double[][] ShrinkTriangle(double[][] triangle, double shrink)
        {
            double[][] shrinkTriangle = new double[3][];
            shrinkTriangle[0] = new double[3];
            shrinkTriangle[1] = new double[3];
            shrinkTriangle[2] = new double[3];
            //
            double[] tmp = new double[3];
            double[] center = new double[3];
            double[] shrinkCenter = new double[3];
            //
            VpVpV(ref tmp, triangle[0], triangle[1], triangle[2]);
            VxS(ref center, tmp, 1.0 / 3.0);
            VxS(ref shrinkCenter, center, shrink);
            //
            VxS(ref tmp, triangle[0], (1 - shrink));
            VpV(ref shrinkTriangle[0], tmp, shrinkCenter);
            //
            VxS(ref tmp, triangle[1], (1 - shrink));
            VpV(ref shrinkTriangle[1], tmp, shrinkCenter);
            //
            VxS(ref tmp, triangle[2], (1 - shrink));
            VpV(ref shrinkTriangle[2], tmp, shrinkCenter);
            //
            return shrinkTriangle;
        }
        public static double Clamp(double value, double min, double max)
        {
            if (value < min) value = min;
            else if (value > max) value = max;
            return value;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // https://github.com/GammaUNC/PQP/blob/master/src/TriDist.cpp
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void VcV(double[] Vr, double[] V)
        {
            Vr[0] = V[0];
            Vr[1] = V[1];
            Vr[2] = V[2];
        }
        public static void VmV(ref double[] Vr, double[] V1, double[] V2)
        {
            Vr[0] = V1[0] - V2[0];
            Vr[1] = V1[1] - V2[1];
            Vr[2] = V1[2] - V2[2];
        }
        public static void VpV(ref double[] Vr, double[] V1, double[] V2)
        {
            Vr[0] = V1[0] + V2[0];
            Vr[1] = V1[1] + V2[1];
            Vr[2] = V1[2] + V2[2];
        }
        public static void VpVpV(ref double[] Vr, double[] V1, double[] V2, double[] V3)
        {
            Vr[0] = V1[0] + V2[0] + V3[0];
            Vr[1] = V1[1] + V2[1] + V3[1];
            Vr[2] = V1[2] + V2[2] + V3[2];
        }
        public static void VxS(ref double[] Vr, double[] V, double s)
        {
            Vr[0] = V[0] * s;
            Vr[1] = V[1] * s;
            Vr[2] = V[2] * s;
        }
        public static void VpVxS(ref double[] Vr, double[] V1, double[] V2, double s)
        {
            Vr[0] = V1[0] + V2[0] * s;
            Vr[1] = V1[1] + V2[1] * s;
            Vr[2] = V1[2] + V2[2] * s;
        }
        public static double VdotV(double[] V1, double[] V2)
        {
            return V1[0] * V2[0] + V1[1] * V2[1] + V1[2] * V2[2];
        }
        public static double VdistV2(double[] V1, double[] V2)
        {
            return (V1[0] - V2[0]) * (V1[0] - V2[0]) + (V1[1] - V2[1]) * (V1[1] - V2[1]) + (V1[2] - V2[2]) * (V1[2] - V2[2]);
        }
        public static void VcrossV(ref double[] Vr, double[] V1, double[] V2)
        {
            Vr[0] = V1[1] * V2[2] - V1[2] * V2[1];
            Vr[1] = V1[2] * V2[0] - V1[0] * V2[2];
            Vr[2] = V1[0] * V2[1] - V1[1] * V2[0];
        }
        public static void Vnorm(ref double[] Vr, double[] V)
        {
            double len = Math.Sqrt(V[0] * V[0] + V[1] * V[1] + V[2] * V[2]);
            if (len > 0)
            {
                Vr[0] = V[0] / len;
                Vr[1] = V[1] / len;
                Vr[2] = V[2] / len;
            }
            else
            {
                Vr[0] = 0;
                Vr[1] = 0;
                Vr[2] = 0;
            }
        }
        public static void SegPoints(ref double[] VEC,
                                     ref double[] X, ref double[] Y,    // closest points
                                     double[] P, double[] A,            // seg 1 origin, vector
                                     double[] Q, double[] B)            // seg 2 origin, vector
        {

            double A_dot_A, B_dot_B, A_dot_B, A_dot_T, B_dot_T;
            double[] T = new double[3];
            double[] TMP = new double[3];
            //
            VmV(ref T, Q, P);
            A_dot_A = VdotV(A, A);
            B_dot_B = VdotV(B, B);
            A_dot_B = VdotV(A, B);
            A_dot_T = VdotV(A, T);
            B_dot_T = VdotV(B, T);
            // t parameterizes ray P,A 
            // u parameterizes ray Q,B 
            double t, u;
            // Compute t for the closest point on ray P,A to ray Q,B
            double denom = A_dot_A * B_dot_B - A_dot_B * A_dot_B;
            t = (A_dot_T * B_dot_B - B_dot_T * A_dot_B) / denom;
            // clamp result so t is on the segment P,A
            if ((t < 0) || double.IsNaN(t)) t = 0; else if (t > 1) t = 1;
            // Find u for point on ray Q,B closest to point at t
            u = (t * A_dot_B - B_dot_T) / B_dot_B;
            // If u is on segment Q,B, t and u correspond to closest points, otherwise, clamp u, recompute and clamp t
            if ((u <= 0) || double.IsNaN(u))
            {
                VcV(Y, Q);
                t = A_dot_T / A_dot_A;
                //
                if ((t <= 0) || double.IsNaN(t))
                {
                    VcV(X, P);
                    VmV(ref VEC, Q, P);
                }
                else if (t >= 1)
                {
                    VpV(ref X, P, A);
                    VmV(ref VEC, Q, X);
                }
                else
                {
                    VpVxS(ref X, P, A, t);
                    VcrossV(ref TMP, T, A);
                    VcrossV(ref VEC, A, TMP);
                }
            }
            else if (u >= 1)
            {
                VpV(ref Y, Q, B);
                t = (A_dot_B + A_dot_T) / A_dot_A;
                //
                if ((t <= 0) || double.IsNaN(t))
                {
                    VcV(X, P);
                    VmV(ref VEC, Y, P);
                }
                else if (t >= 1)
                {
                    VpV(ref X, P, A);
                    VmV(ref VEC, Y, X);
                }
                else
                {
                    VpVxS(ref X, P, A, t);
                    VmV(ref T, Y, P);
                    VcrossV(ref TMP, T, A);
                    VcrossV(ref VEC, A, TMP);
                }
            }
            else
            {
                VpVxS(ref Y, Q, B, u);
                //
                if ((t <= 0) || double.IsNaN(t))
                {
                    VcV(X, P);
                    VcrossV(ref TMP, T, B);
                    VcrossV(ref VEC, B, TMP);
                }
                else if (t >= 1)
                {
                    VpV(ref X, P, A);
                    VmV(ref T, Q, X);
                    VcrossV(ref TMP, T, B);
                    VcrossV(ref VEC, B, TMP);
                }
                else
                {
                    VpVxS(ref X, P, A, t);
                    VcrossV(ref VEC, A, B);
                    if (VdotV(VEC, T) < 0)
                    {
                        VxS(ref VEC, VEC, -1);
                    }
                }
            }
        }

        //--------------------------------------------------------------------------
        // TriDist() 
        //
        // Computes the closest points on two triangles, and returns the 
        // distance between them.
        // 
        // S and T are the triangles, stored tri[point][dimension].
        //
        // If the triangles are disjoint, P and Q give the closest points of 
        // S and T respectively. However, if the triangles overlap, P and Q 
        // are basically a random pair of points from the triangles, not 
        // coincident points on the intersection of the triangles, as might 
        // be expected.
        //--------------------------------------------------------------------------
        public static double TriDist(ref double[] P, ref double[] Q, double[][] S, double[][] T, bool onlyInternal)
        {
            // Compute vectors along the 6 sides
            double[][] Sv = new double[][] { new double[] { 0, 0, 0 }, new double[3] { 0, 0, 0 }, new double[] { 0, 0, 0 } };
            double[][] Tv = new double[][] { new double[] { 0, 0, 0 }, new double[3] { 0, 0, 0 }, new double[] { 0, 0, 0 } };
            double[] VEC = new double[3];
            //
            VmV(ref Sv[0], S[1], S[0]);
            VmV(ref Sv[1], S[2], S[1]);
            VmV(ref Sv[2], S[0], S[2]);
            //
            VmV(ref Tv[0], T[1], T[0]);
            VmV(ref Tv[1], T[2], T[1]);
            VmV(ref Tv[2], T[0], T[2]);
            // For each edge pair, the vector connecting the closest points 
            // of the edges defines a slab (parallel planes at head and tail
            // enclose the slab). If we can show that the off-edge vertex of 
            // each triangle is outside of the slab, then the closest points
            // of the edges are the closest points for the triangles.
            // Even if these tests fail, it may be helpful to know the closest
            // points found, and whether the triangles were shown disjoint
            double[] V = new double[3];
            double[] Z = new double[3];
            double[] minP = new double[3];
            double[] minQ = new double[3];
            double mindd;
            int shown_disjoint = 0;
            //
            mindd = VdistV2(S[0], T[0]) + 1;  // set first minimum safely high
            //
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Find closest points on edges i & j, plus the vector (and distance squared) between these points
                    SegPoints(ref VEC, ref P, ref Q, S[i], Sv[i], T[j], Tv[j]);
                    //
                    VmV(ref V, Q, P);
                    double dd = VdotV(V, V);
                    // Verify this closest point pair only if the distance squared is less than the minimum found thus far.
                    if (dd <= mindd)
                    {
                        VcV(minP, P);
                        VcV(minQ, Q);
                        mindd = dd;
                        //
                        VmV(ref Z, S[(i + 2) % 3], P);
                        double a = VdotV(Z, VEC);
                        VmV(ref Z, T[(j + 2) % 3], Q);
                        double b = VdotV(Z, VEC);
                        //
                        if ((a <= 0) && (b >= 0))
                        {
                            if (onlyInternal) return double.MaxValue;
                            else return Math.Sqrt(dd);
                        }
                        //
                        double p = VdotV(V, VEC);
                        //
                        if (a < 0) a = 0;
                        if (b > 0) b = 0;
                        if ((p - a + b) > 0) shown_disjoint = 1;
                    }
                }
            }
            // No edge pairs contained the closest points.  
            // either:
            // 1. one of the closest points is a vertex, and the
            //    other point is interior to a face.
            // 2. the triangles are overlapping.
            // 3. an edge of one triangle is parallel to the other's face. If
            //    cases 1 and 2 are not true, then the closest points from the 9
            //    edge pairs checks above can be taken as closest points for the
            //    triangles.
            // 4. possibly, the triangles were degenerate.  When the 
            //    triangle points are nearly colinear or coincident, one 
            //    of above tests might fail even though the edges tested
            //    contain the closest points.
            //
            // First check for case 1
            double[] Sn = new double[3];
            double Sn2;
            VcrossV(ref Sn, Sv[0], Sv[1]);  // compute normal to S triangle
            Sn2 = VdotV(Sn, Sn);            // compute square of length of normal
            // If cross product is long enough
            if (Sn2 > 1e-15)
            {
                // Get projection lengths of T points
                double[] Tp = new double[3];
                //
                VmV(ref V, S[0], T[0]);
                Tp[0] = VdotV(V, Sn);
                //
                VmV(ref V, S[0], T[1]);
                Tp[1] = VdotV(V, Sn);
                //
                VmV(ref V, S[0], T[2]);
                Tp[2] = VdotV(V, Sn);
                // If Sn is a separating direction, find point with smallest projection
                int point = -1;
                if ((Tp[0] > 0) && (Tp[1] > 0) && (Tp[2] > 0))
                {
                    if (Tp[0] < Tp[1]) point = 0;
                    else point = 1;
                    //
                    if (Tp[2] < Tp[point]) point = 2;
                }
                else if ((Tp[0] < 0) && (Tp[1] < 0) && (Tp[2] < 0))
                {
                    if (Tp[0] > Tp[1]) point = 0;
                    else point = 1;
                    //
                    if (Tp[2] > Tp[point]) point = 2;
                }
                // If point was found
                if (point >= 0)
                {
                    shown_disjoint = 1;
                    // Test whether the point found, when projected onto the other triangle, lies within the face.
                    VmV(ref V, T[point], S[0]);
                    VcrossV(ref Z, Sn, Sv[0]);
                    if (VdotV(V, Z) > 0)
                    {
                        VmV(ref V, T[point], S[1]);
                        VcrossV(ref Z, Sn, Sv[1]);
                        if (VdotV(V, Z) > 0)
                        {
                            VmV(ref V, T[point], S[2]);
                            VcrossV(ref Z, Sn, Sv[2]);
                            if (VdotV(V, Z) > 0)
                            {
                                // T[point] passed the test - it's a closest point for the T triangle;
                                // the other point is on the face of S
                                VpVxS(ref P, T[point], Sn, Tp[point] / Sn2);
                                VcV(Q, T[point]);
                                return Math.Sqrt(VdistV2(P, Q));
                            }
                        }
                    }
                }
            }
            double[] Tn = new double[3];
            double Tn2;
            VcrossV(ref Tn, Tv[0], Tv[1]);  // compute normal to T triangle
            Tn2 = VdotV(Tn, Tn);            // compute square of length of normal
            // If cross product is long enough
            if (Tn2 > 1e-15)
            {
                // Get projection lengths of S points
                double[] Sp = new double[3];
                //
                VmV(ref V, T[0], S[0]);
                Sp[0] = VdotV(V, Tn);
                //
                VmV(ref V, T[0], S[1]);
                Sp[1] = VdotV(V, Tn);
                //
                VmV(ref V, T[0], S[2]);
                Sp[2] = VdotV(V, Tn);
                // If Tn is a separating direction, find point with smallest projection
                int point = -1;
                if ((Sp[0] > 0) && (Sp[1] > 0) && (Sp[2] > 0))
                {
                    if (Sp[0] < Sp[1]) point = 0;
                    else point = 1;
                    //
                    if (Sp[2] < Sp[point]) point = 2;
                }
                else if ((Sp[0] < 0) && (Sp[1] < 0) && (Sp[2] < 0))
                {
                    if (Sp[0] > Sp[1]) point = 0;
                    else point = 1;
                    //
                    if (Sp[2] > Sp[point]) point = 2;
                }
                // If point was found
                if (point >= 0)
                {
                    shown_disjoint = 1;
                    // Test whether the point found, when projected onto the other triangle, lies within the face.
                    VmV(ref V, S[point], T[0]);
                    VcrossV(ref Z, Tn, Tv[0]);
                    if (VdotV(V, Z) > 0)
                    {
                        VmV(ref V, S[point], T[1]);
                        VcrossV(ref Z, Tn, Tv[1]);
                        if (VdotV(V, Z) > 0)
                        {
                            VmV(ref V, S[point], T[2]);
                            VcrossV(ref Z, Tn, Tv[2]);
                            if (VdotV(V, Z) > 0)
                            {
                                // S[point] passed the test - it's a closest point for the S triangle;
                                // the other point is on the face of T
                                VpVxS(ref Q, S[point], Tn, Sp[point] / Tn2);
                                VcV(P, S[point]);
                                return Math.Sqrt(VdistV2(P, Q));
                            }
                        }
                    }
                }
            }
            // Case 1 can't be shown
            // If one of the tests showed the triangles disjoint - not overlaping, we assume case 3
            // If cross product was not long enough, we assume case 4
            if (shown_disjoint == 1)
            {
                VcV(P, minP);
                VcV(Q, minQ);
                if (onlyInternal) return double.MaxValue;
                else return Math.Sqrt(mindd);
            }
            // Otherwise we conclude case 2: the triangles overlap.
            else return 0;
        }
    }
}
