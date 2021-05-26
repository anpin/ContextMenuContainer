using System;
namespace APES.UI.XF.Sample
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
