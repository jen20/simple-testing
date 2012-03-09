using Simple.Testing.Framework;

namespace SimpleTestingSample.ActionSpecification
{
    /// <summary>
    /// In an action specification the SUT is returned from the On(). The SUT is then 
    /// given as a parameter to the When and the Expects. 
    /// 
    /// It is expected that the when() will issue some behavior that mutates the SUT 
    /// which is then sent to the expectations.
    /// </summary>
    public class ActionSpecifications
    {
        public ActionSpecification<Depositor> when_withdrawing_money_from_empty_account = new ActionSpecification<Depositor>
                                                                             {
                                                                                 On = () => new Depositor(13),
                                                                                 When = depositor => depositor.Withdraw(50.00m),
                                                                                 Expect =
                                                                                     {
                                                                                         depositor => depositor.Balance > 0.01m,
                                                                                         depositor => depositor.AccountIsOpen,
                                                                                         depositor => depositor.Balance < 50m * GetOverallCount() / 5
                                                                                     },
                                                                             };

        private static decimal GetOverallCount()
        {
            return 12;
        }
    }
}