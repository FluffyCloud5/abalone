

namespace abalone
{
    public class GameRunner
    {
        private Func<int[,], int, int[]> _bot1;
        private Func<int[,], int, int[]> _bot2;
        private int[,] _startingGrid = (int[,])Program.startGridTemplate.Clone();
        private int[,]? _finalGrid;
        private int _startingPlayer = 2;



        public string afterGameAnalysis = "Winner = {0}, TotalMoves = {1}"; // 0 corrilates to the winner, 1 corrilates to the the number of moves made.
        public int MaxMovesPerGame = 5000;
        public bool saveFinalGrid = false;
        public bool ShowGame = false;
        public uint sleepXMilliseconds = 0;


        public GameRunner(string bot1, string bot2)
        {
            bot1 = bot1.ToLower();
            bot2 = bot2.ToLower();

            if (bot1 == null || bot2 == null) throw new Exception("ERROR");
            try
            {
                Bots.callBot(bot1, 2, (int[,])Program.startGridTemplate.Clone());
                Bots.callBot(bot2,2, (int[,])Program.startGridTemplate.Clone());
            }
            catch { throw new Exception("ERROR"); }



            _bot1 = (grid, player) => 
            {
                return Bots.callBot(bot1, player, grid);
            };
            _bot2 = (grid, player) =>
            {
                return Bots.callBot(bot2, player, grid);
            };    
        }

        public GameRunner(Func<int[,], int, int[]> bot1, Func<int[,], int, int[]> bot2)
        {
            try
            {
                int[] outputBot1 = bot1((int[,])Program.startGridTemplate.Clone(), 2);
                if (outputBot1.Length != 3)
                {
                    throw new Exception();
                }
                else if (!Program.CheckMove(outputBot1, (int[,])Program.startGridTemplate.Clone()))
                {
                    throw new Exception();
                }

                int[] outputBot2 = bot2((int[,])Program.startGridTemplate.Clone(), 2);
                if (outputBot2.Length != 3)
                {
                    throw new Exception();
                }
                else if (!Program.CheckMove(outputBot2, (int[,])Program.startGridTemplate.Clone()))
                {
                    throw new Exception();
                }
            }
            catch { throw new Exception("ERROR"); }

            _bot1 = bot1;
            _bot2 = bot2;
        }

        public int StartingPlayer
        {
            get => _startingPlayer;
            set
            {
                if (value == 2 || value == 3) _startingPlayer = value;
                else throw new ArgumentOutOfRangeException($"value was out of range; must be 2 or 3. Was: {value}");
            }
        }

        public int[,] FinalGrid
        {
            get
            {
                if (_finalGrid == null) throw new Exception("No game played");

                if (!saveFinalGrid) throw new Exception("Final Grid Not Saved");

                return _finalGrid;
            }
        }

        public int[,] StartingGrid
        {
            get => (int[,])_startingGrid.Clone();
            set
            {
                if (value.GetLength(0) != 9 || value.GetLength(1) != 9 || value == null) throw new Exception("ERROR");

                _startingGrid = (int[,])value.Clone();
            }
        }

        public Func<int[,], int, int[]> Bot1FuncIn
        {
            set
            {
                try
                {
                    int[] outputBot1 = value((int[,])Program.startGridTemplate.Clone(), 2);
                    if (outputBot1.Length != 3)
                    {
                        throw new Exception();
                    }
                    else if (!Program.CheckMove(outputBot1, (int[,])Program.startGridTemplate.Clone()))
                    {
                        throw new Exception();
                    }
                }
                catch { throw new Exception("ERROR"); }

                _bot1 = value;
            }
        }

        public string Bot1StringIn
        {
            set
            {
                value = value.ToLower();

                if (value == null) throw new Exception("ERROR");
                try
                {
                    Bots.callBot(value, 2, (int[,])Program.startGridTemplate.Clone());
                }
                catch { throw new Exception("INVALID BOT"); }



                _bot1 = (grid, player) =>
                {
                    return Bots.callBot(value, player, grid);
                };
            }
        }

        public Func<int[,], int, int[]> Bot2FuncIn
        {
            set
            {
                try
                {
                    int[] outputBot2 = value((int[,])Program.startGridTemplate.Clone(), 2);
                    if (outputBot2.Length != 3)
                    {
                        throw new Exception();
                    }
                    else if (!Program.CheckMove(outputBot2, (int[,])Program.startGridTemplate.Clone()))
                    {
                        throw new Exception();
                    }
                }
                catch { throw new Exception("ERROR"); }

                _bot2 = value;
            }
        }

        public string Bot2StringIn
        {
            set
            {
                value = value.ToLower();

                if (value == null) throw new Exception("ERROR");
                try
                {
                    Bots.callBot(value, 2, (int[,])Program.startGridTemplate.Clone());
                }
                catch { throw new Exception("INVALID BOT"); }



                _bot2 = (grid, player) =>
                {
                    return Bots.callBot(value, player, grid);
                };
            }
        }



        public int RunGame() // 0 is draw, 2 --> 2 won, 3 --> 3 won
        {
            int[,] grid = ((int[,])_startingGrid.Clone());
            



            bool twoWins = false;
            bool threeWins = false;

            int startLine = 0;

            if (ShowGame)
            {
                int left = 0;
                (left, startLine) = Console.GetCursorPosition();
                if (left > 0) startLine++;
                Console.CursorVisible = false;
                Console.Clear();
                Program.Show(grid);
            }
            if (sleepXMilliseconds != 0) Thread.Sleep((int)sleepXMilliseconds);


            int i = 0;
            for (i = 0; i < MaxMovesPerGame; i++)
            {
                int[] move = new int[3];
                if ((i+_startingPlayer) % 2 == 0)
                {
                    move = _bot1((int[,])grid.Clone(), 2);
                }
                else
                {
                    move = _bot2((int[,])grid.Clone(), 3);
                }

                int x = move[0];
                int y = move[1];
                int direction = move[2];


                if (grid[y, x] == ((i+_startingPlayer) % 2) + 2)
                {
                    Program.Move(x, y, direction, grid);
                }

                if (ShowGame)
                {

                    Console.SetCursorPosition(startLine, 0);
                    Program.Show(grid);

                }
                if (sleepXMilliseconds != 0) Thread.Sleep((int)sleepXMilliseconds);


                if (Program.NoPieces(grid, 2) < 9)
                {
                    threeWins = true;
                    break;
                }
                else if (Program.NoPieces(grid, 3) < 9)
                {
                    twoWins = true;
                    break;
                }
            }

            if(saveFinalGrid)
            {
                _finalGrid = grid;
            }

            if (ShowGame)
            {
                try
                {
                    Console.WriteLine(string.Format(afterGameAnalysis, (twoWins) ? 2 : (threeWins ? 3 : 0), i));
                }
                catch { }
            }

            if (twoWins)
                return 2;
            else if (threeWins)
                return 3;
            else return 0;
        }
    }
}
