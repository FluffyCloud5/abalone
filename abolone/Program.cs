using System.Diagnostics;

namespace abalone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NeuralBot bot = NeuralBot.Parse(File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\backProb5.csv"));
            int[][,] grids = GetData.GenerateRandomGrids(20);

            for (int i = 0; i < grids.GetLength(0); i++)
            {
                Console.Clear();
                Show(grids[i]);

                Console.WriteLine((double)Score(grids[i], 2)/2000d-1);
                Console.WriteLine(bot.ComputeOutput( GetData.GetInput(grids[i],2)));

                Console.ReadKey();


            }
            


        }
        //Different main sandboxes
        #region
        //training
        static void main2()
        {
            //Makes the bot

            #region

            int batchSize = 367;
            double learningRate = Math.Pow(10, -3);
            int batches = 200000;//11.613 seconds, 2000 batches  60*20 = 1200s --> 2000*100 = 200000


            //int RecCount = 1000;

            Console.WriteLine("started");
            Stopwatch s = new Stopwatch();
            s.Start();

            Console.Write("loaded Bot in: ");

            //NeuralBot bot = new NeuralBot(61, new int[] { 30, 30 }, 1);
            NeuralBot bot = NeuralBot.Parse(File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\backProb5.csv"));
            
            Console.WriteLine((double)s.ElapsedMilliseconds / 1000);


            s.Restart();
            Console.Write("Loaded data in: ");
            GetData datageter = GetData.Parse(File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_400000recordsCleaned2.txt"));
            //datageter.AddRecords(RecCount);
            Console.WriteLine((double)s.ElapsedMilliseconds / 1000);


            s.Restart();
            Console.Write("initialised in: ");
            SupervisedBackpropagation trainer = new SupervisedBackpropagation(datageter.inputData, datageter.outputData, bot);
            trainer.BatchSize = batchSize;
            trainer.Batches = batches;
            trainer.learningRate = learningRate;
            Console.WriteLine((double)s.ElapsedMilliseconds / 1000);


            s.Restart();
            Console.Write("training time: ");
            trainer.Run();
            Console.WriteLine((double)s.ElapsedMilliseconds / 1000);





            s.Restart();
            Console.Write("saving Time: ");
            File.WriteAllText("C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\backProb5.csv", bot.ToString());
            Console.WriteLine((double)s.ElapsedMilliseconds / 1000);
            #endregion

        } 
        //competing
        static void main3()
        {

            #region
            string str = File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\backProb5.csv");
            //NeuralBot bot = new NeuralBot(61,new int[] { 20,20},1);
            NeuralBot bot = NeuralBot.Parse(str);




            Func<int[,], int, int[]> neuralBot = ((grid, player) =>
            {
                return Bots.NeuralBot(grid, player, bot);
            });

            Func<int[,], int, int[]> bot3 = ((grid, player) =>
            {
                return Bots.ScoringBot(player, grid);
            });


            GameRunner r = new GameRunner(neuralBot, bot3);
            r.ShowGame = true;
            // r.sleepXMilliseconds = 200;

            r.RunGame();


            /*
            r.ShowGame = true;
            int wins = 0;
            int draws = 0;
            int total = 30000;

            
            Stopwatch sw = Stopwatch.StartNew();

            for (long i = 0; i < total; i++)
            {
                int filler = r.RunGame();
                if(filler == 0) draws++;
                if(filler == 2) wins++;

                

                if (i % (int)(total/50) == 0)
                {
                    Console.Clear();

                    Console.WriteLine($"{ (float)i * (float)100 / (float)total}%");
                    if (i != 0)
                    {
                        Console.WriteLine($"expected total time = {(int)(sw.ElapsedMilliseconds * total * 0.001 / i)}s");
                        Console.WriteLine($"expected time remaining = {(int)((sw.ElapsedMilliseconds * total * 0.001 / i) - sw.ElapsedMilliseconds / 1000)}s");
                    }
                }

               
            }
            Console.WriteLine(sw.ElapsedMilliseconds/1000d);
            Console.WriteLine(draws);
            Console.WriteLine(wins);
            */

            //int[][,] rndGrid = GetData.GenerateRandomGrids(1);
            //Show(rndGrid[0]);

            //Console.WriteLine(bot.ComputeOutput(GetData.GetInput(rndGrid[0], 2))[0, 0]);
            //Console.WriteLine(Score(rndGrid[0], 2));

            //Console.WriteLine(bot.ComputeOutput(GetData.GetInput(rndGrid[0], 3))[0, 0]);
            //Console.WriteLine(Score(rndGrid[0], 3));


            //Console.WriteLine(bot.biases[1][0, 0]);
            //Console.WriteLine($"{bot.biases[1].rows}, {bot.biases[1].cols}");
            #endregion

        }
        //generating data
        static void main4()
        {
            int RecCount = 400000;

            Console.WriteLine($"estimated time = {128d / 40000d * (double)RecCount}s"); // if running on one thread.

            GetData datagetter = new GetData();

            Stopwatch s = Stopwatch.StartNew();
            datagetter.parallelAddRecords(RecCount);
            Console.WriteLine(s.ElapsedMilliseconds);
            s.Stop();

            int i = 2;
            string filePath = $"C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_{RecCount}records";



            while (File.Exists(filePath + i + ".txt"))
            {
                i++;
            }

            if (!File.Exists(filePath + ".txt"))
                filePath += ".txt";
            else filePath += i + ".txt";



            File.WriteAllText(filePath, datagetter.ToString());

        }

        static void main5()
        {
            
            Stopwatch sw = Stopwatch.StartNew();
            Thread.Sleep(1000);
            sw.Restart();
            GetData getData;
            {


                string str = File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_400000records2.txt");
                Console.WriteLine(sw.ElapsedMilliseconds);

                sw.Restart();

                getData = GetData.Parse(str);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);

            //Console.WriteLine("input data length: {0}", getData.inputData.Length);

            


            Console.ReadKey();
        }
        //merging data
        static void main6()
        {
            GetData[] getData = new GetData[5];

            getData[0] = GetData.Parse(File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_4000records.txt"));

            getData[0].runCheck();

            for (int i = 1; i < getData.Length; i++)
            {
                getData[i] = GetData.Parse(File.ReadAllText($"C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_4000records{i + 1}.txt"));
                getData[i].runCheck();
            }

            for (int i = 1; i < getData.Length; i++)
            {
                getData[0].Merge(getData[i]);
            }

            //Console.WriteLine(getData[0].inputData.Length);




            File.WriteAllText($"C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_{getData[0].inputData.Length}records.txt", getData[0].ToString());
        }

        static void main7()
        {

            Stopwatch sw = Stopwatch.StartNew();
            GetData datageter = GetData.Parse(File.ReadAllText("C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_400000records.txt"));





            Console.WriteLine("gotData in {0}s", (double)sw.ElapsedMilliseconds / 1000d);





            /*
            int[] forceWINs = new int[]
            { 13,22,86,95,104,108,142,143,155,208,222,227,274,289,315,316,322,331,398,455,466,469,477,509,531,545,570,614,615,630,631,634,667,682,695,725,737,825,882,910,921,932,950,973,988,1023,1039,1044,1069,1127,1128,1143,1145,1168,1171,1182,1213,1229,1266,1306,1366,1395,1435,1464,1488,1489,1506,1569,1581,1655,1701,1727,1732,1735,1740,1748,1779,1797,1820,1825,1828,1833,1867,1921,1934,1953,2022,2035,2062,2071,2092,2124,2201,2216,2221,2246,2248,2272,2283,2286,2301,2307,2314,2317,2348,2369,2375,2394,2404,2407,2418,2453,2479,2505,2554,2558,2578,2635,2654,2697,2700,2704,2711,2752,2762,2777,2784,2785,2788,2789,2807,2933,2954,2975,3002,3006,3007,3085,3092,3123,3142,3148,3164,3173,3176,3228,3236,3254,3274,3277,3292,3294,3339,3384,3398,3420,3595,3631,3642,3699,3736,3744,3752,3767,3770,3788,3802,3825,3835,3910,3922,3932,3935,3940,3946,3948,3957,3977,3980,4014,4022,4041,4102,4112,4138,4152,4176,4210,4287,4303,4315,4366,4419,4469,4478,4496,4511,4515,4529,4536,4537,4549,4565,4580,4602,4618,4624,4634,4638,4643,4647,4661,4674,4695,4727,4730,4756,4766,4792,4815,4872,4873,4879,4909,4913,4918,4924,4929,4938,4945,4979,5051,5087,5102,5126,5198,5235,5244,5246,5288,5310,5348,5376,5421,5442,5451,5460,5494,5516,5526,5549,5554,5569,5598,5604,5650,5656,5669,5714,5723,5735,5788,5808,5859,5892,5934,5941,5986,6026,6029,6036,6069,6086,6116,6138,6145,6163,6184,6212,6270,6328,6330,6356,6363,6411,6434,6445,6449,6478,6495,6497,6553,6554,6606,6615,6621,6632,6682,6696,6719,6735,6747,6755,6775,6809,6847,6892,6903,6906,6911,6939,6969,6976,6981,6995,7027,7033,7046,7060,7082,7083,7097,7127,7150,7158,7173,7178,7279,7288,7305,7307,7321,7385,7398,7399,7410,7424,7445,7530,7548,7561,7575,7594,7622,7641,7645,7683,7687,7692,7695,7699,7736,7740,7742,7751,7794,7860,7863,7906,7920,7935,7950,7972,8001,8041,8049,8068,8069,8075,8095,8099,8116,8130,8230,8236,8255,8275,8298,8358,8366,8382,8385,8392,8443,8475,8496,8505,8514,8524,8532,8553,8554,8619,8633,8642,8678,8697,8747,8772,8773,8785,8834,8836,8845,8862,8884,8910,8916,8988,8990,9014,9065,9108,9119,9123,9128,9144,9193,9251,9270,9319,9325,9329,9337,9339,9346,9364,9374,9403,9406,9431,9477,9480,9550,9551,9567,9625,9643,9649,9731,9783,9787,9822,9833,9877,9897,9925,9943};


            int[] forceLOSSES = new int[]
            { 183,556,644,653,733,1169,1600,1743,1901,2066,2068,2110,2195,2374,2405,2411,2432,2559,2590,2647,3089,3147,3253,3266,3311,3558,3617,3634,3678,3684,3810,3914,4029,4120,4146,4365,4407,4458,4471,4843,4890,5262,5351,5358,5680,5681,5731,5795,5863,5902,6024,6312,6374,6752,6864,6869,6917,7114,7196,7310,7832,7997,8005,8052,8242,8295,8353,8765,8998,9187,9220,9611,9863,9979};

            Console.ReadKey();
            Console.Clear();
            for(int i = 0; i < forceWINs.Length; i++)
            {
                Show(GetData.GetGrid(datageter.inputData[forceWINs[i]]));
                Console.ReadKey();
                Console.Clear();
            }
            */


            /*

            List<int> forceWins = new List<int>();
            List<int> forceLosses = new List<int>();
            for (int i = 0; i < 10000; i++)
            {

                if (datageter.outputData[i][0, 0] == 100000000) forceWins.Add(i);
                else if (datageter.outputData[i][0, 0] == -100000000) forceLosses.Add(i);

            }

            Console.WriteLine("Force Wins: \n");
            Console.Write("{");
            for (int i = 0; i < forceWins.Count; i++)
            {
                Console.Write(forceWins[i]);
                if (i < forceWins.Count - 1) Console.Write(",");
            }
            Console.WriteLine("}\n");

            Console.WriteLine("Force losses: \n");
            Console.Write("{");
            for (int i = 0; i < forceLosses.Count; i++)
            {
                Console.Write(forceLosses[i]);
                if (i < forceLosses.Count - 1) Console.Write(",");
            }
            Console.WriteLine("}\n");
            */



            sw.Restart();

            double max = double.MinValue;
            double min = double.MaxValue;


            double AvgValue = 0;


            int total = datageter.Length;
            for (int i = 0; i < datageter.Length; i++)
            {

                if (datageter.outputData[i][0, 0] > 4000)
                {
                    datageter.outputData[i][0, 0] = 4000;
                }
                else if (datageter.outputData[i][0, 0] < 0)
                {
                    datageter.outputData[i][0, 0] = 0;
                }



                double score = datageter.outputData[i][0, 0];



                AvgValue += (double)score / (double)datageter.Length;

                if (max < score) { max = score; }

                if (min > score) { min = score; }


                /*
                if (i % (int)(total / 50) == 0)
                {
                    Console.Clear();

                    Console.WriteLine($"{(float)i * (float)100 / (float)total}%");
                    if (i != 0)
                    {
                        Console.WriteLine($"expected total time = {(int)(sw.ElapsedMilliseconds * total * 0.001 / i)}s");
                        Console.WriteLine($"expected time remaining = {(int)((sw.ElapsedMilliseconds * total * 0.001 / i) - sw.ElapsedMilliseconds / 1000)}s");
                    }
                }
                */
            }
            sw.Stop();

            // Console.Clear();

            Console.WriteLine("Max: {0}, Min: {1}, Avg: {2}\n", max, min, AvgValue);

            Console.WriteLine("Max to middle: {0}, min to middle: {1}\n", max - AvgValue, AvgValue - min);

            Console.WriteLine("range: {0}", max - min);

            Console.WriteLine("saving...");
            File.WriteAllText("C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_400000recordsCleaned.txt", datageter.ToString());

            Console.WriteLine("saved");
            int goneWrong = 0;

            for (int i = 0; i < datageter.Length; i++)
            {
                datageter.outputData[i][0, 0] = ((datageter.outputData[i][0, 0]) / 2000d) - 1d;

                if (Math.Abs(datageter.outputData[i][0, 0]) > 1.001d)
                {
                    goneWrong++;
                }
            }
            Console.WriteLine("goneWrong: {0}", goneWrong);

            Console.WriteLine("saving...");
            File.WriteAllText("C:\\Users\\kiera\\codingDocuments\\abalone\\abaloneTrainingData\\Depth2_400000recordsCleaned2.txt", datageter.ToString());

            Console.WriteLine("saved");


            for (int i = 0; i < datageter.Length; i++)
            {
                Console.WriteLine(datageter.outputData[i][0, 0]);

                Console.ReadKey();
            }



        }
        #endregion


        //Grids
        #region
        private static int[,] grid = new int[9, 9] { { 2, 2, 2, 2, 2, 0, 0, 0, 0 }, { 2, 2, 2, 2, 2, 2, 0, 0, 0 }, { 1, 1, 2, 2, 2, 1, 1, 0, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 0, 1, 1, 3, 3, 3, 1, 1 }, { 0, 0, 0, 3, 3, 3, 3, 3, 3 }, { 0, 0, 0, 0, 3, 3, 3, 3, 3 } };
        // NOTE: grid[y,x] is the correct format NOT grid[x,y]
        public readonly static int[,] startGridTemplate = new int[9, 9] { { 2, 2, 2, 2, 2, 0, 0, 0, 0 }, { 2, 2, 2, 2, 2, 2, 0, 0, 0 }, { 1, 1, 2, 2, 2, 1, 1, 0, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 0, 1, 1, 3, 3, 3, 1, 1 }, { 0, 0, 0, 3, 3, 3, 3, 3, 3 }, { 0, 0, 0, 0, 3, 3, 3, 3, 3 } };

        public readonly static int[,] gridSlots = new int[61, 2] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 0, 2 }, { 1, 2 }, { 2, 2 }, { 3, 2 }, { 4, 2 }, { 5, 2 }, { 6, 2 }, { 0, 3 }, { 1, 3 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 3 }, { 7, 3 }, { 0, 4 }, { 1, 4 }, { 2, 4 }, { 3, 4 }, { 4, 4 }, { 5, 4 }, { 6, 4 }, { 7, 4 }, { 8, 4 }, { 1, 5 }, { 2, 5 }, { 3, 5 }, { 4, 5 }, { 5, 5 }, { 6, 5 }, { 7, 5 }, { 8, 5 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 }, { 8, 6 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }, { 8, 7 }, { 4, 8 }, { 5, 8 }, { 6, 8 }, { 7, 8 }, { 8, 8 } };

        public readonly static int[,] germanDaisyStart = new int[9, 9] { { 1, 1, 1, 1, 1, 0, 0, 0, 0 }, { 2, 2, 1, 1, 3, 3, 0, 0, 0 }, { 2, 2, 2, 1, 3, 3, 3, 0, 0 }, { 1, 2, 2, 1, 1, 3, 3, 1, 0 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 0, 1, 3, 3, 1, 1, 2, 2, 1 }, { 0, 0, 3, 3, 3, 1, 2, 2, 2 }, { 0, 0, 0, 3, 3, 1, 1, 2, 2 }, { 0, 0, 0, 0, 1, 1, 1, 1, 1 } };

        public readonly static int[] defaultNullMove = { 0, 0, 3 };
        #endregion

        //MOVES GRIDS
        #region
        public static int[,] Move(int[] move, int[,] moveThisGrid)
        {
            if (move.Length != 3) throw new Exception("ERROR");
            return Move(move[0], move[1], move[2], moveThisGrid);
        }

        public static int[,] Move(int x, int y, int direction, int[,] moveThisGrid)
        {
            int player = 0;
            if ((0 <= x && x < 9 && 0 <= y && y < 9) && (moveThisGrid.GetLength(0) == 9 && moveThisGrid.GetLength(1) == 9))
            {
                player = moveThisGrid[y, x];
                if ((player != 2 && player != 3) || moveThisGrid[y, x] == 0 || direction > 5 || direction < 0)
                { return moveThisGrid; }
            }
            else { return moveThisGrid; }

            int opponent;

            if (player == 2)
                opponent = 3;
            else opponent = 2;

            //to find the values of these
            int length = 1;
            int push = 0;
            bool justType = true;
            bool emptyFound = false;
            bool boundryFound = false;
            bool typeBlock = false;

            //finds the values for all the above paramiters
            for (int i = 1; i < 6; i++)
            {
                int[] newXY = IInDir(x, y, direction, i);


                int checkX = newXY[0];
                int checkY = newXY[1];


                if (0 > checkX || checkX > 8 || 0 > checkY || checkY > 8)
                {
                    boundryFound = true;
                    break;
                }

                if (moveThisGrid[checkY, checkX] == 0)
                {
                    boundryFound = true;
                    break;
                }
                else if (moveThisGrid[checkY, checkX] == 1)
                {
                    emptyFound = true;
                    break;
                }
                else if (moveThisGrid[checkY, checkX] == player)
                {
                    if (!justType)
                    {
                        typeBlock = true;
                        break;
                    }
                    length++;
                }
                else
                {
                    justType = false;
                    push++;
                }
            }


            if (typeBlock || (!emptyFound && !boundryFound) || push >= length || length > 3 || (justType && boundryFound)) // checking if the move is invalid.
            {
                return moveThisGrid;
            }
            else
            {
                moveThisGrid[y, x] = 1;
                int affected = length + push;

                if (!boundryFound)
                    affected++;



                for (int i = 1; i < affected; i++)
                {
                    int[] currentXY = IInDir(x, y, direction, i);

                    if (i <= length)
                    {
                        moveThisGrid[currentXY[1], currentXY[0]] = player;
                    }
                    else
                    {
                        moveThisGrid[currentXY[1], currentXY[0]] = opponent;
                    }

                }
            }

            return moveThisGrid;

        }

        static int[] IInDir(int x, int y, int direction, int i)
        {
            int checkX = x;
            int checkY = y;

            //finding new X & Y based on direction
            {
                if (direction == 0)
                {
                    checkX += i;
                }
                else if (direction == 1)
                {
                    checkX += 0;
                    checkY += -i;
                }
                else if (direction == 2)
                {
                    checkX += -i;
                    checkY += -i;
                }
                else if (direction == 3)
                {
                    checkX += -i;
                    checkY += 0;
                }
                else if (direction == 4)
                {
                    checkX += 0;
                    checkY += i;
                }
                else if (direction == 5)
                {
                    checkX += i;
                    checkY += i;
                }
            }
            return new int[] { checkX, checkY };
        }


        //public static int[,] Rotate60(int[,] rotateThisGrid)
        //{
        //    int[,] newGrid = new int[9, 9];

        //    return newGrid;
        //}
        #endregion

        //INFOMATION FOR THE BOTS & GAME
        #region
        public static int[,] GetGrid()
        {
            return (int[,])grid.Clone();
        }

        public static int[,] SwapPlayers(int[,] thisGrid)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (thisGrid[i, j] == 3) thisGrid[i, j] = 2;
                    else if (thisGrid[i, j] == 2) thisGrid[i, j] = 3;
                }
            }
            return thisGrid;
        }

        public static bool CheckMove(int[] move, int[,] checkThisGrid)
        {
            if (move.Length != 3) return false;

            return CheckMove(move[0], move[1], move[2], checkThisGrid);
        }

        public static bool CheckMove(int x, int y, int direction, int[,] checkThisGrid)
        {
            int player = 0;
            if (0 <= x && x < 9 && 0 <= y && y < 9 && (checkThisGrid.GetLength(0) == 9 && checkThisGrid.GetLength(1) == 9))
            {
                player = checkThisGrid[y, x];
                if ((player != 2 && player != 3) || checkThisGrid[y, x] == 0 || direction > 5 || direction < 0)
                { return false; }
            }
            else { return false; }

            int opponent;

            if (player == 2)
                opponent = 3;
            else opponent = 2;

            //to find the values of these
            int length = 1;
            int push = 0;
            bool justType = true;
            bool emptyFound = false;
            bool boundryFound = false;
            bool typeBlock = false;

            //finds the values for all the above paramiters
            for (int i = 1; i < 6; i++)
            {
                int[] newXY = IInDir(x, y, direction, i);


                int checkX = newXY[0];
                int checkY = newXY[1];


                if (0 > checkX || checkX > 8 || 0 > checkY || checkY > 8)
                {
                    boundryFound = true;
                    break;
                }

                if (checkThisGrid[checkY, checkX] == 0)
                {
                    boundryFound = true;
                    break;
                }
                else if (checkThisGrid[checkY, checkX] == 1)
                {
                    emptyFound = true;
                    break;
                }
                else if (checkThisGrid[checkY, checkX] == player)
                {
                    if (!justType)
                    {
                        typeBlock = true;
                        break;
                    }
                    length++;
                }
                else
                {
                    justType = false;
                    push++;
                }
            }


            if (typeBlock || (!emptyFound && !boundryFound) || push >= length || length > 3 || (justType && boundryFound)) // checking if the move is invalid.
            {
                return false;
            }


            return true;

        }

        public static List<int[]> PiecePlaces(int[,] checkThisGrid, int player, bool bothPlayers = false)
        {
            List<int[]> piecePlaces = new List<int[]>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (checkThisGrid[i, j] == player || ((checkThisGrid[i, j] == 3 - player) && (bothPlayers)))
                    {
                        piecePlaces.Add(new int[] { j, i });
                    }
                }
            }
            return piecePlaces;
        }

        public static int NoPieces(int[,] checkThisGrid, int player, bool bothPlayers = false)
        {
            int noPieces = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (checkThisGrid[i, j] == player || ((checkThisGrid[i, j] == 3 - player) && bothPlayers))
                    {
                        noPieces++;
                    }
                }
            }
            return noPieces;
        }

        public static List<int[]> LegalMoves(int[,] checkThisGrid, int player)
        {
            List<int[]> legalMoves = new List<int[]>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (checkThisGrid[i, j] == player)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            if (CheckMove(j, i, k, checkThisGrid))
                                legalMoves.Add(new int[] { j, i, k });
                        }
                    }
                }
            }
            return legalMoves;
        }

        public static int Score(int[,] checkThisGrid, int player)
        {
            //assuming there are at most 14 pieces, and at least 8 of both players.

            int score = 0;
            int opponent = 2;
            if (player == 2)
                opponent = 3;
            List<int[]> piecePlaces = PiecePlaces(checkThisGrid, player);
            List<int[]> opponentPiecePlaces = PiecePlaces(checkThisGrid, opponent);


            int piecesLeft = piecePlaces.Count();
            int opponentLeft = opponentPiecePlaces.Count();

            // adds 250 points for each piece that is still in play
            score += (piecesLeft - 8) * 250;
            // adds 300 for each opponent piece out
            score += (14 - opponentLeft) * 300;


            foreach (int[] piece in piecePlaces)
            {
                int x = piece[0];
                int y = piece[1];

                for (int i = 0; i < 4; i++)
                {
                    if (x == i || y == i || x == 8 - i || y == 8 - i || y - x == 4 - i || x - y == 4 - i)
                    {
                        score += 16 - (4 - i) * (4 - i);
                        break;
                    }
                }
            }
            foreach (int[] piece in opponentPiecePlaces)
            {
                int x = piece[0];
                int y = piece[1];

                for (int i = 0; i < 4; i++)
                {
                    if (x == i || y == i || x == 8 - i || y == 8 - i || y - x == 4 - i || x - y == 4 - i)
                    {
                        score += (5 - i) * (5 - i) * 3;
                        break;
                    }
                }
            }
            return score;
        }

        public static int Winner(int[,] checkThisGrid)
        {
            if (PiecePlaces(checkThisGrid, 3).Count < 9)
            {
                return 2;
            }
            else if (PiecePlaces(checkThisGrid, 2).Count < 9)
            {
                return 3;
            }
            else return 0;
        }
        #endregion

        //FOR CONSOLE
        #region
        public static void DeleteLine(int row = -1, int lineLength = -1)
        {
            if (lineLength < 0)
                lineLength = Console.WindowWidth;
            if (row < 0)
                row = Console.CursorTop;

            Console.SetCursorPosition(0, row);

            for (int i = 0; i < lineLength; i++)
            {

                Console.Write(" ");
            }
            Console.SetCursorPosition(0, row);
        }

        public static void Show(int[,] thisGrid)
        {
            bool showingCursor = false;
            if (Console.CursorVisible == true) showingCursor = true;
            Console.CursorVisible = false;

            for (int y = 0; y < 9; y++)
            {
                string printThis = "";

                for (int j = 0; j < Math.Sqrt((4 - y) * (4 - y)); j++) printThis += "   ";

                for (int x = 0; x < 9; x++)
                {
                    if (thisGrid[y, x] == 1)
                    {
                        printThis += "``    ";
                    }
                    else if (thisGrid[y, x] == 2)
                    {
                        printThis += "OO    ";
                    }
                    else if ((thisGrid[y, x] == 3))
                    {
                        printThis += "##    ";
                    }
                    else if ((thisGrid[y, x] != 0))
                    {
                        printThis += "??    ";
                    }

                }

                for (int i = 0; i < 2; i++)
                {
                    foreach (char cha in printThis)
                    {
                        if (cha == '#')
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                        else if (cha == 'O')
                            Console.ForegroundColor = ConsoleColor.Blue;
                        else
                            Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(cha);
                    }
                    Console.WriteLine();
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }

            if (showingCursor) Console.CursorVisible = true;
        }

        public static int[] ReadMove()
        {
            int row = Console.CursorTop;
            int x = 0;
            int y = 0;
            int direction = 0;



            Console.Write("X = ");
            try
            {
                x = int.Parse(Console.ReadLine());
            }
            catch { }
            DeleteLine(row);
            Console.Write("Y = ");
            try
            { y = int.Parse(Console.ReadLine()); }
            catch { }
            DeleteLine(row);
            Console.Write("direction = ");
            try
            { direction = int.Parse(Console.ReadLine()); }
            catch { }
            DeleteLine(row);

            return new int[3] { x, y, direction };
        }
        #endregion

        //DIFFERENT PLAYABLE GAMEMODES
        #region
        static void TwoPlayer()
        {

            Console.ReadKey();
            Console.Clear();
            Show(grid);

            for (int i = 0; i < 3000; i++)
            {
                int[] readM = ReadMove();

                Console.Clear();

                Move(readM[0], readM[1], readM[2], grid);

                Show(grid);
            }
        }

        static void OnePlayer()
        {

            Console.ReadKey();
            Console.Clear();
            Show(grid);

            for (int i = 0; i < 3000; i++)
            {
                int[] move = new int[3];
                if (i % 2 == 0)
                    move = ReadMove();
                else
                    move = Bots.ScoringBot(3, GetGrid());

                int x = move[0];
                int y = move[1];
                int direction = move[2];

                Console.Clear();

                if (grid[y, x] == (i % 2) + 2)
                {
                    Move(x, y, direction, grid);
                }


                Show(grid);
            }
        }

        public static int NeuralBotVsBot(string bot1, string bot2, OldNeuralBot bot, bool showGame = false)
        {
            bool twoWins = false;
            bool threeWins = false;

            Console.CursorVisible = false;
            Console.Clear();

            for (int i = 0; i < 5000; i++)
            {
                int[] move = new int[3];
                if (i % 2 == 0)
                {
                    move = Bots.OldNeuralBot(2, GetGrid(), bot);
                }
                else
                {
                    move = Bots.OldNeuralBot(3, GetGrid(), bot);
                }

                int x = move[0];
                int y = move[1];
                int direction = move[2];


                if (grid[y, x] == (i % 2) + 2)
                {
                    Move(x, y, direction, grid);
                }

                if (showGame)
                {

                    Console.SetCursorPosition(0, 0);
                    Show(grid);
                }

                if (PiecePlaces(grid, 2).Count < 9)
                {
                    threeWins = true;
                    break;
                }
                else if (PiecePlaces(grid, 3).Count < 9)
                {
                    twoWins = true;
                    break;
                }
            }

            grid = (int[,])startGridTemplate.Clone();

            if (twoWins)
                return 2;
            else return 3;
        }

        public static int MCTSBotVsBot(MCTS mcts, string bot2, bool showGame = false)
        {
            bool twoWins = false;
            bool threeWins = false;

            Console.CursorVisible = false;
            if (showGame)
                Console.Clear();

            int i = 0;
            for (i = 0; i < 5000; i++)
            {
                int[] move = new int[3];
                if (i % 2 == 0)
                {
                    move = MCTSTrainer.ExplorationMove(grid, mcts, 2);
                }
                else
                {
                    move = Bots.callBot(bot2, 3, GetGrid());
                }


                int x = move[0];
                int y = move[1];
                int direction = move[2];


                if (grid[y, x] == (i % 2) + 2)
                {
                    Move(x, y, direction, grid);
                }


                if (showGame)
                {

                    Console.SetCursorPosition(0, 0);
                    Show(grid);
                }

                if (PiecePlaces(grid, 2).Count < 9)
                {
                    threeWins = true;
                    break;
                }
                else if (PiecePlaces(grid, 3).Count < 9)
                {
                    twoWins = true;
                    break;
                }
            }

            grid = (int[,])startGridTemplate.Clone();

            if (showGame)
            {
                Console.WriteLine("Winner = {0}, TotalMoves = {1}", (twoWins) ? 2 : (threeWins ? 3 : 0), i);
            }

            if (twoWins)
                return 2;
            else if (threeWins)
                return 3;
            else return 0;
        }


        public static int BotVsBot(string bot1, string bot2, bool showGame = false)
        {
            GameRunner gr = new GameRunner(bot1, bot2);

            gr.ShowGame = showGame;

            return gr.RunGame();
        }

        static int BotVsBotDebug(string bot1, string bot2)
        {

            Console.CursorVisible = false;
            bool twoWins = false;
            bool threeWins = false;
            string winner = "";
            int rounds = 0;

            Console.ReadKey();


            for (int i = 0; i < 3000; i++)
            {
                Console.Clear();
                Show(grid);
                Console.Write("jump: ");
                string entry = Console.ReadLine();
                int jump = 1;

                switch (entry)
                {
                    case (null or ""):
                        break;
                    default:
                        try
                        {
                            jump = int.Parse(entry);
                        }
                        catch { }
                        break;
                }

                Console.Clear();
                Show(grid);

                for (int j = 0; j < jump; j++)
                {
                    int[] move = new int[3];
                    if (i % 2 == 0)
                    {
                        move = Bots.callBot(bot1, 2, GetGrid());
                    }
                    else
                        move = Bots.callBot(bot2, 3, GetGrid());

                    int x = move[0];
                    int y = move[1];
                    int direction = move[2];



                    if (grid[y, x] == (i % 2) + 2)
                    {
                        Move(x, y, direction, grid);
                    }



                    Console.SetCursorPosition(0, 0);
                    Show(grid);

                    if (PiecePlaces(grid, 2).Count < 9)
                    {
                        threeWins = true;
                        break;
                    }
                    else if (PiecePlaces(grid, 3).Count < 9)
                    {
                        twoWins = true;
                        break;
                    }


                    i++;
                }
                if (twoWins || threeWins)
                {
                    if (twoWins)
                    {
                        winner = "blue";
                    }
                    else winner = "Yellow";
                    rounds = i;
                    break;
                }
                i--;
            }
            Console.Clear();

            Show(grid);
            Console.WriteLine("WE HAVE A WINNER, rounds = " + rounds + ", winner = " + winner);
            Console.ReadKey();

            grid = (int[,])startGridTemplate.Clone();

            if (twoWins)
                return 2;
            else return 3;
        }

        #endregion



        //Heuristic Bot
        #region
        static void HeuristicBotTraining()
        {
            bool twoWins = false;
            bool threeWins = false;
            double Oc = 0.9d;
            for (int i = 0; i < 5000; i++)
            {
                int[] move = Bots.callBot("3", (i % 2) + 2, GetGrid());

                int x = move[0];
                int y = move[1];
                int direction = move[2];

                //MOVES PIECE
                if (grid[y, x] == (i % 2) + 2)
                {
                    Move(x, y, direction, grid);


                    int[,] player2Grid = GetGrid();
                    if ((i % 2) == 1) SwapPlayers(player2Grid);


                    uint[] compactGrid = Functions.CompactGrid(player2Grid);

                    int index = Functions.states.IndexOf(compactGrid);
                    Functions.visited[index]++;
                    if (i % 2 == 0)
                    {
                        gameStateIndexesBot2.Add(index);
                        gameActionsBot2.Add(move[3]);
                    }
                    else
                    {
                        gameStateIndexesBot3.Add(index);
                        gameActionsBot3.Add(move[3]);
                    }
                }
                else
                {
                    throw new Exception("THIS SHOULD NOT HAPPEN");
                }


                //STOPS GAME WHEN DONE
                if (PiecePlaces(grid, 2).Count < 9)
                {
                    threeWins = true;
                    break;
                }
                else if (PiecePlaces(grid, 3).Count < 9)
                {
                    twoWins = true;
                    break;
                }
            }

            grid = (int[,])startGridTemplate.Clone();

            //update method :)
            if (twoWins)
            {
                for (int i = gameStateIndexesBot2.Count - 1; i >= 0; i--)
                {


                    //Functions.actionWinProbabability[gameStateIndexesBot2[i]][gameActionsBot2[i]] = 
                }
            }
            else if (threeWins)
            {

            }
            else //overtime
            {

            }
        }
        static List<int> gameStateIndexesBot2 = new List<int>();
        static List<int> gameStateIndexesBot3 = new List<int>();
        static List<int> gameActionsBot2 = new List<int>();
        static List<int> gameActionsBot3 = new List<int>();
        #endregion

    }
}
