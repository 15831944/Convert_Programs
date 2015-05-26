using System;
using System.Collections.Generic;
using System.Text;

namespace Convert_Programs
{
    [Serializable]
    public class Convert_ProgramsException : Exception
    {
        public Convert_ProgramsException() { }
        public Convert_ProgramsException(string message) : base(message) { }
        public Convert_ProgramsException(string message, Exception inner) : base(message, inner) { }
        protected Convert_ProgramsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
