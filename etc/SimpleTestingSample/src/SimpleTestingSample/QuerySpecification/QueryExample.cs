namespace SimpleTestingSample.QuerySpecification
{
    public class QueryExample
    {
        public Product GetProduct(int id)
        {
            return new Product(id, "TEST", "test description");
        }
    }
}