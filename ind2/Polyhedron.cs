using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ind2
{
    public class Polyhedron
    {
        public List<Line> lines;
        public double[] material = new double[5];
        public Point material_color = new Point();

        public Polyhedron()
        {
            this.lines = new List<Line> { };
        }
        public Polyhedron(List<Line> e)
        {
            this.lines = e;
        }

        //пересечение луча с треугольником
        public bool RayIntersectsTriangle(Ray ray, Point p0, Point p1, Point p2, out double intersect)
        {
            var eps = 0.0001;
            // ребра треугольника
            Point edge1 = p1 - p0;
            Point edge2 = p2 - p0;
            Point h = ray.direction * edge2;
            double a = Form1.Cross(edge1, h);
            intersect = -1;

            // луч параллелен плоскости треугольника
            if (a > -eps && a < eps)
                return false;

            Point s = ray.start - p0;
            double u = Form1.Cross(s, h) / a;
            if (u < 0 || u > 1)
                return false;

            Point q = s * edge1;
            double v = Form1.Cross(ray.direction, q) / a;
            if (v < 0 || u + v > 1)
                return false;

            double t = Form1.Cross(edge2, q) / a;
            //пересечение луча с треугольником
            if (t <= eps)
                return false;

            intersect = t;
            return true;
        }

        //пересечение фигуры с лучом
        public virtual bool FigureIntersection(Ray r, out double intersect, out Point normal)
        {
            intersect = 0;
            normal = null;
            Line side = null;
            foreach (var figure_side in lines)
            {
                //треугольная сторона
                if (figure_side.points.Count == 3)
                {
                    //пересечение луча с треугольником
                    if (RayIntersectsTriangle(r, figure_side.points[0], figure_side.points[1], figure_side.points[2], out double t) && (intersect == 0 || t < intersect))
                    {
                        intersect = t;
                        side = figure_side;
                    }
                }

                //четырехугольная сторона
                else if (figure_side.points.Count == 4)
                {
                    if (RayIntersectsTriangle(r, figure_side.points[0], figure_side.points[1], figure_side.points[3], out double t) && (intersect == 0 || t < intersect))
                    {
                        intersect = t;
                        side = figure_side;
                    }
                    else if (RayIntersectsTriangle(r, figure_side.points[1], figure_side.points[2], figure_side.points[3], out t) && (intersect == 0 || t < intersect))
                    {
                        intersect = t;
                        side = figure_side;
                    }
                }
            }
            if (intersect != 0)
            {
                normal = Form1.norm(side);
                material_color = new Point(side.color.R / 255f, side.color.G / 255f, side.color.B / 255f);
                return true;
            }
            return false;
        }

        public static Polyhedron Hex(int size)
        {
            var hc = size / 2;
            Polyhedron p = new Polyhedron();
            Line l = new Line();
           
            // 1-2-3-4
            l.points = new List<Point> {
                new Point(-hc, hc, -hc), // 1
                new Point(hc, hc, -hc), // 2
                new Point(hc, -hc, -hc), // 3
                new Point(-hc, -hc, -hc) // 4
            };
            p.lines.Add(l);
            l = new Line();

            // 1-2-6-5
            l.points = new List<Point> {
                new Point(-hc, hc, -hc), // 1
                new Point(-hc, hc, hc), // 5
                new Point(hc, hc, hc), // 6 
                new Point(hc, hc, -hc) // 2
            };
            p.lines.Add(l);
            l = new Line();

            // 5-6-7-8
            l.points = new List<Point> {
                new Point(-hc, hc, hc), // 5
                new Point(-hc, -hc, hc), // 8
                new Point(hc, -hc, hc), // 7
                new Point(hc, hc, hc) // 6 
            };
            p.lines.Add(l);
            l = new Line();

            // 6-2-3-7
            l.points = new List<Point> {
                new Point(hc, hc, hc), // 6 
                new Point(hc, -hc, hc), // 7
                new Point(hc, -hc, -hc), // 3
                new Point(hc, hc, -hc) // 2
            };
            p.lines.Add(l);
            l = new Line();

            // 5-1-4-8
            l.points = new List<Point> {
                new Point(-hc, hc, hc), // 5
                new Point(-hc, hc, -hc), // 1
                new Point(-hc, -hc, -hc), // 4
                new Point(-hc, -hc, hc) // 8
            };
            p.lines.Add(l   );
            l = new Line();

            // 4-3-7-8
            l.points = new List<Point> {
                new Point(-hc, -hc, -hc), // 4
                new Point(hc, -hc, -hc), // 3
                new Point(hc, -hc, hc), // 7
                new Point(-hc, -hc, hc) // 8
            };
            p.lines.Add(l);
            l = new Line();

            return p;

        }
      

    }
}
