using LightNap.Core.Data.Comparers;
using LightNap.Core.Data.Converters;
using LightNap.Core.Data.Entities;
using LightNap.Core.Profile.Dto.Response;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Data
{
    /// <summary>
    /// Represents the application database context.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        /// <summary>
        /// Notifications in the DB.
        /// </summary>
        public DbSet<Notification> Notifications { get; set; } = null!;

        /// <summary>
        /// Refresh tokens in the DB.
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        // Xtream API Entities
        public DbSet<XtreamUser> XtreamUsers { get; set; } = null!;
        public DbSet<Package> Packages { get; set; } = null!;
        public DbSet<UserPackage> UserPackages { get; set; } = null!;
        public DbSet<LiveCategory> LiveCategories { get; set; } = null!;
        public DbSet<VodCategory> VodCategories { get; set; } = null!;
        public DbSet<SeriesCategory> SeriesCategories { get; set; } = null!;
        public DbSet<LiveStream> LiveStreams { get; set; } = null!;
        public DbSet<VodMovie> VodMovies { get; set; } = null!;
        public DbSet<Series> Series { get; set; } = null!;
        public DbSet<Episode> Episodes { get; set; } = null!;
        public DbSet<EpgChannel> EpgChannels { get; set; } = null!;
        public DbSet<EpgProgramme> EpgProgrammes { get; set; } = null!;
        public DbSet<UserConnection> UserConnections { get; set; } = null!;
        public DbSet<StreamLog> StreamLogs { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The DbContext options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        public ApplicationDbContext() { }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Notification>()
                .Property(n => n.Data)
                    .HasConversion(new DictionaryStringObjectConverter())
                    .Metadata.SetValueComparer(new DictionaryStringObjectValueComparer());

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .IsRequired();

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .Property(u => u.BrowserSettings)
                .Metadata.SetValueComparer(new BrowserSettingsValueComparer());

            // Xtream API Entity Configurations
            ConfigureXtreamEntities(builder);
        }

        private void ConfigureXtreamEntities(ModelBuilder builder)
        {
            // XtreamUser
            builder.Entity<XtreamUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // UserPackage - Many-to-Many relationship
            builder.Entity<UserPackage>()
                .HasKey(up => up.Id);
            builder.Entity<UserPackage>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPackages)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserPackage>()
                .HasOne(up => up.Package)
                .WithMany(p => p.UserPackages)
                .HasForeignKey(up => up.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // LiveCategory -> LiveStream
            builder.Entity<LiveStream>()
                .HasOne(ls => ls.Category)
                .WithMany(lc => lc.Streams)
                .HasForeignKey(ls => ls.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // VodCategory -> VodMovie
            builder.Entity<VodMovie>()
                .HasOne(vm => vm.Category)
                .WithMany(vc => vc.Movies)
                .HasForeignKey(vm => vm.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // SeriesCategory -> Series
            builder.Entity<Series>()
                .HasOne(s => s.Category)
                .WithMany(sc => sc.Series)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Series -> Episode
            builder.Entity<Episode>()
                .HasOne(e => e.Series)
                .WithMany(s => s.Episodes)
                .HasForeignKey(e => e.SeriesId)
                .OnDelete(DeleteBehavior.Cascade);

            // LiveStream -> EpgChannel (One-to-One)
            builder.Entity<EpgChannel>()
                .HasOne(ec => ec.Channel)
                .WithOne(ls => ls.EpgChannel)
                .HasForeignKey<EpgChannel>(ec => ec.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<EpgChannel>()
                .HasIndex(ec => ec.EpgId)
                .IsUnique();

            // EpgChannel -> EpgProgramme
            builder.Entity<EpgProgramme>()
                .HasOne(ep => ep.EpgChannel)
                .WithMany(ec => ec.Programmes)
                .HasForeignKey(ep => ep.EpgChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            // XtreamUser -> UserConnection
            builder.Entity<UserConnection>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Connections)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // XtreamUser -> StreamLog
            builder.Entity<StreamLog>()
                .HasOne(sl => sl.User)
                .WithMany(u => u.StreamLogs)
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        /// <inheritdoc />
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Make sure all DateTime properties are stored as UTC.
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcValueConverter>();

            // Storing this as a JSON string.
            configurationBuilder.Properties<BrowserSettingsDto>()
                .HaveConversion<BrowserSettingsValueConverter>();
        }
    }
}
