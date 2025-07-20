namespace LamiaSimulation
{
    public interface IQueryable
    {
        void Query<T>(ref QueryResult<T> result, ClientQuery query)
        {
        }

        void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
        }

        void Query<T, T1, T2>(
            ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1, ClientParameter<T2> param2
        )
        {
        }

        void Query<T, T1, T2, T3>(
            ref QueryResult<T> result, ClientQuery query,
            ClientParameter<T1> param1, ClientParameter<T2> param2, ClientParameter<T3> param3
        )
        {

        }
    }
}