using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Database.Model;
using EscapeFromTheWoods.Database.exceptions;

namespace EscapeFromTheWoods
{
    public class Map
    {
        public Map(int xmin, int xmax, int ymin, int ymax, string path)
        {
            Xmin = xmin;
            Xmax = xmax;
            Ymin = ymin;
            Ymax = ymax;
            this.path = path;

        }
        private const int drawingFactor = 8;
        private string path;
        public int Xmin { get; set; }
        public int Xmax { get; set; }
        public int Ymin { get; set; }
        public int Ymax { get; set; }

        public async void WriteEscaperoutesToBitmap(List<List<Tree>> routes, List<Tree> trees,string woodId)
        {
            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((Xmax - Xmin) * drawingFactor, (Ymax - Ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);

            List<Task> ellipseTasks = new List<Task>();
            foreach (Tree t in trees)
            {
                ellipseTasks.Add(DrawEllipseAsync(g, p, t.X * drawingFactor, t.Y * drawingFactor, drawingFactor, drawingFactor));
            }
            await Task.WhenAll(ellipseTasks);

            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].X * drawingFactor + delta;
                int p1y = route[0].Y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);

                List<Task> lineTasks = new List<Task>();
                for (int i = 1; i < route.Count; i++)
                {
                    lineTasks.Add(DrawLineAsync(g, pen, p1x, p1y, route[i].X * drawingFactor + delta, route[i].Y * drawingFactor + delta));
                    p1x = route[i].X * drawingFactor + delta;
                    p1y = route[i].Y * drawingFactor + delta;
                }
                await Task.WhenAll(lineTasks);

                colorN++;
            }
            bm.Save(Path.Combine(path, woodId.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
        }
        private async Task DrawEllipseAsync(Graphics graphics, Pen pen, int x, int y, int width, int height)
        {
            graphics.DrawEllipse(pen, x, y, width, height);
        }
        private async Task DrawLineAsync(Graphics graphics, Pen pen, int x1, int y1, int x2, int y2)
        {
            graphics.DrawLine(pen, x1, y1, x2, y2);
        }

    }
}
