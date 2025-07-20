namespace LamiaSimulation
{
    public class QueryResult<T>
    {
        public T Value { get; set; }

        public QueryResult(T value)
        {
            Value = value;
        }
    }
}