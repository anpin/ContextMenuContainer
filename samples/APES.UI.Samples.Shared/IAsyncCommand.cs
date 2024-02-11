using System.Windows.Input;
namespace APES.UI.XF.Sample
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
        void RaiseCanExecuteChanged();
    }
}
