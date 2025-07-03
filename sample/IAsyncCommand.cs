using System.Threading.Tasks;
using System.Windows.Input;
namespace APES.MAUI.Sample
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
        void RaiseCanExecuteChanged();
    }
}
