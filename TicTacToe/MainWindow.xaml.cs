using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TicTacToe
{
    public partial class MainWindow
    {
        private readonly Dictionary<Player, ImageSource> _imageSources = new Dictionary<Player, ImageSource>()
        {
            { Player.X, new BitmapImage(new Uri("pack://application:,,,/Assets/X15.png")) },
            { Player.O, new BitmapImage(new Uri("pack://application:,,,/Assets/O15.png")) }
        };

        private readonly Image[,] _imageControls = new Image[3, 3];
        private readonly GameState _gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            SetupGameGrid();
            _gameState.MoveMade += OnMoveMade;
            _gameState.GameEnded += OnGameEnded;
            _gameState.GameRestarted += OnGameRestarted;
        }

        private void SetupGameGrid()
        {
            for (var r = 0; r < 3; r++)
            {
                for (var c = 0; c < 3; c++)
                {
                    var imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    _imageControls[r, c] = imageControl;
                }
            }
        }

        private void TransitionToEndScreen(string text, ImageSource winner)
        {
            TurnPanel.Visibility = Visibility.Hidden;
            GameCanvas.Visibility = Visibility.Hidden;
            ResultText.Text = text;
            WinnerImage.Source = winner;
            EndScreen.Visibility = Visibility.Visible;
            Line.Visibility = Visibility.Hidden;
        }

        private void TransitionToGameScreen()
        {
            EndScreen.Visibility = Visibility.Hidden;
            TurnPanel.Visibility = Visibility.Visible;
            GameCanvas.Visibility = Visibility.Visible;
        }

        private (Point, Point) FindLinePoints(WinInfo winInfo)
        {
            double squareSize = GameGrid.Width / 3;
            double margin = squareSize / 2;

            if (winInfo.Type == WinType.Row)
            {
                double y = winInfo.Number * squareSize + margin;
                return (new Point(0, y), new Point(GameGrid.Width, y));
            }

            if (winInfo.Type == WinType.Column)
            {
                double x = winInfo.Number * squareSize + margin;
                return (new Point(x, 0), new Point(x, GameGrid.Height));
            }

            if (winInfo.Type == WinType.Diagonal)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }

            return (new Point(GameGrid.Width, 0), new Point(0, GameGrid.Height));
        }


        private void ShowLine(WinInfo winInfo)
        {
            (Point start, Point end) = FindLinePoints(winInfo);
            Line.X1 = start.X;
            Line.Y1 = start.Y;

            Line.X2 = end.X;
            Line.Y2 = end.Y;

            Line.Visibility = Visibility.Visible;
        }

        private void OnMoveMade(int r, int c)
        {
            Player player = _gameState.GameGrid[r, c];
            _imageControls[r, c].Source = _imageSources[player];
            PlayerImage.Source = _imageSources[_gameState.CurrentPlayer];
        }

        private async void OnGameEnded(GameResult result)
        {
            await Task.Delay(1000);
            if (result.Winner == Player.None)
            {
                TransitionToEndScreen("It's a tie!", null);
            }
            else
            {
                ShowLine(result.WinInfo);
                await Task.Delay(1000);
                string text = "Winner: ";
                ImageSource winner = _imageSources[result.Winner];
                TransitionToEndScreen(text, winner);
            }
        }

        private void OnGameRestarted()
        {
            for (var r = 0; r < 3; r++)
            {
                for (var c = 0; c < 3; c++)
                {
                    _imageControls[r, c].Source = null;
                }
            }

            PlayerImage.Source = _imageSources[_gameState.CurrentPlayer];
            TransitionToGameScreen();
        }

        private void GameGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point clickPosition = e.GetPosition(GameGrid);
            int row = (int)(clickPosition.Y / squareSize);
            int column = (int)(clickPosition.X / squareSize);
            _gameState.MakeMove(row, column);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _gameState.Reset();
        }
    }
}