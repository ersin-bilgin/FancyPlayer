using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Public.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.Core.Tests.Services
{
    [TestClass]
    public class PublicServiceTests
    {
        // These will be initialized during TestInitialize.
#pragma warning disable CS8618
        private ApplicationDbContext _dbContext;
        // Remove when using this member.
#pragma warning disable IDE0052
        private PublicService _publicService;
#pragma warning restore IDE0052
#pragma warning restore CS8618

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddLightNapInMemoryDatabase()
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var serviceProvider = services.BuildServiceProvider();
            _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            _publicService = new PublicService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

    }
}
