using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Logging
{
    public class ErrorViewModel : BaseViewModel
    {
        public string Description { get; set; }
        public string ErrorBody { get; set; }
        public int Index { get; set; }
    }
}