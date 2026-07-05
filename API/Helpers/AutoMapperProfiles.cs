using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(d => d.PhotoUrl,
                       opt => opt.MapFrom(u => u.Photos.FirstOrDefault(p => p.IsMain)!.Url))
            .ForMember(d => d.Age,
                       opt => opt.MapFrom(u => u.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();
    }
}
