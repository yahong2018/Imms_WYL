using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Imms.WebManager.Filters;
using Imms.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using System.Text;
using Imms.Mes.Services;
using Microsoft.AspNetCore.Http.Features;

namespace Imms.WebManager
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
            services.AddSession();
            services.AddMvc(config =>
            {
                config.Filters.Add(new ExtJsResponseBodyFilter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(x =>
            {
                x.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                x.SerializerSettings.Formatting = Formatting.Indented;
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,

                    ValidIssuer = GlobalConstants.JWT_ISSURER_URL,
                    ValidAudience = "api",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalConstants.JWT_SECRET_STRING)),

                    ValidateAudience = false,
                    ValidateIssuer = false,
                };

                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Token == null)
                        {
                            try
                            {
                                byte[] buffer = context.HttpContext.Session.Get(GlobalConstants.AUTHROIZATION_SESSION_KEY);
                                string token = System.Text.Encoding.UTF8.GetString(buffer);

                                context.Token = token;
                            }
                            catch
                            {
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
            });

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 30_000_000; //上传的文件，最大不能超过30MB
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            GlobalConstants.DbContextFactory = new DbContextFactory();
            ImmsDbContext.RegisterModelBuilders(new Imms.Security.Data.SecurityModelBuilder());
            ImmsDbContext.RegisterModelBuilders(new Imms.Mes.Data.MesModelBuilder());

            GlobalConstants.GetCurrentUserDelegate = Security.Data.SystemUserLogic.GetCurrentUser;
            services.AddHttpClient();

            this.RegisterKanbanServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Startup.AppBuilder = app;

            app.UseExceptionHandler(builder =>
              {
                  builder.Run(async context =>
                  {
                      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                      context.Response.ContentType = "application/json";

                      var ex = context.Features.Get<IExceptionHandlerFeature>();
                      await context.Response.WriteAsync(ex?.Error?.Message ?? "出现系统错误");
                  });
              });

            if (env.IsDevelopment())
            {
                //  注释以禁用开发异常处理功能
                // app.UseDeveloperExceptionPage();
                //
            }
            else
            {
                // app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseErrorHandling();
            app.UseAuthentication();
            Imms.HttpContext.Configure(app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>());

            this.StartKanban(app);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void RegisterKanbanServices(IServiceCollection services)
        {
            services.AddSingleton<Imms.Mes.Services.Kanban.Line.DataService, Imms.Mes.Services.Kanban.Line.DataService>();
            services.AddSingleton<Imms.Mes.Services.Kanban.Line.LampService, Imms.Mes.Services.Kanban.Line.LampService>();
            services.AddSingleton<Imms.Mes.Services.Kanban.Workshop.WorkshopDataService, Imms.Mes.Services.Kanban.Workshop.WorkshopDataService>();
            services.AddSingleton<Imms.Mes.Services.Kanban.Factory.FactoryDataService, Imms.Mes.Services.Kanban.Factory.FactoryDataService>();

            services.AddWebSocketManager();
        }

        private void StartKanban(IApplicationBuilder app)
        {
            Imms.Mes.Services.Kanban.Line.DataService dataService = app.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.DataService>();
            Imms.Mes.Services.Kanban.Line.LampService lampService = app.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Line.LampService>();
            Imms.Mes.Services.Kanban.Workshop.WorkshopDataService workshopDataService = app.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Workshop.WorkshopDataService>();
            Imms.Mes.Services.Kanban.Factory.FactoryDataService factoryDataService = app.ApplicationServices.GetService<Imms.Mes.Services.Kanban.Factory.FactoryDataService>();

            lampService.Config();
            dataService.Config();
            dataService.Startup();
            lampService.Startup();

            workshopDataService.Config();
            workshopDataService.Startup();

            factoryDataService.Config();
            factoryDataService.Startup();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            app.MapWebSocketManager("/line", app.ApplicationServices.GetService<Mes.Services.Kanban.Line.LineKanbanService>());
            app.MapWebSocketManager("/workshop", app.ApplicationServices.GetService<Mes.Services.Kanban.Workshop.WorkshopWebService>());
            app.MapWebSocketManager("/factory", app.ApplicationServices.GetService<Mes.Services.Kanban.Factory.FactoryWebService>());
        }

        public static IApplicationBuilder AppBuilder { get; private set; }
    }

    public class DbContextFactory : IDbContextFactory
    {
        public DbContext GetContext()
        {
            return new ImmsDbContext();
        }

        public DbContext GetContext(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
