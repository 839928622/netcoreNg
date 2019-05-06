using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings;
            _mapper = mapper;
            _repo = repo;

            Account acc= new Account
            (
              _cloudinarySettings.Value.CloudName,
              _cloudinarySettings.Value.ApiKey,
              _cloudinarySettings.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}",Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

        return Ok(photo);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
             if (userId !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // User.FindFirst(ClaimTypes.NameIdentifier用来判断传递来的这个token中的id与浏览器中参数的id是否一致，如果不一致，说明用户数据可能被篡改，所以返回401
          return Unauthorized();

          var userFromRepo = await _repo.GetUser(userId);

          var file = photoForCreationDto.File;

          var uploadResult = new ImageUploadResult(); //用来接收从云返回的结果

          if (file.Length > 0)
          {
              using(var stream = file.OpenReadStream())
              {
                  var uploadParams = new ImageUploadParams()
                  {
                    File = new FileDescription(file.Name,stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    
                  };

                  uploadResult = _cloudinary.Upload(uploadParams);



              }
          }

          photoForCreationDto.Url = uploadResult.Uri.ToString();
          photoForCreationDto.PublicId = uploadResult.PublicId;

          var photo = _mapper.Map<Photo>(photoForCreationDto);
          if (!userFromRepo.Photos.Any(u => u.IsMain))
              photo.IsMain = true;

             userFromRepo.Photos.Add(photo); //从数据库里查询出来的user，可以点出Photos。最后添加照片进去。


          if (await _repo.SaveAll())   
          {
             var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);//当保存成功之后，photo才有Id
              return CreatedAtRoute("GetPhoto",new { id = photo.Id },photoToReturn);

          }

              return BadRequest("无法添加照片");
        }

       [HttpPost("{id}/setMain")] 
       public async Task<IActionResult> SetMainPhoto(int userId,int id) {
           if (userId !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
           return Unauthorized();

           var user = await _repo.GetUser(userId);

           if (!user.Photos.Any(p=>p.Id==id))
           return Unauthorized(); //如果查询不到图片 返回401

           var photoFromRepo = await _repo.GetPhoto(id);
           if (photoFromRepo.IsMain)
              return BadRequest("该相片已经是主图");

               var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

                currentMainPhoto.IsMain = false; // 设置当前照片主图为false

                photoFromRepo.IsMain = true; // 把前端传进来的图片设置为主图
               
                if (await _repo.SaveAll())
                  return NoContent();

                  return BadRequest("无法设置主图");


       }

       [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
        if (userId !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
           return Unauthorized();

           var user = await _repo.GetUser(userId);

           if(!user.Photos.Any(p =>p.Id == id))
             return Unauthorized();

             var photoFromRepo = await _repo.GetPhoto(id);

             if(photoFromRepo.IsMain)
             return BadRequest("抱歉不能删除主图");

             //接下来是删除照片：照片url存储在数据库中，且云上也有 需要逐一删除  图片中有些是云上的，有些是随机生成的，需要做判断

             if(photoFromRepo.PublicId != null)
             {
                //图片存储在云上
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams) ;

                if(result.Result == "ok")
                _repo.Delete(photoFromRepo);



             }

             if (photoFromRepo.PublicId == null)
             {
                 // 随机生成的图片
                 _repo.Delete(photoFromRepo);
             }
                if (await _repo.SaveAll())
                return Ok();
//什么都不做，调用savechanges()会报false
                return BadRequest("图片删除失败");



           
    }
    }
}