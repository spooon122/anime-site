using AnimeSite.Core.Models;
using AnimeSite.DataAccess.Entities;
using AutoMapper;

namespace AnimeSite.DataAccess.Mapping;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<UserEntity, User>();
    }
}