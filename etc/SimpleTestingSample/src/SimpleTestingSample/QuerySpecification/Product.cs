namespace SimpleTestingSample.QuerySpecification
{
    public class Product
    {
        public readonly string Code;
        public readonly string Description;
        public readonly int Id;

        public Product(int id, string code, string description)
        {
            Id = id;
            Description = description;
            Code = code;
        }
    }
}