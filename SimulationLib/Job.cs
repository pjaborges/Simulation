using System.Linq;

namespace SimulationLib
{
    public partial class Job
    {
        private int mStartTime;
        private readonly int[] mProcessingTimes;

        #region"Constructor"
        /// <summary>
        /// Initializes a new instance of the <see cref="Job" /> class.
        /// </summary>
        /// <param name="id">The ID of the job.</param>
        /// <param name="name">The name of the job.</param>
        /// <param name="eaTime">The expected arrival time.</param>
        /// <param name="p">The priority.</param>
        /// <param name="pTimes">The processing times.</param>
        /// <param name="dueTime">The due time.</param>
        public Job(int id, string name, int eaTime, Priority p, int[] pTimes, int dueTime = int.MaxValue)
        {
            ID = id;
            Name = name;
            ExpectedArrivalTime = eaTime;
            Priority = p;
            mProcessingTimes = pTimes;
            TotalProcessingTime = pTimes.Sum();
            ExpectedDepartureTime = eaTime + TotalProcessingTime;
            DueTime = dueTime;
            mStartTime = eaTime;
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
        /// Gets the expected arrival time.
        /// </summary>
        /// <returns></returns>
        public int ExpectedArrivalTime { get; private set; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <returns></returns>
        public Priority Priority { get; private set; }

        /// <summary>
        /// Gets the expected departure time.
        /// </summary>
        /// <returns></returns>
        public int ExpectedDepartureTime { get; private set; }

        /// <summary>
        /// Gets the due time.
        /// </summary>
        /// <returns></returns>
        public int DueTime { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <returns></returns>
        public int StartTime
        {
            get
            {
                return mStartTime;
            }
        }

        /// <summary>
        /// Gets the total processing time.
        /// </summary>
        /// <returns></returns>
        public int TotalProcessingTime { get; private set; }

        /// <summary>
        /// Gets thev processing time of a specific task.
        /// </summary>
        /// <param name="taskID">The task ID.</param>
        /// <returns></returns>
        public int ProcessingTime(int taskID)
        {
            return mProcessingTimes[taskID];
        }

        // Public Property Machines As Byte()
        #endregion

        #region"Methods"
        /// <summary>
        /// Returns the departure time.
        /// </summary>
        /// <returns></returns>
        public int DepartureTime()
        {
            return mStartTime + TotalProcessingTime;
        }

        /// <summary>
        /// Return the waiting time.
        /// </summary>
        /// <returns></returns>
        public int WaitingTime()
        {
            return mStartTime - ExpectedArrivalTime;
        }

        /// <summary>
        /// Returns the tardiness.
        /// </summary>
        /// <returns></returns>
        public int Tardiness()
        {
            if (DueTime != int.MaxValue)
            {
                return DepartureTime() - DueTime;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Changes the start time.
        /// </summary>
        /// <param name="value"></param>
        public void ChangeStartTime(int value)
        {
            mStartTime = value;
        }
        #endregion
    }
}
