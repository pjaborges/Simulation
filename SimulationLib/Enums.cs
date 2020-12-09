using System.ComponentModel;

namespace SimulationLib
{
        public enum Priority : byte
    {
        [Description("Low Priority.")]
        Low,
        [Description("Medium Priority.")]
        Medium,
        [Description("High Priority.")]
        High
    }

    public enum DispatchRule : byte
    {
        [Description("First In First Out.")]
        FIFO
        // TODO: add additional rules when implemented.
        // <Description("Shortest Processing Time.")> SPT
    }
}
