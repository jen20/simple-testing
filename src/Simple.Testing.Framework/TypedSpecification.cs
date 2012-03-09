using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Simple.Testing.Framework
{
    public interface TypedSpecification<T> : Specification
    {
        Action GetBefore();
        Delegate GetOn();
        Delegate GetWhen();
        IEnumerable<Expression<Func<T, bool>>> GetAssertions();
        Action GetFinally();
    }
}