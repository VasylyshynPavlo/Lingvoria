using AutoMapper;
using Core.Models.UserModels;
using Core.Models.UserModels.Get;

namespace Core.MapperProfile;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<User, ShortUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarUrl ?? string.Empty))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName ?? string.Empty));
    }
}