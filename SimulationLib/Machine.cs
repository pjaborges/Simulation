using System.Collections.Generic;

namespace SimulationLib
{

    public partial class Machine
    {
        private int mClock;
        // Private mPrecedents As Machine()
        private readonly List<Job> mJobs;
        private int mNumberJobs;
        private int mIddleTime;
        private int mIddlePeriods;
        private readonly int[] mNumberPriorityJobs;

        #region"Constructor"
        /// <summary>
        /// Initializes a new instance of the <see cref="Machine" /> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="size"></param>
        public Machine(int id, string name, int size = 0)
        {
            ID = id;
            Name = name;
            mClock = 0;
            mJobs = new List<Job>(size);
            mNumberPriorityJobs = new int[3];
        }
        #endregion

        #region"Members"
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <returns></returns>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the clock.
        /// </summary>
        /// <returns></returns>
        public int Clock
        {
            get
            {
                return mClock;
            }
        }

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Job> Jobs
        {
            get
            {
                return mJobs;
            }
        }

        /// <summary>
        /// Gets the number of jobs processed.
        /// </summary>
        /// <returns></returns>
        public int NumberOfJobs
        {
            get
            {
                return mNumberJobs;
            }
        }

        /// <summary>
        /// Gets the number of processed jobs of a priority.
        /// </summary>
        /// <param name="p">The priority.</param>
        /// <returns></returns>
        public int NumberJobs(Priority p)
        {
            return mNumberPriorityJobs[(int)p];
        }

        /// <summary>
        /// Gets the first job processed.
        /// </summary>
        /// <returns></returns>
        public Job FirstJob
        {
            get
            {
                if (mNumberJobs == 0)
                    return default;
                return mJobs[0];
            }
        }

        /// <summary>
        /// Gets the iddle time.
        /// </summary>
        /// <returns></returns>
        public int IddleTime
        {
            get
            {
                return mIddleTime;
            }
        }

        /// <summary>
        /// Gets the iddle periods.
        /// </summary>
        /// <returns></returns>
        public int IddlePeriods
        {
            get
            {
                return mIddlePeriods;
            }
        }
        #endregion

        #region"Methods"
        /// <summary>
        /// Adds a job into the machine.
        /// </summary>
        /// <param name="j">The job.</param>
        public void AddJob(Job j)
        {
            mJobs.Add(j);
            mClock = j.DepartureTime();
            mNumberJobs += 1;
            mNumberPriorityJobs[(int)j.Priority] += 1;
        }

        /// <summary>
        /// Computes the Iddle periods and time.
        /// </summary>
        public void IddleStats()
        {
            if (mNumberJobs == 0)
            {
                return;
            }

            int temp;
            Job j1;
            Job j2;
            temp = mJobs[0].StartTime - 0;
            if (temp > 0)
                mIddlePeriods += 1;
            mIddleTime = temp;
            for (int i = 0, loopTo = mJobs.Count - 2; i <= loopTo; i++)
            {
                j1 = mJobs[i];
                j2 = mJobs[i + 1];
                temp = j2.StartTime - j1.DepartureTime();
                if (temp > 0)
                {
                    mIddleTime += temp;
                    mIddlePeriods += 1;
                }
            }
        }

        /// <summary>
        /// Computes the mean iddle time.
        /// </summary>
        /// <returns>The mean iddle time.</returns>
        public double MeanIddleTime()
        {
            return mIddleTime / (double)mIddlePeriods;
        }
#endregion

    }
}
