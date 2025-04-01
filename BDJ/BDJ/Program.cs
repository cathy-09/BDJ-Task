using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDJ.Data;
using BDJ.Data.Models;
using BDJ.Services;
using BDJ.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BDJ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        var configuration = context.Configuration;

                        services.AddScoped<IUser, UserSer>();
                        services.AddScoped<ILine, LineSer>();
                        services.AddScoped<ITrain, TrainSer>();
                        services.AddScoped<IReservationService, ReservationService>();

                        services.AddControllersWithViews();

                        services.AddDbContext<BDJContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                        services.AddDefaultIdentity<User>(options =>
                        {
                            options.Password.RequireDigit = false;
                            options.Password.RequiredLength = 4;
                            options.Password.RequireLowercase = false;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequireUppercase = false;
                            options.SignIn.RequireConfirmedPhoneNumber = false;
                        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<BDJContext>();
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }

                        app.UseHttpsRedirection();
                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}");
                            endpoints.MapRazorPages();
                        });
                    });
                });
    }
}
