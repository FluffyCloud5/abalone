using System.Diagnostics;
using System.Text;
// working on line 883

namespace abalone
{
    public class OldNeuralBotTrainer
    {
        public static readonly bool fastAndStupid = false;
        public static readonly int maxFastMoves = 200;

        public static readonly int maxMoves = 500;


        public static void StartTraining()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int noBots = 100;
            int noGamesPerBot = 10; // must be even
            int keepXElites = 10;
            if (noGamesPerBot >= noBots || noGamesPerBot % 2 != 0 || noBots <= keepXElites)
            {
                throw new Exception("ERROR");
            }
            int populations = 50000;
            double trainingSpeed = 0.5d;
            OldNeuralBot[] bots = new OldNeuralBot[noBots];
            int[] botWinMinusLoss = new int[noBots];
            Random rnd = new Random();

            bool estimateTime = false;

            if (populations <= 100)
            {
                Console.WriteLine("estimated time = {0}s", noGamesPerBot * noBots * populations * 0.3734 / 1000);
            }
            else estimateTime = true;




            for (int k = 0; k < noBots; k++)
            {
                bots[k] = new OldNeuralBot();
                Matrix[] transformMatricies = new Matrix[2] { bots[k].inputToMiddle, bots[k].middleToOutput };
                foreach (Matrix matrix in transformMatricies)
                {
                    for (int i = 0; i < matrix.rows; i++)
                    {
                        for (int j = 0; j < matrix.cols; j++)
                        {
                            matrix[i, j] = (rnd.NextDouble() * 20) - 10;
                        }
                    }
                }
            }

            if (File.Exists("C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\TrainingBot.csv"))
            {
                bots[0] = Functions.readCSVNeuralBot("TrainingBot");
                Console.WriteLine("file read");
            }


            Console.WriteLine("STARTED");
            watch.Restart();
            for (int i = 0; i < populations; i++) //Population will go as upperbound
            {



                //
                //PLAYING THE BOTS OFF EACHOTHER
                //
                var matches = new List<(int bot1, int bot2)>();

                // Prepare a list of unique bot matches
                /*ROUND-ROBIN
                for (int j = 0; j < noBots - 1; j++)
                {
                    for (int k = j + 1; k < noBots; k++)
                    {
                        matches.Add((j, k)); // Store (bot1, bot2) pairs
                    }
                }
                */
                for (int j = 0; j < noBots - 1; j++)
                {
                    for (int k = j + 1; k < j + 1 + (float)noGamesPerBot / 2; k++)
                    {
                        int inK = k;
                        while (inK > noBots - 1) inK -= noBots;

                        matches.Add((j, inK)); // Store (bot1, bot2) pairs
                    }
                }




                Parallel.ForEach(matches, match =>
                {
                    int[] botThatWins = NeuralBotVsBot(bots[match.bot1], bots[match.bot2]);

                    Interlocked.Add(ref botWinMinusLoss[match.bot1], botThatWins[1]);
                    Interlocked.Add(ref botWinMinusLoss[match.bot2], botThatWins[2]);
                });





                //orders bots based of how good they went.  1,2,3,4...
                Array.Sort(botWinMinusLoss, bots);

                /*Old aproach to updating bots
                 * wrong because it assumes sorted desending
                int indexInArray = 10;
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9 - j; k++)
                    {
                        for (int h = 0; h < 2; h++)
                        {
                            bots[indexInArray].inputToMiddle = bots[j].inputToMiddle.Duplicate();
                            bots[indexInArray].middleToOutput = bots[j].middleToOutput.Duplicate();
                            Matrix[] transformMatricies = new Matrix[2] { bots[indexInArray].inputToMiddle, bots[indexInArray].middleToOutput };
                            foreach (Matrix matrix in transformMatricies)
                            {
                                for (int l = 0; l < matrix.rows; l++)
                                {
                                    for (int u = 0; u < matrix.cols; u++)
                                    {
                                        matrix[l, u] = matrix[l, u] + (rnd.NextDouble() - 0.5) * trainingSpeed;
                                    }
                                }
                            }
                            indexInArray++;
                        }
                    }
                }
                if (indexInArray != noBots)
                    throw new Exception("ERROR");
                */

                /*Only best is kept
                for (int j = 0; j < noBots-1;j ++)
                {
                    bots[j].inputToMiddle = bots[noBots-1].inputToMiddle.Duplicate();
                    bots[j].middleToOutput = bots[noBots-1].middleToOutput.Duplicate();
                    Matrix[] transformMatricies = new Matrix[2] { bots[j].inputToMiddle, bots[j].middleToOutput };
                    foreach (Matrix matrix in transformMatricies)
                    {
                        for (int l = 0; l < matrix.rows; l++)
                        {
                            for (int u = 0; u < matrix.cols; u++)
                            {
                                matrix[l, u] = matrix[l, u] + (rnd.NextDouble() - 0.5) * trainingSpeed;
                            }
                        }
                    }

                }
                */

                int khere = 0;
                for (int j = 0; j < noBots - 1 - keepXElites; j++)
                {
                    if (khere >= keepXElites) khere -= keepXElites;
                    bots[j].inputToMiddle = bots[noBots - 1 - khere].inputToMiddle.Duplicate();
                    bots[j].middleToOutput = bots[noBots - 1 - khere].middleToOutput.Duplicate();
                    Matrix[] transformMatricies = new Matrix[2] { bots[j].inputToMiddle, bots[j].middleToOutput };
                    foreach (Matrix matrix in transformMatricies)
                    {
                        for (int l = 0; l < matrix.rows; l++)
                        {
                            for (int u = 0; u < matrix.cols; u++)
                            {
                                matrix[l, u] = matrix[l, u] + (rnd.NextDouble() - 0.5) * trainingSpeed;
                            }
                        }
                    }
                    khere++;

                }

                if (i % 50 == 0)
                {
                    if (estimateTime && i == 50)
                    {
                        watch.Stop();
                        Console.WriteLine("estimated time = {0}mins", (float)watch.ElapsedMilliseconds / 1000 / 100 / 60 * (populations));
                        watch.Start();
                    }
                    Console.WriteLine(i);
                    Functions.createCSVOfNeuralBot(bots[noBots - 1], "TrainingBot");
                }

            }
            Functions.createCSVOfNeuralBot(bots[noBots - 1], "TrainingBot");
            watch.Stop();
            Console.WriteLine("Total time = {0}s", (float)watch.ElapsedMilliseconds / 1000);

            //Console.WriteLine("Press to start");

            //Console.ReadKey();

            //Program.BotVsBot("3", "4", bots[noBots-1], true);

        }

        private static int[] NeuralBotVsBot(OldNeuralBot bot1, OldNeuralBot bot2)
        {
            int oneBonus = 0;
            int twoBonus = 0;

            int[,] grid = (int[,])Program.startGridTemplate.Clone();

            int[,] dontRepeatCheckGrid = new int[9, 9];

            int max = maxMoves;
            if (fastAndStupid) max = maxFastMoves;

            Matrix input1 = new Matrix(62, 1);
            input1[61, 0] = 1;
            Matrix middleLayer1 = new Matrix(21, 1);
            Matrix output1 = new Matrix(84, 1);

            Matrix previousInput1 = new Matrix(62, 1);
            previousInput1[61, 0] = 1;

            Matrix input2 = new Matrix(62, 1);
            input2[61, 0] = 1;
            Matrix middleLayer2 = new Matrix(21, 1);
            Matrix output2 = new Matrix(84, 1);

            Matrix previousInput2 = new Matrix(62, 1);
            previousInput2[61, 0] = 1;
            for (int i = 0; i < max; i++)
            {
                int[] move = new int[3];
                if (i % 2 == 0)
                {
                    move = bot1.ChooseMove(2, grid, input1, middleLayer1, output1, previousInput1);
                }
                else
                {
                    move = bot2.ChooseMove(3, grid, input2, middleLayer2, output2, previousInput2);
                }

                int x = move[0];
                int y = move[1];
                int direction = move[2];


                if (grid[y, x] == (i % 2) + 2)
                {
                    Program.Move(x, y, direction, grid);
                }

                if (Program.NoPieces(grid, 2) < 9)
                {
                    oneBonus += 1000;
                    twoBonus -= 1000;
                    break;
                }
                else if (Program.NoPieces(grid, 3) < 9)
                {
                    twoBonus += 1000;
                    oneBonus -= 1000;
                    break;
                }


                if (fastAndStupid) ;
                else if (max - i == 10)
                {
                    dontRepeatCheckGrid = (int[,])grid.Clone();
                }
                else if (max - i < 10)
                {
                    if (AreEqual<int>(dontRepeatCheckGrid, grid))
                    {
                        oneBonus -= 50;
                        twoBonus -= 50;
                    }
                }

            }

            int player1P = Program.NoPieces(grid, 2);
            int player2P = Program.NoPieces(grid, 3);
            oneBonus += (14 - player1P) * 50;
            twoBonus += (14 - player2P) * 50;

            return new int[] { 0, 0 + oneBonus, 0 + twoBonus };

        }

        public static bool AreEqual<T>(T[,] a, T[,] b)
        {
            if (a == null || b == null)
                return a == b;

            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                return false;

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!EqualityComparer<T>.Default.Equals(a[i, j], b[i, j]))
                        return false;
                }
            }
            return true;
        }

    }

    public class OldNeuralBot
    {
        public OldNeuralBot()
        {
        }

        // TO OPTIMIZE
        public int[] ChooseMove(int player, int[,] grid, Matrix input, Matrix middleLayer, Matrix output, Matrix previousInput, bool fullCalc = false)
        {
            // Program.grid[]  uses this as its input for the grid
            int x = 0;
            int y = 0;
            int direction = 0;


            //GETS input array

            int index = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        if (grid[i, j] == 1) input[index, 0] = 0;
                        else if (player == 2) input[index, 0] = -(grid[i, j] - 1) * 2 + 3;
                        else input[index, 0] = (grid[i, j] - 1) * 2 - 3;

                        index++;
                    }
                }
            }

            if (index != 61 || input[61, 0] != 1)
            {
                throw new Exception("Index != 61 || input[61] != 1");
            }

            //
            //CHATGPT
            //
            double[] tempUpdates = new double[middleLayer.rows]; // Allocate on heap

            // Accumulate updates in tempUpdates
            for (int i = 0; i < input.rows; i++)
            {
                double diff = input[i, 0] - previousInput[i, 0];
                if (diff != 0 || fullCalc)  // Skip unchanged values
                {
                    //double scaledDiff = diff; // Avoid redundant calculations
                    int j = 0;

                    // **Loop Unrolling for Faster Updates**
                    for (; j <= middleLayer.rows - 4; j += 4)
                    {
                        tempUpdates[j] += inputToMiddle[j, i] * diff;
                        tempUpdates[j + 1] += inputToMiddle[j + 1, i] * diff;
                        tempUpdates[j + 2] += inputToMiddle[j + 2, i] * diff;
                        tempUpdates[j + 3] += inputToMiddle[j + 3, i] * diff;
                    }

                    // Handle remaining elements
                    for (; j < middleLayer.rows; j++)
                    {
                        tempUpdates[j] += inputToMiddle[j, i] * diff;
                    }

                    previousInput[i, 0] = input[i, 0];
                }
            }

            // Apply all updates in a single pass
            for (int j = 0; j < middleLayer.rows; j++)
            {
                middleLayer[j, 0] += tempUpdates[j];
            }

            Shrink(middleLayer);


            /*
            for (int i = 0; i < middleLayer.rows; i++)
            {
                if (middleLayer[i, 0] != previousMiddle[i, 0])
                {
                    for (int j = 0; j < output.rows; j++)
                    {
                        output[j, 0] += middleToOutput[j, i] * (middleLayer[i, 0] - previousMiddle[i, 0]);
                    }
                    previousMiddle[i, 0] = middleLayer[i, 0];
                }
            }
            */
            output = middleToOutput * middleLayer;

            int indexBest = 0;
            List<int[]> legalMoves = Program.LegalMoves(grid, player);
            for (int i = 0; i < legalMoves.Count; i++)
            {
                if (output[i, 0] > output[indexBest, 0])
                {
                    indexBest = i;
                }
            }



            // process and return x,y and direction


            return legalMoves[indexBest]; // return in the form of { x , y , direction }
        }



        public void debugNeural()
        {

            Matrix matrix = new Matrix(21, 1);
            int repeat = 1000000;


            int[] move = new int[3];
            Matrix input1 = new Matrix(62, 1);
            input1[61, 0] = 1;
            Matrix middleLayer1 = new Matrix(21, 1);
            Matrix output1 = new Matrix(84, 1);

            Matrix previousInput1 = new Matrix(62, 1);
            previousInput1[61, 0] = 1;

            int[,] grid = (int[,])Program.startGridTemplate.Clone();

            var stopwatch = Stopwatch.StartNew();
            for (int d = 0; d <= repeat; d++)
            {

            }
            stopwatch.Stop();
            Console.WriteLine($"Smart: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();
            for (int d = 0; d <= repeat; d++)
            {

            }
            stopwatch.Stop();
            Console.WriteLine($"Dumb: {stopwatch.ElapsedMilliseconds} ms");

        }
        /*
        
        List<int[]> legalMoves = Program.LegalMoves(grid, 2);
        Program.Move(legalMoves[0][0], legalMoves[0][1], legalMoves[0][2], grid); // 1862 ms  1 million iterations

        move = this.ChooseMove(2, grid, input1, middleLayer1, output1, previousInput1); //6135 ms 1 million iterations

        NeuralBotTrainer.AreEqual<int>((new int[9, 9]), grid); 85 ms
       
        grid = Program.GetGrid();  107 ms

        Program.NoPieces(grid, 2); 96 ms



        */
        //OPTIMIZED
        private static void Shrink(Matrix matrix)
        {

            for (int i = 0; i < matrix.rows - 1; i++)
            {
                matrix[i, 0] = 2 / (1 + Math.Exp(-matrix[i, 0] / 4)) - 1;
            }
            matrix[matrix.rows - 1, 0] = 1;
        }



        //Inputs = 61 nodes (-1 enemy, 0 empty, 1 player) + bias = 1
        //MIDDLE layer 20 nodes 
        //Output  = 6*14 = 84 (highest wins)

        public Matrix inputToMiddle = new Matrix(21, 62);

        public Matrix middleToOutput = new Matrix(84, 21);
    }

    //---NEW stuff below this line---

    // DOBJECT1WrtOBJECT2  <=> derivative(D) of (object1) with (W) respect (r) to (t) (object2) 

    public class NeuralBot
    {
        public NeuralBot(int inputSize, int[] layerSizes, int outputSize) // all of theses are excluding the bias that will be added to the Matricies. 
        {
            // layerSizes.Length + 1 is how many matrixes there will be.
            // each matrix Mi for i in [2,layerSizes.Length] is equal to (layerSize[i-3])*(layerSize[i-2]+1) 

            //CHECKS
            #region
            if (inputSize <= 0 || outputSize <= 0) throw new ArgumentOutOfRangeException();
            for (int i = 0; i < layerSizes.Length; i++)
            {
                if (layerSizes[i] <= 0) throw new ArgumentOutOfRangeException();
            }
            #endregion


            //Initializing the matrixes
            #region
            int[] layers = new int[2 + layerSizes.Length];

            layers[0] = inputSize;
            layers[layers.Length - 1] = outputSize;
            for (int i = 1; i < layers.Length - 1; i++)
            {
                layers[i] = layerSizes[i - 1];
            }



            transformations = new Matrix[layers.Length - 1];

            biases = new Matrix[layers.Length - 1];

            for (int i = 0; i < transformations.Length; i++)
            {
                transformations[i] = new Matrix(layers[i + 1], layers[i]); // no biases

                biases[i] = new Matrix(layers[i + 1], 1);
            }


            RandomiseBot();

            #endregion
        }

        public void RandomiseBot(double min = -1, double max = 1)
        {
            if (min > max) throw new Exception("min must be less than or equal to max");

            for (int i = 0; i < transformations.Length; i++)
            {
                for (int j = 0; j < transformations[i].mat.Length; j++)
                {
                    transformations[i].mat[j] = Random.Shared.NextDouble() * (max - min) + min;
                }
            }

            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].mat.Length; j++)
                {
                    biases[i].mat[j] = Random.Shared.NextDouble() * (max - min) + min;
                }
            }

        }

        public Matrix[] transformations; // these all don't account for bias

        public Matrix[] biases;

        //for saving to computer
        #region
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Clear();

            for (int i = 0; i < transformations.Length; i++)
            {
                sb.Append(transformations[i].ToString());
                sb.Append("TTT");
                sb.Append(biases[i].ToString());
                if (i != transformations.Length - 1) sb.Append("BBB");


            }
            return sb.ToString();
        }

        public static NeuralBot Parse(string str)
        {
            string[] transformationAndBiases = str.Split("BBB");
            Matrix[] transformations = new Matrix[transformationAndBiases.Length];
            Matrix[] biases = new Matrix[transformationAndBiases.Length];

            NeuralBot bot = new NeuralBot(1, new int[] { 1 }, 1);

            int noLayers = transformationAndBiases.Length + 1;

            for (int i = 0; i < transformationAndBiases.Length; i++)
            {
                string[] transformPlusBias = transformationAndBiases[i].Split("TTT");
                transformations[i] = Matrix.Parse(transformPlusBias[0]);
                biases[i] = Matrix.Parse(transformPlusBias[1]);

            }

            bot.biases = biases;
            bot.transformations = transformations;

            return bot;
        }

        #endregion

        //just activation functions and their derivatives
        #region
        //dependencies
        public double ActivationFunction(double num)
        {
            return 2 / (1 + Math.Exp(-num)) - 1;
        }

        public double DActiveFuncWrtZ(double num)
        {
            double expNegX = Math.Exp(-num);

            return 2 * expNegX / ((1 + expNegX) * (1 + expNegX));
        }

        public double FinalActivationFunction(double num)
        {
            return num;
        }

        public double DFinalActiveFuncWrtZ(double num)
        {
            return 1d;
        }




        //uses dependencies, if they work this works
        public Matrix ActiveFunc(Matrix matrix)
        {
            for (int i = 0; i < matrix.mat.Length; i++)
            {
                matrix.mat[i] = ActivationFunction(matrix.mat[i]);
            }
            return matrix;
        }

        public Matrix DActiveFuncWrtZL(Matrix matrix)
        {
            for (int i = 0; i < matrix.mat.Length; i++)
            {
                matrix.mat[i] = DActiveFuncWrtZ(matrix.mat[i]);
            }
            return matrix;
        }

        public Matrix FinalActiveFunc(Matrix matrix)
        {
            for (int i = 0; i < matrix.mat.Length; i++)
            {
                matrix.mat[i] = FinalActivationFunction(matrix.mat[i]);
            }
            return matrix;
        }

        public Matrix DFinalActiveFuncWrtmatrix(Matrix matrix)
        {
            for (int i = 0; i < matrix.mat.Length; i++)
            {
                matrix.mat[i] = DFinalActiveFuncWrtZ(matrix.mat[i]);
            }

            return matrix;
        }



        #endregion

        public Matrix ComputeOutput(Matrix input)
        {
            if (input == null) throw new ArgumentNullException();
            if (input.cols != 1) throw new ArgumentOutOfRangeException();
            if (input.rows != transformations[0].cols) throw new Exception("INPUT MUST BE OF CORRECT SIZE");

            Matrix intermediateLayer = transformations[0] * input;

            intermediateLayer += biases[0];

            for (int i = 1; i < transformations.Length; i++)
            {
                intermediateLayer = ActiveFunc(intermediateLayer);


                intermediateLayer = transformations[i] * intermediateLayer;
                intermediateLayer += biases[i];
            }


            if (intermediateLayer.cols != 1) throw new Exception("Transformations went wrong");

            intermediateLayer = FinalActiveFunc(intermediateLayer);

            return intermediateLayer;
        }
    }

    public class SupervisedBackpropagation
    {
        //data and bot
        private Matrix[] _inputData;
        private Matrix[] _outputData;
        public NeuralBot _bot;


        //PARAMATERS
        #region
        public int Batches = 1;
        public int BatchSize = 1; //Tells how many data records will be used per batch
        //public int SaveEveryXBatches = 5;
        public string FileLocation;
        public double learningRate = 0.001;







        #endregion

        public SupervisedBackpropagation(Matrix[] inputData, Matrix[] outputData, NeuralBot bot) // provide the data for the NeuralBot to train on
        {
            //Checks
            #region
            if (inputData.Length != outputData.Length) throw new Exception("must be the same length");

            if (inputData.Length == 0) throw new ArgumentException("There must be at least one piece of training data.");

            int inputSize = bot.transformations[0].cols;

            int outputSize = bot.transformations[bot.transformations.Length - 1].rows;

            for (int i = 0; i < inputData.Length; i++)
            {
                if (inputData[i].rows != inputSize) throw new Exception($"Wrong input size, inputData[{i}]");

                if (outputData[i].rows != outputSize) throw new Exception($"Wrong output size, outputData[{i}]");
            }
            #endregion

            _inputData = inputData;
            _outputData = outputData;
            _bot = bot;
        }

        //Cost function and derivatives
        #region 
        public double CostFunction(double output, double expectedOutput)
        {
            return (output - expectedOutput) * (output - expectedOutput);
        }

        public double DCostWrtFinalA(double output, double expectedOutput) // with respect to output 
        {
            return (2 * (output - expectedOutput));
        }

        public double CostFunction(Matrix output, Matrix expectedOutput)
        {
            if (output.rows != expectedOutput.rows || output.cols != expectedOutput.cols) throw new Exception("output and expectedOutput must be of the same size");

            double cost = 0;


            for (int i = 0; i < output.mat.Length; i++)
            {
                cost += (output.mat[i] - expectedOutput.mat[i]) * (output.mat[i] - expectedOutput.mat[i]);
            }
            return cost;
        }

        public Matrix DCostWrtFinalAl(Matrix output, Matrix expectedOutput) // with respect to the output neurons
        {
            if (output.rows != expectedOutput.rows || output.cols != expectedOutput.cols) throw new Exception("output and expectedOutput must be of the same size");

            Matrix matrix = new Matrix(output.rows, output.cols);

            for (int i = 0; i < output.mat.Length; i++)
            {
                matrix.mat[i] = DCostWrtFinalA(output.mat[i], expectedOutput.mat[i]);
            }
            return matrix;
        }


        #endregion

        public static Matrix DCostWrtAL(Matrix DCostWrtALPlus1, Matrix ZLPlus1, int layer, NeuralBot bot)  // layer as in transformations[layer] * ActivationLayer + Bias[layer] = ZLayerPlus1
        {
            Matrix __DCostWrtAL = new Matrix(bot.transformations[layer].cols, 1);
            for (int i = 0; i < __DCostWrtAL.rows; i++)
            {
                double sum = 0;

                for (int j = 0; j < bot.transformations[layer].rows; j++)
                {
                    sum += DCostWrtALPlus1[j, 0] * bot.DActiveFuncWrtZ(ZLPlus1[j, 0]) * bot.transformations[layer][j, i];
                }

                __DCostWrtAL[i, 0] = sum;
            }
            return __DCostWrtAL;
        }

        public static Matrix DCostWrtASecondLastLayer(Matrix DCostWrtALFinal, Matrix ZLFinal, NeuralBot bot)
        {
            Matrix __DCostWrtAL = new Matrix(bot.transformations[^1].cols, 1);
            for (int i = 0; i < __DCostWrtAL.rows; i++)
            {
                double sum = 0;

                for (int j = 0; j < bot.transformations[^1].rows; j++)
                {
                    sum += DCostWrtALFinal[j, 0] * bot.DFinalActiveFuncWrtZ(ZLFinal[j, 0]) * bot.transformations[^1][j, i];
                }

                __DCostWrtAL[i, 0] = sum;
            }
            return __DCostWrtAL;
        }

        public static Matrix DCostWrtZL(Matrix DCostWrtZLPlus1, Matrix ZL, int layer, NeuralBot bot)  // layer as in transformations[layer] * ActivationLayer + Bias[layer] = ZLayerPlus1
        {
            Matrix __DCostWrtZL = new Matrix(bot.transformations[layer].cols, 1);

            for (int i = 0; i < bot.transformations[layer].cols; i++)
            {
                double DCostWrtAl = 0;


                for (int j = 0; j < bot.transformations[layer].rows; j++)
                {
                    DCostWrtAl += bot.transformations[layer][j, i] * DCostWrtZLPlus1[j, 0];
                }

                __DCostWrtZL[i, 0] = DCostWrtAl * bot.DActiveFuncWrtZ(ZL[i, 0]);
            }


            return __DCostWrtZL;
        }







        public void Run()
        {
            Matrix[] Cweights = new Matrix[_bot.transformations.Length]; // delta C gradient weights
            Matrix[] Cbiases = new Matrix[_bot.biases.Length];  // delta C gradient biases

            if (Cweights.Length != Cbiases.Length) throw new Exception("edit the following code with the weights and biases");

            for (int i = 0; i < Cweights.Length; i++)
            {
                Cweights[i] = new Matrix(_bot.transformations[i].rows, _bot.transformations[i].cols);
                Cbiases[i] = new Matrix(_bot.biases[i].rows, _bot.biases[i].cols);
            }


            for (int i = 0; i < Batches; i++)
            {
                for (int j = 0; j < Cweights.Length; j++)
                {
                    Cweights[j] = Matrix.ZeroMatrix(Cweights[j].rows, Cweights[j].cols);
                    Cbiases[j] = Matrix.ZeroMatrix(Cbiases[j].rows, Cbiases[j].cols);
                }

                for (int j = 0; j < BatchSize; j++)
                {
                    int currentRec = (i * BatchSize + j) % _inputData.Length;

                    Matrix[] ALayers = new Matrix[_bot.transformations.Length + 1];

                    Matrix[] ZLayers = new Matrix[_bot.transformations.Length + 1];


                    Matrix[] ZLayerDerivatives = new Matrix[_bot.transformations.Length + 1];


                    //finds all the layers
                    #region
                    ALayers[0] = _inputData[currentRec].Duplicate();

                    if (ALayers[0].cols != 1) throw new ArgumentOutOfRangeException();
                    if (ALayers[0].rows != _bot.transformations[0].cols) throw new Exception("INPUT MUST BE OF CORRECT SIZE");



                    for (int k = 1; k < ALayers.Length - 1; k++)
                    {
                        ZLayers[k] = _bot.transformations[k - 1] * ALayers[k - 1];
                        ZLayers[k] += _bot.biases[k - 1];

                        ALayers[k] = _bot.ActiveFunc(ZLayers[k].Duplicate());

                    }

                    ZLayers[^1] = _bot.transformations[^1] * ALayers[^2];
                    ZLayers[^1] += _bot.biases[^1];
                    ALayers[^1] = _bot.FinalActiveFunc(ZLayers[^1].Duplicate());
                    #endregion

                    //find the derivative with respect to the neurons of the hidden layers of the cost function.
                    #region
                    //ALayerDerivatives[^1] = CostFunctionDeriv(ALayers[^1], _outputData[currentRec]);
                    Matrix DCostWrtOutput = DCostWrtFinalAl(ALayers[^1], _outputData[currentRec]);

                    ZLayerDerivatives[^1] = new Matrix(DCostWrtOutput.rows, 1);

                    for (int k = 0; k < DCostWrtOutput.rows; k++)
                    {
                        ZLayerDerivatives[^1][k, 0] = DCostWrtOutput[k, 0] * _bot.DFinalActiveFuncWrtZ(ZLayers[^1][k, 0]);
                    }





                    for (int k = ALayers.Length - 2; k > 0; k--)
                    {
                        ZLayerDerivatives[k] = DCostWrtZL(ZLayerDerivatives[k + 1], ZLayers[k], k, _bot);
                    }
                    #endregion

                    //find the derivative with respect to the weights and biases.
                    #region
                    for (int k = 0; k < _bot.transformations.Length; k++)
                    {
                        for (int e = 0; e < _bot.transformations[k].rows; e++)
                        {
                            for (int u = 0; u < _bot.transformations[k].cols; u++)
                            {
                                Cweights[k][e, u] += ALayers[k][u, 0] * ZLayerDerivatives[k + 1][e, 0];

                            }
                            Cbiases[k][e, 0] += ZLayerDerivatives[k + 1][e, 0];
                        }
                    }


                    #endregion



                }

                for (int j = 0; j < Cweights.Length; j++)
                {
                    for (int k = 0; k < Cweights[j].mat.Length; k++)
                    {
                        double filler = _bot.transformations[j].mat[k] - Cweights[j].mat[k] * learningRate / (double)BatchSize;
                        if (double.IsFinite(filler))
                        {
                            _bot.transformations[j].mat[k] = filler;
                        }
                    }

                    for (int k = 0; k < Cbiases[j].mat.Length; k++)
                    {
                        double filler = _bot.biases[j].mat[k] - Cbiases[j].mat[k] * learningRate / (double)BatchSize;
                        if (double.IsFinite(filler))
                        {
                            _bot.biases[j].mat[k] = filler;
                        }
                    }
                }

                //change the _bot based on the the avg (basically just add it.)
            }
        }
    }

    public class GetData
    {
        public Matrix[] inputData;
        public Matrix[] outputData;



        public GetData()
        {
            inputData = Array.Empty<Matrix>();
            outputData = Array.Empty<Matrix>();
        }

        //for saving to computer     
        #region
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Clear();

            for (int i = 0; i < inputData.Length; i++)
            {
                sb.Append(inputData[i].ToString());
                sb.Append("INPUT");
                sb.Append(outputData[i].ToString());
                if (i < inputData.Length - 1) sb.Append("OUTPUT");
            }

            return sb.ToString();
        }

        public static GetData Parse(string str)
        {
            string[] splitString = str.Split("OUTPUT");
            Matrix[] inputData = new Matrix[splitString.Length];
            Matrix[] outputData = new Matrix[splitString.Length];

            GetData getter = new GetData();

            for (int i = 0; i < splitString.Length; i++)
            {
                string[] transformPlusBias = splitString[i].Split("INPUT");
                inputData[i] = Matrix.Parse(transformPlusBias[0]);
                outputData[i] = Matrix.Parse(transformPlusBias[1]);

            }



            getter.inputData = inputData;
            getter.outputData = outputData;


            return getter;
        }

        #endregion




        public static int[][,] GenerateRandomGrids(int noGrids)
        {
            int[][,] grids = new int[noGrids][,];

            for (int i = 0; i < noGrids; i++)
            {
                grids[i] = (int[,])Program.startGridTemplate.Clone();

                int no2Player = Random.Shared.Next(9, 15);
                int no3Player = Random.Shared.Next(9, 15);
                int noEmptySlots = 61 - no2Player - no3Player;




                for (int j = 0; j < Program.gridSlots.GetLength(0); j++)
                {
                    int rndDistrobuter = Random.Shared.Next(0, no3Player + no2Player + noEmptySlots);
                    int valueOfSlot = 0;
                    if (rndDistrobuter < noEmptySlots)
                    {
                        valueOfSlot = 1;
                        noEmptySlots--;
                    }
                    else if (rndDistrobuter < noEmptySlots + no2Player)
                    {
                        valueOfSlot = 2;
                        no2Player--;
                    }
                    else
                    {
                        valueOfSlot = 3;
                        no3Player--;
                    }

                    grids[i][Program.gridSlots[j, 1], Program.gridSlots[j, 0]] = valueOfSlot;
                }

                if (noEmptySlots != 0 || no3Player != 0 || no2Player != 0)
                {
                    throw new Exception($"smth went wrong: {i}, {noEmptySlots}, {no2Player}, {no3Player}");
                }

            }

            return grids;


        }

        public static Matrix SoftMax(Matrix matrix)
        {
            if (matrix.cols != 1) throw new ArgumentOutOfRangeException();

            double maxValue = double.MinValue;

            double denominator = 0;

            double[] expValues = new double[matrix.rows];

            for (int i = 0; i < matrix.rows; i++)
            {

                if (maxValue < matrix[i, 0]) maxValue = matrix[i, 0];
            }

            for (int i = 0; i < matrix.rows; i++)
            {
                matrix[i, 0] -= (double)maxValue;

                expValues[i] = Math.Exp(matrix[i, 0]);

                denominator += expValues[i];
            }

            for (int i = 0; i < matrix.rows; i++)
            {
                matrix[i, 0] = expValues[i] / denominator;
            }

            return matrix;
        }

        public static Matrix GetInput(int[,] grid, int player)
        {
            Matrix input = new Matrix(61, 1);
            int index = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        if (grid[i, j] == 1) input[index, 0] = 0;
                        else if (player == 2) input[index, 0] = -(grid[i, j] - 1) * 2 + 3;
                        else input[index, 0] = (grid[i, j] - 1) * 2 - 3;

                        index++;
                    }
                }
            }

            if (index != 61)
            {
                throw new Exception("Index != 61 || input[61] != 1");
            }
            return input;
        }


        public static int[,] GetGrid(Matrix input) // player is assumed to be 2
        {
            int[,] grid = (int[,])Program.startGridTemplate.Clone();
            int covered = 0;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        if (input[covered, 0] == 0)
                        {
                            grid[i, j] = 1;
                        }
                        else if (input[covered, 0] == 1)
                        {
                            grid[i, j] = 2;
                        }
                        else if (input[covered, 0] == -1)
                        {
                            grid[i, j] = 3;
                        }
                        else
                        {
                            throw new Exception("input of incorrect format");
                        }



                        covered++;
                    }
                }
            }

            return grid;
        }

        public void Merge(GetData gd)
        {
            inputData = inputData.Concat(gd.inputData).ToArray();
            outputData = outputData.Concat(gd.outputData).ToArray();


        }

        public int Length
        {
            get { return inputData.Length; }
        }

        public void runCheck()
        {
            if (inputData.Length != outputData.Length) { throw new Exception(); }

            int? inputDataSize = null;
            int? outputDataSize = null;

            for (int i = 0; i < inputData.Length; i++)
            {
                if (inputData[i] == null)
                {
                    throw new ArgumentNullException($"inputData[{i}] was null");
                }
                else if (outputData[i] == null)
                {
                    throw new ArgumentNullException($"outputData[{i}] was null");
                }

                if (inputDataSize == null)
                {
                    inputDataSize = inputData[i].mat.Length;
                }
                if (outputDataSize == null)
                {
                    outputDataSize = outputData[i].mat.Length;
                }

                if (inputDataSize != inputData[i].mat.Length)
                {
                    throw new Exception($"argument {i} of input data does not have the same shape");
                }
                if (inputDataSize != inputData[i].mat.Length)
                {
                    throw new Exception($"argument {i} of output data does not have the same shape");
                }


            }
        }

        public void parallelAddRecords(int RecCount)
        {
            int noTasks = Environment.ProcessorCount * 4;
            int[] recCounts = new int[noTasks];

            GetData[] recordHolders = new GetData[noTasks];

            Task[] tasks = new Task[noTasks];

            int minRecs = (RecCount - (RecCount % noTasks)) / noTasks;



            for (int i = 0; i < noTasks; i++)
            {
                int taskIndex = i;

                recCounts[taskIndex] = minRecs;
                if (taskIndex < (RecCount % noTasks))
                {
                    recCounts[taskIndex]++;
                }



                tasks[taskIndex] = Task.Run(() =>
                {
                    recordHolders[taskIndex] = new GetData();
                    recordHolders[taskIndex].AddRecords(recCounts[taskIndex]);
                });

            }

            Task.WaitAll(tasks);

            for (int i = 0; i < noTasks; i++)
            {
                this.Merge(recordHolders[i]);
            }

        }

        public void AddRecords(int RecCount)
        {
            if (RecCount < 0) throw new ArgumentOutOfRangeException("RecCount must be greater than or equal to 0");


            Matrix[] appendingInputData = new Matrix[RecCount];
            Matrix[] appendingOutputData = new Matrix[RecCount];
            int[][,] grids = GenerateRandomGrids(RecCount);


            //initialising
            for (int i = 0; i < RecCount; i++)
            {


                appendingInputData[i] = new Matrix(61, 1);

                appendingOutputData[i] = new Matrix(1, 1);


                int[,] grid = grids[i];
                int player = (i % 2) + 2;



                //getting input matrix
                #region
                appendingInputData[i] = GetInput(grid, player);
                #endregion


                //getting output matrix
                #region

                int score = 0;
                (_, score) = Bots.DepthScoringBotHelper(player, player, grid, 2);
                appendingOutputData[i][0, 0] = (double)score;


                #endregion




            }


            //adding it to the data
            inputData = inputData.Concat(appendingInputData).ToArray();
            outputData = outputData.Concat(appendingOutputData).ToArray();
        }
    }
}
