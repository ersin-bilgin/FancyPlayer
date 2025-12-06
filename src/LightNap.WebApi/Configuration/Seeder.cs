using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace LightNap.WebApi.Configuration
{
    /// <summary>
    /// Class responsible for seeding content in the application upon load.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Seeder"/> class.
    /// </remarks>
    /// <param name="serviceProvider">Service provider to pull dependencies from.</param>
    public partial class Seeder(IServiceProvider serviceProvider)
    {
        private readonly RoleManager<ApplicationRole> _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        private readonly ILogger<Seeder> _logger = serviceProvider.GetRequiredService<ILogger<Seeder>>();
        private readonly UserManager<ApplicationUser> _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        private readonly ApplicationDbContext _db = serviceProvider.GetRequiredService<ApplicationDbContext>();
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IOptions<Dictionary<string, List<SeededUserConfiguration>>> _seededUserConfigurations = serviceProvider.GetRequiredService<IOptions<Dictionary<string, List<SeededUserConfiguration>>>>();
        private readonly IOptions<ApplicationSettings> _applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>();

        /// <summary>
        /// Run seeding functionality necessary every time an application loads, regardless of environment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SeedAsync()
        {
            await this.SeedRolesAsync();
            await this.SeedUsersAsync();
            await this.SeedApplicationContentAsync();
            await this.SeedEnvironmentContentAsync();
        }

        /// <summary>
        /// Seeds the roles in the application.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedRolesAsync()
        {
            foreach (ApplicationRole role in ApplicationRoles.All)
            {
                if (!await this._roleManager.RoleExistsAsync(role.Name!))
                {
                    var result = await this._roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new ArgumentException($"Unable to create role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                    }
                    this._logger.LogInformation("Added role '{roleName}'", role.Name);
                }
            }

            var roleSet = new HashSet<string>(ApplicationRoles.All.Select(role => role.Name!), StringComparer.OrdinalIgnoreCase);

            foreach (var role in this._roleManager.Roles.Where(role => role.Name != null && !roleSet.Contains(role.Name)))
            {
                var result = await this._roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Unable to remove role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }
                this._logger.LogInformation("Removed role '{roleName}'", role.Name);
            }
        }

        /// <summary>
        /// Seeds the users in the application and adds them to their respective roles.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedUsersAsync()
        {
            if (this._seededUserConfigurations.Value is null) { return; }

            // Loop through the dictionary keys (roles) and add/get each user and add them to the role. Note that we sort the roles alphabetically,
            // so the "earliest" alphabetic instance of a new user will use that email/password.
            foreach (var roleToUsers in this._seededUserConfigurations.Value.OrderBy(roleToUser => roleToUser.Key)
                .Select(roleToUser => new { Role = roleToUser.Key, Users = roleToUser.Value }))
            {
                if (!string.IsNullOrWhiteSpace(roleToUsers.Role))
                {
                    if (!await this._roleManager.RoleExistsAsync(roleToUsers.Role)) { throw new ArgumentException($"Unable to find role '{roleToUsers.Role}' to seed users."); }
                }

                foreach (var seededUser in roleToUsers.Users)
                {
                    ApplicationUser user = await this.GetOrCreateUserAsync(seededUser.UserName, seededUser.Email, seededUser.Password);

                    if (!string.IsNullOrWhiteSpace(roleToUsers.Role))
                    {
                        await this.AddUserToRole(user, roleToUsers.Role);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new user in the application.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="email">The email address.</param>
        /// <param name="password">The password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task<ApplicationUser> GetOrCreateUserAsync(string userName, string email, string? password = null)
        {
            ApplicationUser? user = await this._userManager.FindByEmailAsync(email);

            if (user is null)
            {
                bool passwordProvided = !string.IsNullOrWhiteSpace(password);
                string passwordToSet = passwordProvided ? password! : $"P@ssw0rd{Guid.NewGuid()}";

                var registerRequestDto = new RegisterRequestDto()
                {
                    ConfirmPassword = passwordToSet,
                    DeviceDetails = "Seeder",
                    Email = email,
                    Password = passwordToSet,
                    UserName = userName
                };

                user = registerRequestDto.ToCreate(this._applicationSettings.Value.RequireTwoFactorForNewUsers);

                var result = await this._userManager.CreateAsync(user, passwordToSet);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Unable to create user '{userName}' ('{email}'): {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }

                this._logger.LogInformation("Created user '{userName}' ('{email}')", userName, email);
            }

            return user;
        }

        /// <summary>
        /// Adds a user to a specified role if they're not already in it.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The role to add the user to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddUserToRole(ApplicationUser user, string role)
        {
            if (!await this._userManager.IsInRoleAsync(user, role))
            {
                var result = await this._userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    throw new ArgumentException(
                        $"Unable to add user '{user.UserName}' ('{user.Email}') to role '{role}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }
            }

            this._logger.LogInformation("Added user '{userName}' ('{email}') to role '{roleName}'", user.UserName, user.Email, role);
        }

        /// <summary>
        /// Seeds content in the application. This method runs after baseline seeding (like roles and administrators) and provides an opportunity to
        /// seed any content required to be loaded regardless of environment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedApplicationContentAsync()
        {
            await SeedXtreamApiDataAsync();
        }

        /// <summary>
        /// Seeds Xtream API data (categories, streams, VOD, series, EPG).
        /// </summary>
        private async Task SeedXtreamApiDataAsync()
        {
            // Seed Live Categories
            if (!await _db.LiveCategories.AnyAsync())
            {
                var liveCategories = new[]
                {
                    new LiveCategory { Name = "TR: ULUSAL", SortOrder = 1 },
                    new LiveCategory { Name = "TR: SPOR", SortOrder = 2 },
                    new LiveCategory { Name = "TR: SiNEMA", SortOrder = 3 },
                    new LiveCategory { Name = "TR: BELGESEL", SortOrder = 4 },
                    new LiveCategory { Name = "TR: COCUK", SortOrder = 5 },
                    new LiveCategory { Name = "TR: MUZiK", SortOrder = 6 },
                    new LiveCategory { Name = "DE: ALMANYA", SortOrder = 7 },
                    new LiveCategory { Name = "EU: ADULT", SortOrder = 8 },
                    new LiveCategory { Name = "BG: BULGARİSTAN", SortOrder = 9 },
                    new LiveCategory { Name = "USA: AMERİKA", SortOrder = 10 }
                };
                await _db.LiveCategories.AddRangeAsync(liveCategories);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} live categories", liveCategories.Length);
            }

            // Seed VOD Categories
            if (!await _db.VodCategories.AnyAsync())
            {
                var vodCategories = new[]
                {
                    new VodCategory { Name = "Action", SortOrder = 1 },
                    new VodCategory { Name = "Comedy", SortOrder = 2 },
                    new VodCategory { Name = "Drama", SortOrder = 3 },
                    new VodCategory { Name = "Thriller", SortOrder = 4 },
                    new VodCategory { Name = "Horror", SortOrder = 5 }
                };
                await _db.VodCategories.AddRangeAsync(vodCategories);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} VOD categories", vodCategories.Length);
            }

            // Seed Series Categories
            if (!await _db.SeriesCategories.AnyAsync())
            {
                var seriesCategories = new[]
                {
                    new SeriesCategory { Name = "TV Series", SortOrder = 1 },
                    new SeriesCategory { Name = "Documentary Series", SortOrder = 2 }
                };
                await _db.SeriesCategories.AddRangeAsync(seriesCategories);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} series categories", seriesCategories.Length);
            }

            // Seed Live Streams
            if (!await _db.LiveStreams.AnyAsync())
            {
                var ulusalCategory = await _db.LiveCategories.FirstOrDefaultAsync(c => c.Name == "TR: ULUSAL");
                var sporCategory = await _db.LiveCategories.FirstOrDefaultAsync(c => c.Name == "TR: SPOR");

                var liveStreams = new List<LiveStream>();
                
                if (ulusalCategory != null)
                {
                    liveStreams.AddRange(new[]
                    {
                        new LiveStream
                        {
                            CategoryId = ulusalCategory.Id,
                            StreamName = "TR: beIN Sports 1",
                            StreamUrl = "https://example.com/stream/beinsports1.m3u8",
                            Logo = "/images/beinsports1.png",
                            IsActive = true,
                            SortOrder = 1
                        },
                        new LiveStream
                        {
                            CategoryId = ulusalCategory.Id,
                            StreamName = "TR: beIN Sports 1 HD",
                            StreamUrl = "https://example.com/stream/beinsports1hd.m3u8",
                            Logo = "/images/beinsports1hd.png",
                            IsActive = true,
                            SortOrder = 2
                        }
                    });
                }

                if (sporCategory != null)
                {
                    liveStreams.AddRange(new[]
                    {
                        new LiveStream
                        {
                            CategoryId = sporCategory.Id,
                            StreamName = "TR: TRT Spor",
                            StreamUrl = "https://example.com/stream/trtspor.m3u8",
                            Logo = "/images/trtspor.png",
                            IsActive = true,
                            SortOrder = 1
                        }
                    });
                }

                await _db.LiveStreams.AddRangeAsync(liveStreams);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} live streams", liveStreams.Count);

                // Seed EPG Channels for live streams
                var beinSports1 = await _db.LiveStreams.FirstOrDefaultAsync(ls => ls.StreamName == "TR: beIN Sports 1");
                if (beinSports1 != null)
                {
                    var epgChannel = new EpgChannel
                    {
                        ChannelId = beinSports1.Id,
                        EpgId = "beINSports1.tr",
                        DisplayName = "beIN Sports 1"
                    };
                    await _db.EpgChannels.AddAsync(epgChannel);
                    await _db.SaveChangesAsync();

                    // Seed EPG Programmes
                    var now = DateTime.UtcNow;
                    var programmes = new[]
                    {
                        new EpgProgramme
                        {
                            EpgChannelId = epgChannel.Id,
                            Title = "Football Match",
                            Description = "Live football match",
                            StartTime = now,
                            EndTime = now.AddHours(2)
                        },
                        new EpgProgramme
                        {
                            EpgChannelId = epgChannel.Id,
                            Title = "Sports News",
                            Description = "Latest sports news and updates",
                            StartTime = now.AddHours(2),
                            EndTime = now.AddHours(3)
                        }
                    };
                    await _db.EpgProgrammes.AddRangeAsync(programmes);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Seeded EPG channel and programmes for beIN Sports 1");
                }
            }

            // Seed VOD Movies
            if (!await _db.VodMovies.AnyAsync())
            {
                var actionCategory = await _db.VodCategories.FirstOrDefaultAsync(c => c.Name == "Action");
                var comedyCategory = await _db.VodCategories.FirstOrDefaultAsync(c => c.Name == "Comedy");

                var movies = new List<VodMovie>();

                if (actionCategory != null)
                {
                    movies.Add(new VodMovie
                    {
                        CategoryId = actionCategory.Id,
                        Title = "Sample Action Movie",
                        Description = "An exciting action-packed movie with thrilling sequences.",
                        CoverUrl = "/images/movie_action.jpg",
                        StreamUrl = "https://example.com/vod/action_movie.mp4",
                        Duration = 7200, // 120 minutes
                        ReleaseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        Rating = 8.5m,
                        IsActive = true
                    });
                }

                if (comedyCategory != null)
                {
                    movies.Add(new VodMovie
                    {
                        CategoryId = comedyCategory.Id,
                        Title = "Sample Comedy Movie",
                        Description = "A hilarious comedy that will make you laugh.",
                        CoverUrl = "/images/movie_comedy.jpg",
                        StreamUrl = "https://example.com/vod/comedy_movie.mp4",
                        Duration = 5400, // 90 minutes
                        ReleaseDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                        Rating = 7.8m,
                        IsActive = true
                    });
                }

                await _db.VodMovies.AddRangeAsync(movies);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} VOD movies", movies.Count);
            }

            // Seed Series
            if (!await _db.Series.AnyAsync())
            {
                var tvSeriesCategory = await _db.SeriesCategories.FirstOrDefaultAsync(c => c.Name == "TV Series");

                if (tvSeriesCategory != null)
                {
                    var series = new Series
                    {
                        CategoryId = tvSeriesCategory.Id,
                        Title = "Sample TV Series",
                        Description = "An engaging TV series with multiple seasons.",
                        CoverUrl = "/images/series_cover.jpg",
                        Rating = 9.0m
                    };
                    await _db.Series.AddAsync(series);
                    await _db.SaveChangesAsync();

                    // Seed Episodes
                    var episodes = new[]
                    {
                        new Episode
                        {
                            SeriesId = series.Id,
                            SeasonNumber = 1,
                            EpisodeNumber = 1,
                            Title = "Episode 1: Pilot",
                            Description = "The first episode of the series.",
                            StreamUrl = "https://example.com/series/s1e1.mp4",
                            Duration = 2700, // 45 minutes
                            ReleaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new Episode
                        {
                            SeriesId = series.Id,
                            SeasonNumber = 1,
                            EpisodeNumber = 2,
                            Title = "Episode 2: The Beginning",
                            Description = "The story continues.",
                            StreamUrl = "https://example.com/series/s1e2.mp4",
                            Duration = 2700,
                            ReleaseDate = new DateTime(2023, 1, 8, 0, 0, 0, DateTimeKind.Utc)
                        }
                    };
                    await _db.Episodes.AddRangeAsync(episodes);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Seeded series with {Count} episodes", episodes.Length);
                }
            }

            // Seed Xtream Users (test users)
            if (!await _db.XtreamUsers.AnyAsync())
            {
                var users = new[]
                {
                    new XtreamUser
                    {
                        Username = "test",
                        Password = "test", // In production, this should be hashed
                        FullName = "Test User",
                        Status = true,
                        MaxConnections = 2,
                        ExpireDate = DateTime.UtcNow.AddYears(1)
                    },
                    new XtreamUser
                    {
                        Username = "demo",
                        Password = "demo", // In production, this should be hashed
                        FullName = "Demo User",
                        Status = true,
                        MaxConnections = 1,
                        ExpireDate = DateTime.UtcNow.AddMonths(6)
                    }
                };
                await _db.XtreamUsers.AddRangeAsync(users);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} Xtream users", users.Length);
            }
        }

        /// <summary>
        /// Seeds content in the application based on the implementation of a SeedEnvironmentContent partial method in the class. To use this, add a Seeder 
        /// partial class (like Seeder.Development.cs) that implements the private method SeedEnvironmentContent(). It runs after SeedApplicationContentAsync()
        /// and is always executed on load if it exists.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SeedEnvironmentContentAsync()
        {
            this.SeedEnvironmentContent();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Optional partial to implement in a new class (like Seeder.Development.cs) to seed environment-specific content.
        /// </summary>
        partial void SeedEnvironmentContent();
    }
}
