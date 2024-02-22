using GameLogic;
using System;
using Ex02.ConsoleUtils;

namespace GameUi
{
    public class ConnectFourUI
    {
        private BoardGameManager m_GameBoard;
        readonly int r_MaxColsOrRows = 8;
        readonly int r_MinColsOrRows = 4;
        eGameType m_GameType;

        public ConnectFourUI()
        {
            initializeGame();
        }

        public void StartGame()
        {
            printBoard();
            string userInput = getUserInput();

            while (!m_GameBoard.IsBoardFull())
            {
                bool isQuit;
                int columnIntInput;
                isQuit = isNotQuitAndNotValid(userInput, out columnIntInput);

                if (isQuit)
                {
                    m_GameBoard.GameScores.AddOnePointToOpponent(m_GameBoard.CurrentPlayer);
                    break;
                }
                if (!m_GameBoard.IsInMatrixRange(columnIntInput))
                {
                    Console.WriteLine("Not in matrix range. Please try again");
                    userInput = getUserInput();
                    continue;
                }
                else if (m_GameBoard.IsColFull(columnIntInput - 1))
                {
                    Console.WriteLine("This column is full, please try again");
                    userInput = getUserInput();
                    continue;
                }

                playerMove(columnIntInput);
                Screen.Clear();
                printBoard();
                if (m_GameBoard.IsGameOver(m_GameBoard.CurrentPlayer))
                {
                    break;
                }

                if (m_GameType == eGameType.AgainstComputer)
                {
                    if (computerMove())
                    {
                        break;
                    }
                }

                Screen.Clear();
                printBoard();
                if (m_GameBoard.IsGameOver(m_GameBoard.CurrentPlayer))
                {
                    break;
                }
                else
                {
                    userInput = getUserInput();
                }
            }

            if (m_GameBoard.GameScores.IsZeroTie())
            {
                Console.WriteLine("It's a tie");
            }
            else
            {
                Console.WriteLine(m_GameBoard.GetWinner() + " you're the winner!");
            }

            printFinalScores();
            if(isCountinueSameGame())
            {
                newGame();
            }
        }

        private eGameType getGameTypeFromUser()
        {
            Console.WriteLine("Select game type:");
            Console.WriteLine("1. Two Players");
            Console.WriteLine("2. Against Computer");
            int choice;

            while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            {
                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            }

            return (eGameType)(choice - 1);
        }

        private void initializeGame()
        {
            string stringInputRows, stringInputCols;
            int intInputRows, intInputCols;

            Console.Write("Enter the number of rows (minimum 4, maximum 8): ");
            stringInputRows = Console.ReadLine();
            while (!(int.TryParse(stringInputRows, out intInputRows) && isMatrixSizeValid(intInputRows)))
            {
                Console.WriteLine("Invalid input. Please enter a number of rows between 4 and 8.");
                stringInputRows = Console.ReadLine();
            }

            Console.Write("Enter the number of columns (minimum 4, maximum 8): ");
            stringInputCols = Console.ReadLine();
            while (!(int.TryParse(stringInputCols, out intInputCols) && isMatrixSizeValid(intInputCols)))
            {
                Console.WriteLine("Invalid input. Please enter a number of columns between 4 and 8.");
                stringInputCols = Console.ReadLine();
            }

            m_GameBoard = new BoardGameManager(intInputRows, intInputCols);
            m_GameType = getGameTypeFromUser();
        }

        private bool isMatrixSizeValid(int i_RowsOrCols)
        {
            return i_RowsOrCols >= r_MinColsOrRows &&
                i_RowsOrCols <= r_MaxColsOrRows;
        }

        private void printBoard()
        {
            eShapes[,] board = m_GameBoard.Board;
            int boardRows = board.GetLength(0);
            int boardColumns = board.GetLength(1);

            Console.Write("  ");
            for (int i = 1; i <= boardColumns; i++)
            {
                Console.Write(i + "   ");
            }

            for (int i = boardRows - 1; i >= 0; i--)
            {
                Console.WriteLine();
                for (int j = 0; j < boardColumns; j++)
                {
                    Console.Write(string.Format("| {0} ", (char)board[i, j]));
                }

                Console.WriteLine("|");
                Console.Write(new string('=', boardColumns * 4 + 1));
            }

            Console.WriteLine();
        }

        private string getUserInput()
        {
            ePlayer currentPlayer = (m_GameBoard.CurrentPlayer == eShapes.X) ? ePlayer.Player1 : ePlayer.Player2;
            string colMessage = string.Format("{0} Select a column to drop your piece 1- {1}", currentPlayer, m_GameBoard.Columns);

            Console.WriteLine(colMessage);

            return Console.ReadLine();
        }

        private bool isNotQuitAndNotValid(string i_ColInput, out int io_ColumnIntInput)
        {
            string userInput = i_ColInput;
            bool isQuit = isLeave(userInput);

            io_ColumnIntInput = getInputInt(userInput);
            while (!isQuit && io_ColumnIntInput == -1)
            {
                Console.WriteLine("Please try again");
                userInput = getUserInput();
                isQuit = isLeave(userInput);
                io_ColumnIntInput = getInputInt(userInput);
            }

            return isQuit;
        }

        private int playerMove(int i_ColInput)
        {
            return m_GameBoard.MakePlayerMove(i_ColInput - 1);
        }


        private int getInputInt(string i_StringInput)
        {
            int intInput;

            if (!int.TryParse(i_StringInput, out intInput))
            {
                intInput = -1;
            }

            return intInput;
        }

        private bool computerMove()
        {
            bool isComputerWin = false;
            int winningRow, winningCol;

            if (m_GameBoard.IsColumnOfFourExist(out winningRow, out winningCol))
            {
                m_GameBoard.DropPiece(winningRow, winningCol, eShapes.O);
                Screen.Clear();
                printBoard();
                m_GameBoard.GameScores.PlayerTwoScores++;
                isComputerWin = true;
            }
            else
            {
                m_GameBoard.MakeComputerMove();
            }

            return isComputerWin;
        }

        private bool isLeave(string i_InputString)
        {
            return i_InputString == "Q";
        }

        private bool isCountinueSameGame()
        {
            bool isCountinue = false;

            Console.WriteLine("Do you want to countinue with the same board, press y(yes) or n(no)");
            string userInput = Console.ReadLine();

            while (userInput != "y" && userInput != "n")
            {
                Console.WriteLine("Invalid input, please try again");
                userInput = Console.ReadLine();
            }

            return userInput == "n" ? isCountinue : !isCountinue;
        }

        private void newGame()
        {
            m_GameBoard.SetCurrentPlayer();
            m_GameBoard.InitBoard();
            Screen.Clear();
            StartGame();
        }

        private void printFinalScores()
        {
            string finalScores = string.Format("Player 1 scores: {0}, Player 2 scores: {1}"
                                , m_GameBoard.GameScores.PlayerOneScores, m_GameBoard.GameScores.PlayerTwoScores);

            Console.WriteLine(finalScores);
        }
    }
}

