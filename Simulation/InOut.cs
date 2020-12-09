using System;
using System.Diagnostics;
using System.IO;
using SimulationLib;

namespace Simulation
{
    public static partial class InOut
    {
        /// <summary>
        /// Writes the job list.
        /// </summary>
        /// <param name="prblm">The problem.</param>
        /// <param name="path">The path.</param>
        public static void WriteJobList(Problem prblm, string path)
        {
            StreamWriter sWriter = null;
            try
            {
                sWriter = new StreamWriter(path);
                sWriter.WriteLine("--- -------- ---");
                sWriter.WriteLine("### JOB LIST ###");
                sWriter.WriteLine("--- -------- ---");
                sWriter.Write(string.Format("{0} | {1} | {2}", " Job ", "Priority", "Arrival"));
                for (int i = 0, loopTo = prblm.NumberTasksPerJob - 1; i <= loopTo; i++)
                    sWriter.Write(string.Format(" |  T.{0} ", i));
                sWriter.WriteLine(" | Processing");
                sWriter.Write(string.Format("{0} | {1} | {2}", "-----", "--------", "-------"));
                for (int i = 0, loopTo1 = prblm.NumberTasksPerJob - 1; i <= loopTo1; i++)
                    sWriter.Write(" | -----");
                sWriter.WriteLine(" | ----------");
                foreach (var j in prblm.Jobs)
                {
                    sWriter.Write(string.Format("{0,5} | {1,8} | {2,7}", j.ID, j.Priority, j.ExpectedArrivalTime));
                    for (int p = 0, loopTo2 = prblm.NumberTasksPerJob - 1; p <= loopTo2; p++)
                        sWriter.Write(string.Format(" | {0,5}", j.ProcessingTime(p)));
                    sWriter.WriteLine(string.Format(" | {0,10}", j.TotalProcessingTime));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sWriter is object)
                    sWriter.Dispose();
            }
        }

        /// <summary>
        /// Reads the problem.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Problem ReadProblem(string path)
        {
            // TODO implement when a protocol for input file is defined.
            Reader MyReader;
            try
            {
                MyReader = new Reader(path);
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Logs the problem summary.
        /// </summary>
        /// <param name="prblm">The problem.</param>
        public static void LogProblemSummary(Problem prblm)
        {
            Trace.WriteLine("--- ------- ---");
            Trace.WriteLine("### PROBLEM ###");
            Trace.WriteLine("--- ------- ---");
            Trace.WriteLine(string.Format("Last job processing: {0}", prblm.Jobs.Job(prblm.NumberOfJobs - 1).DepartureTime()));
            Trace.WriteLine(string.Format("Using priority queues: {0}", prblm.HasPriorityQueues));
            Trace.WriteLine("");
            int nJobs;
            int pJobs;
            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5}",
                "Priority", "  Jobs  ", "% of Total Jobs", "Total Processing Time", "% of Total Processing Time", "Mean Processing Time"));
            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5}",
                "--------", "--------", "---------------", "---------------------", "--------------------------", "--------------------"));
            foreach (var i in new[] { 0, 1, 2 })
            {
                Priority p = (Priority)i;
                nJobs = prblm.JobsInPriority(p);
                pJobs = prblm.ProcessingTimeInPriority(p);
                Trace.WriteLine(string.Format("{0,8} | {1,8} | {2,15:p2} | {3,21} | {4,26:p2} | {5,20:f3}",
                    ((Priority)p).ToString(), nJobs, nJobs / prblm.NumberOfJobs, pJobs, pJobs / prblm.TotalProcessingTime, (double)pJobs / (double)nJobs));
            }

            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5}",
                "--------", "--------", "---------------", "---------------------", "--------------------------", "--------------------"));
            Trace.WriteLine(string.Format("{0,8} | {1,8} | {2,15:p2} | {3,21} | {4,26:p2} | {5,20:f3}",
                "Total", prblm.NumberOfJobs, 1, prblm.TotalProcessingTime, 1, prblm.TotalProcessingTime / prblm.NumberOfJobs));
            Trace.WriteLine("");
        }

        /// <summary>
        /// Writes the solution.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void WriteSolution(string path)
        {
            // TODO: Implement it.
            // Jobs: tabular containing metrics regarding time and which machine process it.
            // Machines Sequence of jobs processed with respective times.
            StreamWriter sWriter = null;
            try
            {
                sWriter = new StreamWriter(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sWriter is object)
                    sWriter.Dispose();
            }
        }

        /// <summary>
        /// Logs the solution summary.
        /// </summary>
        /// <param name="prblm">The problem.</param>
        public static void LogSolutionSummary(Problem prblm)
        {
            Machine m;
            Jobs q;
            int lj = 0;
            int twt = 0;
            var waitingTime = new double[3];
            for (int i = 0, loopTo = prblm.NumberOfQueues - 1; i <= loopTo; i++)
            {
                Priority p = (Priority)i;
                q = prblm.Queue(p);
                lj += q.TotalLateJobs();
            }

            int temp;
            for (int i = 0, loopTo1 = prblm.NumberOfMachines - 1; i <= loopTo1; i++)
            {
                m = prblm.Machines[i];
                foreach (var j in m.Jobs)
                {
                    temp = j.WaitingTime();
                    if (temp > 0d)
                    {
                        twt += j.WaitingTime();
                        waitingTime[(int)j.Priority] += temp;
                    }
                }
            }

            int onTime = prblm.NumberOfJobs - lj;
            Trace.WriteLine("");
            Trace.WriteLine("--- ------ ---");
            Trace.WriteLine("### QUEUES ###");
            Trace.WriteLine("--- ------ ---");
            Trace.WriteLine("");
            Trace.WriteLine(string.Format("{0} | {1}", "Priority", "Max Size"));
            Trace.WriteLine(string.Format("{0} | {1}", "--------", "--------"));
            if (prblm.NumberOfQueues > 1)
            {
                for (int i = 0, loopTo2 = prblm.NumberOfQueues - 1; i <= loopTo2; i++)
                {
                    Priority p = (Priority)i;
                    Trace.WriteLine(string.Format("{0,8} | {1,8}", p.ToString(), prblm.Queue(p).MaxSize));
                }

                Trace.WriteLine(string.Format("{0} | {1}", "--------", "--------"));
                Trace.WriteLine(string.Format("{0,8} | {1,8}", "Total", prblm.MaxJobsQueued));
            }
            else
            {
                Trace.WriteLine(string.Format("{0,8} | {1,8}", "NA", prblm.Queue(0).MaxSize));
            }

            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("--- ---- ---");
            Trace.WriteLine("### JOBS ###");
            Trace.WriteLine("--- ---- ---");
            Trace.WriteLine(string.Format("On Time Jobs: {0} ({1:p2})", onTime, onTime / prblm.NumberOfJobs));
            Trace.WriteLine("");
            double wt;
            int ljp;
            int nJobs;
            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6}",
                "Priority", "Late Jobs", "% of Jobs", "% of Late Jobs", "Waiting Time", "% of Waiting Time", "Mean Wainting Time"));
            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6}",
                "--------", "---------", "---------", "--------------", "------------", "-----------------", "------------------"));
            if (prblm.NumberOfQueues > 1)
            {
                for (int i = 0, loopTo3 = prblm.NumberOfQueues - 1; i <= loopTo3; i++)
                {
                    Priority p = (Priority)i;
                    q = prblm.Queue(p);
                    ljp = q.LateJobs(p);
                    wt = waitingTime[i];
                    nJobs = prblm.JobsInPriority(p);
                    Trace.WriteLine(string.Format("{0,8} | {1,9} | {2,9:p2} | {3,14:p2} | {4,12} | {5,17:p2} | {6,18:f3}",
                        ((Priority)p).ToString(), ljp, (double)ljp / (double)nJobs, (double)ljp / (double)lj, wt, wt / twt, wt / (double)ljp));
                }
            }
            else
            {
                q = prblm.Queue(0);
                foreach (var i in new[] { 0, 1, 2 })
                {
                    Priority p = (Priority)i;
                    ljp = q.LateJobs(p);
                    wt = waitingTime[i];
                    nJobs = prblm.JobsInPriority(p);
                    Trace.WriteLine(string.Format("{0,8} | {1,9} | {2,9:p2} | {3,14:p2} | {4,12} | {5,17:p2} | {6,18:f3}",
                        ((Priority)p).ToString(), ljp, (double)ljp / (double)nJobs, (double)ljp / (double)lj, wt, wt / twt, wt / (double)ljp));
                }
            }

            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6}",
                "--------", "---------", "---------", "--------------", "------------", "-----------------", "------------------"));
            Trace.WriteLine(string.Format("{0,8} | {1,9} | {2,9:p2} | {3,14:p2} | {4,12} | {5,17:p2} | {6,18:f3}",
                "Total", lj, lj / prblm.NumberOfJobs, 1, twt, 1, twt / (double)lj));
            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("--- -------- ---");
            Trace.WriteLine("### MACHINES ###");
            Trace.WriteLine("--- -------- ---");
            Trace.WriteLine("");
            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10}",
                "Machine", "  Jobs  ", "% of Jobs", "Iddle Time", "Iddle Periods", "Mean Iddle Time", "  Start  ", "Makespan", "  Low  ", " Medium ", "  High  "));
            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10}",
                "-------", "--------", "---------", "----------", "-------------", "---------------", "---------", "--------", "-------", "--------", "--------"));
            int totalJobs = 0;
            for (int i = 0, loopTo4 = prblm.NumberOfMachines - 1; i <= loopTo4; i++)
            {
                m = prblm.Machines[i];
                m.IddleStats();
                if (m.NumberOfJobs > 0)
                {
                    totalJobs += m.NumberOfJobs;
                    Trace.WriteLine(string.Format("{0,7} | {1,8} | {2,9:p2} | {3,10} | {4,13} | {5,15:f3} | {6,9} | {7,8} | {8,7} | {9,8} | {10,8}",
                        m.ID, m.NumberOfJobs, m.NumberOfJobs / prblm.NumberOfJobs, m.IddleTime, m.IddlePeriods, m.MeanIddleTime(), m.FirstJob.StartTime, m.Clock, m.NumberJobs(Priority.Low), m.NumberJobs(Priority.Medium), m.NumberJobs(Priority.High)));
                }
                else
                {
                    Trace.WriteLine(string.Format("{0,7} | {1,8} | {2,9:p2} | {3,10} | {4,13} | {5,15:f3} | {6,9} | {7,8} | {8,7} | {9,8} | {10,8}",
                        m.ID, m.NumberOfJobs, m.NumberOfJobs / prblm.NumberOfJobs, m.IddleTime, m.IddlePeriods, m.MeanIddleTime(), double.NaN, m.Clock, m.NumberJobs(Priority.Low), m.NumberJobs(Priority.Medium), m.NumberJobs(Priority.High)));
                }
            }

            Trace.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10}", "-------", "--------", "---------", "----------", "-------------", "---------------", "---------", "--------", "-------", "--------", "--------"));
            int tIddleTime = prblm.Machines.TotalIddleTime();
            int tIddlePeriods = prblm.Machines.TotalIddlePeriods();
            int makespam = prblm.Machines.Makespam();
            Trace.WriteLine(string.Format("{0,7} | {1,8} | {2,9:p2} | {3,10} | {4,13} | {5,15:f3} | {6,9:f3} | {7,8} | {8,7} | {9,8} | {10,8}", "Total", totalJobs, 1, tIddleTime, tIddlePeriods, tIddleTime / (double)tIddlePeriods, "NA", makespam, -1, -1, -1));
        }

        public partial class Reader
        {
            private readonly StreamReader LinePtr;
            private readonly System.IO.Compression.GZipStream UnGZPtr;

            #region"Constructor"
            /// <summary>
            /// Initializes a new instance of the <see cref="Reader" /> class.
            /// </summary>
            /// <param name="path">The file path.</param>
            public Reader(string path)
            {
                // Check file extension in order to build the stream
                string extension;
                extension = Path.GetExtension(path);
                if (extension == ".gz")
                {
                    UnGZPtr = new System.IO.Compression.GZipStream(File.OpenRead(path), System.IO.Compression.CompressionMode.Decompress);
                    LinePtr = new StreamReader(UnGZPtr);
                }
                else // assumes file is not compressed
                {
                    LinePtr = new StreamReader(path);
                }
            }
            #endregion

            #region"Menbers"
            public bool EndOfStream
            {
                get
                {
                    return LinePtr.EndOfStream;
                }
            }

            public string ReadLine()
            {
                // will return Nothing if EOF
                return LinePtr.ReadLine();
            }

            public int Peek()
            {
                return LinePtr.Peek();
            }
            #endregion

            #region"Methods"
            public void Dispose()
            {
                LinePtr.Dispose();
                if (UnGZPtr is object)
                    UnGZPtr.Dispose();
            }

            public void Close()
            {
                LinePtr.Close();
                if (UnGZPtr is object)
                    UnGZPtr.Close();
            }
            #endregion
        }
    }
}
