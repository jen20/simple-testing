using Simple.Testing.Framework;

namespace SimpleTestingSample.QuerySpecification
{
    /// <summary>
    /// A query specification is intended to be used on a method with a return value. The 
    /// general idea is that the On() will build the SUT. The when() will call the method 
    /// returning the methods return value. 
    /// 
    /// The expectations are then on the returned value. 
    /// 
    /// You may wonder how you can assert on the sut after the call. You can't using this 
    /// template. This is by design; see CQS by Bertrand Meyer: you should not mutate state 
    /// of the object when querying. If you want to break this rule use a more open template 
    /// and specialize it.
    /// </summary>
    public class QuerySpecifications
    {
        public Specification it_returns_something_interesting = new QuerySpecification<QueryExample, Product>
                                                                    {
                                                                        On = () => new QueryExample(),
                                                                        When = obj => obj.GetProduct(14),
                                                                        Expect =
                                                                            {
                                                                                product => product.Id == 14,
                                                                                product => product.Code == "TEST",
                                                                                product => product.Description != "test description"
                                                                            },
                                                                    };
    }
}