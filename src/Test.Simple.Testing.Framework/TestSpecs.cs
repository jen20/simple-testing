using System;
using Simple.Testing.Framework;

namespace Test.Simple.Testing.Framework
{
    public class TestSpecs
    {
        public static ActionSpecification<int> SpecWithExceptionInBefore = new ActionSpecification<int>
                                                                               {
                                                                                   Before = () => { throw new ArgumentException("test"); },
                                                                                   Expect = {x => x.Equals(3)}
                                                                               };

        public static TypedSpecification<int> SpecWithExceptionInOn = new ActionSpecification<int>
                                                                          {
                                                                              On = () => { throw new ArgumentException("test2"); },
                                                                              Expect = {x => x.Equals(3)}
                                                                          };

        public static TypedSpecification<int> SpecWithNoWhen = new ActionSpecification<int>
                                                                   {
                                                                       On = () => 3,
                                                                       Expect = { x => x.Equals(3) }
                                                                   };

        public static TypedSpecification<int> SpecWithExceptionInWhen = new ActionSpecification<int>
                                                                            {
                                                                                On = () => 3,
                                                                                When = data => {throw new ArgumentException("test3");},
                                                                                Expect = { x => x.Equals(3) }
                                                                            };

        public static TypedSpecification<int> SpecWithExceptionInFinally = new ActionSpecification<int>
                                                                               {
                                                                                   On = () => 3,
                                                                                   When = data => data++,
                                                                                   Expect = { x => x.Equals(3) },
                                                                                   Finally = () => { throw new ArgumentException("test4"); }
                                                                               };
        
        public static TypedSpecification<int> SpecWithExceptionInExpectation = new ActionSpecification<int>
                                                                                   {
                                                                                       On = () => 3,
                                                                                       When = data => data++,
                                                                                       Expect = {x => MethodThatThrows(x) }
                                                                                   };

        public static TypedSpecification<int> SpecWithSinglePassingExpectation = new ActionSpecification<int>
                                                                                     {
                                                                                         On = () => 3,
                                                                                         When = data => data++,
                                                                                         Expect = { x => x == x }
                                                                                     };

        private static bool MethodThatThrows(object o)
        {
            throw new ArgumentException("methodthatthrows");
        }
    }
}