using System;

namespace LamiaSimulation
{
    public class ClientActionException: Exception
    {
        public ClientActionException()
        {
        }

        public ClientActionException(string message) : base(message)
        {
        }

        public ClientActionException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ClientQueryException: Exception
    {
        public ClientQueryException()
        {
        }

        public ClientQueryException(string message) : base(message)
        {
        }

        public ClientQueryException(string message, Exception inner) : base(message, inner)
        {
        }
    }

}