using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ind2
{
    public class Ball : Polyhedron
    {
        double radius;
        static double eps = 0.0001;
        public Ball(Point p, double r)
        {
            this.lines.Add(new Line(new List<Point>()));
            this.lines[0].points.Add(p);
            radius = r;
        }

        public static bool RaySphereIntersection(Ray r, Point sphere_pos, double sphere_rad, out double t)
        {
            Point k = r.start - sphere_pos;
            double b = Form1.Cross(k, r.direction);
            double c = Form1.Cross(k, k) - sphere_rad * sphere_rad;
            double d = b * b - c;
            t = 0;
            if (d >= 0)//луч пересекает сферу или касается
            {
                double sqrtd = Math.Sqrt(d);
                double t1 = -b + sqrtd;
                double t2 = -b - sqrtd;

                double minT = Math.Min(t1, t2); //мин решений
                double maxT = Math.Max(t1, t2);// макс из решений

                if (minT > eps)
                {
                    t = minT;
                }
                else
                {
                    t = maxT;
                }

                return t > eps;
            }
            return false;
        }

        public override bool FigureIntersection(Ray r, out double t, out Point normal)
        {
            t = 0;
            normal = null;
            if (RaySphereIntersection(r, lines[0].points[0], radius, out t) && (t > eps))
            {
                normal = (r.start + r.direction * t) - lines[0].points[0];
                normal = Form1.Normalize(normal);
                return true;
            }
            return false;
        }
    }
}
