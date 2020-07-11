using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace eurylochos
{
    class UndirectedGraphKopie
    {
        int[,] AdjMatrix;
        string[] VertexAliases;

        /// <summary>
        /// Constructor vor a Graph with a n x n Matrix (Adjacency Matrix)
        /// and an Array of String with Aliases for the Vertexes
        /// </summary>
        /// <param name="InitMatrix"></param> Adjacency Matrix for the Graph
        /// <param name="InitAliases"></param> Aliases for the Vertexes in this Graph
        public UndirectedGraphKopie(int[,] InitMatrix, string[] InitAliases)
        {
            if (InitMatrix == null)
            {
                throw new ArgumentNullException();
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

            this.AdjMatrix = new int[InitMatrix.GetLength(0), InitMatrix.GetLength(1)];
            for (int i = 0; i < InitMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < InitMatrix.GetLength(1); j++)
                {
                    this.AdjMatrix[i, j] = InitMatrix[i, j];
                }
            }

            this.VertexAliases = new string[InitAliases.Length];
            InitAliases.CopyTo(this.VertexAliases, 0);

        }
        /// <summary>
        /// Contstructor for a Graph with a n x n Matrix (Adjacency Matrix)
        /// </summary>
        /// <param name="InitMatrix"></param> Adjacency Matrix for the Graph
        public UndirectedGraphKopie(int[,] InitMatrix)
        {
            if (InitMatrix == null)
            {
                throw new ArgumentNullException();
            }
            if (InitMatrix.GetLength(0) != InitMatrix.GetLength(1))
            {
                throw new ArgumentException("Matrix must be square");
            }

            this.AdjMatrix = new int[InitMatrix.GetLength(0), InitMatrix.GetLength(1)];
            for (int i = 0; i < InitMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < InitMatrix.GetLength(1); j++)
                {
                    this.AdjMatrix[i, j] = InitMatrix[i, j];
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

        public int[] GetShortestHamiltonianCircle()
        {
            //Calculates the shortest hamiltonian circle for an complete undirected Graph!!!

            //Calculates the shortest hamiltonian circle this Graph contains. (AKA solving the Traveling Salesman Problem)
            //The Algorithm performs a depth-first Search. For each step the Algorithm determines the Vertexes which have not yet been by the current Path.
            //Then it adds these Vertexes into a Queue (FIFO-Structure) here it prefers the Vertexes which are close to the current step to the ones further away.
            //All possible circles will be calculated and the shortest one found will be returned.
            //The shortest circle found so far will be saved also it length(aka costs) also the half of its length
            //Whenever a (partial-)solution has a length > than the length of the shortest found circle this path in the search tree will be discarded
            //If there are less than half of the vertexes visited so far it will also discard the path if its length > than the half of the shortest found circle.
            //this works because the algorithm will find every possible circle twice, once clockwise and once counterclockwise.

            //The Algorithm will allways calculate a Circle which starts and ends at the Vertex at index 0.
            //The Algorithm is designed to work on a undirected complete graph (a graph where each vertex has an edge to every other vertex).
            //This means in the adjacency matrix only the main diagonal is zero.
            //The Algorithm will probably not work on Graphs that are not complete and it will eventually fail (or run into an infinite loop) on Graphs that are not hamiltonian.
            //Note that this Algorithm does not check for a Graph to be hamiltonian but assumes the Graph is hamiltonian.
            //If this Algorithm is run on a Graph which is not complete, it should be checked if the graph is hamiltonian beforehand.


            //Stores the Number of Vertexes the Graph consists of. This is stored in a Variable because it is used quite often and does never change.
            int Length = this.AdjMatrix.GetLength(0);

            // The ShortestCircle found so far will be stored in this Array
            int[] ShortestCircle = new int[Length + 1];

            // This Array stores the Circle which is currently calculated, it does not at all time contain a circle
            // When the Algorithm reaches the state that Depth == Length it will contain a hamiltonian path.
            // if there is a edge between that paths last vertex and its starting vertex it becomes a hamiltonian circle by adding that edge to its path.
            int[] CurrentCircle = new int[Length + 1];

            // Upper Boundry for calculating new Circles and length of the shortest current found hamiltonian circle
            long ShortestCircleLength = int.MaxValue; 

            // Used to save the Length of the Current Circle, which is allways updated when vertexes are added to / removed from the current circle.
            long CurrentCircleLength = 0;

            // Saves the current Depth, the amount of vertexes allready added to the Current Circle.
            int Depth = 1;

            // Used to buffer the Vertex which will be visited Next used for the System.Collections.Generic.Queue<T>.TryDequeue(T) method
            int NextVertex = 0;

            // Buffers all vertexes visited so far for faster calculation
            bool[] Visited = new bool[Length];
            bool[] Sorted = new bool[Length];

            int MinValue;
            int MinVertex;

            // Array of Queues, theses queues buffer the vertexes which must still be visited on the corresponding level (Depth)
            Queue<int>[] VertexQueue = new Queue<int>[Length];

            //Initialize Starting point of the Circle
            CurrentCircle[0] = 0;

            //Initialize ShortestCircle with an invalid value, user to check if a circle has been found at all
            ShortestCircle[0] = -1;

            
            //Initialize 
            for (int i = 0; i < Length; i++)
            {
                Visited[i] = false; //Initialize set every vertex to unvisited.
                Sorted[i] = false;
                VertexQueue[i] = new Queue<int>(Length); //Create a Queue for every Level of the search tree (Depth)
            }
            Visited[0] = true; //Set starting vertex to visited
            Sorted[0] = true;

            for (int i = 0; i < Length; i++)
            {
                MinValue = int.MaxValue;
                MinVertex = 0;
                for (int j = 0; j < Length; j++)
                {
                    if (!Sorted[j])
                    {
                        if (MinValue > this.AdjMatrix[CurrentCircle[0], j] && this.AdjMatrix[CurrentCircle[0],j] > 0)
                        {
                            MinValue = this.AdjMatrix[CurrentCircle[0], j];
                            MinVertex = j;
                        }
                    }
                }
                VertexQueue[1].Enqueue(MinVertex);
                Sorted[MinVertex] = true;
            }

            for (int i = 1; i < Length; i++)
            {
                VertexQueue[1].Enqueue(i); //add all vertexes except the starting vertex to the Queue at the first level (Depth = 1).
            }

            while (true) //infinite loop, will end when all possible solutions have been calculated all queues in VertexQueue[] are empty and Depth == 0
            {

                //DEBUG
                bool[] test = new bool[Length];
                for (int i = 0; i < Length; i++)
                {
                    test[i] = false;
                }
                //System.Console.WriteLine(Depth);
                for (int i = 0; i < (Depth); i++)
                {
                    if (test[CurrentCircle[i]])
                    {
                        System.Console.WriteLine("Error");
                        //System.Console.ReadLine();
                        for (int j = 0; j < Length; j++)
                        {
                            System.Console.Write(CurrentCircle[i] + "-->");
                        }
                        System.Console.WriteLine();
                        System.Console.WriteLine(Depth);
                        //System.Console.ReadLine();
                    }
                    else
                    {
                        test[CurrentCircle[i]] = true;
                    }
                }
                //DEBUG

                if (Depth == 0) //if Depth == 0 means that all queues in VertexQueue[] are empty and therefore every possible circle was calculated
                {
                    if (ShortestCircle[0] == -1)
                    {
                        return null; //return null if no circle was found
                    }
                    return ShortestCircle; //return the found shortest circle
                }

                //at this point CurrentCircle is a hamiltonian path, if the is a edge from the last vertex in this path, to the starting vertex with index 0.
                //vertex 0 will be added to the path, the length/costs of the edge between these two will be added to CurrentCircleLength.
                //Therefore CurrentCircle becomes a hamiltonian circle and a possible solution for the problem.
                //If it is shorter than the currently shortest found circle, or there has not been a circle found yet (ShortestCircleLength == int.MaxValue)
                //The CurrentCircle will be stored in ShortestCircle
                if (Depth == Length)  
                {
                    for (int i = 0; i <= Depth; i++)
                    {
                        System.Console.Write(CurrentCircle[i] + " ");
                    }
                    System.Console.Write((CurrentCircleLength + this.AdjMatrix[CurrentCircle[Depth - 1], CurrentCircle[0]]));
                    System.Console.WriteLine();
                    if (AdjMatrix[CurrentCircle[Depth - 1], 0] > 0) //check if there is a edge from the last vertex to the starting vertex
                    {
                        CurrentCircle[Depth] = CurrentCircle[0];
                        if ((CurrentCircleLength + AdjMatrix[CurrentCircle[Depth - 1], CurrentCircle[0]]) < ShortestCircleLength) //TODO deside what to do if there are two circles with the same length;
                        {
                            


                            ShortestCircleLength = CurrentCircleLength + AdjMatrix[CurrentCircle[Depth - 1], CurrentCircle[0]];
                            CurrentCircle.CopyTo(ShortestCircle, 0);
                            //TODO remove debug output
                            System.Console.WriteLine("new shortest Path has been found");
                            //System.Console.ReadLine();
                            for (int i = 0; i < (Length + 1); i++)
                            {
                                System.Console.Write(this.GetAlias(CurrentCircle[i]) + " --> ");
                            }
                            System.Console.WriteLine(" with length: " + ShortestCircleLength);
                        }
                    }

                    /**
                    CurrentCircle[Depth] = 0;
                    if (CurrentCircleLength < ShortestCircleLength)
                    {
                        CurrentCircle.CopyTo(ShortestCircle, 0);
                        ShortestCircleLength = CurrentCircleLength;
                    }
                    */

                    Visited[CurrentCircle[Depth - 1]] = false;
                    CurrentCircleLength = CurrentCircleLength - this.AdjMatrix[CurrentCircle[Depth - 1],CurrentCircle[Depth - 2]];
                    CurrentCircle[Depth - 1] = 0;
                    Depth--; //Depth-- so the loop will continue to calculate the next possible circle
                    continue;
                }

                // if there is a vertex left in VertexQueue[Depth] this vertex will be added to CurrentCircle.
                if (VertexQueue[Depth].TryDequeue(out NextVertex))
                {
                    //DEBUG

                    //if (Depth < Length)
                    //{
                    //    for (int i = 0; i < (Length); i++)
                    //    {
                    //        System.Console.Write(CurrentCircle[i] + " ");
                    //    }
                    //    System.Console.WriteLine();
                    //}
                    


                    //DEBUGEND
                    if (((Depth) < (Length / 2)) && ((CurrentCircleLength + this.AdjMatrix[CurrentCircle[Depth - 1], NextVertex]) > ((ShortestCircleLength / 2) + 1)))
                    {
                        //if the CurrentCircle + nextVertex has not yet visited half of the vertexes but is allready longer than the half of the length of the shortest circle found.
                        //this path will be discarded and the loop will continue with the next possibility.
                        continue;
                    }
                    if ((CurrentCircleLength + this.AdjMatrix[CurrentCircle[Depth - 1], NextVertex]) > ShortestCircleLength)
                    {
                        //path will also be discarded it is not yet a hamiltonian path but allready longer than the shortest found circle
                        continue;
                    }

                    CurrentCircle[Depth] = NextVertex; //add NextVertex to the CurrentCircle
                    CurrentCircleLength = CurrentCircleLength + this.AdjMatrix[CurrentCircle[Depth], CurrentCircle[Depth - 1]]; //update the Length of the CurrentCircle
                    Visited[NextVertex] = true; //update the array of allready visited Vertexes

                    Depth++; //go deeper down the rabithole
                    if (Depth == Length)
                    {
                        continue;
                    }
                    VertexQueue[Depth].Clear(); //this Queue should be empty at this point, this line only exists for the case of something unforeseen to happen.


                    // Add all not yet visited vertexes to the Queue for level Depth
                    //bool[] Sorted = new bool[Length]; // saves if the vertex has to be added to the Queue 
                    //Sorted[i] == true means Vertex i is eather allready visited by CurrentCircle or it is a element in VertexQueue[Depth]

                    for (int i = 0; i < Length; i++) 
                    { 
                        Sorted[i] = Visited[i]; // set Sorted[i] = true for every Vertex i which was allready visited by CurrentCircle 
                    } 

                    // There are (Depth - Length) Vertexes in the Graph which has not yet been visited by CurrentCircle
                    // These will be added to the Queue starting with the Vertex which is closest to the last Vertex in CurrentCircle
                    for (int i = 0; i < (Length - Depth); i++)  
                    {  
                        // add nearest vertex who is not yet visited and not in current Queue to the Queue
                        MinValue = int.MaxValue;  
                        MinVertex = 0;
                        for (int j = 1; j < Length; j++)  
                        {
                            if (!Sorted[j])  
                            {  
                                if ((this.AdjMatrix[CurrentCircle[Depth - 1], j] < MinValue) && (this.AdjMatrix[CurrentCircle[Depth - 1], j] > 0))  
                                {  
                                    MinValue = this.AdjMatrix[CurrentCircle[Depth - 1], j];  
                                    MinVertex = j;  
                                }  
                            }  
                        }  
                        VertexQueue[Depth].Enqueue(MinVertex);   
                        Sorted[MinVertex] = true;   
                    }  

                    //
                    // old Method for Adding Vertexes simply by their index instead of prioritising by distance to current Vertex
                    //
                    //for (int i = 0; i < Length; i++)
                    //{
                    //    if (!Visited[i]) //Add all vertexes that are not yet visited by the CurrentCircle to the VertexQueue on the current Depth
                    //    {
                    //        VertexQueue[Depth].Enqueue(i);
                    //    }
                    //}

                }
                else
                {
                    //if there is no Vertex in VertexQueue[Depth] left the next possible Vertex for the previous Level will be calculated
                    Depth--;
                    if (Depth > 0)
                    {
                        Visited[CurrentCircle[Depth]] = false;
                        CurrentCircleLength = CurrentCircleLength - this.AdjMatrix[CurrentCircle[Depth], CurrentCircle[Depth - 1]];
                        CurrentCircle[Depth] = 0;
                    }
                }
            }




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
        private int GetPathLength(int[] Path)
        {
            int PathLength = 0;
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
                PathLength = PathLength + this.AdjMatrix[Path[i - 1], Path[i]]; // Add the length of all edges in the Path to PathLength

            }
            return PathLength;
        }
    }
}