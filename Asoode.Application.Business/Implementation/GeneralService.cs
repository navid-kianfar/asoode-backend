using Asoode.Application.Abstraction.Contracts;

namespace Asoode.Application.Business.Implementation;

internal class GeneralService : IGeneralService
{
    private readonly Dictionary<string, Dictionary<string, object>> _repository;
    
    public GeneralService()
    {
        _repository = new Dictionary<string, Dictionary<string, object>>();
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t =>
                t.IsEnum &&
                !string.IsNullOrEmpty(t.Namespace) &&
                t.Namespace.Contains("Asoode.")
            )
            .ToList()
            .ForEach(enm =>
            {
                var values = Enum.GetValues(enm).Cast<object>();
                _repository[enm.Name] = values.ToDictionary(value => Enum.GetName(enm, value));
            });
    }

    public Dictionary<string, Dictionary<string, object>> Enums()
    {
        return _repository;
    }
}