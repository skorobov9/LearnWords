using LearnForeignWords.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnForeignWords.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace LearnForeignWords
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
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddDbContext<WordTestContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<WordTestContext>();
            services.AddControllersWithViews().AddJsonOptions(o => {
                o.JsonSerializerOptions.ReferenceHandler  = ReferenceHandler.Preserve;
            }); ;
            services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = null;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {


                endpoints.MapControllerRoute(
   name: "AdditionallyCollections",
   pattern: "UserCollections/Create",
   defaults: new { controller = "UserCollections", action = "Create" });
                endpoints.MapControllerRoute(
   name: "AdditionallyCollections",
   pattern: "AdditionallyCollections/{id}",
   defaults: new { controller = "Test", action = "IndexAdditionally" });
                endpoints.MapControllerRoute(
  name: "AdditionallyCollections words",
  pattern: "AdditionallyCollections/{id}/Words",
  defaults: new { controller = "Test", action = "GetWords", local = "2" });
                endpoints.MapControllerRoute(
 name: "AdditionallyCollections cards",
 pattern: "AdditionallyCollections/{id}/Cards",
 defaults: new { controller = "Test", action = "GetCards", local = "2"});
                endpoints.MapControllerRoute(
name: "AdditionallyCollections test",
pattern: "AdditionallyCollections/{id}/Test/{type}",
defaults: new { controller = "Test", action = "Start", local = "2" });
                endpoints.MapControllerRoute(
      name: "collection all",
      pattern: "Themes/{id}/Collections/all",
      defaults: new { controller = "Test", action = "IndexAll" });   
                endpoints.MapControllerRoute(
   name: "collection all test",
   pattern: "Themes/{themeid}/Collections/all/Test/{type}",
   defaults: new { controller = "Test", action = "Start", local = "1", id = "0" });
                endpoints.MapControllerRoute(
    name: "collection all cards",
    pattern: "Themes/{themeid}/Collections/all/Cards",
    defaults: new { controller = "Test", action = "GetCards", local = "1", id = "0" });
                endpoints.MapControllerRoute(
      name: "collection all words",
      pattern: "Themes/{themeid}/Collections/all/Words",
      defaults: new { controller = "Test", action = "GetWords", local = "1", id = "0" });   
                endpoints.MapControllerRoute(
   name: "collection test",
   pattern: "Themes/{themeid}/Collections/{id}/Test/{type}",
   defaults: new { controller = "Test", action = "Start", local = "0"});          
                endpoints.MapControllerRoute(
   name: "collection cards",
   pattern: "Themes/{themeid}/Collections/{id}/Cards",
   defaults: new { controller = "Test", action = "GetCards", local = "0" });        
              
                endpoints.MapControllerRoute(
      name: "collection words",
      pattern: "Themes/{themeid}/Collections/{id}/Words",
      defaults: new { controller = "Test", action = "GetWords", local = "0" });
             

           endpoints.MapControllerRoute(
       name: "collection ",
       pattern: "Themes/{themeid}/Collections/{id}",
       defaults: new { controller = "Test", action = "Index" });   
               
                 endpoints.MapControllerRoute(
   name: "AdditionallyCollections",
   pattern: "UserCollections/{id}",
   defaults: new { controller = "Test", action = "Index", local="3" });

                endpoints.MapControllerRoute(
        name: "theme",
        pattern: "Themes",
		defaults: new { controller = "Themes", action = "Index"});
                endpoints.MapControllerRoute(
        name: "theme details",
        pattern: "Themes/{id?}",
        defaults: new { controller = "Themes", action = "Details" });
              
      endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<GameHub>("/testgame");
            });



        }
    }
}
