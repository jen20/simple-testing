using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace Simple.Testing.Resharper
{
    [ActionHandler("Simple.Testing.Resharper.About")]
    public class AboutAction : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show(
              "Simple.Testing\nSimple.Testing\n\nUnit test provider for the Simple.Testing Framework",
              "About Simple.Testing",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}
