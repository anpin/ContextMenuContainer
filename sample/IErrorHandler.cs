using System;

namespace APES.MAUI.Sample
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
