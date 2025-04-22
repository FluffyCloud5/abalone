// WidgetProvider.cs

namespace abalone
{
    public class Functions
    {
        //FOR COMPACTING GRID
        #region
        public static uint[] CompactGrid(int[,] grid)
        {
            uint[] compactedGrid = new uint[4];


            uint unit = 0;
            int whichCompactInt = 0;
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    uint info = 0;
                    switch (grid[y, x])
                    {
                        case (0):
                            info = 4;
                            break;
                        case (1):
                            info = 1;
                            break;
                        case (2):
                            info = 2;
                            break;
                        case (3):
                            info = 3;
                            break;
                        default:
                            info = 0;
                            break;
                    }
                    if (info != 4)
                    {
                        if (unit == 16)
                        {
                            whichCompactInt++;
                            unit = 0;
                        }
                        compactedGrid[whichCompactInt] += info * (uint)(Math.Pow(4, unit));
                        unit++;
                    }

                }
            }

            return compactedGrid;
        }

        public static int[,] UncompactGrid(uint[] compactedGrid)
        {
            int[,] grid = (int[,])Program.startGridTemplate.Clone();
            int[,] halfUncompacted = new int[4, 16];


            for (int i = 0; i < 4; i++)
            {
                uint num = compactedGrid[i];
                for (int j = 15; j >= 0; j--)
                {
                    uint get = (uint)MathF.Pow(4, j);
                    while (num >= get)
                    {
                        num -= get;
                        halfUncompacted[i, j]++;
                    }
                    if (halfUncompacted[i, j] == 0)
                        halfUncompacted[i, j] = 4;
                }
            }

            uint unit = 0;
            int whichCompactInt = 0;
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (grid[y, x] != 0)
                    {
                        if (unit == 16)
                        {
                            whichCompactInt++;
                            unit = 0;
                        }
                        grid[y, x] = halfUncompacted[whichCompactInt, unit];
                        unit++;
                    }
                }
            }

            return grid;
        }

        public static int Compare(uint[] compactGrid1, uint[] compactGrid2) // if grid1 > grid2 then 1
        {

            if(compactGrid1.Length != 4 || compactGrid2.Length != 4) throw new Exception("WRONG SIZE COMPACT GRID");

            for (int i = 0; i < 4; i++)
            {
                if (compactGrid1[i] < compactGrid2[i]) return -1;
                else if (compactGrid1[i] > compactGrid2[i]) return 1;
            }

            return 0;
        }
        #endregion


        //FOR HEURISTIC BOT
        #region
        public static int AddState(uint[] compactGrid, double scoreHere, int actions)
        {
            int index = states.Count;
            states.Add(compactGrid);
            score.Add(scoreHere);
            visited.Add(0);
            actionWinProbabability.Add(new double[actions]);
            for (int i = 0; i < actions; i++)
            {
                actionWinProbabability[index][i] = (double)i / (double)actions;
            }
            //winningState.Add(false);

            return index;
        }

        public static List<uint[]> states = new List<uint[]>(); // uint[] are four long and contain the compacted state.

        public static List<double> score = new List<double>(); // contains the score for each action of each state.

        public static List<double[]> actionWinProbabability = new List<double[]>(); // contains the chance of winning for each action of each state.

        public static List<int> visited = new List<int>();

        public static void createCSVOfNeuralBot(OldNeuralBot bot, string name, string fileLocation = "C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\")
        {

            string csvContent = bot.inputToMiddle.ToString() + "," + bot.middleToOutput.ToString();

            // Writing to a CSV file
            File.WriteAllText(fileLocation + name + ".csv", csvContent);
        }

        public static OldNeuralBot readCSVNeuralBot(string name ,string fileLocation = "C:\\Users\\kiera\\codingDocuments\\abaloneNeuralBot\\")
        {
            string csvContent = File.ReadAllText(fileLocation + name+ ".csv");

            int commaIndex = csvContent.IndexOf(",");

            string inputToMiddle = csvContent.Substring(0, commaIndex);
            string middleToOutput = csvContent.Substring(commaIndex + 1);

            OldNeuralBot bot = new OldNeuralBot();
            bot.inputToMiddle = Matrix.Parse(inputToMiddle);
            bot.middleToOutput = Matrix.Parse(middleToOutput);
            return bot;

        }
        #endregion


        //Saving to Computer
        #region
        public static void CSV(string file, string fileName, string fileLocation = "C:\\Users\\kiera\\codingDocuments\\abalone\\")
        {
            string csvContent = file;
            // Writing to a CSV file
            File.WriteAllText(fileLocation + fileName + ".csv", csvContent);
        }

        public static void bin(byte[] fileContent, string fileName, string fileLocation = "C:\\Users\\kiera\\codingDocuments\\abalone\\")
        {
            File.WriteAllBytes(fileLocation + fileName + ".bin", fileContent);
        }

        public static byte[] readBin( string fileName, string fileLocation = "C:\\Users\\kiera\\codingDocuments\\abalone\\")
        {
            return File.ReadAllBytes(fileLocation + fileName + ".bin");
        }

        public static string ReadCSV(string fileName, string fileLocation = "C:\\Users\\kiera\\codingDocuments\\abalone\\")
        {
            return File.ReadAllText(fileLocation + fileName + ".csv");
        }
        #endregion



    }
}
