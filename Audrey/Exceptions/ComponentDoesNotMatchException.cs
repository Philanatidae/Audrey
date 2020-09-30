using System;
using System.Collections.Generic;
using System.Text;

namespace Audrey.Exceptions
{
    public class ComponentDoesNotMatchException : Exception
    {
        public ComponentDoesNotMatchException()
            : base("Component does not match Type of ComponentMap")
        {
        }
    }
}
