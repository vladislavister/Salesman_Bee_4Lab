using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vladislavister
{
    class Hive
    {
        public class Bee
        {
            public int status; // 0 = inactive, 1 = active, 2 = scout
            public int[] memoryMatrix; // problem-specific. a path of cities.
            public double measureOfQuality; // smaller values are better. total distance of path.
            public int numberOfVisits;

            public Bee(int status, int[] memoryMatrix, double measureOfQuality, int numberOfVisits)
            {
                this.status = status;
                this.memoryMatrix = new int[memoryMatrix.Length];
                Array.Copy(memoryMatrix, this.memoryMatrix, memoryMatrix.Length);
                this.measureOfQuality = measureOfQuality;
                this.numberOfVisits = numberOfVisits;
            }

            public override string ToString()
            {
                string s = "";
                s += "Status = " + this.status + "\n";
                s += " Memory = " + "\n";
                for (int i = 0; i < this.memoryMatrix.Length - 1; ++i)
                    s += this.memoryMatrix[i] + "->";
                s += this.memoryMatrix[this.memoryMatrix.Length - 1] + "\n";
                s += " Quality = " + this.measureOfQuality.ToString("F4");
                s += " Number visits = " + this.numberOfVisits;
                return s;
            }
        } // Bee

        static Random random = null; // multipurpose

        public CitiesData citiesData; // this is the problem-specific data we want to optimize

        public int totalNumberBees; // mostly for readability in the object constructor call
        public int numberInactive;
        public int numberActive;
        public int numberScout;

        public int maxNumberCycles; // one cycle represents an action by all bees in the hive
                                    //public int maxCyclesWithNoImprovement; // deprecated

        public int maxNumberVisits; // max number of times bee will visit a given food source without finding a better neighbor
        public double probPersuasion = 0.90; // probability inactive bee is persuaded by better waggle solution
        public double probMistake = 0.01; // probability an active bee will reject a better neighbor food source OR accept worse neighbor food source

        public Bee[] bees;
        public int[] bestMemoryMatrix; // problem-specific
        public double bestMeasureOfQuality;
        public int[] indexesOfInactiveBees; // contains indexes into the bees array

        public override string ToString()
        {
            string s = "";
            s += "Best path found: ";
            for (int i = 0; i < this.bestMemoryMatrix.Length - 1; ++i)
                s += this.bestMemoryMatrix[i] + "->";
            s += this.bestMemoryMatrix[this.bestMemoryMatrix.Length - 1] + "\n";

            s += "Path quality:    ";
            if (bestMeasureOfQuality < 10000.0)
                s += bestMeasureOfQuality.ToString("F4") + "\n";
            else
                s += bestMeasureOfQuality.ToString("#.####e+00");
            s += "\n";
            return s;
        }

        public Hive(int totalNumberBees, int numberInactive, int numberActive, int numberScout, int maxNumberVisits,
          int maxNumberCycles, CitiesData citiesData)
        {
            random = new Random(0);

            this.totalNumberBees = totalNumberBees;
            this.numberInactive = numberInactive;
            this.numberActive = numberActive;
            this.numberScout = numberScout;
            this.maxNumberVisits = maxNumberVisits;
            this.maxNumberCycles = maxNumberCycles;
            //this.maxCyclesWithNoImprovement = maxCyclesWithNoImprovement;

            //this.citiesData = new CitiesData(citiesData.cities.Length); // hive's copy of problem-specific data
            this.citiesData = citiesData; // reference to CityData

            // this.probPersuasion & this.probMistake are hard-coded in class definition

            this.bees = new Bee[totalNumberBees];
            this.bestMemoryMatrix = citiesData.GenerateRandomMemoryMatrix(); // alternative initializations are possible
            this.bestMeasureOfQuality = citiesData.PathLength(this.bestMemoryMatrix);

            this.indexesOfInactiveBees = new int[numberInactive]; // indexes of bees which are currently inactive

            for (int i = 0; i < totalNumberBees; ++i) // initialize each bee, and best solution
            {
                int currStatus; // depends on i. need status before we can initialize Bee
                if (i < numberInactive)
                {
                    currStatus = 0; // inactive
                    indexesOfInactiveBees[i] = i; // curr bee is inactive
                }
                else if (i < numberInactive + numberScout)
                {
                    currStatus = 2; // scout
                }
                else
                {
                    currStatus = 1; // active
                }

                int[] randomMemoryMatrix = citiesData.GenerateRandomMemoryMatrix();
                double mq = citiesData.PathLength(randomMemoryMatrix);
                int numberOfVisits = 0;

                bees[i] = new Bee(currStatus, randomMemoryMatrix, mq, numberOfVisits); // instantiate current bee

                // does this bee have best solution?
                if (bees[i].measureOfQuality < bestMeasureOfQuality) // curr bee is better (< because smaller is better)
                {
                    Array.Copy(bees[i].memoryMatrix, this.bestMemoryMatrix, bees[i].memoryMatrix.Length);
                    this.bestMeasureOfQuality = bees[i].measureOfQuality;
                }

            } // each bee

        } // TravelingSalesmanHive ctor






        public void Solve(bool doProgressBar) // find best Traveling Salesman Problem solution
        {
            bool pb = doProgressBar; // just want a shorter variable
            int numberOfSymbolsToPrint = 10; // 10 units so each symbol is 10.0% progress
            int increment = this.maxNumberCycles / numberOfSymbolsToPrint;
            if (pb) Console.WriteLine("\nEntering SBC Traveling Salesman Problem algorithm main processing loop\n");
            if (pb) Console.WriteLine("Progress: |==========|"); // 10 units so each symbol is 10% progress
            if (pb) Console.Write("           ");
            int cycle = 0;
            using (StreamWriter sw = new StreamWriter("232.txt", false, System.Text.Encoding.Default))
            {
                while (cycle < this.maxNumberCycles)
                {
                    for (int i = 0; i < this.totalNumberBees; ++i) // each bee
                    {

                        sw.WriteLine(bees[i].ToString());


                        if (this.bees[i].status == 1) // active bee
                            ProcessActiveBee(i);
                        else if (this.bees[i].status == 2) // scout bee
                            ProcessScoutBee(i);
                        else if (this.bees[i].status == 0) // inactive bee
                            ProcessInactiveBee(i);

                    } // for each bee
                    ++cycle;
                    sw.WriteLine();
                    // print a progress bar
                    if (pb && cycle % increment == 0)
                        Console.Write("^");
                } // main while processing loop

                if (pb) Console.WriteLine(""); // end the progress bar
            }
        }// Solve()

        private void ProcessInactiveBee(int i)
        {
            return; // not used in this implementation
        }

        private void ProcessActiveBee(int i)
        {
            int[] neighbor = citiesData.GenerateNeighborMemoryMatrix(bees[i].memoryMatrix); // find a neighbor solution
            double neighborQuality = citiesData.PathLength(neighbor); // get its quality
            double prob = random.NextDouble(); // used to determine if bee makes a mistake; compare against probMistake which has some small value (~0.01)
            bool memoryWasUpdated = false; // used to determine if bee should perform a waggle dance when done
            bool numberOfVisitsOverLimit = false; // used to determine if bee will convert to inactive status

            if (neighborQuality < bees[i].measureOfQuality) // active bee found better neighbor (< because smaller values are better)
            {
                if (prob < probMistake) // bee makes mistake: rejects a better neighbor food source
                {
                    ++bees[i].numberOfVisits; // no change to memory but update number of visits
                    if (bees[i].numberOfVisits > maxNumberVisits) numberOfVisitsOverLimit = true;
                }
                else // bee does not make a mistake: accepts a better neighbor
                {
                    Array.Copy(neighbor, bees[i].memoryMatrix, neighbor.Length); // copy neighbor location into bee's memory
                    bees[i].measureOfQuality = neighborQuality; // update the quality
                    bees[i].numberOfVisits = 0; // reset counter
                    memoryWasUpdated = true; // so that this bee will do a waggle dance 
                }
            }
            else // active bee did not find a better neighbor
            {
                //Console.WriteLine("c");
                if (prob < probMistake) // bee makes mistake: accepts a worse neighbor food source
                {
                    Array.Copy(neighbor, bees[i].memoryMatrix, neighbor.Length); // copy neighbor location into bee's memory
                    bees[i].measureOfQuality = neighborQuality; // update the quality
                    bees[i].numberOfVisits = 0; // reset
                    memoryWasUpdated = true; // so that this bee will do a waggle dance 
                }
                else // no mistake: bee rejects worse food source
                {
                    ++bees[i].numberOfVisits;
                    if (bees[i].numberOfVisits > maxNumberVisits) numberOfVisitsOverLimit = true;
                }
            }

            if (numberOfVisitsOverLimit == true)
            {
                bees[i].status = 0; // current active bee transitions to inactive
                bees[i].numberOfVisits = 0; // reset visits (and no change to this bees memory)
                int x = random.Next(numberInactive); // pick a random inactive bee. x is an index into a list, not a bee ID
                bees[indexesOfInactiveBees[x]].status = 1; // make it active
                indexesOfInactiveBees[x] = i; // record now-inactive bee 'i' in the inactive list
            }
            else if (memoryWasUpdated == true) // current bee returns and performs waggle dance
            {
                // first, determine if the new memory is a global best. note that if bee has accepted a worse food source this can't be true
                if (bees[i].measureOfQuality < this.bestMeasureOfQuality) // the modified bee's memory is a new global best (< because smaller is better)
                {
                    Array.Copy(bees[i].memoryMatrix, this.bestMemoryMatrix, bees[i].memoryMatrix.Length); // update global best memory
                    this.bestMeasureOfQuality = bees[i].measureOfQuality; // update global best quality
                }
                DoWaggleDance(i);
            }
            else // number visits is not over limit and memory was not updated so do nothing (return to hive but do not waggle)
            {
                return;
            }
        } // ProcessActiveBee()

        private void ProcessScoutBee(int i)
        {
            int[] randomFoodSource = citiesData.GenerateRandomMemoryMatrix(); // scout bee finds a random food source. . . 
            double randomFoodSourceQuality = citiesData.PathLength(randomFoodSource); // and examines its quality
            if (randomFoodSourceQuality < bees[i].measureOfQuality) // scout bee has found a better solution than its current one (< because smaller measure is better)
            {
                Array.Copy(randomFoodSource, bees[i].memoryMatrix, randomFoodSource.Length); // unlike active bees, scout bees do not make mistakes
                bees[i].measureOfQuality = randomFoodSourceQuality;
                // no change to scout bee's numberOfVisits or status

                // did this scout bee find a better overall/global solution?
                if (bees[i].measureOfQuality < bestMeasureOfQuality) // yes, better overall solution (< because smaller is better)
                {
                    Array.Copy(bees[i].memoryMatrix, this.bestMemoryMatrix, bees[i].memoryMatrix.Length); // copy scout bee's memory to global best
                    this.bestMeasureOfQuality = bees[i].measureOfQuality;
                } // better overall solution

                DoWaggleDance(i); // scout returns to hive and does waggle dance

            } // if scout bee found better solution
        } // ProcessScoutBee()

        private void DoWaggleDance(int i)
        {
            for (int ii = 0; ii < numberInactive; ++ii) // each inactive/watcher bee
            {
                int b = indexesOfInactiveBees[ii]; // index of an inactive bee
                if (bees[b].status != 0) throw new Exception("Catastrophic logic error when scout bee waggles dances");
                if (bees[b].numberOfVisits != 0) throw new Exception("Found an inactive bee with numberOfVisits != 0 in Scout bee waggle dance routine");
                if (bees[i].measureOfQuality < bees[b].measureOfQuality) // scout bee has a better solution than current inactive/watcher bee (< because smaller is better)
                {
                    double p = random.NextDouble(); // will current inactive bee be persuaded by scout's waggle dance?
                    if (this.probPersuasion > p) // this inactive bee is persuaded by the scout (usually because probPersuasion is large, ~0.90)
                    {
                        Array.Copy(bees[i].memoryMatrix, bees[b].memoryMatrix, bees[i].memoryMatrix.Length);
                        bees[b].measureOfQuality = bees[i].measureOfQuality;
                    } // inactive bee has been persuaded
                } // scout bee has better solution than watcher/inactive bee
            } // each inactive bee
        } // DoWaggleDance()

    } // class ShortestPathHive
}
