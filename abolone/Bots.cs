namespace abalone
{
    public class Bots
    {

        public static int[] callBot(string botName, int player, int[,] grid)
        {
            switch (botName.ToLower())
            {
                case ("rndbot" or "1"):
                    return RndBot(player, grid);
                case ("scoringbot" or "2"):
                    return ScoringBot(player, grid);
                case ("heuristicbot" or "3"):
                    return HeuristicBot(player, grid);
                case ("purescoringbot" or "4"):
                    return DeterministicScoringBot(player, grid);
                case ("depthscoringbot" or "5"):
                    return DepthScoringBot(player, grid);
                case ("simulatorbot" or "6"):
                    return SimulatorBot(player, grid);
                default: throw new ArgumentException("ERROR");
            }
        }

        //bot template

        public static int[] BotTemplate(int player, int[,] grid) //tells the bot which player it is
        {
            // Program.grid[]  uses this as its input for the grid
            int x = 0;
            int y = 0;
            int direction = 0;

            bool works = Program.CheckMove(0, 0, 5, grid); // smth like this can be used to check if it works.

            // process and return x,y and direction

            return new int[3] { x, y, direction }; // return in the form of { x , y , direction }
        }  

        public static int[] RndBot(int player, int[,] grid)
        {
            int x;
            int y;
            int direction;

            do
            {
                x = Random.Shared.Next(0, 9);
                y = Random.Shared.Next(0, 9);
                direction = Random.Shared.Next(0, 6);
            } while (!Program.CheckMove( x, y, direction, grid) || player != grid[y, x]);


            return new int[3] { x, y, direction };
        }

        public static int[] ScoringBot(int player, int[,] grid)
        {

            List<int[]> legalMoves = Program.LegalMoves(grid, player);

            int bestScore = -50000;
            int[] bestMove = new int[3] { 0, 0, 0 };
            int denominator = 1;


            int i = 0;
            foreach (int[] legalMove in legalMoves)
            {
                int[,] actionedGrid = Program.Move(legalMove[0], legalMove[1], legalMove[2], (int[,])grid.Clone());

                int score = Program.Score(actionedGrid, player);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = legalMove;

                    denominator = 2;
                }
                else if (score == bestScore)
                {
                    if (Random.Shared.Next(0, denominator) == 0)
                    {
                        bestScore = score;
                        bestMove = legalMove;
                    }
                    denominator++;
                }



                i++;
            }



            return bestMove;
        }

        public static int[] DeterministicScoringBot(int player, int[,] grid)
        {
            int x = 0;
            int y = 0;
            int direction = 0;

            List<int[]> legalMoves = Program.LegalMoves(grid, player);

            int bestScore = -5000000;
            int[] bestMove = new int[3] { 0, 0, 0 };



            int i = 0;
            foreach (int[] legalMove in legalMoves)
            {
                int[,] actionedGrid = Program.Move(legalMove[0], legalMove[1], legalMove[2], (int[,])grid.Clone());

                int score = Program.Score(actionedGrid, player);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = legalMove;
                }

                i++;
            }



            return bestMove;
        }

        public static int[] SimulatorBot( int player, int[,] grid)
        {
            int opponent = (player == 2) ? 3 : 2;

            int gamesPerPossibleMove = 3;

            GameRunner runner = new GameRunner("2", "2");

            List<int[]> legalMoves = Program.LegalMoves(grid, player);

            int bestScore = -50000;
            int[] bestMove = new int[3] { 0, 0, 0 };
            int denominator = 1;



            int i = 0;
            foreach (int[] legalMove in legalMoves)
            {
                int[,] actionedGrid = Program.Move(legalMove[0], legalMove[1], legalMove[2], (int[,])grid.Clone());
                runner.StartingGrid = actionedGrid;

                int score = 0;

                for (int j = 0; j < gamesPerPossibleMove; j++)
                {
                    int gameResult = runner.RunGame();

                    if (gameResult == player)
                    {
                        score++;
                    }
                    else if (!(gameResult == 0))
                    {
                        score--;
                    }
                }


                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = legalMove;

                    denominator = 2;
                }
                else if (score == bestScore)
                {
                    if (Random.Shared.Next(0, denominator) == 0)
                    {
                        bestScore = score;
                        bestMove = legalMove;
                    }
                    denominator++;
                }
                i++;
            }


                return bestMove;
        } // Kind of sucks

        public static int[] DepthScoringBot(int player, int[,] grid)
        {
            int opponent = player == 2 ? 3 : 2;
            int depth = 2;
            int x = 0;
            int y = 0;
            int direction = 0;


            List<int[]> legalMoves = Program.LegalMoves(grid, player);

            int bestScore = -50000000;
            int[] bestMove = new int[3] { 0, 0, 0 };

            int denominator = 1;


            int i = 0;
            foreach (int[] legalMove in legalMoves)
            {
                int[,] actionedGrid = Program.Move(legalMove[0], legalMove[1], legalMove[2], (int[,])grid.Clone());

                List<int[]> legalMovesIndented = Program.LegalMoves(actionedGrid, opponent);

                int score = 50000000;

                foreach (int[] legalMoveIndented in legalMovesIndented)
                {
                    int[,] actionedGridIndented = Program.Move(legalMoveIndented[0], legalMoveIndented[1], legalMoveIndented[2], (int[,])actionedGrid.Clone());
                    int scoreIndented = Program.Score(actionedGridIndented, player);
                    if (scoreIndented < score)
                    {
                        score = scoreIndented;
                    }

                }


                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = legalMove;

                    denominator = 2;
                }
                else if (score == bestScore)
                {
                    if (Random.Shared.Next(0, denominator) == 0)
                    {
                        bestScore = score;
                        bestMove = legalMove;
                    }
                    denominator++;
                }
                i++;
            }



            return bestMove;
        }

        public static (int[]? expectedMove, int expectedScore) DepthScoringBotHelper(int player, int playerMoving, int[,] grid, int depth)
        {
            if (depth < 0) throw new ArgumentOutOfRangeException("Depth must be a natural number or 0");

            bool same = player == playerMoving;



            int winner = Program.Winner(grid);
            if(winner != 0)
            {
                if (winner == player)
                {
                    return (null, 100000000);
                }
                else return (null, -100000000);

            }
            if(depth == 0)
            {
                return (null, Program.Score(grid, player));
            }
            else
            {
                List<int[]> legalMoves = Program.LegalMoves(grid, playerMoving);
                int[][,] actionedGrids = new int[legalMoves.Count][,];
                int[] scoresOfGrids = new int[legalMoves.Count];
                for(int i = 0;  i < legalMoves.Count; i++)
                {
                    actionedGrids[i] = Program.Move(legalMoves[i], (int[,])grid.Clone());
                    (_, scoresOfGrids[i]) = DepthScoringBotHelper(player, (playerMoving == 2) ? 3 : 2, actionedGrids[i], depth - 1);

                    if (same && scoresOfGrids[i] == 100000000)
                    {
                        return (legalMoves[i], scoresOfGrids[i]);
                    }
                    else if (!same && scoresOfGrids[i] == -100000000)
                    {
                        return (legalMoves[i], scoresOfGrids[i]);
                    }
                }



                int? expectedScore = null;
                int[] expectedMove = (int[])Program.defaultNullMove.Clone();
                int denominator = 2;

                if (!same)
                {
                    
                    for (int i = 0; i < legalMoves.Count; i++)
                    {
                        int[] legalMove = legalMoves[i];
                        int[,] actionedGrid = actionedGrids[i];

                        int score = scoresOfGrids[i];

                        if (expectedScore == null)
                        {
                            expectedScore = score;
                            expectedMove = legalMove;
                        }
                        if (score < expectedScore)
                        {
                            expectedScore = score;
                            expectedMove = legalMove;

                            denominator = 2;
                        }
                        else if (score == expectedScore)
                        {
                            if (Random.Shared.Next(0, denominator) == 0)
                            {
                                expectedScore = score;
                                expectedMove = legalMove;
                            }
                            denominator++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < legalMoves.Count; i++)
                    {
                        int[] legalMove = legalMoves[i];
                        int[,] actionedGrid = actionedGrids[i];

                        int score = scoresOfGrids[i];

                        if (expectedScore == null)
                        {
                            expectedScore = score;
                            expectedMove = legalMove;
                        }
                        if (score > expectedScore)
                        {
                            expectedScore = score;
                            expectedMove = legalMove;

                            denominator = 2;
                        }
                        else if (score == expectedScore)
                        {
                            if (Random.Shared.Next(0, denominator) == 0)
                            {
                                expectedScore = score;
                                expectedMove = legalMove;
                            }
                            denominator++;
                        }
                    }
                }

                //Handle for when no legal moves needed


                return (expectedMove, (int)expectedScore);
            }
        }

        public static int[] HeuristicBot(int player, int[,] grid)
        {
            Random rnd = new Random();

            if(player == 3) Program.SwapPlayers(grid);

            player = 2;
            int x = 0;
            int y = 0;
            int direction = 0;
            int gridIndex = -1;
            uint[] compactGrid = Functions.CompactGrid(grid);
            //int indexReturn = -1;

            List<int[]> actions = Program.LegalMoves(grid, player);
            int noActions = actions.Count;

            gridIndex = Functions.states.IndexOf(compactGrid);

            if (gridIndex == -1)
            {
                gridIndex = Functions.AddState(compactGrid, 1000d, noActions);
                return actions[0];
            }

            double bestWinChance = 0d;
            int bestActionIndex = -1;

            for (int i = 0; i < noActions; i++)
            {
                if (Functions.actionWinProbabability[gridIndex][i] > bestWinChance)
                {
                    bestWinChance = Functions.actionWinProbabability[gridIndex][i];
                    bestActionIndex = i;
                }
            }


            x = actions[bestActionIndex][0];
            y = actions[bestActionIndex][1];
            direction = actions[bestActionIndex][2];

            /*
            int[] nextStates = new int[noActions];
            for(int i = 0; i < noActions; i++)
            {
                uint[] nextStateCompact = Functions.CompactGrid(Program.Move(actions[i][0], actions[i][1], actions[i][2], (int[,])grid.Clone()));

                int indexNextState = Functions.states.IndexOf(nextStateCompact);

                if(indexNextState == -1)
                {
                    indexNextState = Functions.AddState(nextStateCompact, 1000d, noActions);
                }

                nextStates[i] = indexNextState;
            }

            double[] nextStateScores = new double[noActions];

            //to find nextStateScores
            for (int i = 0; i < noActions; i++)
            {
                nextStateScores[i] = Functions.score[nextStates[i]];
            }



            double sumScores = nextStateScores.Sum();
            double rndNum = rnd.NextDouble() * sumScores;

            for(int i = 0; i < noActions; i++)
            {
                rndNum -= nextStateScores[i];
                if(rndNum<= 0)
                {
                    indexReturn = i;
                    break;
                }
            }
            if(indexReturn == -1)
            {
                indexReturn = noActions - 1;
            }


            return actions[indexReturn];
            */

            return new int[4] { x, y, direction, bestActionIndex };
        }

        public static int[] NeuralBot(int[,] grid, int player,NeuralBot bot)
        {
            List<int[]> legalMoves = Program.LegalMoves(grid, player);



            double bestScore = double.MinValue;
            int[] bestMove = new int[3] { 0, 0, 0 };
            int denominator = 1;


            int i = 0;
            foreach (int[] legalMove in legalMoves)
            {
                int[,] actionedGrid = Program.Move(legalMove[0], legalMove[1], legalMove[2], (int[,])grid.Clone());

                Matrix input = GetData.GetInput(actionedGrid, player);

                Matrix output = bot.ComputeOutput(input);

                double score = output[0,0];

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = legalMove;

                    denominator = 2;
                }
                else if (score == bestScore)
                {
                    if (Random.Shared.Next(0, denominator) == 0)
                    {
                        bestScore = score;
                        bestMove = legalMove;
                    }
                    denominator++;
                }



                i++;
            }



            return bestMove;
        }




        // RIGHT NOW: inputs --> middle layer --> outputs
        
        public static int[] OldNeuralBot(int player, int[,] grid, OldNeuralBot bot)
        {
            Matrix input1 = new Matrix(62, 1);
            input1[61, 0] = 1;
            Matrix middleLayer1 = new Matrix(21, 1);
            Matrix output1 = new Matrix(84, 1);

            Matrix previousInput1 = new Matrix(62, 1);
            previousInput1[61, 0] = 1;
            return bot.ChooseMove(player, grid, input1, middleLayer1, output1, previousInput1, true);

            // return in the form of { x , y , direction }
        }
        

    }
}
