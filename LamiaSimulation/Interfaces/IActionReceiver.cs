namespace LamiaSimulation
{
    public interface IActionReceiver
    {
        void PerformAction(ClientAction action)
        {
        }

        void PerformAction<T>(ClientAction action, ClientParameter<T> param1)
        {
        }

        void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
        }

        void PerformAction<T1, T2, T3>(
            ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2, ClientParameter<T3> param3
        )
        {
        }
    }
}