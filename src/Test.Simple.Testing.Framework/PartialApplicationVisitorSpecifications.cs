using System;
using System.Linq.Expressions;
using Simple.Testing.Framework;

namespace Test.Simple.Testing.Framework
{
    public class PartialApplicationVisitorSpecifications
    {
        public Specification can_exchange_parameters_for_values = new QuerySpecification<PartialApplicationVisitor, Expression<Func<bool>>>
                                                                      { 
                                                                          On = () => null,
                                                                          When = q => PartialApplicationVisitor.Apply<int>(x => 44 == x + 1, 43),
                                                                          Expect =
                                                                              {
                                                                                  result => result.Compile().Invoke()
                                                                              }
                                                                      };
    }
}