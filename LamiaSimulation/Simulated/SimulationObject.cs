using System;

namespace LamiaSimulation
{
    public abstract class SimulationObject
    {
        public string ID { get; set; }

        protected SimulationObject()
        {
            ID ??= Guid.NewGuid().ToString();
            Simulation.lastID = ID;
        }
    }
}