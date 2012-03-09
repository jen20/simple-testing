using System;
using System.Linq.Expressions;

namespace Simple.Testing.Framework
{
    public class ExpectationResult
    {
        public bool Passed;
        public string Text;
        public Exception Exception;
        public Expression OriginalExpression;
    }
}