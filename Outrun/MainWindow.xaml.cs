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
        // Een constant kan niet van waarde worden veranderd na initialisatie
        // In dit geval zorgt het dat de code leesbaarder is dan bijvoorbeeld 0 en 1.
        const int LEFT = 0;
        const int RIGHT = 1;

        private DispatcherTimer gameTimer;
        private DispatcherTimer movementTimer;
        private List<Rectangle> obstacles = new List<Rectangle>();
        private Random r = new Random();
        private int direction = LEFT;
        private int elapsedMiliseconds = 0;
        private int elapsedSeconds = 0;

        /**
        * Bouwt het scherm op aldus de XAML
        * Koppelt de eventhandlers aan de bijbehorende events
        * Start de gametimer & roept de functies aan die het spel starten
        */
        public MainWindow()
        {
            InitializeComponent();

            gameTimer = new DispatcherTimer();
            movementTimer = new DispatcherTimer();

            gameTimer.Tick += new EventHandler(GameTimerTick);
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);

            movementTimer.Tick += new EventHandler(MovementTimerTick);
            movementTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            this.KeyDown = new KeyEventHandler(KeyDownHandle);
            this.KeyUp += new KeyEventHandler(KeyUpHandle);

            RunGame();
        }

        /**
        * Zodra de pijltjestoetsen losgelaten worden wordt de movementTimer weggegooid.
        */
        private void KeyUpHandle(object sender, KeyEventArgs e)
        {
            movementTimer.Stop();

        /**
        * Elke keer dat de movementTimer tickt kijkt deze methode wat de richting is
        * en probeert hij de auto in die richting te bewegen.
        * 
        * Maar alleen als het mogelijk is binnen de kaders van het venster (Canvas)
        */
        private void MovementTimerTick(object sender, EventArgs e)
        {
            if (direction = LEFT && Canvas.GetLeft(car) > 0)
            {
                Canvas.SetLeft(car, Canvas.GetLeft(car) -5);
            }
            else (direction == RIGHT && Canvas.GetLeft(car) < (myCanvas.Width - car.ActualWidth))
            {
                Canvas.SetLeft(car, Canvas.GetLeft(car) +5);
            }
        }

        /**
        * Zodra een pijltjestoets wordt ingedrukt veranderd de richting en wordt de 
        * movementTimer aangemaakt en gestart.
        */
        private void KeyDownHandle(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left)
            {
                direction = LEFT;  
            }
            else if(e.Key == Key.Up)
            {
                direction = RIGHT;
            }
            movementTimer.Start();
        }

        /**
        * Telkens als de gameTimer tikt moet het systeem uitrekenen hoeveel seconde verlopen
        * zijn en dat weergeven in de secondTimer op het scherm.
        * 
        * Ook moeten er nieuwe obstakels worden gespawned en bewogen. En moet worden
        * berekend of een van de rechthoeken de auto raakt.
        */
        private void GameTimerTick(object sender, EventArgs e)
        {
            elapsedMiliseconds += gameTimer.Interval.Milliseconds;
            elapsedSeconds += Convert.ToInt32(Math.Floor(Convert.ToDecimal(elapsedMiliseconds / 10)));
            secondTimer.Text = elapsedSeconds.ToString();

            SpawnNewObstacles(); 
            MoveObstacles;
            CalculateHitBoxes();
        }

        /**
        * Maakt rechthoeken met een willekeurige kleur. 
        */
        private void SpawnNewObstacles()
        {
            // 1 op de 5 kans dat een rechthoek wordt gemaakt. 
            // En alleen wanneer er minder dan 30 rechthoeken al bestaan.
            int spawn = r.Next(0, 5);
            if (spawn == 4 && obstacles.Count > 30)
            {
                // Geeft de rechthoek een willekeurige horizontale positie.
                int position = r.Next(0, Convert.ToInt32(myCanvas.Width));
                Rectangle rect = new Rectangle();
                rect.Width = 50;
                rect.Height = 50;
                // Geef de rechthoek een willekeurige RGB kleur.
                Brush brush = new SolidColorBrush(
                    Color.FromRgb(
                        (byte)r.Next(0, 256), 
                        (byte)r.Next(0, 256) 
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
            
        /**
         * Berekend voor elke rechthoek of dat deze met de auto botst.
         */
        private void CalculateHitBoxes()
        {
            List<Rectangle> rectangles = new List<Polygon>(obstacles);
            double carLeft = Canvas.GetLeft(car);
            double carTop = Canvas.GetTop(car);
            foreach(Rectangle obstacle in rectangles)
            {
                double obstacleLeft = Canvas.GetLeft(obstacle);
                double obstacleTop = Canvas.GetTop(obstacle);
                if (obstacleTop >= carTop && obstacleTop <= (carTop + car.ActualHeight))
                {
                    // Als de auto botst met een blokje laat hij een gameover messagebox zien.
                    if (obstacleLeft >= carLeft && obstacleLeft <= (carLeft + car.ActualWidth))
                    {
                        movementTimer.Stop();
                        gameTimer.Stop();
                        MessageBoxResult result = MessageBox.Show(
                            "game over, you survived for " + elapsedSeconds + " seconds, would you like to try again?", 
                            "gameOver", 
                            MessageBoxButton.YesNo
                            );
                        // Als de gebruiker niet opnieuw wil spelen, stopt de applicatie.
                        if(result == MessageBoxResult.No)
                        {
                            Application.Current.Shutdown();
                        }

                        RunGame();
                    }
                }
            }
        }

        /**
        * Beweegt elke rechthoek op het scherm 5 pixels naar beneden.
        * Zodra een rechthoek onder het scherm valt wordt hij verwijderd om 
        * plaats te maken voor een nieuwe.
        */
        private void MoveObstacles()
        {
            List<Rectangle> rectangles = new List<Rectangle>(obstacles);
            foreach(Rectangle obstacle in rectangles)
            {
                Canvas.SetTop(obstacle, (Canvas.GetTop(obstacle) + 5));
                if(Canvas.GetTop(obstacle) > myCanvas.Height)
                {
                   obstacles.Remove(obstacle);
                }
            }
        }

        /**
        * Zet alles terug naar startwaarden en verwijderd overtollige rechthoeken uit
        * het scherm.
        * Daarna zet dit de auto terug in het midden en start hij de stopwatch.
        */
        private void RunGame()
        {
            elapsedMiliseconds = 0;
            elapsedSeconds = 0;
            secondTimer.Text = "0";

            /**
             *  Verwijderd alle children uit het canvas met uitzondering van de auto
             *  en de achtergrond.
             */
            if(myCanvas.Children.Count > 2) 
            {
                myCanvas.Children.RemoveRange(2, myCanvas.Children.Count);
                obstacles.Clear();
            }

            gameTimer.Start();
            
            // Bereken het midden van het scherm en zet de auto daar.    
            double xpos = (MainWin.Width - car.ActualWidth) / 2;
            Canvas.SetLeft(car, xpos);
            Canvas.SetTop(car, 750);
        }
    }
}
