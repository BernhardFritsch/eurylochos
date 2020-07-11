using System;

namespace eurylochos
{
    class Program
    {
        static void Main(string[] args)
        {
            //run program with parameter Program.exe --file C:\.........

            int Length = 0;
            const double EarthRadius = 6371000.785; //average radius of the earth in meter
            string FilePath = null;
            string[] Input = null;
            System.Collections.Generic.List<Vertex> Vertexes = new System.Collections.Generic.List<Vertex>();

            if (args.Length > 0)
            {
                if (args[0] == "--file")
                {
                    if (args.Length > 1)
                    {
                        FilePath = args[1];
                    }
                }
                else
                {
                    System.Console.WriteLine("Unknown argument " + args[0] + ", abort");
                    return;
                }
            }

            if (FilePath == null)
            {
                System.Console.WriteLine("You need to specify a file from where to read the Data");
                return;
            }

            try
            {
                Input = System.IO.File.ReadAllLines(FilePath);
            } 
            catch (UnauthorizedAccessException e)
            {
                System.Console.WriteLine("Failed to Access File at " + FilePath + " missing Authorization");
                return;
            }
            catch (ArgumentException e)
            {
                System.Console.WriteLine("Invalid FilePath, Path contains invalid characters or only white space");
                return;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Unknown error occured while opending the File at " + FilePath);
                return;
            }

            if (Input == null)
            {
                System.Console.WriteLine("some unknown error occured while Reading from the input file, aborting");
                return;
            }

            string[] Line;
            bool dataline = false;

            for (int i = 0; i < Input.Length; i++)
            {
                dataline = false;
                switch (Input[i].Substring(0,1))
                {
                    case "0":
                        Length++;
                        dataline = true;
                        break;
                    case "1":
                        Length++;
                        dataline = true;
                        break;
                    case "2":
                        Length++;
                        dataline = true;
                        break;
                    case "3":
                        Length++;
                        dataline = true;
                        break;
                    case "4":
                        Length++;
                        dataline = true;
                        break;
                    case "5":
                        Length++;
                        dataline = true;
                        break;
                    case "6":
                        Length++;
                        dataline = true;
                        break;
                    case "7":
                        Length++;
                        dataline = true;
                        break;
                    case "8":
                        Length++;
                        dataline = true;
                        break;
                    case "9":
                        Length++;
                        dataline = true;
                        break;
                    default:
                        //do nothing
                        break;
                }
                if (dataline)
                {
                    Line = Input[i].Split(",");
                    Vertexes.Add(new Vertex(int.Parse(Line[0]), Line[1], Line[2], Line[3], Line[4], Line[5], Line[6], Line[7]));
                }
            }
            
            System.Console.WriteLine(Length);

            int[,] AdjacencyMatrix = new int[Length, Length];
            string[] Aliases = new string[Length];
            Vertex[] VertexesArray = Vertexes.ToArray();

            double LatStart = 0.0;
            double LonStart = 0.0;
            double LatDest = 0.0;
            double LonDest = 0.0;
            double DeltaLat = 0.0;
            double DeltaLon = 0.0;
            double distance = 0.0;

            for (int i = 0; i < Length; i++)
            {
                Aliases[i] = VertexesArray[i].Standort;
                AdjacencyMatrix[i, i] = 0;
                LatStart = double.Parse(VertexesArray[i].Breitengrad, System.Globalization.CultureInfo.InvariantCulture) * Math.PI / 180.0;
                LonStart = double.Parse(VertexesArray[i].Laengengrad, System.Globalization.CultureInfo.InvariantCulture) * Math.PI / 180.0;
                for (int j = 0; j < Length; j++)
                {
                    if (i != j)
                    {
                        LatDest = double.Parse(VertexesArray[j].Breitengrad, System.Globalization.CultureInfo.InvariantCulture) * Math.PI / 180.0;
                        LonDest = double.Parse(VertexesArray[j].Laengengrad, System.Globalization.CultureInfo.InvariantCulture) * Math.PI / 180.0;
                        DeltaLat = LatDest - LatStart;
                        DeltaLon = LonDest - LonStart;
                        //caluclate Distances between two points (GPS coordinates) on the surface of the earth with the haversine formula

                        distance = 2.0 * EarthRadius * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((DeltaLat) / 2.0), 2.0) + (Math.Cos(LatStart) * Math.Cos(LatDest) * Math.Pow(Math.Sin(DeltaLon / 2.0), 2.0))));

                        if (double.IsNaN(distance))
                        {
                            throw new ArithmeticException("some calculation for the distances based on the GPS-Koordinates went wrong, and it was most likely tried to calculate the Square Route of a negative Value, the calculation does only support Real Numbers not complex numbers");
                        }

                        AdjacencyMatrix[i, j] = (int)distance;
                        AdjacencyMatrix[j, i] = AdjacencyMatrix[i, j];
                    }
                }
            }

            System.DateTime StartTime = System.DateTime.Now;
            //System.Console.WriteLine(StartTime);
            //System.Console.ReadLine();

            UndirectedGraph Graph = new UndirectedGraph(AdjacencyMatrix, Aliases);
            int[] ShortestPath = Graph.GetShortestHamiltonianCircle(true);
            if (ShortestPath != null)
            {
                System.Console.WriteLine("Shortest Hamilton Circle has been found and is:");
                for (int i = 0; i < ShortestPath.Length; i++)
                {
                    if (i < (ShortestPath.Length - 1))
                    {
                        System.Console.Write(Graph.GetAlias(ShortestPath[i]) + " --> ");
                    }
                    else
                    {
                        System.Console.Write(Graph.GetAlias(ShortestPath[i]));
                    }
                }
                System.Console.WriteLine();

                System.Console.WriteLine("With Length: ");
                char[] PathLengthPrint = Graph.GetPathLength(ShortestPath).ToString().ToCharArray();
                for (int i = (PathLengthPrint.Length - 1); i >= 0; i--)
                {
                    System.Console.Write(PathLengthPrint[i]);
                    if ((i % 3) == 0 && i > 0)
                    {
                        System.Console.Write(".");
                    }
                }
                System.Console.Write(" meters");
                System.Console.WriteLine();

                
                //System.Console.WriteLine("With Length" + Graph.GetPathLength(ShortestPath));
            }
            else
            {
                System.Console.WriteLine("The Graph is not hamiltonian therefore no hamiltonian circle could been found");
            }
            System.Console.WriteLine();
            System.DateTime EndTime = System.DateTime.Now;

            System.TimeSpan Duration = EndTime - StartTime;

            System.Console.WriteLine("The Algorithm started at " + StartTime + " and finished at " + EndTime + " it took " + Duration.Days + " Days " + Duration.Hours + " Hours " + Duration.Minutes + " Minutes and " + Duration.Seconds + " Seconds to finish.");
        }

        public struct Vertex
        {
            public Vertex(int InitID, string InitStandort, string InitStrasse, string InitHausnummer, string InitPLZ, string InitOrt, string InitBreitengrad, string InitLaengengrad)
            {
                this.ID = InitID;
                this.Standort = InitStandort;
                this.Strasse = InitStrasse;
                this.Hausnummer = InitHausnummer;
                this.PLZ = InitPLZ;
                this.Ort = InitOrt;
                this.Breitengrad = InitBreitengrad;
                this.Laengengrad = InitLaengengrad;
            }

            public int ID;
            public string Standort;
            public string Strasse;
            public string Hausnummer;
            public string PLZ;
            public string Ort;
            public string Breitengrad;
            public string Laengengrad;
            
        }
    }
}
