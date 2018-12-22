// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using draw = System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace CheckerPlus
{
    public partial class MouseCheck : Window
    {
        public MouseCheck()
        {
            InitializeComponent();
            runenemy();
        }

        System.Windows.Threading.DispatcherTimer dt2 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dt = new System.Windows.Threading.DispatcherTimer();

        KeyCheck key = new KeyCheck();

        void runenemy()
        {
            dt2.Tick += Dt2_Tick;
            dt2.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dt2.Start();
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 0, 0, 80);
            dt.Start();
        }

        readonly double rotate = 25;
        double rts = 0;
        int pos = 1;
        bool right = false;

        private void Dt_Tick(object sender, EventArgs e)
        {
            switch (rts)
            {
                case -25:
                case 25:
                    pos = right ? 2 : 0;
                    right = !right;
                    break;
                case 0:
                    pos = 1;
                    break;
            }

            if (pos == 1 && !right)
            {
                rts -= rotate / 20;
                RotateTransform rotateTransform = new RotateTransform(rts);
                enemy_image.RenderTransform = rotateTransform;
            }
            else if (pos == 1 && right)
            {
                rts += rotate / 20;
                RotateTransform rotateTransform = new RotateTransform(rts);
                enemy_image.RenderTransform = rotateTransform;
            }
            else if (rts != 0)
            {
                if (pos == 2)
                    rts -= rotate / 10;
                else
                    rts += rotate / 10;
                RotateTransform rotateTransform = new RotateTransform(rts);
                enemy_image.RenderTransform = rotateTransform;
            }
        }

        int jh = 100;
        bool lr = true;

        private void Dt2_Tick(object sender, EventArgs e)
        {
            if (jh >= 100)
                lr = true;
            if (jh <= 0)
                lr = false;

            if (!lr)
            {
                var left = Canvas.GetLeft(enemy_image);
                Canvas.SetLeft(enemy_image, left + 1);
                jh++;
            }
            else
            {
                var left = Canvas.GetLeft(enemy_image);
                Canvas.SetLeft(enemy_image, left - 1);
                jh--;
            }
        }

        bool isDrawing = false;

        PathFigure currentFigure;

        void StartFigure(Point start)
        {
            var currentPath =
                new Line()
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = start.X + 1,
                    Y2 = start.Y + 1,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Stroke = Brushes.Red,
                    StrokeThickness = 3,
                };

            DrawingTarget.Children.Add(currentPath);
        }

        void AddFigurePoint(Point point)
        {
            try
            {
                if (point != null && currentFigure != null)
                    currentFigure.Segments.Add(new LineSegment(point, isStroked: true));
            }
            catch
            {
            }
        }

        void EndFigure()
        {
            currentFigure = null;
        }

        private void button3_Copy2_Click(object sender, RoutedEventArgs e)
        {
            DrawingTarget.Children.Clear();
        }

        #region Hooks

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        void DrawingMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
                return;

            if(e.LeftButton != MouseButtonState.Pressed)
            {
                isDrawing = false;
                Mouse.Capture(null);
                DrawingTarget.Children.Clear();
                messagekey("Левую кнопку мыши");
                return;
            }

            var point = Mouse.GetPosition(DrawingTarget);
            var currentPath =
                new Line()
                {
                    X1 = point.X,
                    Y1 = point.Y,
                    X2 = point.X + 1,
                    Y2 = point.Y + 1,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Stroke = Brushes.Red,
                    StrokeThickness = 3,
                };
            DrawingTarget.Children.Add(currentPath);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DrawingMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if(isDrawing)
            {
                isDrawing = false;
                Mouse.Capture(null);
                DrawingTarget.Children.Clear();                
                return;
            }
            Mouse.Capture(DrawingTarget);
            isDrawing = true;
            StartFigure(e.GetPosition(DrawingTarget));
        }

        #endregion

        #region Helper

        #endregion

        void messagekey(string key)
        {
            MessageBox.Show("Вы нажали: " + key, "CheckerPlus", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void DrawingT_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            Mouse.Capture(null);
            DrawingTarget.Children.Clear();
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    messagekey("Правую кнопку мыши");
                    break;
                case MouseButton.Left:
                    messagekey("Левую кнопку мыши");
                    break;
                case MouseButton.Middle:
                    messagekey("Средную кнопку мыши");
                    break;
                case MouseButton.XButton1:
                    messagekey("Дополнительную кнопку мыши #1");
                    break;
                case MouseButton.XButton2:
                    messagekey("Дополнительную кнопку мыши #2");
                    break;
                default:
                    messagekey($"Дополнительную кпопку {e.ChangedButton}");
                    MessageBox.Show($"Неизвестная клавиша {e.ChangedButton.GetHashCode()}");
                    break;
            }
        }

        private void DrawingTarget_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
                return;

            isDrawing = false;
            Mouse.Capture(null);
            DrawingTarget.Children.Clear();
            //MessageBox.Show("Не покидайте поле!", "CheckerPlus", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton != MouseButtonState.Pressed)
            {
                isDrawing = false;
                Mouse.Capture(null);
                DrawingTarget.Children.Clear();
                messagekey("Левую кнопку мыши");
                return;
            }
        }
    }
}
