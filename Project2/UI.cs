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
            InitializeGame();
        }

        public void StartGame()
        {
            PrintBoard();
            string userInput = GetUserInput();

            while (!m_Game.IsBoardFull())
            {
                bool isQuit;
                int columnIntInput;
                (isQuit, columnIntInput) = isNotQuitAndNotValid(userInput);
                if (isQuit)
                {
                    m_Game.GameScores.addOnePoint(m_Game.CurrentPlayer);
                    break;
                }
                if (!m_Game.IsValidLocation(columnIntInput))
                {
                    Console.WriteLine("Please try again");
                    userInput = GetUserInput();
                    continue;
                }
                PlayerMove(columnIntInput);
                Screen.Clear();
                PrintBoard();
                if (m_Game.IsGameOver())
                {
                    break;
                }
                if (m_GameType == eGameType.AgainstComputer)
                {
                    ComputerMove();
                }
                Screen.Clear();
                PrintBoard();
                if (m_Game.IsGameOver())
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
                Console.WriteLine(m_Game.getWinner() + " you're the winner!");
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

        private void InitializeGame()
        {
            string stringInputRows, stringInputCols;
            int intInputRows, intInputCols;
            Console.Write("Enter the number of rows (minimum 4, maximum 8): ");
            stringInputRows = Console.ReadLine();
            while (!(int.TryParse(stringInputRows, out intInputRows) && IsMatrixSizeValid(intInputRows)))
            {
                Console.WriteLine("Invalid input. Please enter a number of rows between 4 and 8.");
                stringInputRows = Console.ReadLine();
            }

            Console.Write("Enter the number of columns (minimum 4, maximum 8): ");
            stringInputCols = Console.ReadLine();
            while (!(int.TryParse(stringInputCols, out intInputCols) && IsMatrixSizeValid(intInputCols)))
            {
                Console.WriteLine("Invalid input. Please enter a number of columns between 4 and 8.");
                stringInputCols = Console.ReadLine();
            }

            m_Game = new ConnectFourGame(intInputRows, intInputCols);
            m_GameType = GetGameTypeFromUser();
        }

        private bool IsMatrixSizeValid(int i_RowsOrCols)
        {
            return i_RowsOrCols >= r_MinColsOrRows &&
                i_RowsOrCols <= r_MaxColsOrRows;
        }

        private void PrintBoard()
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
            Console.WriteLine("Select a column to drop your piece 1-" + m_Game.Columns);

            return Console.ReadLine();
        }

        private (bool, int) isNotQuitAndNotValid(string i_ColInput)
        {
            string userInput = i_ColInput;
            bool isQuit = IsLeave(userInput);
            int intInput = getInputInt(userInput);

            while (!isQuit && intInput == -1)
            {
                Console.WriteLine("Please try again");
                userInput = GetUserInput();
                isQuit = IsLeave(userInput);
                intInput = getInputInt(userInput);
            }

            return (isQuit, intInput);
        }

        private int PlayerMove(int i_ColInput)
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

        public bool IsColumnValid(int i_Column)
        {
            return i_Column >= 1 && i_Column <= m_Game.Board.GetLength(1);
        }

        private void ComputerMove()
        {
            m_Game.MakeComputerMove();
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
                InitializeGame();
            }
            Screen.Clear();
            StartGame();
        }
    }
}

