using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ind2
{
    public class Ray
    {
        public Point start; // нач точка
        public Point direction;//направление

        public Ray(Point st, Point end)
        {
            start = st;
            direction = Form1.Normalize(end - st);
        }

        public Ray() { }

        public Ray(Ray r)
        {
            start = r.start;
            direction = r.direction;
        }

        public Ray Reflect(Point hit_point, Point normal)
        {
            Point reflect_dir = direction - 2 * normal * Form1.Cross(direction, normal);
            return new Ray(hit_point, hit_point + reflect_dir);
        }

        public Ray Refract(Point hit_point, Point normal, double refraction, double coef)
        {
            Ray new_ray = new Ray();
            new_ray.start = new Point(hit_point);

            double scalar = Form1.Cross(normal, direction);
            double refract_result = refraction / coef;
            double theta_formula = 1 - refract_result * refract_result * (1 - scalar * scalar);
            if (theta_formula >= 0)
            {
                double cos_theta = Math.Sqrt(theta_formula);
                new_ray.direction = Form1.Normalize(direction * refract_result - (cos_theta + refract_result * scalar) * normal);//направление луча
                return new_ray;
            }
            else //луч не преломляется
                return null;
        }
    }

}
