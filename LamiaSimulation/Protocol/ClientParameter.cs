using System;

namespace LamiaSimulation
{
    public class ClientParameter<T>
    {
        private T _value;

        public T Get => _value;
        public void Set(T value) => _value = value;

        public ClientParameter(object value)
        {
            _value = (T) value;
        }

        public T Coerce<T>() => (T)Convert.ChangeType(_value, typeof(T));
    }
}