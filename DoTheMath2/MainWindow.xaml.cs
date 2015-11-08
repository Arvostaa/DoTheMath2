using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/*Created by David Tvildiani*/
namespace DoTheMath2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            myInit();
        }

        /// <summary>
        /// Initialize Snake Game, add crashed and Food eatten Event Handlers
        /// </summary>
        private void myInit()
        {
            Snake = new Game(GameBoard);
            Snake.SnakeCrashed += (object s, EventArgs e) =>
            {
                gameStopped();
               
            };
            Snake.FoodEatten += (object s, EventArgs e) =>
            {
              
                score++;
                LabelScore.Dispatcher.BeginInvoke((Action)(() => { LabelScore.Content = score; }));
            };
            _NotificationCleaner = new Timer(3000);
            _NotificationCleaner.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                
            };
            _NotificationCleaner.Start();
        }

        private int score;
        public Game Snake { get; set; }
        Timer _NotificationCleaner;
        int _notificationPlacedTime;

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            Snake.StartPlay(Direction.Right);
            LabelScore.Content = "0";
            score = 0;
            ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = true;
            ButtonPause.IsEnabled = true;
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            if (Snake.GameState == Game.State.Running)
            {
                Snake.Pause();
                b.Content = "Resume";
            }
            else if (Snake.GameState == Game.State.Paused)
            {
                Snake.Resume();
                b.Content = "Pause";
            }

        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Snake != null && Snake.GameState == Game.State.Running)
            {
                e.Handled = true;
                switch (e.Key)
                {

                    case Key.Right:
                        Snake.ChangeDirection(Direction.Right);
                        break;
                    case Key.Left:
                        Snake.ChangeDirection(Direction.Left);
                        break;
                    case Key.Up:
                        Snake.ChangeDirection(Direction.Up);
                        break;
                    case Key.Down:
                        Snake.ChangeDirection(Direction.Down);
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            gameStopped();
        }

        private void gameStopped()
        {
            Snake.Reset();
            ButtonStart.Dispatcher.BeginInvoke((Action)(() => { ButtonStart.IsEnabled = true; }));
            ButtonStop.Dispatcher.BeginInvoke((Action)(() => { ButtonStop.IsEnabled = false; }));
            ButtonPause.Dispatcher.BeginInvoke((Action)(() => { ButtonPause.IsEnabled = false; ButtonPause.Content = "Pause"; }));
        }

    }
}
