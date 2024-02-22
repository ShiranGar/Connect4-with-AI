using GameLogic;
using System;
using Ex02.ConsoleUtils;

namespace UI
{
    public class ConnectFourUI
    {
        private ConnectFourGame m_Game;
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
            string userInput = GetUserInput();

            while (!m_Game.IsBoardFull())
            {
                bool isQuit;
                int columnIntInput;
                isQuit = isNotQuitAndNotValid(userInput, out columnIntInput);

                if (isQuit)
                {
                    m_Game.GameScores.AddOnePointToOpponent(m_Game.CurrentPlayer);
                    break;
                }
                if(!m_Game.IsInMatrixRange(columnIntInput))
                {
                    Console.WriteLine("Not in matrix range. Please try again");
                    userInput = GetUserInput();
                    continue;
                }
                else if (m_Game.IsColFull(columnIntInput - 1))
                {
                    Console.WriteLine("This column is full, please try again");
                    userInput = GetUserInput();
                    continue;
                }

                playerMove(columnIntInput);
                Screen.Clear();
                printBoard();
                if (m_Game.IsGameOver(m_Game.CurrentPlayer))
                {
                    break;
                }

                if (m_GameType == eGameType.AgainstComputer)
                {
                    if(computerMove())
                    {
                        break;
                    }
                }

                Screen.Clear();
                printBoard();
                if (m_Game.IsGameOver(m_Game.CurrentPlayer))
                {
                    break;
                }
                else
                {
                    userInput = GetUserInput();
                }
            }

            if (m_Game.GameScores.IsZeroTie())
            {
                Console.WriteLine("It's a tie");
            }
            else
            {
                Console.WriteLine(m_Game.GetWinner() + " you're the winner!");
            }

            Console.WriteLine(m_Game.GameScores.ShowScores());
            NewGame();
        }

        public eGameType GetGameTypeFromUser()
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

            m_Game = new ConnectFourGame(intInputRows, intInputCols);
            m_GameType = GetGameTypeFromUser();
        }

        private bool isMatrixSizeValid(int i_RowsOrCols)
        {
            return i_RowsOrCols >= r_MinColsOrRows &&
                i_RowsOrCols <= r_MaxColsOrRows;
        }

        private void printBoard()
        {
            eShapes[,] board = m_Game.Board;
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

        public string GetUserInput()
        {
            ePlayer currentPlayer = (m_Game.CurrentPlayer == eShapes.X) ? ePlayer.Player1 : ePlayer.Player2;
            string colMessage = string.Format("{0} Select a column to drop your piece 1- {1}", currentPlayer, m_Game.Columns);

            Console.WriteLine(colMessage);

            return Console.ReadLine();
        }

        private bool isNotQuitAndNotValid(string i_ColInput, out int io_ColumnIntInput)
        {
            string userInput = i_ColInput;
            bool isQuit = IsLeave(userInput);

            io_ColumnIntInput = getInputInt(userInput);
            while (!isQuit && io_ColumnIntInput == -1)
            {
                Console.WriteLine("Please try again");
                userInput = GetUserInput();
                isQuit = IsLeave(userInput);
                io_ColumnIntInput = getInputInt(userInput);
            }

            return isQuit;
        }

        private int playerMove(int i_ColInput)
        {
            return m_Game.MakePlayerMove(i_ColInput - 1);
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

            if(m_Game.CheckConnect4(out winningRow, out winningCol))
            {
                m_Game.DropPiece(winningRow, winningCol, eShapes.O);
                Screen.Clear();
                printBoard();
                m_Game.GameScores.PlayerTwoScores++;
                isComputerWin = true;
            }
            else
            {
                m_Game.MakeComputerMove();
            }

            return isComputerWin;
        }

        public bool IsLeave(string i_InputString)
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

        public void NewGame()
        {
            m_Game.SetCurrentPlayer();
            if (isCountinueSameGame())
            {
                m_Game.InitBoard();
            }
            else
            {
                initializeGame();
            }

            Screen.Clear();
            StartGame();
        }
    }
}

