using System;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace SimulationLib
{
    public static partial class GenerateProblem
    {

        /// <summary>
        /// Generates a random problem
        /// </summary>
        /// <param name="nJobs"></param>
        /// <param name="nMachines"></param>
        /// <param name="withPriorityQueues"></param>
        /// <param name="jobProcessing"></param>
        /// <param name="jobArrival"></param>
        /// <param name="priorities"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static Problem RandomInstance(int nJobs, 
            int nMachines, 
            bool withPriorityQueues, 
            Tuple<int, int>[] jobProcessing, 
            Tuple<int, int> jobArrival, 
            Tuple<double, double, double> priorities = null,
            int seed = 0)
        {
            Jobs prblmJobs;
            Machines prblmMachines;
            Jobs[] prblmQueues;

            // These can be computed while generating the problem.
            // Avoids computations in the Problem object.
            var prblmTotalProcessingTime = default(long);
            var prblmJobsInPriority = new int[3];
            var prblmProcessingTimeInPriority = new int[3];
            Random rnd;
            try
            {
                rnd = new Random(seed);
                if (priorities is null) // default
                {
                    priorities = Tuple.Create(0.34d, 0.33d, 0.33d);
                }
                else // If priorities IsNot Nothing Then 'test if sum equals one
                {
                    double sum = priorities.Item1 + priorities.Item2 + priorities.Item3;
                    if (Math.Abs(sum - 1d) > double.Epsilon)
                    {
                        throw new ArgumentException("The sum of the priorities does not equal one.");
                    }
                }

                // Generate jobs
                prblmJobs = new Jobs(maxCapacity: nJobs);
                Job job;
                int[] processingTimes;
                int eArrival = 0;
                double rTemp;
                Priority p;
                for (int i = 0, loopTo = nJobs - 1; i <= loopTo; i++)
                {
                    processingTimes = new int[jobProcessing.Length];
                    rTemp = rnd.NextDouble(); // randomize the priority of the job
                                              // processingTime = 0

                    // randomize processing times
                    var loopTo1 = jobProcessing.Length - 1;
                    for (int j = 0; j <= loopTo1; j++)
                    {
                        processingTimes[j] = jobProcessing[j].Item1 + rnd.Next(0, jobProcessing[j].Item2 - jobProcessing[j].Item1 + 1);
                        prblmTotalProcessingTime = (long)(prblmTotalProcessingTime + processingTimes[j]);
                    }
                    // processingTime = JobProcessing(0) + (JobProcessing(1) - JobProcessing(0)) * r

                    if (rTemp < priorities.Item1) // job falls into priority 0
                    {
                        p = Priority.Low;
                    }
                    else if (rTemp < priorities.Item1 + priorities.Item2) // job falls into priority 1
                    {
                        p = Priority.Medium;
                    }
                    else // job falls into priority 2
                    {
                        p = Priority.High;
                    }

                    job = new Job(i, string.Format("J.{0}", i), eArrival, p, processingTimes);
                    prblmJobsInPriority[(int)p] += 1;
                    prblmProcessingTimeInPriority[(int)p] += job.TotalProcessingTime;

                    // Generate the next arrival time and enqueue the current job
                    // job = New Job(i, String.Format("J.{0}", i), eArrival, processingTime, rnd.Next(0, 3))
                    // eArrival += -15 * Math.Log(r)
                    eArrival += rnd.Next(jobArrival.Item1, jobArrival.Item2);
                    prblmJobs.Enqueue(job);
                }

                // Generate priority queues
                if (withPriorityQueues)
                {
                    prblmQueues = new Jobs[3];
                }
                // mHasPriorityQueues = True
                else
                {
                    prblmQueues = new Jobs[1];
                }

                for (int i = 0, loopTo2 = prblmQueues.Count() - 1; i <= loopTo2; i++)
                    prblmQueues[Conversions.ToInteger(i)] = new Jobs();

                // Generate machines
                prblmMachines = new Machines(nMachines);
                return new Problem(prblmJobs, prblmMachines, prblmQueues, jobProcessing.Count(), prblmTotalProcessingTime, prblmJobsInPriority, prblmProcessingTimeInPriority);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}", ex.Message));
            }
        }
    }
}
