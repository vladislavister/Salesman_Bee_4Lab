using System;
using System.Collections.Generic;
using System.Text;

namespace Vladislavister
{
    class CitiesData
    {
        bool mod;
        int start, end;
        public int[] cities;
        public double[,] citiesDistance;
        public double[,] coordinates;
        //public int start;
        private Random rand = new Random(0);
        public CitiesData(double[,] citiesDistance, double[,] coordinates, bool mod = false, int start = 0, int end = 0, int[] cities = null)
        {
            this.cities = cities;
            this.start = start;
            this.end = end;
            this.mod = mod;
            this.coordinates = coordinates;
            this.citiesDistance = citiesDistance;
            // start = startCity;
        }
        public double PathLength(int[] path)
        {
            double answer = 0;
            for (int i = 0; i < path.Length - 1; ++i)
            {

                answer += citiesDistance[path[i], path[i + 1]];
            }
            answer += citiesDistance[path[path.Length - 1], path[0]];
            return answer;
        }
        public int[] GenerateRandomMemoryMatrix()
        {
            int[] result;
            // // problem-specific
            if (!mod)
            {
                result = new int[this.citiesDistance.GetLength(0)];
                result[0] = 0;
                for (int i = 1; i < this.citiesDistance.GetLength(0); ++i)
                {
                    List<int> arage = new List<int>();
                    for (int j = 0; j < this.citiesDistance.GetLength(0); ++j)
                    {
                        if (citiesDistance[result[i - 1], j] != -1)
                        {
                            bool flag = true;
                            for (int k = 0; k < i; ++k)
                            {
                                if (result[k] == j) flag = false;
                            }
                            if (flag) arage.Add(j);
                        }
                    }
                    if (arage.Count == 0) throw new Exception("no path");
                    int randomcity = rand.Next(0, arage.Count);
                    result[i] = arage[randomcity];
                }
            }
            else
            {
                bool f = false;
                result = new int[this.cities.Length];
                result[0] = start;
                if (start != end) f = true;
                if (f) result[this.cities.Length - 1] = end;
                int t = 0;
                if (!f)
                {
                    for (int i = 1; i < cities.Length; ++i)
                    {
                        List<int> arage = new List<int>();
                        foreach (int j in cities)
                        {
                            if (citiesDistance[result[i - 1], j] != -1)
                            {
                                bool flag = true;
                                for (int k = 0; k < i; ++k)
                                {
                                    if (result[k] == j) flag = false;
                                }
                                if (flag) arage.Add(j);
                            }
                        }
                        if (arage.Count == 0) throw new Exception("no path");
                        int randomcity = rand.Next(0, arage.Count);
                        result[i] = arage[randomcity];
                    }
                }
                else
                {
                    for (int i = 1; i < cities.Length - 1; ++i)
                    {
                        List<int> arage = new List<int>();
                        foreach (int j in cities)
                        {
                            if (citiesDistance[result[i - 1], j] != -1)
                            {
                                bool flag = true;
                                for (int k = 0; k < i; ++k)
                                {
                                    if (result[k] == j) flag = false;
                                }
                                if (flag) arage.Add(j);
                            }
                        }
                        if (arage.Count == 0) throw new Exception("no path");
                        int randomcity = rand.Next(0, arage.Count);
                        result[i] = arage[randomcity];
                    }
                }
            }
            return result;
        } // GenerateRandomMemoryMatrix()

        public int[] GenerateNeighborMemoryMatrix(int[] memoryMatrix)
        {
            if (memoryMatrix.Length == 1) return memoryMatrix;
            int[] result = new int[memoryMatrix.Length];
            Array.Copy(memoryMatrix, result, memoryMatrix.Length);

            int ranIndex = rand.Next(1, result.Length - 1); // [1, Length-2] inclusive

            int adjIndex;
            if (ranIndex == result.Length - 1)
                adjIndex = 1;
            else
                adjIndex = ranIndex + 1;

            int tmp = result[ranIndex];
            result[ranIndex] = result[adjIndex];
            result[adjIndex] = tmp;

            return result;
        } // GenerateNeighborMemoryMatrix()

    } // class CitiesData
}
