using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Domain.Models;
using OnlineStore.Web;
using OnlineStore.Web.Data;
using System;
using System.IO;
using System.Linq;

namespace SeleniumTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly string _baseAddress = "https://localhost";
        private IWebHost _webHost;
        private Action<IWebHostBuilder> _addDevelopmentConfigs;
        private readonly bool _verbose;

        public CustomWebApplicationFactory(
            bool verbose = false,
            Action<IWebHostBuilder> addDevelopmentConfigurations = null)
        {
            _verbose = verbose;

            WriteToLog($"CustomWebApplicationFactory.ctor starting...");

            if (addDevelopmentConfigurations != null)
            {
                _addDevelopmentConfigs = addDevelopmentConfigurations;
            }

            ClientOptions.BaseAddress = new Uri(_baseAddress);

            WriteToLog($"CustomWebApplicationFactory.ctor calling CreateServer()...");

            CreateServer(CreateWebHostBuilder());

            WriteToLog($"CustomWebApplicationFactory.ctor exiting...");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.Configure(app =>
            {
                app.UseStaticFiles(); // Serve static files
            });

            // Configure and seed the database
            builder.ConfigureServices(services =>
            {
                // Remove existing database configuration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OnlineStoreContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<OnlineStoreContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Build service provider and seed database
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<OnlineStoreContext>();
                    dbContext.Database.EnsureCreated();
                    SeedDatabase(dbContext); // Ensure seeding happens
                }
            });
        }

        private void SeedDatabase(OnlineStoreContext dbContext)
        {
            Console.WriteLine("Seeding database...");
            if (!dbContext.Categories.Any(c => c.Name == "Test Category"))
            {
                dbContext.Categories.Add(new Category { Name = "Test Category" });
                dbContext.SaveChanges();
                Console.WriteLine("Database seeded with Test Category.");
            }
            else
            {
                Console.WriteLine("Test Category already exists.");
            }
        }


        private void WriteToLog(string message)
        {
            if (_verbose)
            {
                Console.WriteLine(message);
            }
        }

        public string GetServerAddress()
        {
            var serverAddresses = _webHost.ServerFeatures.Get<IServerAddressesFeature>();

            if (serverAddresses == null)
            {
                throw new InvalidOperationException($"Could not get instance of IServerAddressFeature.");
            }

            return serverAddresses.Addresses.FirstOrDefault();
        }

        public string GetServerAddressForRelativeUrl(string url)
        {
            var baseAddr = GetServerAddress();
            return $"{baseAddr}/{url}";
        }

        public TestServer TestServer { get; private set; }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            WriteToLog($"CustomWebApplicationFactory.CreateServer() starting...");

            _webHost = builder.Build();
            _webHost.Start();

            return InitializeTestServer();
        }

        private TestServer InitializeTestServer()
        {
            var builder = new WebHostBuilder().UseStartup<TStartup>();
            _addDevelopmentConfigs?.Invoke(builder);
            return new TestServer(builder);
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            WriteToLog($"CustomWebApplicationFactory.CreateWebHostBuilder() starting...");

            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "OnlineStore.Web"));
            builder.UseContentRoot(contentRoot);
            builder.UseStartup<TestStartup>();
            _addDevelopmentConfigs?.Invoke(builder);

            return builder;
        }

        protected IServiceScope _scope;
        protected IServiceScope Scope
        {
            get
            {
                if (_scope == null)
                {
                    var scopeFactory = TestServer.Services.GetRequiredService<IServiceScopeFactory>();
                    if (scopeFactory == null)
                    {
                        throw new InvalidOperationException("Could not create instance of IServiceScopeFactory.");
                    }
                    _scope = scopeFactory.CreateScope();
                }
                return _scope;
            }
        }

        public T CreateInstance<T>()
        {
            var provider = Scope.ServiceProvider;
            return provider.GetRequiredService<T>();
        }

        protected override void Dispose(bool disposing)
        {
            _addDevelopmentConfigs = null;

            _scope?.Dispose();
            _scope = null;

            _webHost?.Dispose();
            _webHost = null;

            base.Dispose(disposing);
        }
    }
}
