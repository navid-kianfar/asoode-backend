using System.Collections.Generic;

namespace Asoode.Core.Contracts.General;

public interface IGeneralBiz
{
    Dictionary<string, Dictionary<string, object>> Enums();
}