
using CommunityResourceSharing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace CommunityResourceSharing
{
    public class Program
    {
        public static void Main(string[] args)
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


            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<AppDbContext>();    
            builder.Services.AddOpenApi();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddSwaggerGen(); // 👈 Add this

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger(); // 👈 Enable Swagger middleware
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community Resource Sharing API V1");
                    c.RoutePrefix = string.Empty; // Launch swagger at root (http://localhost:5043)
                });

            }
            app.MapIdentityApi<IdentityUser>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
