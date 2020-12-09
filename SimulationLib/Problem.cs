using System;

namespace SimulationLib
{
    public partial class Problem
    {
        private readonly int[] mCounterJobsInPriority;
        private readonly int[] mProcessingTimeInPriority;

        #region"Constructor"
        /// <summary>
        /// Initializes a new instance of the <see cref="Problem" /> class.
        /// </summary>
        /// <param name="jobs">The jobs.</param>
        /// <param name="machines">The machines.</param>
        /// <param name="queues">The queues.</param>
        /// <param name="nTasksPerJob">The number of tasks per job.</param>
        /// <param name="totalProcessingTime">The total processing time.</param>
        /// <param name="counterJobsInPriority">The counter of jobs per priority.</param>
        /// <param name="processingTimeInPriority">The total processing time per priority.</param>
        public Problem(Jobs jobs,
            Machines machines, 
            Jobs[] queues, 
            int nTasksPerJob, 
            long totalProcessingTime,
            int[] counterJobsInPriority,
            int[] processingTimeInPriority)
        {
            try
            {
                Jobs = jobs;
                NumberOfJobs = jobs.Count;
                Machines = machines;
                Queues = queues;
                if (NumberOfQueues > 1)
                {
                    HasPriorityQueues = true;
                }
                NumberTasksPerJob = nTasksPerJob;
                TotalProcessingTime = totalProcessingTime;
                mCounterJobsInPriority = counterJobsInPriority;
                mProcessingTimeInPriority = processingTimeInPriority;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region"Members"
        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <returns></returns>
        public Jobs Jobs { get; private set; }

        /// <summary>
        /// Gets the machines.
        /// </summary>
        /// <returns></returns>
        public Machines Machines { get; private set; }

        /// <summary>
        /// Gets the queues.
        /// </summary>
        /// <returns></returns>
        public Jobs[] Queues { get; private set; }

        /// <summary>
        /// Gets the queue of paricular priority.
        /// </summary>
        /// <param name="p">The priority.</param>
        /// <returns></returns>
        public Jobs Queue(Priority p)
        {
            return Queues[(int)p];
        }

        /// <summary>
        /// Gets the number of tasks in a job
        /// </summary>
        /// <returns></returns>
        public int NumberTasksPerJob { get; private set; }

        /// <summary>
        /// Gets the number of jobs.
        /// </summary>
        /// <returns></returns>
        public int NumberOfJobs { get; private set; }

        /// <summary>
        /// Gets the number of machines.
        /// </summary>
        /// <returns></returns>
        public int NumberOfMachines
        {
            get
            {
                return Machines.Count;
            }
        }

        /// <summary>
        /// Gets the number of queues.
        /// </summary>
        /// <returns></returns>
        public int NumberOfQueues
        {
            get
            {
                return Queues.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasPriorityQueues { get; private set; }

        /// <summary>
        /// Gets the total processing time
        /// </summary>
        /// <returns></returns>
        public long TotalProcessingTime { get; private set; }

        /// <summary>
        /// Gets or sets the maximum jobs enqueued.
        /// </summary>
        /// <returns></returns>
        public int MaxJobsQueued { get; set; }


        /// <summary>
        /// Gets the total jobs in a priority.
        /// </summary>
        /// <param name="p">The priority.</param>
        /// <returns></returns>
        public int JobsInPriority(Priority p)
        {
            return mCounterJobsInPriority[(int)p];
        }

        /// <summary>
        /// Gets the total processing time in a priority.
        /// </summary>
        /// <param name="p">The priority.</param>
        /// <returns></returns>
        public int ProcessingTimeInPriority(Priority p)
        {
            return mProcessingTimeInPriority[(int)p];
        }
        #endregion

        #region"Methods"
        #endregion
    }
}
