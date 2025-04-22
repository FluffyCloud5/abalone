using System.Numerics;
using System.Text;

//WONT WORK RN BECAUSE I'M IN THE MIDDLE OF TRANSITIONING TO A SORTED GRID


//Things to do:
// 1. Make the Move in trainer only return the move for the grid, and not actually move it.
// 2. Make the Training sytem more optimal to handle edge cases, and be more abstract. 
// 3. maybe make it so that the MCTS can learn from games between two scoringbots.
// 4. Make the MCTS use more current techniques (play out with scoring/or rnd bots)


//Things to optimize: 
// 1. Make a more memory effecient format for storing MCTS infomation in a file.


namespace abalone
{
    public class MCTS
    {
        public MCTS()
        {

        }

        public int GetIndexOrAdd(uint[] compactGrid) // ADDS STATE IF NOT THERE AND RETURNS INDEX.
        {
            (int index, bool existsAlready) = BinarySearchDiagnostics(compactGrid);
            if (existsAlready) { return index; }
            else if (index == -1 ) { index = states.Count; }
            states.Insert(index,compactGrid);
            wins.Insert(index,0);
            draws.Insert(index, 0);
            losses.Insert(index, 0);

            return index;
        }

        private (int index, bool existsAlready) BinarySearchDiagnostics(uint[] compactGrid)
        {
            if (states.Count == 0) return (0,false);
            int L = 0;
            int U = states.Count;
            int index = 0;


            for (int i = 0; i < states.Count+10; i++)
            {
                int dif = U - L;

                if (dif < 0)
                {
                    throw new Exception("Not supposed to happen, list is probably not sorted");
                }
                if(dif == 0)
                {
                    return (L, false);
                }
                if (dif == 1)
                {
                    int compare = Functions.Compare(compactGrid, states[L]);
                    if (compare == 0)
                        return (L, true);
                    else if(compare == 1)
                        return (L+1, false);
                    else return (L, false);
                }
                else if (dif % 2 == 0)
                {
                    index = (U + L) / 2;
                    int compare = Functions.Compare(compactGrid, states[index]);
                    if (compare == -1)
                    {
                        U = index;
                    }
                    else if (compare == 1)
                    {
                        L = index + 1;
                    }
                    else
                    {
                        return (index, true);
                    }
                }
                else if (dif % 2 == 1)
                {
                    index = (U + L - 1) / 2;
                    int compare = Functions.Compare(compactGrid, states[index]);
                    if (compare == -1)
                    {
                        U = index;
                    }
                    else if (compare == 1)
                    {
                        L = index + 1;
                    }
                    else
                    {
                        return (index, true);
                    }
                }

            }

            throw new Exception("THIS SHOULDNT HAPPEN");
        }

        public int BinarySearch(uint[] compactGrid)
        {
            if (states.Count == 0) return -1;
            int L = 0; //inclusive lower
            int U = states.Count; //non inclusive upper
            int index = 0;

            int maxLooks = states.Count+10;
            int i = 0;
            while(L <= U && i < maxLooks)
            {
                i++;
                int dif = U - L;

                if (dif < 0)
                {
                    throw new Exception("Not supposed to happen, list is probably not sorted");
                }
                if (dif == 0)
                {
                    return -1;
                }
                if (dif == 1)
                {
                    if (Functions.Compare(compactGrid, states[L]) == 0)
                        return L;
                    return -1;
                }
                else if (dif % 2 == 0)
                {
                    index = (U + L) / 2;
                    int compare = Functions.Compare(compactGrid, states[index]);
                    if (compare == -1)
                    {
                        U = index;
                    }
                    else if (compare == 1)
                    {
                        L = index +1;
                    }
                    else
                    {
                        return index;
                    }
                }
                else if (dif % 2 == 1)
                {
                    index = (U + L - 1) / 2;
                    int compare = Functions.Compare(compactGrid, states[index]);
                    if (compare == -1)
                    {
                        U = index;
                    }
                    else if (compare == 1)
                    {
                        L = index + 1;
                    }
                    else
                    {
                        return index;
                    }
                }
            }
            throw new Exception("THIS SHOULDNT HAPPEN");





            /*
            for (int i = 0; i < states.Count; i++)
            {
                int dif = U - L;
                
                if(dif <= 0)
                {
                    throw new Exception("Not supposed to happen, list is probably not sorted");
                }
                if (dif == 1)
                {
                    if (Functions.Compare(compactGrid, states[L]) == 0)
                        return L;
                    return -1;
                }
                else if (dif % 2 == 0)
                {
                    index = (U + L) / 2;
                    int compare = Functions.Compare(compactGrid, states[index]);
                    if (compare == -1)
                    {
                        U = index;
                    }
                    else if(compare == 1)
                    {
                        L = index+1;
                    }
                    else
                    {
                        return index;
                    }
                }
                else if (dif % 2 == 1)
                {
                    index = (U + L - 1) / 2;
                    int compare = Functions.Compare(compactGrid, states[index]);
                    if (compare == -1)
                    {
                        U = index;
                    }
                    else if (compare == 1)
                    {
                        L = index + 1;
                    }
                    else
                    {
                        return index;
                    }
                }

            }
            */

        }

        public void Sort()
        {
            for (int i = 1; i < states.Count; i++)
            {
                int insertIndex = -1;
                for(int j = 0; j < i; j++)
                {
                    int comparison = Functions.Compare(states[i], states[j]);
                    if (comparison == -1)
                    {
                        insertIndex = j;
                        break;
                    }
                }

                if(insertIndex != -1)
                {
                    MoveIndex(insertIndex,i);
                }
                insertIndex = -1;
            }
        }

        private void MoveIndex(int destination , int position)
        {
            uint[] state = states[position];
            int win = wins[position];
            int loss = losses[position];
            int draw = draws[position];

            states.RemoveAt(position);
            wins.RemoveAt(position);
            losses.RemoveAt(position);
            draws.RemoveAt(position);

            states.Insert(destination, state);
            wins.Insert(destination, win);
            losses.Insert(destination, loss);
            draws.Insert(destination, draw);
        } // FOR SORT 

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("States,Wins,Draws,Losses");

            for (int i = 0; i < states.Count; i++)
            {
                // Create the state string by joining the four numbers with periods.
                sb.AppendFormat("{0}.{1}.{2}.{3},{4},{5},{6}",
                    states[i][0], states[i][1], states[i][2], states[i][3],
                    wins[i], draws[i], losses[i]);

                if (i != states.Count - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        public byte[] ToByte()
        {
            List<byte> list = new List<byte>();
            byte[] bytes;

            for (int i = 0; i < states.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bytes = BitConverter.GetBytes(states[i][j]);
                    for (int k = 0; k < 4; k++)
                    {
                        list.Add(bytes[k]);
                    }
                }
                bytes = BitConverter.GetBytes(wins[i]);
                for (int j = 0; j < 4; j++)
                {
                    list.Add(bytes[j]);
                }
                bytes = BitConverter.GetBytes(losses[i]);
                for (int j = 0; j < 4; j++)
                {
                    list.Add(bytes[j]);
                }
                bytes = BitConverter.GetBytes(draws[i]);
                for (int j = 0; j < 4; j++)
                {
                    list.Add(bytes[j]);
                }
            }
            return list.ToArray();
        }

        public static MCTS Parse(byte[] inputBytes)
        {
            if (inputBytes.Length % 28 != 0) throw new Exception("NO");

            MCTS mcts = new MCTS();

            int index = 0;

            for (int i = 0; i < inputBytes.Length / 28; i++)
            {
                mcts.states.Add(new uint[4]);


                for (int j = 0; j < 4; j++)
                {
                    index = i * 28 + 4 * j;
                    mcts.states[i][j] = BitConverter.ToUInt32(inputBytes, index);
                }

                index += 4;
                mcts.wins.Add(BitConverter.ToInt32(inputBytes, index));
                index += 4;
                mcts.losses.Add(BitConverter.ToInt32(inputBytes, index));
                index += 4;
                mcts.draws.Add(BitConverter.ToInt32(inputBytes, index));
            }
            return mcts;
        }

        public static MCTS Parse(string str)
        {
            MCTS mcts = new MCTS();

            string[] lines = str.Split(Environment.NewLine);

            if (lines[0] != "States,Wins,Draws,Losses")
            {
                throw new Exception("Invalid MCTS string format");
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length != 4)
                {
                    throw new Exception("Invalid MCTS string format");
                }
                string[] stateParts = parts[0].Split('.');
                if (stateParts.Length != 4)
                {
                    throw new Exception("Invalid MCTS string format");
                }
                uint[] state = new uint[4];
                for (int j = 0; j < 4; j++)
                {
                    state[j] = uint.Parse(stateParts[j]);
                }
                mcts.states.Add(state);
                mcts.wins.Add(int.Parse(parts[1]));
                mcts.draws.Add(int.Parse(parts[2]));
                mcts.losses.Add(int.Parse(parts[3]));
            }

            return mcts;
        }

        public static void trim(MCTS mcts)
        {
            int upperBound = mcts.states.Count;


            for (int i = 0; i < upperBound; i++)
            {

                if (mcts.wins[i] == 0 && mcts.losses[i] == 0 && mcts.draws[i] == 0)
                {
                    mcts.states.RemoveAt(i);
                    mcts.wins.RemoveAt(i);
                    mcts.losses.RemoveAt(i);
                    mcts.draws.RemoveAt(i);
                    i--;
                    upperBound--;
                }

            }
        }

        public List<uint[]> states = new List<uint[]>(); // uint[] are four long and contain the compacted state.

        //Wins, losses & draws of states from player 2 perspective.
        #region
        public List<int> wins = new List<int>();

        public List<int> losses = new List<int>();

        public List<int> draws = new List<int>();
        #endregion
    }

    public class MCTSTrainer
    {
        public static MCTS Train(int games, MCTS? mcts)
        {
            if (mcts == null)
            {
                mcts = new MCTS();
            }
            int maxMovesPerGame = 4000;
            int[,] grid = (int[,])Program.startGridTemplate.Clone();
            int winner = 0;

            int whoPlays = 2;
            for (int i = 0; i < games; i++)
            {
                List<uint[]> statesVisited = new List<uint[]>();

                //GAME
                for (int j = 0; j < maxMovesPerGame; j++)
                {
                    statesVisited.Add(Functions.CompactGrid(grid));
                    int[] move = ExplorationMove(grid, mcts, whoPlays);
                    Program.Move(move, grid);

                    


                    //check if there is a winner
                    winner = Program.Winner(grid);
                    if (winner == 2 || winner == 3) break;



                    //end of move
                    whoPlays = 5 - whoPlays; // switch player

                }
                statesVisited.Add(Functions.CompactGrid(grid));


                // end of game
                winner = Program.Winner(grid);
                UpdateMCTS(statesVisited, winner, mcts);



                grid = (int[,])Program.startGridTemplate.Clone();
                whoPlays = 3 - (i % 2);

            }

            return mcts;
        }

        public static double UCT(int winsParent, int lossesParent, int drawsParent, int winsChild, int lossesChild, int drawsChild)
        {
            int visitedParent = winsParent + lossesParent + drawsParent + 1;
            int visitedChild = winsChild + lossesChild + drawsChild;



            if (visitedChild == 0)
            {
                return 1000000d;
            }
            return ((double)(winsChild - lossesChild) / (double)(visitedChild)) + Math.Sqrt(2 * Math.Log((double)visitedParent) / (double)visitedChild);
        }

        public static int[] ExplorationMove(int[,] grid, MCTS mcts, int player)
        {
            uint[] compactGrid = Functions.CompactGrid(grid);

            //GETTING INDEX OF STATE
            #region
            int indexState = mcts.GetIndexOrAdd(compactGrid);
            #endregion

            // Getting Move, And adding moves.
            #region

            List<int[]> legalMoves = Program.LegalMoves(grid, player);

            double? bestUCT = null;
            int bestUCTIndex = -1;

            int denominator = 2;

            for ( int k = 0; k < legalMoves.Count; k++)
            {
                int[,] actionedGrid = Program.Move(legalMoves[k][0], legalMoves[k][1], legalMoves[k][2], (int[,])grid.Clone());
                uint[] compactActionedGrid = Functions.CompactGrid(actionedGrid);

                int indexActionedGrid = mcts.BinarySearch(compactActionedGrid);

                double uct = 0;

                if (indexActionedGrid == -1)
                {
                    uct = 1000000d;
                }
                else
                {
                    uct = UCT(mcts.wins[indexState], mcts.losses[indexState], mcts.draws[indexState], mcts.wins[indexActionedGrid], mcts.losses[indexActionedGrid], mcts.draws[indexActionedGrid]);
                }

                if (bestUCT == null)
                {
                    bestUCT = uct;
                    bestUCTIndex = k;
                    denominator = 2;
                }
                else if (uct > bestUCT)
                {
                    bestUCT = uct;
                    bestUCTIndex = k;
                    denominator = 2;
                }
                else if (uct == bestUCT)
                {
                    if (Random.Shared.Next(0, denominator) == 0)
                    {
                        bestUCT = uct;
                        bestUCTIndex = k;
                    }
                    denominator++;
                }
            }


            #endregion

            //RETURN
            #region
            if (bestUCT == null)
                return Program.defaultNullMove;
            else return legalMoves[bestUCTIndex];
            #endregion
        }

        public static int[] newExplorationMove(int[,] grid, MCTS mcts, int player)
        {
            uint[] compactGrid = Functions.CompactGrid(grid);

            //GETTING INDEX OF STATE
            #region
            int indexState = mcts.GetIndexOrAdd(compactGrid);
            #endregion

            // Getting Move, And adding moves.
            #region

            List<int[]> legalMoves = Program.LegalMoves(grid, player);

            double? bestUCT = null;
            int bestUCTIndex = -1;

            int denominator = 2;

            for (int k = 0; k < legalMoves.Count; k++)
            {
                int[,] actionedGrid = Program.Move(legalMoves[k][0], legalMoves[k][1], legalMoves[k][2], (int[,])grid.Clone());
                uint[] compactActionedGrid = Functions.CompactGrid(actionedGrid);

                int indexActionedGrid = mcts.BinarySearch(compactActionedGrid);

                double uct = 0;

                if (indexActionedGrid == -1)
                {
                    uct = 1000000d;
                }
                else
                {
                    uct = UCT(mcts.wins[indexState], mcts.losses[indexState], mcts.draws[indexState], mcts.wins[indexActionedGrid], mcts.losses[indexActionedGrid], mcts.draws[indexActionedGrid]);
                }

                if (bestUCT == null)
                {
                    bestUCT = uct;
                    bestUCTIndex = k;
                    denominator = 2;
                }
                else if (uct > bestUCT)
                {
                    bestUCT = uct;
                    bestUCTIndex = k;
                    denominator = 2;
                }
                else if (uct == bestUCT)
                {
                    if (Random.Shared.Next(0, denominator) == 0)
                    {
                        bestUCT = uct;
                        bestUCTIndex = k;
                    }
                    denominator++;
                }
            }


            #endregion

            //RETURN
            #region
            if (bestUCT == null)
                return Program.defaultNullMove;
            else return legalMoves[bestUCTIndex];
            #endregion
        }

        public static void UpdateMCTS(List<uint[]> statesVisited, int winner, MCTS mcts)
        {
            int index = 0;
            
            if (winner == 0)
            {

                for (int j = 0; j < statesVisited.Count; j++)
                {
                    index = mcts.GetIndexOrAdd(statesVisited[j]);
                    
                    mcts.draws[index]++;
                }
            }
            else if (winner == 2)
            {

                for (int j = 0; j < statesVisited.Count; j++)
                {
                    index = mcts.GetIndexOrAdd(statesVisited[j]);

                    mcts.wins[index]++;
                }
            }
            
            else if (winner == 3)
            {
                
                for (int j = 0; j<statesVisited.Count; j++)
                {
                    index = mcts.GetIndexOrAdd(statesVisited[j]);
                    
                    mcts.losses[index]++;
                }         
            }
        }
    }
}