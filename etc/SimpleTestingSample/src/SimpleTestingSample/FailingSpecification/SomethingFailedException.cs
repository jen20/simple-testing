using System;

namespace SimpleTestingSample.FailingSpecification
{
    public class SomethingFailedException : Exception
    {
        public readonly int ErrorCode;

        public SomethingFailedException(string msg, int errorCode)
            : base(msg)
        {
            ErrorCode = errorCode;
        }
    }
}