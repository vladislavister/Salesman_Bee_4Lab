using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;




namespace Vladislavister
{

    public class Program
    {
        public static int[,] ReadFileCityToMatrix(string filename)
        {
            int[,] citymatrix;
            using (StreamReader sr = new StreamReader(filename))
            {
                string sfr = sr.ReadToEnd();
                string[] row = sfr.Split('\n');
                string[] col = row[0].Split(' ');
                citymatrix = new int[row.Length, col.Length];
                for (int i = 0; i < row.Length; ++i)
                {
                    col = row[i].Split(' ');
                    for (int j = 0; j < col.Length; j++)
                    {
                        citymatrix[i, j] = Convert.ToInt32(col[j]);
                    }
                }
            }
            return citymatrix;
        }
        public static double[,] ReadFileCoordinatesCity(string filename)
        {
            double[,] citymatrix;
            using (StreamReader sr = new StreamReader(filename))
            {
                string sfr = sr.ReadToEnd();
                string[] row = sfr.Split('\n');
                string[] col = row[0].Split(' ');
                citymatrix = new double[row.Length, col.Length];
                for (int i = 0; i < row.Length; ++i)
                {
                    col = row[i].Split(' ');
                    for (int j = 0; j < col.Length; j++)
                    {
                        citymatrix[i, j] = Convert.ToInt32(col[j]);

                    }

                }
            }
            return citymatrix;
        }
        public static double Distance(int start, int end, double[,] coordinates)
        {
            double result = Math.Sqrt(Math.Pow((coordinates[end, 0] - coordinates[start, 0]), 2) + Math.Pow((coordinates[end, 1] - coordinates[start, 1]), 2));
            return result;
        }
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
        }
        public static double[,] CoordinatesToDistanceMatrix(double[,] coordinates)
        {
            double[,] distanceMatrix = new double[coordinates.GetLength(0), coordinates.GetLength(0)];
            for (int i = 0; i < coordinates.GetLength(0); i++)
            {
                for (int j = 0; j < coordinates.GetLength(0); j++)
                {
                    if (i == j) distanceMatrix[i, j] = -1;
                    else
                        distanceMatrix[i, j] = Distance(i, j, coordinates);
                    Console.Write("{0:0.#} ", distanceMatrix[i, j]);
                }
                Console.WriteLine();
            }
            using (StreamWriter sw = new StreamWriter("242.txt", false, System.Text.Encoding.Default))
            {
                for (int i = 0; i < coordinates.GetLength(0); i++)
                {
                    for (int j = 0; j < coordinates.GetLength(0); j++)
                    {
                        sw.Write("{0:0.#} ", distanceMatrix[i, j]);
                    }
                    sw.WriteLine();
                }
            }
            return distanceMatrix;
        }
        static void RandomCoordinates()
        {
            Random rand = new Random();
            string path = "5.txt";
            StreamWriter sr = new StreamWriter(path);

            string data = null;
            for (int i = 0; i < 300; i++)
            {
                if (i == 299)
                {
                    data += $"{rand.Next(50)} {rand.Next(50)}";
                }
                else
                data += $"{rand.Next(50)} {rand.Next(50)}\n";
            }

            sr.Write(data);
            sr.Close();
        }

        public static void Main(string[] args)
        {
            //try
            //{
            ////    Console.WriteLine("");
            //RandomCoordinates();
            string filename;
            Console.WriteLine("Enter filename: ");
            filename = Console.ReadLine();

            double[,] citycoordinates = ReadFileCoordinatesCity(filename);
            double[,] citymatrix = CoordinatesToDistanceMatrix(citycoordinates);
            //int [,] citymatrix= ReadFileCityToMatrix("1.txt");
            CitiesData citiesData = new CitiesData(citymatrix, citycoordinates);
            Console.WriteLine("Number of cities = " + citiesData.citiesDistance.GetLength(0));
            

            int totalNumberBees = 6;
            int numberInactive = 1;
            int numberActive = 3;
            int numberScout = 2;

            int maxNumberVisits = 3;
            int maxNumberCycles = 10;


            Hive hive = new Hive(totalNumberBees, numberInactive, numberActive, numberScout, maxNumberVisits, maxNumberCycles, citiesData);


            bool doProgressBar = false;
            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch1.Start();


            hive.Solve(doProgressBar);

            Console.WriteLine("\n===================\nBee ALGORITHM:");
            Console.WriteLine(hive);
            stopWatch1.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts1 = stopWatch1.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts1.Hours, ts1.Minutes, ts1.Seconds,
                ts1.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime1);
            Console.ReadKey();

        } // Main()
    } // class Program

}

