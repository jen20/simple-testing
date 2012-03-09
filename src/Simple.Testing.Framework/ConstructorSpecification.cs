using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Simple.Testing.Framework
{
    public class ConstructorSpecification<TObject> : TypedSpecification<TObject>
    {
        public Action Before;
        public Func<TObject> When;
        public List<Expression<Func<TObject, bool>>> Expect = new List<Expression<Func<TObject, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() {return (Action) (() => {});}
        public Delegate GetWhen() { return When; }
        public IEnumerable<Expression<Func<TObject, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }
}