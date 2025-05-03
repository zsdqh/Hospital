using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Clock.xaml
    /// </summary>
    public partial class Clock : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool isDragging = false;

        private int minutes = 0;
        public int Minutes
        {
            get { return minutes; }
            set
            {
                if (value < 0 || value > 59) return;
                minutes = value;
                OnPropertyChanged(nameof(Minutes));
                RotateLines();
            }
        }

        private int hours = 0;
        public int Hours
        {
            get { return hours; }
            set
            {
                if (value < 0 || value > 23) return;
                hours = value;
                OnPropertyChanged(nameof(Hours));
                RotateLines();
            }
        }

        private Line hour_line;
        private Line minute_line;

        public Clock()
        {
            InitializeComponent();
            DataContext = this;

            // Создание линий
            hour_line = new Line
            {
                StrokeThickness = 4,
                Stroke = Brushes.Gray,
                StrokeEndLineCap = PenLineCap.Triangle
            };

            minute_line = new Line
            {
                StrokeThickness = 4,
                Stroke = Brushes.Black,
                StrokeEndLineCap = PenLineCap.Triangle
            };

            myGrid.Children.Add(minute_line);
            myGrid.Children.Add(hour_line);

            hour_line.MouseDown += Line_MouseDown;
            hour_line.MouseMove += Line_MouseMove;
            hour_line.MouseUp += Line_MouseUp;

            minute_line.MouseDown += Line_MouseDown;
            minute_line.MouseMove += Line_MouseMove;
            minute_line.MouseUp += Line_MouseUp;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            (sender as Line).CaptureMouse();
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Line line = sender as Line;
                Point currentPoint = e.GetPosition(this);

                double x1 = this.ActualWidth / 2, y1 = this.ActualHeight / 2; // Центр
                double x2 = this.ActualWidth / 2, y2 = 0; // Верхнияя центральная точка(начало отсчета угла наклона
                double x3 = currentPoint.X, y3 = currentPoint.Y; // Позиция курсора

                double SideA = Math.Sqrt(Math.Pow(x2 - x3, 2) + Math.Pow(y2 - y3, 2));
                double SideB = Math.Sqrt(Math.Pow(x1 - x3, 2) + Math.Pow(y1 - y3, 2)); // Нахождение сторон треугольника по координатам 
                double SideC = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

                double angleA = Math.Acos((Math.Pow(SideB, 2) + Math.Pow(SideC, 2) - Math.Pow(SideA, 2)) / (2 * SideB * SideC)) * (180 / Math.PI); //Угол для вычисления времени относительно позиции мышки

                if (x3 < x1) angleA = 360 - angleA; // Курсор находится в первой половине часов

                if (line == minute_line)
                    Minutes = (int)Math.Round(angleA / 6.0); // Угол наклона минутной стрелки 6 градусов = 1минута
                else
                {
                    bool is_evening = Hours>11; // Первая или вторая половина дня
                    Hours = (int)Math.Round(angleA / 30.0) + (is_evening?12:0);// Угол наклона часовой стрелки 30 градусов = 1минута

                }
            }
        }

        private void Line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            (sender as Line).ReleaseMouseCapture();
        }

        private void ResetLines()
        {
            // Обнуление позиций линий
            hour_line.X1 = minute_line.X1 = this.ActualWidth / 2;
            hour_line.Y1 = minute_line.Y1 = this.ActualHeight / 2;

            hour_line.X2 = this.ActualWidth / 2;
            hour_line.Y2 = this.ActualHeight * 0.25;

            minute_line.X2 = this.ActualWidth / 2;
            minute_line.Y2 = this.ActualHeight * 0.1;
        }

        private void RotateLines()
        {
            ResetLines();
            hour_line.RenderTransform = new RotateTransform(((Hours + Minutes / 60.0) % 12) * 360 / 12.0, hour_line.X1, hour_line.Y1);
            minute_line.RenderTransform = new RotateTransform(Minutes * 360 / 60.0, minute_line.X1, minute_line.Y1);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RotateLines();
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Проверка свойства IsEnabled
            if (this.IsEnabled)
            {
                this.hour_line.Stroke = Brushes.Gray;
                this.minute_line.Stroke = Brushes.Black;
                // Обновляем изображение часов на активное
                this.ClockImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/clock.png"));
            }
            else
            {
                this.hour_line.Stroke = Brushes.LightGray;
                this.minute_line.Stroke = Brushes.LightGray;
                // Обновляем изображение часов на неактивное
                this.ClockImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NotActiveClock.png"));
            }
        }
    }
}
