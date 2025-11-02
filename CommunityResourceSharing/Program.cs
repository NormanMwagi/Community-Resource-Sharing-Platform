
using CommunityResourceSharing.Data;
using CommunityResourceSharing.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using CommunityResourceSharing.Services;


namespace CommunityResourceSharing
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
           
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DbConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DbConnection"))
                )
            );

            builder.Services.AddScoped<TokenService>();
            builder.Services.AddAuthorization();
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddOpenApi();
            builder.Services.AddAutoMapper(typeof(Program));

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var secretKey = jwtSettings["SecretKey"];
            //jwt
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
                )
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true, 
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey!))                    
                    };
              });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Community Resource Sharing API",
                    Version = "v1",
                    Description = "API for managing resources and borrowing requests within the community."
                });

                // ✅ Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token below prefixed with 'Bearer ' (without quotes). Example: **Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...**"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {        
                    {            
                        new OpenApiSecurityScheme            
                        {                
                            Reference = new OpenApiReference                
                            {                    
                                Type = ReferenceType.SecurityScheme,                    
                                Id = "Bearer"                
                            }           
                        },            
                        new string[] {}         
                    }    
                });
            });


            var app = builder.Build();
            //seed admin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedDefaultAdminAsync(userManager, roleManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger(); //Enable Swagger
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community Resource Sharing API V1");
                    c.RoutePrefix = string.Empty; // Launch swagger at root (http://localhost:5043)
                });

            }
            //app.MapIdentityApi<IdentityUser>();

            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Auth Header: {context.Request.Headers["Authorization"]}");
                await next();
            });
          
            app.UseAuthentication();
            app.UseAuthorization();
           
            app.MapControllers();            

            app.Run();
        }
        //Method to seed admin user
        private static async Task SeedDefaultAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "normanmwagi23@gmail.com";
            string adminPassword = "MwagiMamba@123";
            string adminRole = "Admin";
            string userRole = "User";

            // Ensure the Admin role exists
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            if(!await roleManager.RoleExistsAsync(userRole))
            {
                await roleManager.CreateAsync(new IdentityRole(userRole));
            }

            // Check if the admin user exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    isAdmin = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createAdminResult = await userManager.CreateAsync(newAdmin, adminPassword);
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, adminRole);
                    Console.WriteLine("Default admin user created successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to create admin user:");
                    foreach (var error in createAdminResult.Errors)
                        Console.WriteLine($"- {error.Description}");
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists skipping creation.");
            }
        }
    }
}
