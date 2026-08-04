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
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(str => DateOnly.Parse(str));
        CreateMap<Message, MessageDto>()
            .ForMember(
                d => d.SenderPhotoUrl,
                options => options.MapFrom(
                    s => s.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url
            ))
            .ForMember(
                d => d.RecipientPhotoUrl,
                options => options.MapFrom(
                    s => s.Recipient.Photos.FirstOrDefault(p => p.IsMain)!.Url
            ));
    }
}
