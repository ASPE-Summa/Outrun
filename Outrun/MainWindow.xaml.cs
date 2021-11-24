using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        private List<TextBlock> obstacles = new List<TextBlock>();
        
        private int direction = LEFT;
        public MainWindow()
        {
            InitializeComponent();
            Canvas.SetLeft(car, (myCanvas.ActualWidth - car.ActualWidth) / 2);
            Canvas.SetBottom(car, 50);
            gameTimer = new DispatcherTimer();
            movementTimer = new DispatcherTimer();

            gameTimer.Tick += new EventHandler(GameTimerTick);
            gameTimer.Interval = new TimeSpan(0, 0, 1);

            movementTimer.Tick += new EventHandler(MovementTimerTick);
            movementTimer.Interval = new TimeSpan(0, 0, 1);

            this.KeyDown += new KeyEventHandler(KeyDownHandle);
            this.KeyUp += new KeyEventHandler(KeyUpHandle);
            //Game ended
        }

        private void KeyUpHandle(object sender, KeyEventArgs e)
        {
            movementTimer.Stop();
        }

        private void MovementTimerTick(object sender, EventArgs e)
        {
            if (direction == LEFT && car.Margin.Left > 0)
            {
                Canvas.SetLeft(car, -5);
            }
            else if (direction == RIGHT && car.Margin.Right < System.Windows.SystemParameters.PrimaryScreenWidth)
            {
                Canvas.SetLeft(car, 5);
            }
        }

        private void KeyDownHandle(object sender, KeyEventArgs e)
        {
            movementTimer.Stop();
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
            //throw new NotImplementedException();
        }
    }
}
