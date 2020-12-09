using System.Collections.Generic;
using System.Linq;

namespace SimulationLib
{
    public partial class Jobs : Queue<Job>
    {
        private int mMaxSize;
        private readonly int[] mLatePriorityJobs; // counter for late jobs considering the priority

        #region"Constructor"
        /// <summary>
        /// Initializes a new instance of the <see cref="Jobs" /> class.
        /// </summary>
        /// <param name="disptchRule">The dispatch rule.</param>
        /// <param name="maxCapacity">The maximum capacity.</param>
        public Jobs(DispatchRule disptchRule = DispatchRule.FIFO, int maxCapacity = int.MaxValue)
        {
            DispatchRule = disptchRule;
            MaxCapacity = maxCapacity;
            mLatePriorityJobs = new int[3];
        }
        #endregion

        #region"Members"
        /// <summary>
        /// Gets the dispatching rule for this queue.
        /// </summary>
        /// <returns></returns>
        public DispatchRule DispatchRule { get; private set; }

        /// <summary>
        /// Gets the maximum capacity of the queue.
        /// </summary>
        /// <returns></returns>
        public int MaxCapacity { get; private set; }

        /// <summary>
        /// Gets the maximum size.
        /// </summary>
        /// <returns></returns>
        public int MaxSize
        {
            get
            {
                return mMaxSize;
            }
        }

        /// <summary>
        /// Gets the number of late jobs in a priotity.
        /// </summary>
        /// <param name="p">The priority.</param>
        /// <returns></returns>
        public int LateJobs(Priority p)
        {
            return mLatePriorityJobs[(int)p];
        }

        /// <summary>
        /// Gets the job with the ID.
        /// </summary>
        /// <param name="indx">The job ID.</param>
        /// <returns></returns>
        public Job Job(int id)
        {
            return this.ElementAt(id);
        }
        #endregion

        #region"Methods"
        /// <summary>
        /// Enqueues a job in the queue.
        /// </summary>
        /// <param name="j">The job.</param>
        public new void Enqueue(Job j)
        {
            base.Enqueue(j);
            if (Count > mMaxSize)
            {
                mMaxSize = Count;
            }

            mLatePriorityJobs[(int)j.Priority] += 1;
        }

        /// <summary>
        /// Returns the total of late jobs.
        /// </summary>
        /// <returns></returns>
        public int TotalLateJobs()
        {
            return mLatePriorityJobs.Sum();
        }

        /// <summary>
        /// Returns true if queue is full.
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            if (mMaxSize >= MaxCapacity)
                return true;
            return false;
        }
        #endregion
    }
}
