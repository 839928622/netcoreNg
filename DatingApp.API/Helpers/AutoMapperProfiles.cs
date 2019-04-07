using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
           CreateMap<User,UserForListDto>()
           .ForMember(dest => dest.PhotoUrl,options => {
            options.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
           })
           .ForMember(dest => dest.Age,options => {
            options.ResolveUsing(d => d.DateOfBirth.CalculateAge());
           }) //使用生日来计算 age
           ; //与MVC一样，使用auto mapper需要设置
           CreateMap<User,UserForDetailDto>()
           .ForMember(dest => dest.PhotoUrl,options => {
            options.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
           })
           .ForMember(dest => dest.Age,options => {
            options.ResolveUsing(d => d.DateOfBirth.CalculateAge());
           })
           ;
           CreateMap<Photo,PhotosForDetailDto>();
        }
    }
}