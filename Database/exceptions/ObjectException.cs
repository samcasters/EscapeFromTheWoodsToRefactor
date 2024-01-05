﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Database.exceptions
{
    public class ObjectException : Exception
    {
        public ObjectException(string message) : base(message)
        {
        }

        public ObjectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
