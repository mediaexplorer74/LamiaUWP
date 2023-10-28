namespace LamiaSimulation
{
    public interface ISimulated
    {
        void Simulate(float deltaTime);

        void LoadedFromSave();
    }
}