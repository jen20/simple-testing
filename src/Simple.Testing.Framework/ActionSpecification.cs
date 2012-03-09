using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Simple.Testing.Framework
{
    public class ActionSpecification<TSut> : TypedSpecification<TSut>
    {
        public Action Before;
        public Func<TSut> On;
        public Action<TSut> When;
        public List<Expression<Func<TSut, bool>>> Expect = new List<Expression<Func<TSut, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen() { return When; }
        public IEnumerable<Expression<Func<TSut, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }
}