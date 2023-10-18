using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using OpenCvSharp;
using Point = System.Windows.Point;

namespace WpfApp3
{
    internal class ellipseData
    {

        private Canvas canvas;
        private List<Ellipse> ellipseList = new List<Ellipse>();
        private List<Line> lineList = new List<Line>();
        private List<Point> pointList = new List<Point>();
        TextBlock plain = new TextBlock();

        public bool isDrawing = false;
        public Polygon polygon;

        public Point centroid;

        public ellipseData(double x, double y, Canvas canvas)
        {
            this.canvas = canvas;
            createEllipse(x, y);
            isDrawing = true;

        }







        public void createEllipse(double x, double y)
        {
            
            if(ellipseList.Count > 0)
            {
                
                Ellipse previousEllipse = ellipseList[0];
                double distance = CalculateDistance(
                    Canvas.GetLeft(previousEllipse) + previousEllipse.Width / 2,
                    Canvas.GetTop(previousEllipse) + previousEllipse.Height / 2,
                    x,
                    y);


                if (distance < 50) // 일정 거리 미만인 경우에만 선을 그리고 plain을 추가합니다
                {

                    isDrawing = false;
                    Line line = new Line
                    {
                        Stroke = Brushes.LightBlue,
                        StrokeThickness = 2
                    };

                    line.X1 = Canvas.GetLeft(ellipseList[ellipseList.Count - 1]) + 5;
                    line.Y1 = Canvas.GetTop(ellipseList[ellipseList.Count - 1]) + 5;
                    line.X2 = Canvas.GetLeft(ellipseList[0]) + 5;
                    line.Y2 = Canvas.GetTop(ellipseList[0]) + 5;
                    canvas.Children.Add(line);

                    lineList.Add(line);
                    Debug.WriteLine("Create Line");


                    
                    TextBlock plain = new TextBlock
                    {
                        Text = "Plain",
                        Foreground = Brushes.Black,
                        FontSize = 12
                    };
                    this.plain = plain;
                    Canvas.SetLeft(plain, x);
                    Canvas.SetTop(plain, y);
                    canvas.Children.Add(plain);

                    DrawPolygon(ellipseList);

                    GetPolygonCenter(polygon);


                    Debug.WriteLine("Create Line and Plain");
                    return;




                }
            }



            Ellipse ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.LightBlue
            };
            ellipseList.Add(ellipse);
            Canvas.SetLeft(ellipse, x-0.5);
            Canvas.SetTop(ellipse, y-0.5);
            canvas.Children.Add(ellipse);

            if (ellipseList.Count > 1)
            {
                Line line = new Line
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2
                };

                line.X1 = Canvas.GetLeft(ellipseList[ellipseList.Count - 2]) + 5;
                line.Y1 = Canvas.GetTop(ellipseList[ellipseList.Count - 2]) + 5;
                line.X2 = Canvas.GetLeft(ellipseList[ellipseList.Count - 1]) + 5;
                line.Y2 = Canvas.GetTop(ellipseList[ellipseList.Count - 1]) + 5;
                canvas.Children.Add(line);

                lineList.Add(line);
                Debug.WriteLine("Create Line");

            }
        }




        private void DrawPolygon(List<Ellipse> ellipses)
        {



            if (ellipses.Count >= 3) // 최소 3개의 꼭지점이 필요합니다.
            {
                Polygon polygon = new Polygon
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.LightBlue
                };

                // Polygon의 Points 속성에 꼭지점을 추가
                foreach (Ellipse ellipse in ellipses)
                {
                    Point point = new Point(Canvas.GetLeft(ellipse) + ellipse.Width / 2, Canvas.GetTop(ellipse) + ellipse.Height / 2);
                    polygon.Points.Add(point);
                }

                // 마지막 꼭지점과 첫 번째 꼭지점을 연결하여 다각형을 완성
                polygon.Points.Add(polygon.Points[0]);

                

                // 캔버스에 다각형 추가
                canvas.Children.Add(polygon);
                this.polygon = polygon;



                Point centroid = GetPolygonCenter(polygon);

                Debug.WriteLine("Centroid: " + centroid.X + ", " + centroid.Y);

                

                polygon.MouseDown += Polygon_MouseDown;
                polygon.MouseMove += Polygon_MouseMove;
                polygon.MouseUp += Polygon_MouseUp;


            }
        }

        private Point GetPolygonCenterbyLine(List<Line> lineList)
        {
            // 선분의 끝점들을 모아 폴리곤을 만듦
            List<Point> polygonPoints = new List<Point>();
            foreach (var line in lineList)
            {
                polygonPoints.Add(new Point(line.X1, line.Y1));
                polygonPoints.Add(new Point(line.X2, line.Y2));
            }

            // 폴리곤을 시계 방향으로 정렬
            double polygonArea = Math.Abs(polygonPoints.Take(polygonPoints.Count - 1)
                                                      .Select((point, index) => (polygonPoints[index + 1].X - point.X) * (polygonPoints[index + 1].Y + point.Y))
                                                      .Sum() / 2);

            if (polygonArea < 0)
            {
                polygonPoints.Reverse(); // 시계 방향으로 정렬되어 있지 않다면 배열을 뒤집음
            }

            // 겹치는 점을 제외한 나머지 점들을 사용하여 무게 중심을 계산
            double centerX = polygonPoints.Sum(p => p.X) / polygonPoints.Count;
            double centerY = polygonPoints.Sum(p => p.Y) / polygonPoints.Count;
               
            //Debug x,y
            Debug.WriteLine("CenterX: " + centerX);
            return new Point(centerX, centerY);
        }


        private Point GetPolygonCenter(List<Line> lineList)
        {
            List<Point> polygonPoints = new List<Point>();
            foreach (var line in lineList)
            {
                polygonPoints.Add(new Point(line.X1, line.Y1));
                polygonPoints.Add(new Point(line.X2, line.Y2));
            }
            double polygonArea = Math.Abs(polygonPoints.Take(polygonPoints.Count - 1)
                                                      .Select((point, index) => (polygonPoints[index + 1].X - point.X) * (polygonPoints[index + 1].Y + point.Y))
                                                      .Sum() / 2);

            if (polygonArea < 0)
            {
                polygonPoints.Reverse();
            }
            double centerX = polygonPoints.Sum(p => p.X) / polygonPoints.Count;
            double centerY = polygonPoints.Sum(p => p.Y) / polygonPoints.Count;
            return new Point(centerX, centerY);
        }







        private bool isDragging = false;
        private Point startPoint;

        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Polygon polygon = (Polygon)sender;

            // 마우스가 폴리곤 위에서 눌렸을 때
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                startPoint = e.GetPosition(polygon);
                polygon.CaptureMouse();
            }












        }

        private void Polygon_MouseMove(object sender, MouseEventArgs e)
        {
            Polygon polygon = (Polygon)sender;

            // 마우스 이동 중일 때
            if (isDragging)
            {
                Point newPoint = e.GetPosition(canvas);
                double deltaX = newPoint.X - startPoint.X;
                double deltaY = newPoint.Y - startPoint.Y;

                // move polygon.Points
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] = new Point(polygon.Points[i].X + deltaX, polygon.Points[i].Y + deltaY);
                }

                //move lineList
                for (int i = 0; i < lineList.Count; i++)
                {
                    lineList[i].X1 += deltaX;
                    lineList[i].X2 += deltaX;
                    lineList[i].Y1 += deltaY;
                    lineList[i].Y2 += deltaY;
                }

                //move ellipseList
                for (int i = 0; i < ellipseList.Count; i++)
                {
                    Canvas.SetLeft(ellipseList[i], Canvas.GetLeft(ellipseList[i]) + deltaX);
                    Canvas.SetTop(ellipseList[i], Canvas.GetTop(ellipseList[i]) + deltaY);
                }

                //move plain
                Canvas.SetLeft(plain, Canvas.GetLeft(plain) + deltaX);
                Canvas.SetTop(plain, Canvas.GetTop(plain) + deltaY);




                startPoint = newPoint;
            }
        }

        private void Polygon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Polygon polygon = (Polygon)sender;

            // 마우스 버튼이 눌린 상태에서 놓였을 때
            if (isDragging)
            {
                isDragging = false;
                polygon.ReleaseMouseCapture();
            }
        }





        private double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }


    }
}
