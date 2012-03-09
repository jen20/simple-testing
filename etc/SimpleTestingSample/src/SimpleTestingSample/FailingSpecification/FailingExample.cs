namespace SimpleTestingSample.FailingSpecification
{
    public class FailingExample
    {
        public void CauseFailure()
        {
            throw new SomethingFailedException("Something failed!", 17);
        }
    }
}