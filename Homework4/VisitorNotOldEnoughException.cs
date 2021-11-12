using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework4
{
    class VisitorNotOldEnoughException : InvalidOperationException
    {
        public VisitorNotOldEnoughException(Student visitor, int minimumAge)
            : base($"{visitor.Name} ({visitor.Age} years old) is not old enough to enter! They have to be at least {minimumAge} years old.")
        { }
    }
}
