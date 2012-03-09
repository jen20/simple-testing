using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Simple.Testing.Framework
{
    public class FailingSpecification<TSut, TException> : TypedSpecification<TException> where TException : Exception
    {
        public Action Before;
        public Func<TSut> On;
        public WhenAction<TSut> When;
        public List<Expression<Func<TException, bool>>> Expect = new List<Expression<Func<TException, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen()
        {
            return (Func<TSut, TException>) (x =>
                                                 {
                                                     try
                                                     {
                                                         When(x);
                                                     }
                                                     catch (TException ex)
                                                     {
                                                         return ex;
                                                     }
                                                     return null;
                                                 });
        }


        public IEnumerable<Expression<Func<TException, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }
}