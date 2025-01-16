using Microsoft.EntityFrameworkCore;
using OnlineStore.Web.Data;

namespace OnlineStore.Web
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("TestStartup: Configuring services...");
            services.AddControllersWithViews();
            services.AddDbContext<OnlineStoreContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles(); // Serve static files

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }



    }

}
