namespace Simple.Testing.Framework
{
    public static class SpecificationExtensions
    {
        public static SpecificationToRun AsRunnable(this Specification specification)
        {
            return new SpecificationToRun(specification, null);
        }
    }
}
