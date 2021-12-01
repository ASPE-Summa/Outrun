using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Outrun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int LEFT = 0;
        const int RIGHT = 1;

        private DispatcherTimer gameTimer;
        private DispatcherTimer movementTimer;
        private List<Rectangle> obstacles = new List<Rectangle>();
        private Random r = new Random();
        private int direction = LEFT;
        private int elapsedMiliseconds = 0;
        private int elapsedSeconds = 0;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer = new DispatcherTimer();
            movementTimer = new DispatcherTimer();

            gameTimer.Tick += new EventHandler(GameTimerTick);
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);

            movementTimer.Tick += new EventHandler(MovementTimerTick);
            movementTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            this.KeyDown += new KeyEventHandler(KeyDownHandle);
            this.KeyUp += new KeyEventHandler(KeyUpHandle);

            RunGame();
        }

        private void KeyUpHandle(object sender, KeyEventArgs e)
        {
            movementTimer.Stop();
        }

        private void MovementTimerTick(object sender, EventArgs e)
        {
            if (direction == LEFT && Canvas.GetLeft(car) > 0)
            {
                Canvas.SetLeft(car, Canvas.GetLeft(car) -5);
            }
            else if (direction == RIGHT && Canvas.GetLeft(car) < (myCanvas.Width - car.ActualWidth))
            {
                Canvas.SetLeft(car, Canvas.GetLeft(car) +5);
            }
        }

        private void KeyDownHandle(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left)
            {
                direction = LEFT;  
            }
            else if(e.Key == Key.Right)
            {
                direction = RIGHT;
            }
            movementTimer.Start();
        }

        private void GameTimerTick(object sender, EventArgs e)
        {
            elapsedMiliseconds += gameTimer.Interval.Milliseconds;
            elapsedSeconds = Convert.ToInt32(Math.Floor(Convert.ToDecimal(elapsedMiliseconds / 1000)));
            secondTimer.Text = elapsedSeconds.ToString();

            SpawnNewObstacles(); 
            MoveObstacles();
            CalculateHitBoxes();
        }

        private void SpawnNewObstacles()
        {
            int spawn = r.Next(0, 5);
            if (spawn == 4 && obstacles.Count < 30)
            {
                int position = r.Next(0, Convert.ToInt32(myCanvas.Width));
                Rectangle rect = new Rectangle();
                rect.Width = 50;
                rect.Height = 50;
                Brush brush = new SolidColorBrush(
                    Color.FromRgb(
                        (byte)r.Next(0, 256), 
                        (byte)r.Next(0, 256), 
                        (byte)r.Next(0, 256)
                        )
                    );

                rect.Fill = brush;
                rect.Stroke = Brushes.White;

                myCanvas.Children.Add(rect);
                Canvas.SetLeft(rect, position);
                Canvas.SetTop(rect, 0);
                obstacles.Add(rect);
            }
        }
            

        private void CalculateHitBoxes()
        {
            ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>(obstacles);
            double carLeft = Canvas.GetLeft(car);
            double carTop = Canvas.GetTop(car);
            foreach(Rectangle obstacle in rectangles)
            {
                double obstacleLeft = Canvas.GetLeft(obstacle);
                double obstacleTop = Canvas.GetTop(obstacle);
                if (obstacleTop >= carTop && obstacleTop <= (carTop + car.ActualHeight))
                {
                    if (obstacleLeft >= carLeft && obstacleLeft <= (carLeft + car.ActualWidth))
                    {
                        movementTimer.Stop();
                        gameTimer.Stop();
                        MessageBoxResult result = MessageBox.Show(
                            "game over, you survived for " + elapsedSeconds + " seconds, would you like to try again?", 
                            "gameOver", 
                            MessageBoxButton.YesNo
                            );
                        if(result == MessageBoxResult.No)
                        {
                            Application.Current.Shutdown();
                        }

                        RunGame();
                    }
                }
            }
        }

        private void MoveObstacles()
        {
            ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>(obstacles);
            foreach(Rectangle obstacle in rectangles)
            {
                Canvas.SetTop(obstacle, (Canvas.GetTop(obstacle) + 5));
                if(Canvas.GetTop(obstacle) > myCanvas.Height)
                {
                   obstacles.Remove(obstacle);
                }
            }
        }

        private void RunGame()
        {
            elapsedMiliseconds = 0;
            elapsedSeconds = 0;
            secondTimer.Text = "0";

            if(myCanvas.Children.Count > 2) 
            {
                myCanvas.Children.RemoveRange(2, myCanvas.Children.Count);
                obstacles.Clear();
            }

            gameTimer.Start();
            double xpos = (MainWin.Width - car.ActualWidth) / 2;
            Canvas.SetLeft(car, xpos);
            Canvas.SetTop(car, 750);
        }
    }
}
