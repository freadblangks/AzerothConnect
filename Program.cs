using AzerothConnect.Database.Contexts;
using AzerothConnect.Services;

using Microsoft.EntityFrameworkCore;

namespace AzerothConnect;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddServices(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    public static void AddServices(WebApplicationBuilder app)
    {
        app.Services.AddRazorPages();

        app.Services.AddDbContext<AuthDbContext>(db =>
        {
            var mysqlInfo = app.Configuration.GetConnectionString("Auth");
            if (string.IsNullOrEmpty(mysqlInfo))
            {
                throw new NullReferenceException("The MySQL connection string was null or empty.");
            }

            var mysqlVersion = ServerVersion.AutoDetect(mysqlInfo);
            db.UseMySql(mysqlInfo, mysqlVersion);
        });

        app.Services.AddScoped<AccountService>();
    }
}
