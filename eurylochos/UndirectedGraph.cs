using System;
using System.Collections.Generic;

namespace eurylochos
{
    class UndirectedGraph
    {
        int[,] WeightedAdjacencyMatrix;
        string[] VertexAliases;

        /// <summary>
        /// Constructor for a Graph with a n x n Matrix (Adjacency Matrix)
        /// and an Array of String with Aliases for the Vertexes
        /// </summary>
        /// <param name="InitMatrix"></param> Adjacency Matrix for the Graph
        /// <param name="InitAliases"></param> Aliases for the Vertexes in this Graph
        public UndirectedGraph(int[,] InitMatrix, string[] InitAliases)
        {
            if (InitMatrix == null)
            {
                throw new ArgumentNullException("Argument must not be null, call constructor Graph(int[,] InitMatrix) for a Graph without Aliases");
            }
            if (InitAliases == null)
            {
                throw new ArgumentNullException("Argument must not be null, call constructor Graph(int[,] InitMatrix) for a Graph without Aliases");
            }

            if (InitMatrix.GetLength(0) != InitMatrix.GetLength(1))
            {
                throw new ArgumentException("Matrix must be square");
            }

            if (InitMatrix.GetLength(0) != InitAliases.Length)
            {
                throw new ArgumentException("Either all or none vertex must have an alias, call Graph(int[,] InitMatrix) for a constructor without Aliases");
            }

            if (!(this.CheckInit(InitMatrix)))
            {
                throw new ArgumentException("Adjacency Matrix for a undirected Graph must be symetric to the main diagonal");
            }

            this.WeightedAdjacencyMatrix = new int[InitMatrix.GetLength(0), InitMatrix.GetLength(1)];
            for (int i = 0; i < InitMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < InitMatrix.GetLength(1); j++)
                {
                    this.WeightedAdjacencyMatrix[i, j] = InitMatrix[i, j];
                }
            }

            this.VertexAliases = new string[InitAliases.Length];
            InitAliases.CopyTo(this.VertexAliases, 0);

        }
        /// <summary>
        /// Contstructor for a Graph with a n x n Matrix (Adjacency Matrix)
        /// </summary>
        /// <param name="InitMatrix"></param> Adjacency Matrix for the Graph
        public UndirectedGraph(int[,] InitMatrix)
        {
            if (InitMatrix == null)
            {
                throw new ArgumentNullException();
            }
            if (InitMatrix.GetLength(0) != InitMatrix.GetLength(1))
            {
                throw new ArgumentException("Matrix must be square");
            }

            this.WeightedAdjacencyMatrix = new int[InitMatrix.GetLength(0), InitMatrix.GetLength(1)];
            for (int i = 0; i < InitMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < InitMatrix.GetLength(1); j++)
                {
                    this.WeightedAdjacencyMatrix[i, j] = InitMatrix[i, j];
                }
            }

            this.VertexAliases = null;

            if (!this.CheckInit(InitMatrix))
            {
                throw new ArgumentException("Adjacency Matrix for a undirected Graph must be symetric to the main diagonal");
            }
        }

        private bool CheckInit(int[,] Matrix)
        {
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (!(Matrix[i,j] == Matrix[j,i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the shortest possible hamiltonian circle for this graph. Although it may work on all kinds of undirected graphs the code was designed to run on a complete undirected graph.
        /// will return null if no hamiltonian circle could be found.
        /// Especially the output of the progression bar will be useless if the graph is not complete.
        /// </summary>
        /// <param name="noisy">if noisy == true a progression bar will be printed to the System.Console output stream, if false no output will be done at all</param>
        /// <returns></returns>

        public int[] GetShortestHamiltonianCircle(bool noisy)
        {
            /**
             * Calculates the shortest hamiltonian circle in this graph AKA solving the traveling salesman problem.
             * This code follows a Branch and Bound approach.
             * With a depth first search all possible circles for this graph will be calculated.
             * The shortest hamiltonian circle found and its length will be stored.
             * A Path in the tree built by the depth first search will be discarded if it is longer than the length of the shortest circle found so far,
             * or if the path has visited less than half of the graphs vertexes but is allready longer than the half of the shortest circle found.
             * This works because the depth first search will find every possible hamiltonian circle twice, once clockwise and once counter clockwise.
             */



            // Variable deklaration
            int Length = this.WeightedAdjacencyMatrix.GetLength(0); // The Length of the Graph, The number of Vertexes in the Graph
            int Depth = 1; // The Level on which the depth first search currently operates
            int[] ShortestCircle = new int[Length + 1]; // saves the shortest hamiltonian circle that could be found so far
            long ShortestCircleLength = long.MaxValue; // saves the length of the shortest found hamiltonian circle to prevent constant calculation of that length
            int[] CurrentCircle = new int[Length + 1]; // contains the vertexes of the path that is currently calculated and will hopefully end in containing a hamiltonian circle
            long CurrentCircleLength = 0; // saves the length of the current path or circle to prevent constant calculation of that length
            bool[] Visited = new bool[Length]; // saves for every Vertex if it was visited by the current path/circle
            bool[] Sorted = new bool[Length]; // used to sort vertexes by their distance from the current vertex
            Queue<int>[] VertexQueues = new Queue<int>[Length]; // Array of Queues(FIFO structure) to queue the remaining vertexes for every level.
            int NextVertex = -1; // saves the next Vertex to be calculated used for Queue.TryDequeue(out NextVertex) method

            long Possibilities = 1; // stores the amount of possible circle, if the graph is complete. used for progress calculation
            long Calculated = 0; // the amount of circles that have been found.
            long Discarded = 0; // the amount of circles that have been discarded for being longer than the shortest found circle, before becoming a hamiltonian path.
            long IteratorCount = 0; // the amount of leafs in the tree that have been calculated by the depth first search. Does not really serve a purpose but is a interesting piece of information.

            int Progress = 0; // percentage of the progress
            bool[] ProgressPrinted = new bool[101]; // to only re-print the progress bar when a new percentage has been reached.

            //Variable Initialization
            for (long i = 1; i < Length; i++)
            {
                Possibilities *= i;
            }

            for (int i = 0; i < 101; i++)
            {
                ProgressPrinted[i] = false;
            }

            Visited[0] = true;
            Sorted[0] = true;
            CurrentCircle[0] = 0;
            ShortestCircle[0] = -1;
            VertexQueues[0] = null;

            for (int i = 1; i < Length; i++)
            {
                Visited[i] = false;
                Sorted[i] = false;
                CurrentCircle[i] = -1; // Dummy Value
                VertexQueues[i] = new Queue<int>(Length);
            }

            // will the VertexQueue for the first level, vertexes that are closer to the starting vertex will be prioritized over those who are further away.
            int MinLength = int.MaxValue;
            int MinVertex = -1;
            for (int i = 1; i < Length; i++)
            {
                for (int j = 1; j < Length; j++)
                {

                    if (MinLength > this.WeightedAdjacencyMatrix[CurrentCircle[Depth - 1], j] && !Sorted[j])
                    {
                        MinVertex = j;
                        MinLength = this.WeightedAdjacencyMatrix[CurrentCircle[Depth - 1], j];
                    }
                }
                if (MinVertex == -1)
                {
                    throw new Exception(); //this should not be able to happen, but just in case for debugging. May occur if the graph has only 1 vertex
                }
                Sorted[MinVertex] = true;
                VertexQueues[Depth].Enqueue(MinVertex);
                MinLength = int.MaxValue;
                MinVertex = -1;
            }


            while (Depth > 0)
            {

                //Print Progress
                if (noisy)
                {
                    Progress = (int)((((double)Calculated + (double)Discarded) / (double)Possibilities) * 100.0);
                    if (!ProgressPrinted[Progress])
                    {
                        System.Console.Clear();
                        System.Console.WriteLine("Progress: " + Progress + "%");
                        System.Console.Write("|");
                        for (int i = 1; i < 101; i++)
                        {
                            if (i <= Progress)
                            {
                                System.Console.Write(">");
                            }
                            else
                            {
                                System.Console.Write(" ");
                            }
                        }
                        System.Console.Write("|");
                        System.Console.WriteLine();
                        ProgressPrinted[Progress] = true;
                    }
                }
                
                if (Depth == Length)
                {
                    // At his point a hamiltonian path has been found, now it will try to complete it to a hamiltonian circle.
                    // If a circle is found it will be stored in ShortestCircle if it is shorter than the circle currently stored in ShortestCircle.
                    Calculated++;
                    IteratorCount++;

                    //Add start vertex as last vertex to complete circle, if there is a edge between them two
                    if (WeightedAdjacencyMatrix[CurrentCircle[Length - 1], CurrentCircle[0]] != 0)
                    {
                        CurrentCircle[Length] = CurrentCircle[0];
                        CurrentCircleLength += this.WeightedAdjacencyMatrix[CurrentCircle[Length - 1], CurrentCircle[Length]];

                        //Save Current Circle as Shortest Circle if it is shorter than the shortest one found so far
                        if (CurrentCircleLength < ShortestCircleLength)
                        {
                            ShortestCircleLength = CurrentCircleLength;
                            CurrentCircle.CopyTo(ShortestCircle, 0);
                        }

                        //remove last vertex again
                        CurrentCircleLength -= this.WeightedAdjacencyMatrix[CurrentCircle[Length - 1], CurrentCircle[Length]];
                        CurrentCircle[Length] = -1;
                    }

                    //remove last vertex from CurrentPath so that the next possibility will be calculated
                    Depth--;
                    CurrentCircleLength -= this.WeightedAdjacencyMatrix[CurrentCircle[Depth], CurrentCircle[Depth - 1]];
                    Visited[CurrentCircle[Depth]] = false;
                    CurrentCircle[Depth] = -1;
                }
                else //calculation to find all possible paths
                {
                    if (VertexQueues[Depth].TryDequeue(out NextVertex))
                    {
                        //Add next possible Vertex to the CurrentCircle
                        CurrentCircle[Depth] = NextVertex;
                        Visited[CurrentCircle[Depth]] = true;
                        CurrentCircleLength += this.WeightedAdjacencyMatrix[CurrentCircle[Depth - 1], CurrentCircle[Depth]];

                        //If Path is allready Longer than the shortest found circle, or it has visited less than half of the vertexes and is allready longer than the half
                        //of the shortest found circle it is discarded
                        if ((CurrentCircleLength > ShortestCircleLength) || (Depth < (Length / 2) && CurrentCircleLength > (ShortestCircleLength / 2)))
                        {
                            Visited[CurrentCircle[Depth]] = false;
                            CurrentCircleLength -= this.WeightedAdjacencyMatrix[CurrentCircle[Depth - 1], CurrentCircle[Depth]];
                            CurrentCircle[Depth] = -1;
                            long DiscardedSub = 1;
                            for (int i = 1; i < (Length - Depth); i++)
                            {
                                DiscardedSub *= i;
                            }
                            Discarded += DiscardedSub;
                            IteratorCount++;
                        }
                        else
                        {
                            //Go one step deeper
                            Depth++;
                            //Add all Vertexes that are unvisited by the CurrentCircle to the queue for the next level
                            Visited.CopyTo(Sorted, 0);
                            MinLength = int.MaxValue;
                            MinVertex = -1;
                            for (int i = Depth; i < Length; i++)
                            {
                                for (int j = 1; j < Length; j++)
                                {
                                    if (MinLength > this.WeightedAdjacencyMatrix[CurrentCircle[Depth - 1], j] && !Sorted[j])
                                    {
                                        MinLength = this.WeightedAdjacencyMatrix[CurrentCircle[Depth - 1], j];
                                        MinVertex = j;
                                    }
                                }
                                if (MinVertex == -1)
                                {
                                    throw new Exception(); //this should not be able to happen, but just in case for debugging
                                }
                                Sorted[MinVertex] = true;
                                VertexQueues[Depth].Enqueue(MinVertex);
                                MinVertex = -1;
                                MinLength = int.MaxValue;
                            }
                        }
                    }
                    else //if there are no more vertexes Left, go one step back and try the next vertex there
                    {
                        Depth--;
                        if (Depth > 0)
                        {
                            CurrentCircleLength -= this.WeightedAdjacencyMatrix[CurrentCircle[Depth], CurrentCircle[Depth - 1]];
                            Visited[CurrentCircle[Depth]] = false;
                            CurrentCircle[Depth] = -1;
                        }
                    }
                }
            }

            if (ShortestCircle[0] == -1)
            {
                // if no hamiltonian circle has been found return null
                return null;
            }
            else
            {
                // otherwise return the shortest hamiltonian circle found
                return ShortestCircle;
            }

        }
        /// <summary>
        /// Calculates the shortest possible hamiltonian circle for this graph. Although it may work on all kinds of undirected graphs the code was designed to run on a complete undirected graph.
        /// will return null if no hamiltonian circle could be found.
        /// Especially the output of the progression bar will be useless if the graph is not complete.
        /// </summary>
        /// <returns></returns>
        public int[] GetShortestHamiltonianCircle()
        {
            return this.GetShortestHamiltonianCircle(false);
        }

        private bool ContainsVertex(int[] Path, int Vertex)
        {
            for (int i = 0; i < Path.Length; i++)
            {
                if (Path[i] == Vertex)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// returns the Aliases of the Vertexes in a Path
        /// </summary>
        /// <param name="Path"></param>
        /// <returns>An Array of String with the Aliases of the Vertexes in the Path, return null if there are no Aliases in the current Graph</returns>
        public String[] GetAliases(int[] Path)
        {
            if (this.VertexAliases == null)
            {
                return null;
            }


            String[] result = new string[Path.Length];

            for (int i = 0; i < Path.Length; i++)
            {
                if (Path[i] < this.VertexAliases.Length)
                {
                    result[i] = this.VertexAliases[Path[i]];
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the Graph has Aliases, false if it is has no Aliases</returns>
        public bool HasAliases()
        {
            if (this.VertexAliases == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// returns The Alias for a Vertex
        /// </summary>
        /// <param name="Vertex"></param>
        /// <returns>a String with the Alias of the specified Vertex, return null if the Graph has no Aliases</returns>
        public String GetAlias(int Vertex)
        {
            if (this.VertexAliases == null)
            {
                return null;
            }
            if (Vertex >= 0 && Vertex < VertexAliases.Length)
            {
                return VertexAliases[Vertex];
            }
            throw new ArgumentOutOfRangeException();
        }
        /// <summary>
        /// Calculates the Length/Costs for a specified Path
        /// </summary>
        /// <param name="Path"></param>
        /// <returns>The Length/Costs of the Path as int, return 0 if the path consists of only 1 vertex</returns>
        public long GetPathLength(int[] Path)
        {
            long PathLength = 0;
            if (Path[0] == -1)
            {
                return int.MaxValue;
            }
            if (Path.Length < 2)
            {
                return 0; // If path only consists of 1 vertex (or even is null) it has no length and returns 0
            }
            for (int i = 1; i < Path.Length; i++)
            {
                if (Path[i] == -1)
                {
                    break;
                }
                PathLength = PathLength + this.WeightedAdjacencyMatrix[Path[i - 1], Path[i]]; // Add the length of all edges in the Path to PathLength

            }
            return PathLength;
        }
    }
}