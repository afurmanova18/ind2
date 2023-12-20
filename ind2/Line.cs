using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ind2
{
    public class Line
    {
        public List<Point> points;
        public Color color;
        public Line()
        {
            this.points = new List<Point> { };
        }
        public Line(List<Point> p)
        {
            this.points = p;
        }
    }
}
