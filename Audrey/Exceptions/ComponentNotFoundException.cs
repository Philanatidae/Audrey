using System;
namespace Audrey.Exceptions
{
    public class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException() : base("Component not found for entity")
        {
        }
    }
}
