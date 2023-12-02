using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoApiContable.Entities;
using System.Text;
using ProyectoApiContable.Helpers;
using ProyectoApiContable.Services;
using StackExchange.Redis;

namespace ProyectoApiContable
{
    public class Startup
    {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers();

                // Add DbContext
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(Configuration
                        .GetConnectionString("DefaultConnection"));
                });
                
                // Add AutoMapper
                services.AddAutoMapper(typeof(AutoMapperProfiles));
                
                // Add Servicio de Email
                services.AddTransient<IEmailService, EmailService>();

                // Add Redis
                string redisConnectionString = Configuration.GetConnectionString("Redis");
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
                
                //Agregar Servicio de Redis
                services.AddScoped<IRedisServices, RedisServices>();

                // Add Identity
                services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                }).AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                // Add Authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }
    }