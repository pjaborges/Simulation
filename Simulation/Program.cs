using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Parameters;
using SimulationLib;

namespace Simulation
{
    internal static partial class Program
    {
        private static DBParameter Params;
        private static Problem CurrentProblem;
        private static bool UseRandomJobs;
        private static Stopwatch Timer;

        public static void Main(string[] args)
        {
            try
            {
                Timer = new Stopwatch();

                // Set console size
                Console.WindowHeight = 40;
                Console.WindowWidth = 140;

                Console.WriteLine("........................");
                Console.WriteLine(" SSSS  II  MMM       MMM");
                Console.WriteLine("SSSS   II  MM MM   MM MM");
                Console.WriteLine("SS     II  MM  MMMMM  MM");
                Console.WriteLine("SS     II  MM   MMM   MM  -----");
                Console.WriteLine(" SSS   II  MM         MM  -----");
                Console.WriteLine("   SS  II  MM         MM  -----");
                Console.WriteLine("   SS  II  MM         MM");
                Console.WriteLine(" SSSS  II  MM         MM");
                Console.WriteLine("SSSS   II  MM         MM");
                Console.WriteLine("........................");

                // Generates the parameters
                GenerateParameters();
                if (args.Count() == 0 || args[0] == "-h")
                {
                    Console.Write(Params.ToString());
                    return;
                }

                SetParameters(Params.Tokenize(args));
                Params.Validate();
                PreparePipeline();
                WriteParameters();
                if (UseRandomJobs)
                {
                    // Generate random Problem
                    Timer.Start();
                    Console.Write("Generating problem ... ");
                    RandomProblem();
                    Timer.Stop();
                    Console.WriteLine(string.Format("{0} msec.", Timer.ElapsedMilliseconds));
                    Console.WriteLine();
                    InOut.WriteJobList(CurrentProblem, Params["-ojf"].Value.ToString());
                }
                else
                {
                    // TODO: read from file
                    InOut.ReadProblem(Program.Params["-i"].Value.ToString());
                }

                // Logs
                InOut.LogProblemSummary(CurrentProblem);

                // Solving the given problem
                Timer.Restart();
                Console.Write("Solving problem ... ");
                Solver.Run(CurrentProblem);
                Console.WriteLine(string.Format("Solution Found in {0} msec.", Timer.ElapsedMilliseconds));
                Console.WriteLine();

                // Solution
                InOut.WriteSolution(Params["-osof"].Value.ToString());

                // Logs
                InOut.LogSolutionSummary(CurrentProblem);
                Console.WriteLine("Press any key to exit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit.");
            }
            finally
            {
                Console.ReadKey();
            }
        }

        #region"Generate/Read/Set Parameters"
        private static void GenerateParameters()
        {
            Params = new DBParameter();
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    {
                        Params.Add(new Parameter("-dirsep", false, "The directory path separator.", new StringParam(true, @"\", @"\")));
                        break;
                    }

                case PlatformID.Unix:
                    {
                        Params.Add(new Parameter("-dirsep", false, "The directory path separator.", new StringParam(true, "/", "/")));
                        break;
                    }

                default:
                    {
                        Params.Add(new Parameter("-dirsep", false, "The directory path separator.", new StringParam(true, "/", "/")));
                        break;
                    }
            }

            // Pipeline parameters
            Params.Add(new Parameter("-t", true, "The maximum number of threads allowed.", new Int32Param(false, Environment.ProcessorCount, Environment.ProcessorCount, rangeVls: Tuple.Create(1, Environment.ProcessorCount))));

            // Import parameters
            Params.Add(new Parameter("-i", true, "The input file path.", new FileParam(false, false, string.Empty, string.Empty, new[] { ".dat", ".txt" })));
            // Problem parameters
            Params.Add(new Parameter("-pq", true, "Flag to indicate priority queues are used.", new BoolParam(false, false, false)));

            // Export parameters
            Params.Add(new Parameter("-odir", true, "The full path for the output directory.", new DirectoryParam(true, true, "", "")));
            Params.Add(new Parameter("-osof", true, "The solution file name.", new FileParam(false, false, "Solution", "Solution", new[] { ".sol", ".txt" })));
            Params.Add(new Parameter("-osuf", true, "The summary file name.", new LogParam("Summary", "Summary", new[] { ".log", ".out", ".txt" })));
            Params.Add(new Parameter("-ojf", true, "The generated jobs file name.", new FileParam(false, false, "RndJobs", "RndJons", new[] { ".dat", ".txt" })));
        }

        private static void SetParameters(Dictionary<string, string> tokens)
        {
            foreach (var prmToken in tokens.Keys)
            {
                if (tokens[prmToken] is object)
                {
                    Params[prmToken].Value = tokens[prmToken];
                }
                else if (Params[prmToken].NameType == "BoolParam")
                {
                    Params[prmToken].Value = true;
                }
            }

            // Exceptions:
            // 1. parameters that are constructed from other parameters e.g. output files
            // 2. Parameters that can change the value due to the value of another parameter

            if (tokens.ContainsKey("-t"))
            {
                int.TryParse(tokens["-t"], out int t);
                Params["-t"].Value = Math.Min(t, Environment.ProcessorCount);
            }

            // Construct the output file paths.
            string outDir = Params["-odir"].Value.ToString();
            Params["-osof"].Value = string.Format("{0}{1}{2}", outDir, Params["-dirsep"].Value, Params["-osof"].Value);
            Params["-osuf"].Value = string.Format("{0}{1}{2}", outDir, Params["-dirsep"].Value, Params["-osuf"].Value);
            Params["-ojf"].Value = string.Format("{0}{1}{2}", outDir, Params["-dirsep"].Value, Params["-ojf"].Value);

            // Checking if output directory exists.
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir); // create it.
            }
            else
            {
                // TODO: Check files existance, ask if replace. Now replaces

            } // check if file exists
        }

        private static void PreparePipeline()
        {
            try
            {
                if (Params["-i"].Value.ToString() == "")
                {
                    // no input file was set in the arguments, generate random jobs.
                    UseRandomJobs = true;
                }

                Trace.Listeners.Clear();
                if (File.Exists(Params["-osuf"].Value.ToString()))
                {
                    File.Delete(Params["-osuf"].Value.ToString());
                }

                var twtl = new TextWriterTraceListener(Params["-osuf"].Value.ToString())
                {
                    Name = "TextLogger",
                    TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime
                };
                Trace.Listeners.Add(twtl);
                var ctl = new ConsoleTraceListener(false) { TraceOutputOptions = TraceOptions.DateTime };
                Trace.Listeners.Add(ctl);
                Trace.AutoFlush = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void WriteParameters()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Trace.WriteLine("******************************");
            Trace.WriteLine(string.Format("* Date: {0}", DateTime.Now));
            Trace.WriteLine(string.Format("* Name: {0}", assembly.GetName().Name));
            Trace.WriteLine(string.Format("* Version: {0}", assembly.GetName().Version));
            Trace.WriteLine(string.Format("* Operative System: {0}", Environment.OSVersion.Platform));
            Trace.WriteLine(string.Format("* NetCore v{0}", Environment.Version.ToString()));
            Trace.WriteLine("* Contact: Paulo Borges");
            Trace.WriteLine("* email: pjaborges@gmail.com");
            Trace.WriteLine("******************************");
            Trace.WriteLine("");
            Trace.WriteLine("PARAMETERS");

            // Pipeline
            Trace.WriteLine(string.Format("Maximum threads: {0,12}", Params["-t"].Value));
            Trace.WriteLine("");

            // Problem parameters
            Trace.WriteLine(string.Format("Use priority queues: {0}", Params["-pq"].Value));
            Trace.WriteLine("");

            // Input files
            if (Params["-i"].Value.ToString() == "")
            {
                Trace.WriteLine(string.Format("Input file: {0,11}", "None"));
                Trace.WriteLine(string.Format("Use random jobs: {0,11}", UseRandomJobs));
            }
            else
            {
                Trace.WriteLine(string.Format("Input file: {0,11}", Path.GetFileName(Params["-i"].Value.ToString())));
            }

            Trace.WriteLine("");

            // Export
            Trace.WriteLine(string.Format("Output directory: {0}", Params["-odir"].Value));
            Trace.WriteLine(string.Format("Solution file: {0}", Path.GetFileName(Params["-osof"].Value.ToString())));
            Trace.WriteLine(string.Format("Summary file: {0}", Path.GetFileName(Params["-osuf"].Value.ToString())));
            Trace.WriteLine("");
        }
        #endregion

        /// <summary>
        /// This is hard coded for testing porposes...
        /// TODO: Define a protocol file for input and code the parser.
        /// </summary>
        private static void RandomProblem()
        {
            // 1. Simulating 150 jobs with 8 machines.

            // 2. Priority queues are used. (third parameter).

            // 3. The fourth parameter simulates the processing time of tasks.
            // - This example simulates 7 tasks for each job. Each task has a random processing time between {min, max} time units.
            // - The total processing time of each job is the sum of the respective tasks processing times.
            var tsks = new Tuple<int, int>[7];
            tsks[0] = Tuple.Create(10, 20);
            tsks[1] = Tuple.Create(20, 25);
            tsks[2] = Tuple.Create(10, 20);
            tsks[3] = Tuple.Create(120, 360);
            tsks[4] = Tuple.Create(120, 360);
            tsks[5] = Tuple.Create(10, 20);
            tsks[6] = Tuple.Create(20, 25);

            // 4. The fifth parameter is used to simulate the arrival time.
            // - A random value will be added to the total processing time of the precious job.
            // - This example used a value between 25 and 120 time units.

            // 5. The last parameters refer to the proportion oj jobs to be generated within each priority and a seed since there is some randomizations.


            CurrentProblem = GenerateProblem.RandomInstance(150, 4, (bool)Params["-pq"].Value, tsks, Tuple.Create(35, 120), priorities: Tuple.Create(0.3d, 0.4d, 0.3d), seed: 0);
        }
    }
}
