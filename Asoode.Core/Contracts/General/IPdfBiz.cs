using System.Threading.Tasks;
using Asoode.Core.Primitives;

namespace Asoode.Core.Contracts.General;

public interface IPdfBiz
{
    Task<OperationResult<bool>> FromHtml(string html, string destination);
}