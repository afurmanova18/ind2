using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ind2
{
    public partial class Form1 : Form
    {
        static Bitmap bmp;
        List<Polyhedron> All_figures = new List<Polyhedron>();
        public int height, width;
        public Color[,] colors;
        public Point cameraPoint = new Point();
        public Light light;
        public Point[,] pixels;
        int line_num = 5;
        public Form1()
        {
            InitializeComponent();
            height = pictureBox1.Height;
            width = pictureBox1.Width;
            bmp = new Bitmap(width, height);
            colors = new Color[width, height];
            pixels = new Point[width, height];

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeBitmap();
            CreatePixelMap();
            RenderImage();
            DisplayImage();
        }

        private void InitializeBitmap()
        {
            bmp = new Bitmap(width, height);
            colors = new Color[width, height];
            pixels = new Point[width, height];
            light = new Light(new Point(0f, -3f, 1.9f), new Point(1f, 1f, 1f));
            cameraPoint = new Point(0, -30, 0);
            All_figures = new List<Polyhedron>();
            Set_figures();
        }

        private void RenderImage()
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    Ray r = new Ray(cameraPoint, pixels[i, j]);
                    r.start = new Point(pixels[i, j]);
                    Point color = RayTrace(r, 10, 1);

                    // Нормализация цвета
                    if (color.x > 1.0f)
                    {
                        color.x = 1.0f;
                    }
                    if (color.y > 1.0f)
                    {
                        color.y = 1.0f;
                    }
                    if (color.z > 1.0f)
                    {
                        color.z = 1.0f;
                    }

                    colors[i, j] = Color.FromArgb((int)(255 * color.x), (int)(255 * color.y), (int)(255 * color.z));
                }
            }
        }

        private void DisplayImage()
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    bmp.SetPixel(i, j, colors[i, j]);
                }
            }
            pictureBox1.Image = bmp;
        }

        public void CreatePixelMap()
        {
            var points = All_figures[line_num].lines[0].points;

            Point dir1 = (points[1] - points[0]) / (width - 1);
            Point dir2 = (points[2] - points[3]) / (width - 1);
            Point start1 = points[0];
            Point start2 = points[3];

            for (int i = 0; i < width; ++i)
            {
                Point sideDir = (start2 - start1) / (height - 1);
                Point current1 = start1;

                for (int j = 0; j < height; ++j)
                {
                    pixels[i, height - 1 - j] = current1;
                    current1 += sideDir;
                }

                start1 += dir1;
                start2 += dir2;
            }
        }

        public static double[,] MultiplyMatrix(double[,] m1, double[,] m2)
        {
            double[,] m = new double[1, 4];

            for (int i = 0; i < 4; i++)
            {
                var temp = 0.0;
                for (int j = 0; j < 4; j++)
                {
                    temp += m1[0, j] * m2[j, i];
                }
                m[0, i] = temp;
            }
            return m;
        }

        public static Polyhedron move(Polyhedron poly, double posx, double posy, double posz)
        {
            Polyhedron movedPolyhedron = new Polyhedron();
            foreach (var line in poly.lines)
            {
                Line movedLines = new Line();
                foreach (var point in line.points)
                {
                    double[,] m = new double[1, 4];
                    m[0, 0] = point.x;
                    m[0, 1] = point.y;
                    m[0, 2] = point.z;
                    m[0, 3] = 1;

                    double[,] matr = new double[4, 4]
                {   { 1, 0, 0, 0},
                    { 0, 1, 0, 0 },
                    {0, 0, 1, 0 },
                    { posx, -posy, posz, 1 } };

                    var res = MultiplyMatrix(m, matr);

                    movedLines.points.Add(new Point(res[0, 0], res[0, 1], res[0, 2]));
                }
                movedPolyhedron.lines.Add(movedLines);

            }
            return movedPolyhedron;
        }


        public static Polyhedron rotate(Polyhedron poly, double x_angle, double y_angle, double z_angle)
        {
            Polyhedron rotatedPolyhedron = new Polyhedron();
            foreach (var line in poly.lines)
            {
                Line rotatedLines = new Line();
                foreach (var point in line.points)
                {
                    double[,] m = new double[1, 4];
                    m[0, 0] = point.x;
                    m[0, 1] = point.y;
                    m[0, 2] = point.z;
                    m[0, 3] = 1;

                    var angle = x_angle * Math.PI / 180;
                    double[,] matrx = new double[4, 4]
                {   { Math.Cos(angle), 0, Math.Sin(angle), 0},
                    { 0, 1, 0, 0 },
                    {-Math.Sin(angle), 0, Math.Cos(angle), 0 },
                    { 0, 0, 0, 1 } };

                    angle = y_angle * Math.PI / 180;
                    double[,] matry = new double[4, 4]
                    {  { 1, 0, 0, 0 },
                    { 0, Math.Cos(angle), -Math.Sin(angle), 0},
                    {0, Math.Sin(angle), Math.Cos(angle), 0 },
                    { 0, 0, 0, 1 } };

                    angle = z_angle * Math.PI / 180;
                    double[,] matrz = new double[4, 4]
                    {  { Math.Cos(angle), -Math.Sin(angle), 0, 0},
                    { Math.Sin(angle), Math.Cos(angle), 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 } };

                    var res = MultiplyMatrix(m, matrx);
                    res = MultiplyMatrix(res, matry);
                    res = MultiplyMatrix(res, matrz);

                    rotatedLines.points.Add(new Point(res[0, 0], res[0, 1], res[0, 2]));
                }
                rotatedPolyhedron.lines.Add(rotatedLines);
            }
            return rotatedPolyhedron;
        }

        public void Add_wall(int wall_number, Color color, bool f)
        {
            Polyhedron wall = new Polyhedron(new List<Line>() { Polyhedron.Hex(10).lines[wall_number] });
            wall.lines[0].color = color;

            if (f)
            {
                wall.material = new double[] { 1, 0, 0.05, 0.7, 1 };
            }
            else
                wall.material = new double[] { 0, 0, 0.05, 0.7, 1 };

            All_figures.Add(wall);

        }
        public void Set_figures()
        {
            Add_wall(0, Color.GreenYellow,false);//пол
            if(checkBox6.Checked)
                Add_wall(1, Color.MediumSeaGreen, true);//передняя
            else
                Add_wall(1, Color.MediumSeaGreen, false);//передняя
            Add_wall(2, Color.White,false);//потолок
            if(checkBox3.Checked)
                Add_wall(3, Color.DeepSkyBlue, true);//правая
            else
                Add_wall(3, Color.DeepSkyBlue, false);

            if (checkBox5.Checked)
                Add_wall(4, Color.HotPink, true);//левая
            else
                Add_wall(4, Color.HotPink, false);//левая
            if (checkBox4.Checked)
                Add_wall(5, Color.Black, true);//задняя
            else
                Add_wall(5, Color.Black, false);//задняя



            Polyhedron cube = Polyhedron.Hex(3);
            cube.lines = move(cube, 3, 0, -4).lines;
            if (checkBox1.Checked)
                cube.material = new double[] { 1, 0, 0.3f, 0.7f, 1f };
            else
                cube.material = new double[] { 0, 0, 0.1, 0.7, 1.5 };
            foreach (var x in cube.lines)
                x.color = Color.LightSalmon;
            All_figures.Add(cube);


            Polyhedron cube2 = Polyhedron.Hex(5);
            cube2.lines = rotate(cube2, 0, 0, -20).lines;
            cube2.lines = move(cube2, -1.5, 2, -4).lines;
            if (checkBox2.Checked)
                cube2.material = new double[] { 1, 0, 0.3f, 0.7f, 1f };
            else
                cube2.material = new double[] { 0, 0, 0.1, 0.7, 1.5 };
            foreach (var x in cube2.lines)
                x.color = Color.PapayaWhip;
            All_figures.Add(cube2);

        }


        public static Point Normalize(Point vector)
        {
            double length = vector.Length();
            if (length > 0)
            {
                return new Point(vector.x / length, vector.y / length, vector.z / length);
            }
            else
            {
                return new Point(0, 0, 0);
            }
        }


        public static Point norm(Line S)
        {
            if (S.points.Count() < 3)
                return new Point(0, 0, 0);
            Point U = S.points[1] - S.points[0];
            Point V = S.points[S.points.Count - 1] - S.points[0];
            Point normal = U * V;


            return Normalize(normal);
        }

        public Point RayTrace(Ray r, int iter, double env)
        {
            if (iter <= 0)
                return new Point(0, 0, 0);
            double shortest_intersect = 0;//точка пересечения,ближайшая
            Point normal = null;
            double[] material = new double[5];
            Point material_color = new Point();
            Point res_color = new Point(0, 0, 0);
            bool isAngleSharp = false;

            foreach (var fig in All_figures)
            {
                //проверяем пересение луча с фигурогй
                if (fig.FigureIntersection(r, out double intersect, out Point norm))
                    if (intersect < shortest_intersect || shortest_intersect == 0)
                    {
                        shortest_intersect = intersect;
                        normal = norm;
                        material = fig.material;
                        material_color = fig.material_color;
                    }
            }

            if (shortest_intersect == 0)//если не пересекается с фигурой
                return new Point(0, 0, 0);

            if (Cross(r.direction, normal) > 0)
            {
                normal *= -1;
                isAngleSharp = true;
            }

            //Точка пересечения луча с фигурой
            Point hit_point = r.start + r.direction * shortest_intersect;


            Point ambient_coef = light.color_light * material[2];
            ambient_coef.x = (ambient_coef.x * material_color.x);
            ambient_coef.y = (ambient_coef.y * material_color.y);
            ambient_coef.z = (ambient_coef.z * material_color.z);
            res_color += ambient_coef;
            // диффузное освещение
            if (IsVisible(light.start_point, hit_point))//виден ли источник света с данной точки
                res_color += light.Shade(hit_point, normal, material_color, material[3]);
            else
                res_color += light.Shade(hit_point, normal, material_color, material[3]) / 5 * 3;//если нет освещение уменьшается


            if (material[0] > 0)//коэффициент отражения больше 0
            {
                Ray reflected_ray = r.Reflect(hit_point, normal);//отраженный луч
                res_color = material[0] * RayTrace(reflected_ray, iter - 1, env);
            }

            if (material[1] > 0) // если коэф преломления
            {
                double refract_coef;
                if (isAngleSharp)
                    refract_coef = material[4];
                else
                    refract_coef = 1 / material[4];

                Ray refracted_ray = r.Refract(hit_point, normal, material[1], refract_coef);//преломленный луч

                if (refracted_ray != null)
                    res_color = material[1] * RayTrace(refracted_ray, iter - 1, material[4]);
            }
            return res_color;
        }

        public static double Cross(Point p1, Point p2)
        {
            return p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
        }

        public bool IsVisible(Point light_point, Point hit_point)
        {
            double distance = (light_point - hit_point).Length(); // длина вектора от точки до источника
            Ray r = new Ray(hit_point, light_point);
            foreach (var fig in All_figures)
                if (fig.FigureIntersection(r, out double t, out Point n))
                    if (t < distance && t > 0.0001)
                        return false;
            return true;
        }
    }

}
