using System;
using System.Collections.Generic;
using System.Text;

namespace AccelerationMVVM.Persistence
{
    public class DataException : Exception
    {
        public DataException(String message) : base(message) { }
    }
}
