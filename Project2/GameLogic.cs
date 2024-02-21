using System.Collections.Generic;
using System;

namespace GameLogic
{
    public class ConnectFourGame
    {
        private eShapes[,] m_Board;
        private int m_RowCount;
        private int m_ColumnCount;
        private readonly int r_WinningScore = int.MaxValue;
        private readonly int r_NumOfSameShape = 4;
        private GameScores m_GameScores;
        private eShapes m_CurrentPlayer;

        public eShapes CurrentPlayer
        {
            get
            {
                return m_CurrentPlayer;
            }
        }

        public GameScores GameScores
        {
            get
            {
                return m_GameScores;
            }
            set
            {
                m_GameScores = value;
            }
        }
        public ConnectFourGame(int i_Rows, int i_Columns)
        {
            m_RowCount = i_Rows;
            m_ColumnCount = i_Columns;
            m_Board = new eShapes[m_RowCount, m_ColumnCount];
            m_CurrentPlayer = eShapes.X;
            m_GameScores = new GameScores();
            InitBoard();
        }

        public int Columns
        {
            get
            {
                return m_ColumnCount;
            }
        }

        public int MakePlayerMove(int i_Col)
        {
            int row = GetNextOpenRow(i_Col);

            DropPiece(row, i_Col, m_CurrentPlayer);
            SetCurrentPlayer();
            return row;
        }

        public void MakeComputerMove()
        {
            int depth = CalculateDepth(m_ColumnCount);
            (int col, int _) = Minimax(m_Board, depth, true);
            int row = GetNextOpenRow(col);
            DropPiece(row, col, m_CurrentPlayer);
            SetCurrentPlayer();
        }

        public bool IsGameOver()
        {
            bool isFinished = false;

            if (WinningMove(eShapes.X))
            {
                m_GameScores.PlayerOneScores++;
                isFinished = !isFinished;
            }
            else if (WinningMove(eShapes.O))
            {
                m_GameScores.PlayerTwoScores++;
                isFinished = !isFinished;
            }

            return isFinished || IsBoardFull();
        }

        public eShapes[,] Board
        {
            get 
            { 
                return m_Board;
            }
        }

        public void SetCurrentPlayer()
        {
            m_CurrentPlayer = (m_CurrentPlayer == eShapes.X) ? eShapes.O : eShapes.X;
        }

        public void InitBoard()
        {
            for (int i = 0; i < m_RowCount; i++)
            {
                for (int j = 0; j < m_ColumnCount; j++)
                {
                    m_Board[i, j] = eShapes.Empty;
                }
            }

            m_CurrentPlayer = eShapes.X;
        }

        private bool isInMatrixRange(int i_Col)
        {
            return i_Col <= m_ColumnCount && i_Col > 0;
        }

        private bool isColNotFull(int i_Col)
        {
            return m_Board[m_RowCount - 1, i_Col] == eShapes.Empty;
        }

        public bool IsValidLocation(int i_Col)
        {
            return isInMatrixRange(i_Col) && isColNotFull(i_Col - 1);
        }

        private int GetNextOpenRow(int col)
        {
            int openRow = -1;

            for (int r = 0; r < m_RowCount; r++)
            {
                if (m_Board[r, col] == eShapes.Empty)
                {
                    openRow = r;
                    break;
                }
            }
            return openRow; 
        }

        private void DropPiece(int row, int col, eShapes piece)
        {
            m_Board[row, col] = piece;
        }

        private bool WinningMove(eShapes piece)
        {
            bool isWin = false;

            for (int r = 0; r < m_RowCount && !isWin; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    if (m_Board[r, c] == piece &&
                        m_Board[r, c + 1] == piece &&
                        m_Board[r, c + 2] == piece &&
                        m_Board[r, c + 3] == piece)
                    {
                        isWin = !isWin;
                        break;
                    }
                }
            }

            for (int c = 0; c < m_ColumnCount && !isWin; c++)
            {
                for (int r = 0; r < m_RowCount - 3; r++)
                {
                    if (m_Board[r, c] == piece &&
                        m_Board[r + 1, c] == piece &&
                        m_Board[r + 2, c] == piece &&
                        m_Board[r + 3, c] == piece)
                    {
                        isWin = !isWin;
                        break;
                    }
                }
            }
 
            for (int r = 0; r < m_RowCount - 3 && !isWin; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    if (m_Board[r, c] == piece &&
                        m_Board[r + 1, c + 1] == piece &&
                        m_Board[r + 2, c + 2] == piece &&
                        m_Board[r + 3, c + 3] == piece)
                    {
                        isWin = !isWin;
                        break;
                    }
                }
            }

            for (int r = 0; r < m_RowCount - 3 && !isWin; r++)
            {
                for (int c = 3; c < m_ColumnCount; c++)
                {
                    if (m_Board[r, c] == piece &&
                        m_Board[r + 1, c - 1] == piece &&
                        m_Board[r + 2, c - 2] == piece &&
                        m_Board[r + 3, c - 3] == piece)
                    {
                        isWin = !isWin;
                        break;
                    }
                }
            }

            return isWin;
        }

        public bool IsBoardFull()
        {
            bool isFull = true;

            for (int c = 0; c < m_ColumnCount; c++)
            {
                if (m_Board[m_RowCount - 1, c] == eShapes.Empty)
                {
                    isFull = !isFull;
                    break;
                }
            }

            return isFull;
        }

        private List<int> GetValidMoves()
        {
            List<int> validMoves = new List<int>();

            for (int col = 0; col < m_ColumnCount; col++)
            {
                if (isColNotFull(col))
                {
                    validMoves.Add(col);
                }
            }

            return validMoves;
        }

        private (int, int) Minimax(eShapes[,] i_Board, int i_Depth, bool i_MaximizingPlayer)
        {
            int bestScore = i_MaximizingPlayer ? int.MinValue : int.MaxValue;
            int bestColumn = -1;

            if (i_Depth == 0 || IsTerminalNode())
            {
                bestScore = EvaluateBoard();
            }
            else
            {
                List<int> validMoves = GetValidMoves();

                foreach (int col in validMoves)
                {
                    int row = GetNextOpenRow(i_Board, col);
                    if (row == -1) continue; 
                    i_Board[row, col] = i_MaximizingPlayer ? eShapes.O : eShapes.X;

                    int score = Minimax(i_Board, i_Depth - 1, !i_MaximizingPlayer).Item2;

                    if (i_MaximizingPlayer)
                    {
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestColumn = col;
                        }
                    }
                    else
                    {
                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestColumn = col;
                        }
                    }

                    i_Board[row, col] = eShapes.Empty;
                }
            }

            return (bestColumn, bestScore);
        }

        private bool IsTerminalNode()
        {
            return WinningMove(eShapes.X) || WinningMove(eShapes.O) || IsBoardFull();
        }

        private int EvaluateBoard()
        {
            int score = 0;

            for (int r = 0; r < m_RowCount; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r, c + 1], m_Board[r, c + 2], m_Board[r, c + 3] };
                    score += EvaluateWindow(window);
                }
            }
 
            for (int c = 0; c < m_ColumnCount; c++)
            {
                for (int r = 0; r < m_RowCount - 3; r++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r + 1, c], m_Board[r + 2, c], m_Board[r + 3, c] };
                    score += EvaluateWindow(window);
                }
            }
 
            for (int r = 0; r < m_RowCount - 3; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r + 1, c + 1], m_Board[r + 2, c + 2], m_Board[r + 3, c + 3] };
                    score += EvaluateWindow(window);
                }
            }
 
            for (int r = 0; r < m_RowCount - 3; r++)
            {
                for (int c = 3; c < m_ColumnCount; c++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r + 1, c - 1], m_Board[r + 2, c - 2], m_Board[r + 3, c - 3] };
                    score += EvaluateWindow(window);
                }
            }

            return score;
        }

        private int EvaluateWindow(eShapes[] i_Window)
        {
            int playerPieces = 0;
            int computerPieces = 0;
            int emptySpaces = 0;
            int retValue;

            foreach (eShapes piece in i_Window)
            {
                if (piece == eShapes.X)
                {
                    playerPieces++;
                }
                else if (piece == eShapes.O)
                {
                    computerPieces++;
                }
                else
                {
                    emptySpaces++;
                }
            }

            if (playerPieces == r_NumOfSameShape)
            {
                retValue = -r_WinningScore;
            }
            else if (computerPieces == r_NumOfSameShape)
            {
                retValue = r_WinningScore;
            }
            else
            {
                retValue = computerPieces - playerPieces;
            }

            return retValue;
        }

        private int GetNextOpenRow(eShapes[,] i_Board, int i_Col)
        {
            int openRow = -1;

            for (int r = 0; r < m_RowCount; r++)
            {
                if (i_Board[r, i_Col] == eShapes.Empty)
                {
                    openRow = r;
                    break;
                }
            }

            return openRow;
        }

        private int CalculateDepth(int i_ColumnCount)
        {
            int minSize = 4;
            int maxSize = 8;
            int minDepth = 3;  
            int maxDepth = 6;  
            double depth = minDepth + ((maxDepth - minDepth) / (double)(maxSize - minSize)) * (i_ColumnCount - minSize);

            return (int)Math.Round(depth);
        }

        public ePlayer getWinner()
        {
            return m_GameScores.PlayerOneScores > m_GameScores.PlayerTwoScores ?
                        ePlayer.Player1 : ePlayer.Player2;
        } 
    }
}
