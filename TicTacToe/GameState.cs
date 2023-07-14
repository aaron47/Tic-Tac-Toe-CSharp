﻿using System;
using System.Windows.Controls;

namespace TicTacToe
{
    public class GameState
    {
        public Player[,] GameGrid { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int TurnsPassed { get; private set; }
        public bool GameOver { get; private set; }

        public event Action<int, int> MoveMade;
        public event Action<GameResult> GameEnded;
        public event Action GameRestarted;

        public GameState()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
        }

        private bool CanMakeMove(int row, int column)
        {
            return !GameOver && GameGrid[row, column] == Player.None;
        }

        private bool IsGridFull()
        {
            return TurnsPassed == 9;
        }

        private void SwitchCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach (var (row, column) in squares)
            {
                if (GameGrid[row, column] != player)
                {
                    return false;
                }
            }

            return true;
        }

        private bool DidMoveWin(int r, int c, out WinInfo winInfo)
        {
            (int, int)[] row = { (r, 0), (r, 1), (r, 2) };
            (int, int)[] column = { (0, c), (1, c), (2, c) };
            (int, int)[] mainDiagonal = { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antiDiagonal = { (0, 2), (1, 1), (2, 0) };


            if (AreSquaresMarked(row, CurrentPlayer))
            {
                winInfo = new WinInfo
                {
                    Type = WinType.Row,
                    Number = r
                };
                return true;
            }

            if (AreSquaresMarked(column, CurrentPlayer))
            {
                winInfo = new WinInfo
                {
                    Type = WinType.Column,
                    Number = c
                };
                return true;
            }

            if (AreSquaresMarked(mainDiagonal, CurrentPlayer))
            {
                winInfo = new WinInfo
                {
                    Type = WinType.Diagonal,
                };
                return true;
            }

            if (AreSquaresMarked(antiDiagonal, CurrentPlayer))
            {
                winInfo = new WinInfo
                {
                    Type = WinType.AntiDiagonal,
                };
                return true;
            }

            winInfo = null;
            return false;
        }

        private bool DidMoveEndGame(int r, int c, out GameResult gameResult)
        {
            if (DidMoveWin(r, c, out WinInfo winInfo))
            {
                gameResult = new GameResult
                {
                    Winner = CurrentPlayer,
                    WinInfo = winInfo
                };
                return true;
            }

            if (IsGridFull())
            {
                gameResult = new GameResult
                {
                    Winner = Player.None,
                };
                return true;
            }

            gameResult = null;
            return false;
        }

        public void MakeMove(int r, int c)
        {
            if (!CanMakeMove(r, c))
            {
            }

            GameGrid[r, c] = CurrentPlayer;
            TurnsPassed++;

            if (DidMoveEndGame(r, c, out GameResult gameResult))
            {
                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameResult);
            }
            else
            {
                SwitchCurrentPlayer();
                MoveMade?.Invoke(r, c);
            }
        }

        public void Reset()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
            GameRestarted?.Invoke();
        }
    }
}