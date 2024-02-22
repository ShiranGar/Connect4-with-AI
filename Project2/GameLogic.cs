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

        public ConnectFourGame(int i_Rows, int i_Columns)
        {
            m_RowCount = i_Rows;
            m_ColumnCount = i_Columns;
            m_Board = new eShapes[m_RowCount, m_ColumnCount];
            m_CurrentPlayer = eShapes.X;
            m_GameScores = new GameScores();
            InitBoard();
        }

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

        public int Columns
        {
            get
            {
                return m_ColumnCount;
            }
        }

        public eShapes[,] Board
        {
            get
            {
                return m_Board;
            }
        }

        public int MakePlayerMove(int i_Col)
        {
            int openRow = getNextOpenRow(i_Col);

            DropPiece(openRow, i_Col, m_CurrentPlayer);
            SetCurrentPlayer();

            return openRow;
        }

        public void MakeComputerMove()
        {
            int depth = calculateDepth(m_ColumnCount);
            (int col, int notRelevant) = minimax(m_Board, depth, true);
            int row = getNextOpenRow(col);

            DropPiece(row, col, m_CurrentPlayer);
            SetCurrentPlayer();
        }

        public bool IsGameOver(eShapes i_CurrentPlayer)
        {
            bool isFinished = false;

            if(i_CurrentPlayer == eShapes.X)
            {
                if (winningMove(eShapes.O))
                {
                    m_GameScores.PlayerTwoScores++;
                    isFinished = !isFinished;
                }
            }
            else if (winningMove(eShapes.X))
            {
                m_GameScores.PlayerOneScores++;
                isFinished = !isFinished;
            }

            return isFinished || IsBoardFull();
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

        public bool IsInMatrixRange(int i_Col)
        {
            return i_Col <= m_ColumnCount && i_Col > 0;
        }

        public bool IsColFull(int i_Col)
        {
            return m_Board[m_RowCount - 1, i_Col] != eShapes.Empty;
        }

        public bool IsValidLocation(int i_Col)
        {
            return IsInMatrixRange(i_Col) && IsColFull(i_Col - 1);
        }

        private int getNextOpenRow(int i_Col)
        {
            int openRow = -1;

            for (int r = 0; r < m_RowCount; r++)
            {
                if (m_Board[r, i_Col] == eShapes.Empty)
                {
                    openRow = r;
                    break;
                }
            }

            return openRow; 
        }

        public void DropPiece(int i_Row, int i_Col, eShapes i_Piece)
        {
            m_Board[i_Row, i_Col] = i_Piece;
        }

        private bool winningMove(eShapes i_Piece)
        {
            bool isWin = false;

            for (int r = 0; r < m_RowCount && !isWin; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    if (m_Board[r, c] == i_Piece &&
                        m_Board[r, c + 1] == i_Piece &&
                        m_Board[r, c + 2] == i_Piece &&
                        m_Board[r, c + 3] == i_Piece)
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
                    if (m_Board[r, c] == i_Piece &&
                        m_Board[r + 1, c] == i_Piece &&
                        m_Board[r + 2, c] == i_Piece &&
                        m_Board[r + 3, c] == i_Piece)
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
                    if (m_Board[r, c] == i_Piece &&
                        m_Board[r + 1, c + 1] == i_Piece &&
                        m_Board[r + 2, c + 2] == i_Piece &&
                        m_Board[r + 3, c + 3] == i_Piece)
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
                    if (m_Board[r, c] == i_Piece &&
                        m_Board[r + 1, c - 1] == i_Piece &&
                        m_Board[r + 2, c - 2] == i_Piece &&
                        m_Board[r + 3, c - 3] == i_Piece)
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

        private List<int> getValidMoves()
        {
            List<int> validMoves = new List<int>();

            for (int col = 0; col < m_ColumnCount; col++)
            {
                if (!IsColFull(col))
                {
                    validMoves.Add(col);
                }
            }

            return validMoves;
        }

        private (int, int) minimax(eShapes[,] i_Board, int i_Depth, bool i_MaximizingPlayer)
        {
            int bestScore = i_MaximizingPlayer ? int.MinValue : int.MaxValue;
            int bestColumn = -1;

            if (i_Depth == 0 || isTerminalNode())
            {
                bestScore = evaluateBoard();
            }
            else
            {
                List<int> validMoves = getValidMoves();

                foreach (int col in validMoves)
                {
                    int row = getNextOpenRow(i_Board, col);

                    if (row == -1)
                    {
                        continue;
                    }

                    i_Board[row, col] = i_MaximizingPlayer ? eShapes.O : eShapes.X;
                    int score = minimax(i_Board, i_Depth - 1, !i_MaximizingPlayer).Item2;

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

        private bool isTerminalNode()
        {
            return winningMove(eShapes.X) || winningMove(eShapes.O) || IsBoardFull();
        }

        private int evaluateBoard()
        {
            int score = 0;

            for (int r = 0; r < m_RowCount; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r, c + 1], m_Board[r, c + 2], m_Board[r, c + 3] };

                    score += evaluateWindow(window);
                }
            }
 
            for (int c = 0; c < m_ColumnCount; c++)
            {
                for (int r = 0; r < m_RowCount - 3; r++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r + 1, c], m_Board[r + 2, c], m_Board[r + 3, c] };

                    score += evaluateWindow(window);
                }
            }
 
            for (int r = 0; r < m_RowCount - 3; r++)
            {
                for (int c = 0; c < m_ColumnCount - 3; c++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r + 1, c + 1], m_Board[r + 2, c + 2], m_Board[r + 3, c + 3] };

                    score += evaluateWindow(window);
                }
            }
 
            for (int r = 0; r < m_RowCount - 3; r++)
            {
                for (int c = 3; c < m_ColumnCount; c++)
                {
                    eShapes[] window = { m_Board[r, c], m_Board[r + 1, c - 1], m_Board[r + 2, c - 2], m_Board[r + 3, c - 3] };

                    score += evaluateWindow(window);
                }
            }

            return score;
        }

        private int evaluateWindow(eShapes[] i_Window)
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

        private int getNextOpenRow(eShapes[,] i_Board, int i_Col)
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

        private int calculateDepth(int i_ColumnCount)
        {
            int minSize = 4;
            int maxSize = 8;
            int minDepth = 3;  
            int maxDepth = 6;  
            double depth = minDepth + ((maxDepth - minDepth) / (double)(maxSize - minSize)) * (i_ColumnCount - minSize);

            return (int)Math.Round(depth);
        }

        public ePlayer GetWinner()
        {
            return m_GameScores.PlayerOneScores > m_GameScores.PlayerTwoScores ?
                        ePlayer.Player1 : ePlayer.Player2;
        }

        public bool CheckConnect4(out int io_WinningRow, out int io_WinningCol)
        {
            io_WinningRow = -1;
            io_WinningCol = -1;
            bool isWinningMoveExist = false;
                                 
            for (int col = 0; col < m_ColumnCount && !isWinningMoveExist; col++)
            {                        
                for (int row = 0; row < m_RowCount - 3; row++)
                {            
                    if (m_Board[row, col] == eShapes.O &&
                        m_Board[row + 1, col] == eShapes.O &&
                        m_Board[row + 2, col] == eShapes.O &&
                        m_Board[row + 3, col] == eShapes.Empty)

                    {
                        io_WinningRow = row + 3;
                        io_WinningCol = col;
                        isWinningMoveExist = true;
                        break;
                    }
                }
            }

            return isWinningMoveExist;
        }
    }
}

