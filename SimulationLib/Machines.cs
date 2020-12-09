using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationLib
{
    public partial class Machines : List<Machine>
    {
        #region"Constructor"
        /// <summary>
        /// Initializes a new instance of the <see cref="Machines" /> class.
        /// </summary>
        /// <param name="howMany">Number of machines.</param>
        public Machines(int howMany) : base(howMany - 1)
        {
            try
            {
                for (int i = 0, loopTo = howMany - 1; i <= loopTo; i++)
                    Add(new Machine(i, string.Format("M.{0}", i)));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region"Members"
        #endregion

        #region"Methods"
        /// <summary>
        /// Adds precedent machines.
        /// </summary>
        /// <param name="mchns">The precedent machines.</param>
        public void AddPrecendents(Machine[] mchns)
        {
            // TODO add the machines the precede this machine.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a job into a machine.
        /// </summary>
        /// <param name="m">A machine.</param>
        /// <param name="j">A job.</param>
        public void AddJob(Machine m, Job j)
        {
            if (j.StartTime < m.Clock)
            {
                j.ChangeStartTime(m.Clock);
            }

            this.ElementAt(m.ID).AddJob(j);
        }

        /// <summary>
        /// Returns an available machine or none depending on the machine clock.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <returns></returns>
        public Machine AvailableMachine(int clock)
        {
            foreach (var mchn in this)
            {
                if (mchn.Clock <= clock)
                {
                    return mchn;
                }
            }

            return default;
        }

        /// <summary>
        /// Returns the next clock when the machine is available.
        /// </summary>
        /// <returns></returns>
        public int NextAvailableClock()
        {
            return this.AsQueryable().Min(m => m.Clock);
        }

        /// <summary>
        /// Returns the total iddle time.
        /// </summary>
        /// <returns></returns>
        public int TotalIddleTime()
        {
            return this.AsQueryable().Sum(m => m.IddleTime);
        }

        /// <summary>
        /// Returns the total iddle periods.
        /// </summary>
        /// <returns></returns>
        public int TotalIddlePeriods()
        {
            return this.AsQueryable().Sum(m => m.IddlePeriods);
        }

        /// <summary>
        /// Returns the makespam.
        /// </summary>
        /// <returns></returns>
        public int Makespam()
        {
            return this.AsQueryable().Max(m => m.Clock);
        }
        #endregion
    }
}
