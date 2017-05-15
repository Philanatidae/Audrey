using System;

namespace Audrey.Exceptions
{
    public class TypeNotComponentException : Exception
    {
        public TypeNotComponentException() : base("Type does not extend IComponent")
        {
        }
    }
}
