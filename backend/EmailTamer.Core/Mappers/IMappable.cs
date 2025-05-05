using AutoMapper;

namespace EmailTamer.Core.Mappers;

public interface IMappable
{
    public static abstract void AddProfileMapping(Profile profile);
}