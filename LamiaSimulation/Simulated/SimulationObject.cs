using System;

namespace LamiaSimulation
{
    public abstract class SimulationObject
    {
        public readonly string ID;

        protected SimulationObject()
        {
            ID = Guid.NewGuid().ToString();
            Simulation.lastID = ID;
        }
    }
}