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
           }) // User中存在用户注册的日期但不存在年龄，使用这种方式（扩展方法）计算出年龄
           ;

           CreateMap<Photo,PhotosForDetailDto>();
           CreateMap<UserForUpdateDto,User>();
           CreateMap<PhotoForCreationDto,Photo>();//用户上传时的照片
           CreateMap<Photo,PhotoForReturnDto>();//服务器下传给用户的照片
           CreateMap<UserForRegisterDto,User>(); //用户注册dto
           CreateMap<MessageForCreationDto,Message>(); // 注册消息dto 和 消息实体之间的map关系
           CreateMap<Message,MessageForCreationDto>(); //这个是用来返回给前台的消息 

           CreateMap<Message,MessageToReturnDto>()
           .ForMember(
                m=> m.SenderPhotoUrl, 
                opt => opt.MapFrom
                  ( u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url
                  )) // 发送者的主照片
             .ForMember(
                m=> m.RecipientPhotoUrl, 
                opt => opt.MapFrom
                  ( u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url
                  )); // 接收者的主照片       消息 和 用于返回前台的消息dto
        }
    }
}