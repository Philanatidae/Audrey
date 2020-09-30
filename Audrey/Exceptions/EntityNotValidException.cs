using System;
using System.Collections.Generic;
using System.Text;

namespace Audrey.Exceptions
{
    public class EntityNotValidException : Exception
    {
        public EntityNotValidException()
            :base("Entity is not valid within the Engine.")
        {
        }
    }
}
