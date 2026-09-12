using Application.Users;
using AutoMapper;
using Domain.Users;


namespace Application.Commom.MappingProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<User, UserResponse>()
            .ForCtorParam(
                nameof(UserResponse.UserId),
                opt => opt.MapFrom(src => src.UserId)
            )
            .ForCtorParam(
                nameof(UserResponse.FullName),
                opt => opt.MapFrom(src => src.FullName)
            )
            .ForCtorParam(
                nameof(UserResponse.Email),
                opt => opt.MapFrom(src => src.Email)
            );

        }
    }
}