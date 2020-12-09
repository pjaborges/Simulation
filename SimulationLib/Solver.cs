using System;
using System.Linq;

namespace SimulationLib
{
    public static partial class Solver
    {

        /// <summary>
        /// Method to call the solver
        /// </summary>
        public static void Run(Problem prblm)
        {
            if (prblm.HasPriorityQueues)
            {
                SolvexQxM(prblm);
            }
            else
            {
                Solve1QxM(prblm);
            }
        }

        /// <summary>
        /// This method solves the problem without priority queues
        /// Dispatch rule is FIFO.
        /// </summary>
        /// <param name="prblm">The problem.</param>
        private static void Solve1QxM(Problem prblm)
        {
            Machine m;
            Jobs q;
            Job currentJob;
            Job queuedJob;
            int nextClockEvent;
            try
            {
                q = prblm.Queue(0);
                while (prblm.Jobs.Count > 0)
                {
                    currentJob = prblm.Jobs.Dequeue();
                    m = prblm.Machines.AvailableMachine(currentJob.StartTime);
                    if (m is object)
                    {
                        if (q.Count == 0)
                        {
                            prblm.Machines.AddJob(m, currentJob);
                        }
                        else
                        {
                            nextClockEvent = prblm.Machines.NextAvailableClock();
                            while (q.Count > 0 && nextClockEvent <= currentJob.StartTime)
                            {
                                queuedJob = q.Dequeue();
                                m = prblm.Machines.AvailableMachine(nextClockEvent);
                                prblm.Machines.AddJob(m, queuedJob);
                                nextClockEvent = prblm.Machines.NextAvailableClock();
                            }

                            if (nextClockEvent <= currentJob.StartTime)
                            {
                                m = prblm.Machines.AvailableMachine(nextClockEvent);
                                prblm.Machines.AddJob(m, currentJob);
                            }
                            else
                            {
                                q.Enqueue(currentJob);
                            }
                        }
                    }
                    else
                    {
                        q.Enqueue(currentJob);
                    }
                }

                nextClockEvent = prblm.Machines.NextAvailableClock();
                while (q.Count > 0)
                {
                    queuedJob = q.Dequeue();
                    m = prblm.Machines.AvailableMachine(nextClockEvent);
                    prblm.Machines.AddJob(m, queuedJob);
                    nextClockEvent = prblm.Machines.NextAvailableClock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// This method solves the problem for several priority queues.
        /// Dispatch rule is FIFO in all the queues.
        /// </summary>
        /// <param name="prblm">The problem.</param>
        private static void SolvexQxM(Problem prblm) // ,
        {
            Machine m;
            Jobs q;
            Job currentJob;
            Job queuedJob;
            int nextClockEvent;
            var jobsInQueue = default(int);
            try
            {
                while (prblm.Jobs.Count > 0)
                {
                    currentJob = prblm.Jobs.Dequeue();
                    m = prblm.Machines.AvailableMachine(currentJob.StartTime);
                    if (m is object)
                    {
                        // a machine is available

                        if (prblm.Queues.Sum(j => j.TotalLateJobs()) == 0)
                        {
                            prblm.Machines.AddJob(m, currentJob);
                        }
                        else
                        {
                            nextClockEvent = prblm.Machines.NextAvailableClock();
                            for (int i = prblm.NumberOfQueues - 1; i >= 0; i -= 1)
                            {
                                q = prblm.Queue((Priority)i);
                                while (q.Count > 0 && nextClockEvent <= currentJob.StartTime)
                                {
                                    queuedJob = q.Dequeue();
                                    m = prblm.Machines.AvailableMachine(nextClockEvent);
                                    prblm.Machines.AddJob(m, queuedJob);
                                    nextClockEvent = prblm.Machines.NextAvailableClock();
                                    jobsInQueue -= 1;
                                }
                            }

                            if (nextClockEvent <= currentJob.StartTime)
                            {
                                m = prblm.Machines.AvailableMachine(nextClockEvent);
                                prblm.Machines.AddJob(m, currentJob);
                            }
                            else
                            {
                                prblm.Queue(currentJob.Priority).Enqueue(currentJob);
                                jobsInQueue += 1;
                            }
                        }
                    }
                    else // enqueue the job since no machine to process it is available
                    {
                        prblm.Queue(currentJob.Priority).Enqueue(currentJob);
                        jobsInQueue += 1;
                    }

                    if (jobsInQueue > prblm.MaxJobsQueued)
                    {
                        // Update max jobs queued
                        prblm.MaxJobsQueued = jobsInQueue;
                    }
                }

                // Process jobs in the queues by priority
                nextClockEvent = prblm.Machines.NextAvailableClock();
                for (int i = prblm.NumberOfQueues - 1; i >= 0; i -= 1)
                {
                    q = prblm.Queue((Priority)i);
                    while (q.Count > 0)
                    {
                        queuedJob = q.Dequeue();
                        m = prblm.Machines.AvailableMachine(nextClockEvent);
                        prblm.Machines.AddJob(m, queuedJob);
                        nextClockEvent = prblm.Machines.NextAvailableClock();
                        jobsInQueue -= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
