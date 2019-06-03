using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // public void ConfigureServices(IServiceCollection services)
        // {
        //     services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        //     .AddJsonOptions(options => {
        //       options.SerializerSettings.ReferenceLoopHandling = 
        //               Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        //     });

        //     services.AddDbContext<DataContext>(d =>d.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
        //     .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning))
        //     );//这里可以使用上面注入的IConfiguration Configration ，Configuration对应的是appsettings.json这个json文件，里面可以获取到我们写入的配置信息。
        //     services.BuildServiceProvider().GetService<DataContext>().Database.Migrate(); // 这个命令会在运行的时候，执行未经处理的迁移migtaions。没有数据库的创建数据库，并应用迁移到数据库。有了数据库且已经迁移则无影响。 这个是自动化部署的内容之一。因为我们无法在本地使用dotnet ef migrations add [迁移的名字] 然后dotnet ef database uodate 。这样是行不通的。因为远程的容器/数据库不允许这么做。如果远程允许。那么上面的命令可以不需要，然后手动更新表结构至远程。
        //     services.AddCors(); //允许夸源访问
        //     services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings")); //绑定json中的设置到 类中的设置，然后CloudinarySettings类中的属性值将匹配至json中配置的值
        //     services.AddAutoMapper();
        //     services.AddTransient<Seed>();
        //     services.AddScoped<IAuthRepository,AuthRepository>(); // 对于注入服务的这种理解需要加强 目前还不了解
        //     services.AddScoped<IDatingRepository,DatingRepository>();
            
        //     // 这里是告诉 mvc 我们使用何种Authentication
        //     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options=>{
        //      options.TokenValidationParameters = new TokenValidationParameters
        //      {
        //         ValidateIssuerSigningKey = true,
        //          IssuerSigningKey = new SymmetricSecurityKey(
        //              Encoding
        //              .ASCII
        //              .GetBytes(Configuration.GetSection("AppSettings:Token").Value)), // 由于我们的key是字符串，因此要转换为byte[] 
        //             ValidateIssuer = false, // 由于是本地项目 设置为false
        //             ValidateAudience = false // localhost 

        //      };

        //     });

        //     services.AddScoped<LogUserActivity>();
        // }


         public void ConfigureServices(IServiceCollection services)
        {



            services.AddDbContext<DataContext>(d =>d.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));//这里可以使用上面注入的IConfiguration Configration ，Configuration对应的是appsettings.json这个json文件，里面可以获取到我们写入的配置信息。

            IdentityBuilder builder = services.AddIdentityCore<User>(options=>
            {
              options.Password.RequireDigit = true;
              options.Password.RequiredLength = 9 ;
              options.Password.RequireNonAlphanumeric = true; // 需要非字符数字 的符号
              options.Password.RequireLowercase = true;
              options.Password.RequireUppercase = true;
             

            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);//
            builder.AddEntityFrameworkStores<DataContext>(); // 告诉identtiy 我们想要使用ef作为存储
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options=>{
             options.TokenValidationParameters = new TokenValidationParameters
             {
                ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding
                     .ASCII
                     .GetBytes(Configuration.GetSection("AppSettings:Token").Value)), // 由于我们的key是字符串，因此要转换为byte[] 
                    ValidateIssuer = false, // 由于是本地项目 设置为false
                    ValidateAudience = false // localhost 

             };


            });

            services.AddAuthorization(options => { // 授权
            options.AddPolicy("RequireAdmin",policy=>policy.RequireRole("Admin"));

               options.AddPolicy("ModeratePhotoRole",policy=>policy.RequireRole("Admin","Moderator"));

                  options.AddPolicy("VipOnly",policy=>policy.RequireRole("VIP"));

            });
            services.AddMvc(options =>
            {
              var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();// 全局鉴权
                options.Filters.Add(new AuthorizeFilter(policy));
                
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => {
              options.SerializerSettings.ReferenceLoopHandling = 
                      Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddCors(); //允许夸源访问
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings")); //绑定json中的设置到 类中的设置，然后CloudinarySettings类中的属性值将匹配至json中配置的值

           services.AddAutoMapper();
           //Mapper.Reset(); 
            services.AddTransient<Seed>();
           // services.AddScoped<IAuthRepository,AuthRepository>(); // 对于注入服务的这种理解需要加强 目前还不了解
            services.AddScoped<IDatingRepository,DatingRepository>();
            
            // 这里是告诉 mvc 我们使用何种Authentication

            services.AddScoped<LogUserActivity>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
                app.UseExceptionHandler(buider =>{
                 buider
                 .Run
                 (async context => 
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                     
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                         {
                             context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                         }
                    }
                 );

                });
            }

            // app.UseHttpsRedirection();
            seeder.SeedUsers(); 
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();// 上面配置好 authentication 这里要use ,效果是控制器上任何有authorize的，都会受到保护
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Fallback" , action = "Index"}
                );
            });
        }
    }
}
