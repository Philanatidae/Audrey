using System;
namespace Audrey.Exceptions
{
    public class ComponentAlreadyExistsException : Exception
    {
        public ComponentAlreadyExistsException() : base("Component already exists")
        {
        }
    }
}
