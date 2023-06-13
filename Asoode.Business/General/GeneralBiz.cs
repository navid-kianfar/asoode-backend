using System;
using System.Collections.Generic;
using System.Linq;
using Asoode.Core.Contracts.General;

namespace Asoode.Business.General;

internal class GeneralBiz : IGeneralBiz
{
    private readonly Dictionary<string, Dictionary<string, object>> _repository;
    private readonly IServiceProvider _serviceProvider;

    public GeneralBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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