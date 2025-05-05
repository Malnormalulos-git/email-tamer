using AutoMapper;

namespace EmailTamer.Core.Mappers;

public abstract class MappableProfile : Profile
{
    protected MappableProfile()
    {
        var typesToScan = GetType().Assembly.DefinedTypes;
        var addProfileMappingMethods = typesToScan
            .Where(type => type.IsAssignableTo(typeof(IMappable)) && !type.IsAbstract)
            .Select(type => type.GetMethod(nameof(IMappable.AddProfileMapping))!)
            .ToList();

        foreach (var method in addProfileMappingMethods)
        {
            method.Invoke(null, [this]);
        }
    }
}