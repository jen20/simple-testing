namespace SimpleTestingSample.ActionSpecification
{
    public class Depositor
    {
        public readonly bool AccountIsOpen = true;
        public readonly decimal Balance = 50.00m;
        private readonly int _depositorId;

        public Depositor(int depositorId)
        {
            _depositorId = depositorId;
        }

        public void Withdraw(decimal amount)
        {
        }
    }
}