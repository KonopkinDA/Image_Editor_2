using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Point[] linePoints;
        List<Point> lineSpecialPoints;
        List<Point> tempPointList;
        bool isLine = true;
        bool isClick = false;
        int x, y;
        SplineTuple[] splines;

        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
            lineSpecialPoints = new List<Point>();
            tempPointList = new List<Point>();
            comboBox1.Items.Add("Линейная");
            comboBox1.Items.Add("Кубическая");
            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox1.SelectedIndexChanged += InterpolationChange;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                Bitmap imageT = new Bitmap(openFileDialog.FileName);
                image = new Bitmap(imageT, pictureBox1.Width, pictureBox1.Height);
                imageT.Dispose();
                pictureBox1.Image = image;
            }

            while (lineSpecialPoints.Count > 0)
            {
                lineSpecialPoints.RemoveAt(0);
            }

            while (tempPointList.Count > 0)
            {
                tempPointList.RemoveAt(0);
            }

            Graphics g = panel1.CreateGraphics();
            for (int i = 0; i < panel1.Height; i++)
            {
                g.DrawLine(new Pen(Color.White, 10), new Point(0, i), new Point(panel1.Width, i));
            }

            linePoints = new Point[panel1.Width];

            for (int i = 0; i < linePoints.Length; i++)
            {
                linePoints[i].X = i;
                linePoints[i].Y = 255- i;
            }

            lineSpecialPoints.Add(linePoints[0]);
            lineSpecialPoints.Add(linePoints[linePoints.Length - 1]);

            g.DrawLine(new Pen(Color.Black, 2), linePoints[0], linePoints[linePoints.Length-1]);

            DrawGistogramm();

        }

        //Сохранение
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDileFialog = new SaveFileDialog();
            saveDileFialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveDileFialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            saveDileFialog.RestoreDirectory = true;
            image = (Bitmap)pictureBox1.Image;

            if (saveDileFialog.ShowDialog() == DialogResult.OK)
            {
                if (image != null)
                {
                    image.Save(saveDileFialog.FileName);
                }
            }
        }

        //Прорисовка Гистограммы
        public void DrawGistogramm()
        {
            int[] gistoLevels = new int[256];

            Graphics g2 = panel2.CreateGraphics();
            for (int i = 0; i < panel2.Height; i++)
            {
                g2.DrawLine(new Pen(Color.White, 10), new Point(0, i), new Point(panel2.Width, i));
            }

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    gistoLevels[(int)((image.GetPixel(i, j).R + image.GetPixel(i, j).G + image.GetPixel(i, j).B) / 3)] += 1;
                }
            }

            int max = 0;
            for (int i = 0; i < gistoLevels.Length; i++)
            {
                if (gistoLevels[i] > max)
                {
                    max = gistoLevels[i];
                }
            }

            float q = panel1.Height / (float)max;
            for (int i = 0; i < gistoLevels.Length; i++)
            {
                gistoLevels[i] = (int)(gistoLevels[i] * q);
            }

            for (int k = 0; k < 256; k++)
            {
                g2.DrawLine(new Pen(Color.Black, 1), new Point(k, panel2.Height), new Point(k, panel2.Height - gistoLevels[k]));
            }
        }

        public void DrawGistogramm(Bitmap image2)
        {
            int[] gistoLevels = new int[256];

            Graphics g2 = panel2.CreateGraphics();
            for (int i = 0; i < panel2.Height; i++)
            {
                g2.DrawLine(new Pen(Color.White, 10), new Point(0, i), new Point(panel2.Width, i));
            }

            for (int i = 0; i < image2.Width; i++)
            {
                for (int j = 0; j < image2.Height; j++)
                {
                    gistoLevels[(int)((image2.GetPixel(i, j).R + image2.GetPixel(i, j).G + image2.GetPixel(i, j).B) / 3)] += 1;
                }
            }

            int max = 0;
            for (int i = 0; i < gistoLevels.Length; i++)
            {
                if (gistoLevels[i] > max)
                {
                    max = gistoLevels[i];
                }
            }

            float q = panel1.Height / (float)max;
            for (int i = 0; i < gistoLevels.Length; i++)
            {
                gistoLevels[i] = (int)(gistoLevels[i] * q);
            }

            for (int k = 0; k < 256; k++)
            {
                g2.DrawLine(new Pen(Color.Black, 1), new Point(k, panel2.Height), new Point(k, panel2.Height - gistoLevels[k]));
            }
        }


        //События мыши для интерполяции
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClick)
            {
                Graphics g = panel1.CreateGraphics();
                Point newPoint;
                

                x = (int)(MousePosition.X - this.Location.X - panel1.Location.X - 14);
                y = (int)(MousePosition.Y - this.Location.Y - panel1.Location.Y - 35);

                newPoint = new Point(x, y);

                for (int i = 0; i < panel1.Height; i++)
                {
                    g.DrawLine(new Pen(Color.White, 10), new Point(0, i), new Point(panel1.Width, i));
                }

                while (tempPointList.Count > 0)
                {
                    tempPointList.RemoveAt(0);
                }

                bool isAdded = false;
                foreach (var item in lineSpecialPoints)
                {
                    if (newPoint.X > item.X)
                    {
                        tempPointList.Add(item);
                    }
                    else
                    {
                        if (!isAdded)
                        {
                            tempPointList.Add(newPoint);
                            isAdded = true;
                        }
                        tempPointList.Add(item);
                    }
                }

                foreach (var item in tempPointList)
                {
                    Rectangle rect = new Rectangle(item.X, item.Y, 5, 5);
                    g.DrawEllipse(new Pen(Color.Red, 5), rect);
                }

                if (isLine)
                {
                    //Отрисовка при интерполяции линиями
                    for (int i = 0; i < tempPointList.Count - 1; i++)
                    {
                        g.DrawLine(new Pen(Color.Black, 3), tempPointList[i], tempPointList[i + 1]);

                    }
                }
                else
                {
                    //Отрисовка при интерполяции сплайном
                    Point[] tempPointCuve = new Point[linePoints.Length];
                    InterpolateCube(tempPointList).CopyTo(tempPointCuve, 0);

                    g.DrawCurve(new Pen(Color.Black, 3), tempPointCuve);
                }

            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isClick = false;

            Graphics g = panel1.CreateGraphics();

            while (lineSpecialPoints.Count > 0)
            {
                lineSpecialPoints.RemoveAt(0);
            }

            while (tempPointList.Count > 0)
            {
                lineSpecialPoints.Add(tempPointList[0]);
                tempPointList.RemoveAt(0);
            }

            for (int i = 0; i < panel1.Height; i++)
            {
                g.DrawLine(new Pen(Color.White, 10), new Point(0, i), new Point(panel1.Width, i));
            }


            foreach (var item in lineSpecialPoints)
            {
                Rectangle rect = new Rectangle(item.X, item.Y, 5, 5);
                g.DrawEllipse(new Pen(Color.Red, 5), rect);
            }

            if (isLine)
            {
                //Интерполяция линиями и отрисовка
                InterpolateLine();
                for (int i = 0; i < lineSpecialPoints.Count - 1; i++)
                {
                    g.DrawLine(new Pen(Color.Black, 3), lineSpecialPoints[i], lineSpecialPoints[i + 1]);
                }
            }
            else
            {
                //Интерполяция сплайном и отрисовка
                InterpolateCube(lineSpecialPoints).CopyTo(linePoints, 0);
                g.DrawCurve(new Pen(Color.Black, 3), linePoints);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isClick = true;

            Graphics g = panel1.CreateGraphics();
            Point newPoint;

            x = (int)(MousePosition.X - this.Location.X - panel1.Location.X - 14);
            y = (int)(MousePosition.Y - this.Location.Y - panel1.Location.Y - 35);

            bool isAdded = false;



            newPoint = new Point(x, y);

            for (int i = 0; i < panel1.Height; i++)
            {
                g.DrawLine(new Pen(Color.White, 10), new Point(0, i), new Point(panel1.Width, i));
            }

            foreach(var item in lineSpecialPoints)
            {
                if (newPoint.X > item.X)
                {
                    tempPointList.Add(item);
                }
                else
                {
                    if (!isAdded)
                    {
                        tempPointList.Add(newPoint);
                        isAdded = true;
                    }
                    tempPointList.Add(item);
                }
            }

            if (isLine)
            {
                //Начальная отрисовка линии
                for (int i = 0; i < tempPointList.Count - 1; i++)
                {
                    g.DrawLine(new Pen(Color.Black, 3), tempPointList[i], tempPointList[i + 1]);

                }
            }
            else
            {
                //Начальная отрисовка сплайна
                Point[] tempPointCuve = new Point[linePoints.Length];
                InterpolateCube(tempPointList).CopyTo(tempPointCuve, 0);

                g.DrawCurve(new Pen(Color.Black, 3), tempPointCuve);
            }
        }


        //Интерполяция
        private struct SplineTuple
        {
            public double a, b, c, d, x;
        }

        public void InterpolateLine()
        {
            Point curPoint = new Point();
            Point curPoint2 = new Point();
            int k = 0;
            curPoint = lineSpecialPoints[k];
            curPoint2 = lineSpecialPoints[k + 1];
            k++;
            for (int i = 0; i < linePoints.Length; i++)
            {
                if (linePoints[i].X < curPoint2.X)
                {
                    float a = curPoint.Y;
                    float b = (curPoint2.Y - curPoint.Y);
                    float c = (curPoint2.X - curPoint.X);
                    float d = (linePoints[i].X - curPoint.X);
                    int ny = (int)(a + (b / c) * d);
                    linePoints[i] = new Point(linePoints[i].X, (int)ny);
                }
                else
                {
                    float a = curPoint.Y;
                    float b = (curPoint2.Y - curPoint.Y);
                    float c = (curPoint2.X - curPoint.X);
                    float d = (linePoints[i].X - curPoint.X);
                    int ny = (int)(a + (b / c) * d);
                    linePoints[i] = new Point(linePoints[i].X, (int)ny);
                    if (k < lineSpecialPoints.Count - 1)
                    {
                        curPoint = lineSpecialPoints[k];
                        curPoint2 = lineSpecialPoints[k + 1];
                        k++;
                    }
                }
            }
        }

        public Point[] InterpolateCube(List<Point> nsp)
        {
            Point[] np = new Point[linePoints.Length];

            double[] ax = new double[nsp.Count];
            double[] ay = new double[nsp.Count];
            for(int i = 0; i < nsp.Count; i++)
            {
                ax[i] = nsp[i].X;
                ay[i] = nsp[i].Y;
            }

            BuildSpline(ax, ay, nsp.Count);

            for(int i = 0; i < linePoints.Length; i++)
            {
                double a = Convert.ToDouble(linePoints[i].X);
                np[i].X = linePoints[i].X;
                np[i].Y = (int)Interpolate(Convert.ToDouble(linePoints[i].X));
                if (np[i].Y > 255)
                {
                    np[i].Y = 255;
                }
                if (np[i].Y < 0)
                {
                    np[i].Y = 0;
                }
            }

            return np;
        }

        public void BuildSpline(double[] x, double[] y, int n)
        {
            splines = new SplineTuple[n];
            for (int i = 0; i < n; ++i)
            {
                splines[i].x = x[i];
                splines[i].a = y[i];
            }
            splines[0].c = splines[n - 1].c = 0.0;

            double[] alpha = new double[n - 1];
            double[] beta = new double[n - 1];
            alpha[0] = beta[0] = 0.0;
            for (int i = 1; i < n - 1; ++i)
            {
                double hi = x[i] - x[i - 1];
                double hi1 = x[i + 1] - x[i];
                double A = hi;
                double C = 2.0 * (hi + hi1);
                double B = hi1;
                double F = 6.0 * ((y[i + 1] - y[i]) / hi1 - (y[i] - y[i - 1]) / hi);
                double z = (A * alpha[i - 1] + C);
                alpha[i] = -B / z;
                beta[i] = (F - A * beta[i - 1]) / z;
            }

            for (int i = n - 2; i > 0; --i)
            {
                splines[i].c = alpha[i] * splines[i + 1].c + beta[i];
            }

            for (int i = n - 1; i > 0; --i)
            {
                double hi = x[i] - x[i - 1];
                splines[i].d = (splines[i].c - splines[i - 1].c) / hi;
                splines[i].b = hi * (2.0 * splines[i].c + splines[i - 1].c) / 6.0 + (y[i] - y[i - 1]) / hi;
            }
        }

        public double Interpolate(double x)
        {
            if (splines == null)
            {
                return double.NaN; 
            }

            int n = splines.Length;
            SplineTuple s;

            if (x <= splines[0].x) 
            {
                s = splines[0];
            }
            else if (x >= splines[n - 1].x) 
            {
                s = splines[n - 1];
            }
            else 
            {
                int i = 0;
                int j = n - 1;
                while (i + 1 < j)
                {
                    int k = i + (j - i) / 2;
                    if (x <= splines[k].x)
                    {
                        j = k;
                    }
                    else
                    {
                        i = k;
                    }
                }
                s = splines[j];
            }

            double xD = x - s.x;
            return s.a + (s.b + (s.c / 2.0 + s.d * xD / 6.0) * xD) * xD;
        }

        public void InterpolationChange(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                isLine = true;
            }
            else
            {
                isLine = false;
            }
        }



        private void panel1_Click(object sender, EventArgs e)
        {
            //isClick = true;

        }

        private void button2_Click(object sender, EventArgs e)

        {
            Bitmap image2 = new Bitmap(image);

            for (int i = 0; i < image2.Width; i++)
            {
                for (int j = 0; j < image2.Height; j++)
                {
                    int s = (int)((image2.GetPixel(i, j).R + image2.GetPixel(i, j).G + image2.GetPixel(i, j).B) / 3);
                    for (int k = 1; k < linePoints.Length; k++)
                    {
                        if (s == linePoints[k].X)
                        {
                            double o1, o2;
                            o1 = 255 - linePoints[k].Y;
                            o2 = linePoints[k].X;
                            double o = o1 / o2;
                            int r = (int)(255 * (Convert.ToDouble(image2.GetPixel(i, j).R) / 255.0 * o));
                            int g = (int)(255 * (Convert.ToDouble(image2.GetPixel(i, j).G) / 255.0 * o));
                            int b = (int)(255 * (Convert.ToDouble(image2.GetPixel(i, j).B) / 255.0 * o));
                            if (r > 255)
                            {
                                r = 255;
                            }
                            if (g > 255)
                            {
                                g = 255;
                            }
                            if (b > 255)
                            {
                                b = 255;
                            }
                            if (r < 0)
                            {
                                r = 0;
                            }
                            if (g < 0)
                            {
                                g = 0;
                            }
                            if (b < 0)
                            {
                                b = 0;
                            }
                            Color c = Color.FromArgb(image2.GetPixel(i, j).A, r, g, b);
                            image2.SetPixel(i, j, c);
                            break;
                        }
                    }
                }
            }

            pictureBox1.Image = image2;

            DrawGistogramm(image2);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
