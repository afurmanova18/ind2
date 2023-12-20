using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ind2
{
    public class Light
    {
        public Point start_point;//источник
        public Point color_light;//цвет источника

        public Light(Point p, Point c)
        {
            start_point = p;
            color_light = c;
        }

        public Point Shade(Point hit_point, Point normal, Point material_color, double diffuse_coef)
        {
            Point dir = start_point - hit_point;
            dir = Form1.Normalize(dir);//нормализуем вектор направления
            Point diff;
            if (Form1.Cross(normal, dir) > 0)//меньше 90 гр
                diff = diffuse_coef * color_light * Form1.Cross(normal, dir);
            else
                diff = diffuse_coef * color_light * 0.5;
            return new Point(diff.x * material_color.x, diff.y * material_color.y, diff.z * material_color.z);
        }
    }
}
