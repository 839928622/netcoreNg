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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => {
              options.SerializerSettings.ReferenceLoopHandling = 
                      Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddDbContext<DataContext>(d =>d.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));//这里可以使用上面注入的IConfiguration Configration ，Configuration对应的是appsettings.json这个json文件，里面可以获取到我们写入的配置信息。
            services.AddCors(); //允许夸源访问
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            services.AddScoped<IAuthRepository,AuthRepository>(); // 对于注入服务的这种理解需要加强 目前还不了解
            services.AddScoped<IDatingRepository,DatingRepository>();
            
            // 这里是告诉 mvc 我们使用何种Authentication
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
            // seeder.SeedUsers();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();// 上面配置好 authentication 这里要use ,效果是控制器上任何有authorize的，都会受到保护
            app.UseMvc();
        }
    }
}
