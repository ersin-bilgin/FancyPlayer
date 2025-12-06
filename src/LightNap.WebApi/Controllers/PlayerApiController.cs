using LightNap.Core.Api;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Models;
using LightNap.Core.Streaming.Dto.Response;
using LightNap.Core.Streaming.Interfaces;
using LightNap.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Xtream Codes API compatible gateway controller for IPTV players.
    /// This controller acts as a gateway, authenticating users with username/password
    /// and delegating to the modern StreamingService.
    /// </summary>
    [ApiController]
    [Route("api/player")]
    public class PlayerApiController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IStreamingService _streamingService;
        private readonly IIdentityService _identityService;
        private readonly ILogger<PlayerApiController> _logger;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        public PlayerApiController(
            ICacheService cacheService,
            IStreamingService streamingService,
            IIdentityService identityService,
            ILogger<PlayerApiController> logger)
        {
            _cacheService = cacheService;
            _streamingService = streamingService;
            _identityService = identityService;
            _logger = logger;
        }

        /// <summary>
        /// Get live TV categories.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), 200)]
        public async Task<IActionResult> GetAction(
            [FromQuery] string? username,
            [FromQuery] string? password,
            [FromQuery] string? action,
            [FromQuery] string? category_id,
            [FromQuery] string? vod_id,
            [FromQuery] string? series_id,
            [FromQuery] string? stream_id,
            [FromQuery] string? limit)
        {
            // Validate username/password for all requests
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Unauthorized(new { error = "Username and password are required" });
            }

            // Handle authentication endpoint (no action parameter)
            if (string.IsNullOrEmpty(action))
            {
                return await GetUserInfo(username, password);
            }

            // Authenticate user before processing any action
            var authResult = await AuthenticateUserAsync(username, password);
            if (!authResult.Success)
            {
                return Unauthorized(new { error = authResult.ErrorMessage });
            }

            switch (action?.ToLower())
            {
                case "get_live_categories":
                    return await GetLiveCategories();

                case "get_live_streams":
                    return await GetLiveStreams(category_id);

                case "get_vod_categories":
                    return await GetVodCategories();

                case "get_vod_streams":
                    return await GetVodStreams(category_id);

                case "get_vod_info":
                    return await GetVodInfo(vod_id);

                case "get_series_categories":
                    return await GetSeriesCategories();

                case "get_series":
                    return await GetSeries(category_id);

                case "get_series_info":
                    return await GetSeriesInfo(series_id);

                case "get_epg":
                    return await GetEpg(category_id);

                case "get_short_epg":
                    return await GetShortEpg(stream_id, limit);

                case "get_simple_data_table":
                    return await GetSimpleDataTable(stream_id);

                default:
                    return BadRequest(new { error = "Invalid action" });
            }
        }

        private async Task<IActionResult> GetUserInfo(string username, string password)
        {
            var cacheKey = $"xtream:user_info:{username}";
            
            var cached = await _cacheService.GetAsync<UserInfoDto>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Authenticate user using IdentityService
            try
            {
                var loginRequest = new LoginRequestDto
                {
                    Login = username,
                    Password = password,
                    RememberMe = false,
                    DeviceDetails = "Xtream API Gateway"
                };

                var loginResult = await _identityService.LogInAsync(loginRequest);
                
                // If login requires 2FA or email verification, return error
                if (loginResult.Type != LoginSuccessType.AccessToken)
                {
                    return Unauthorized(new { error = "Additional authentication required" });
                }

                var userInfo = new UserInfoDto
                {
                    Username = username,
                    Password = password,
                    Message = "User authenticated successfully",
                    Auth = 1,
                    Status = "Active",
                    ExpDate = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeSeconds().ToString(),
                    IsTrial = "0",
                    ActiveCons = "1",
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    MaxConnections = "2",
                    AllowedOutputFormats = new[] { "m3u8", "ts", "mp4" },
                    ServerUrl = Request.Scheme + "://" + Request.Host,
                    Port = "8080"
                };

                await _cacheService.SetAsync(cacheKey, userInfo, CacheExpiration);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Authentication failed for user: {Username}", username);
                return Unauthorized(new { error = "Invalid username or password" });
            }
        }

        private async Task<(bool Success, string? ErrorMessage)> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequestDto
                {
                    Login = username,
                    Password = password,
                    RememberMe = false,
                    DeviceDetails = "Xtream API Gateway"
                };

                var loginResult = await _identityService.LogInAsync(loginRequest);
                
                if (loginResult.Type != LoginSuccessType.AccessToken)
                {
                    return (false, "Additional authentication required");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Authentication failed for user: {Username}", username);
                return (false, "Invalid username or password");
            }
        }

        private async Task<IActionResult> GetLiveCategories()
        {
            var cacheKey = "xtream:live_categories";
            
            var cached = await _cacheService.GetAsync<List<CategoryDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var categoriesResponse = await _streamingService.GetLiveCategoriesAsync();
            var categories = categoriesResponse.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentId = c.ParentId
            }).ToList();

            await _cacheService.SetAsync(cacheKey, categories, CacheExpiration);
            return Ok(categories);
        }

        private async Task<IActionResult> GetLiveStreams(string? categoryId)
        {
            var cacheKey = $"xtream:live_streams:{categoryId ?? "all"}";
            
            var cached = await _cacheService.GetAsync<List<StreamDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var streamsResponse = await _streamingService.GetLiveStreamsAsync(categoryId);
            var streams = streamsResponse.Select((s, index) => new StreamDto
            {
                Num = index + 1,
                Name = s.Name,
                StreamType = s.StreamType,
                StreamId = s.StreamId,
                StreamIcon = $"{Request.Scheme}://{Request.Host}{s.StreamIcon}",
                EpgChannelId = s.EpgChannelId,
                Added = s.Added,
                CategoryId = s.CategoryId,
                CustomSid = s.CustomSid,
                TvArchive = s.TvArchive,
                DirectSource = s.DirectSource,
                TvArchiveDuration = s.TvArchiveDuration
            }).ToList();

            await _cacheService.SetAsync(cacheKey, streams, CacheExpiration);
            return Ok(streams);
        }

        private async Task<IActionResult> GetVodCategories()
        {
            var cacheKey = "xtream:vod_categories";
            
            var cached = await _cacheService.GetAsync<List<CategoryDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var categoriesResponse = await _streamingService.GetVodCategoriesAsync();
            var categories = categoriesResponse.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentId = c.ParentId
            }).ToList();

            await _cacheService.SetAsync(cacheKey, categories, CacheExpiration);
            return Ok(categories);
        }

        private async Task<IActionResult> GetVodStreams(string? categoryId)
        {
            var cacheKey = $"xtream:vod_streams:{categoryId ?? "all"}";
            
            var cached = await _cacheService.GetAsync<List<VodStreamDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var streamsResponse = await _streamingService.GetVodStreamsAsync(categoryId);
            var streams = streamsResponse.Select((s, index) => new VodStreamDto
            {
                Num = index + 1,
                Name = s.Name,
                StreamType = s.StreamType,
                StreamId = s.StreamId,
                StreamIcon = $"{Request.Scheme}://{Request.Host}{s.StreamIcon}",
                Rating = s.Rating,
                Rating5Based = s.Rating5Based,
                Added = s.Added,
                CategoryId = s.CategoryId,
                ContainerExtension = s.ContainerExtension,
                CustomSid = s.CustomSid,
                DirectSource = s.DirectSource
            }).ToList();

            await _cacheService.SetAsync(cacheKey, streams, CacheExpiration);
            return Ok(streams);
        }

        private async Task<IActionResult> GetVodInfo(string? vodId)
        {
            if (string.IsNullOrEmpty(vodId))
            {
                return BadRequest(new { error = "vod_id parameter is required" });
            }

            var cacheKey = $"xtream:vod_info:{vodId}";
            
            var cached = await _cacheService.GetAsync<VodInfoDto>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var vodInfoResponse = await _streamingService.GetVodInfoAsync(vodId);
            if (vodInfoResponse == null)
            {
                return NotFound(new { error = "VOD not found" });
            }

            var vodInfo = new VodInfoDto
            {
                Info = new VodInfoDetailDto
                {
                    Name = vodInfoResponse.Info.Name,
                    Cover = $"{Request.Scheme}://{Request.Host}{vodInfoResponse.Info.Cover}",
                    Plot = vodInfoResponse.Info.Plot,
                    Cast = vodInfoResponse.Info.Cast,
                    Director = vodInfoResponse.Info.Director,
                    Genre = vodInfoResponse.Info.Genre,
                    ReleaseDate = vodInfoResponse.Info.ReleaseDate,
                    Rating = vodInfoResponse.Info.Rating,
                    Rating5Based = vodInfoResponse.Info.Rating5Based,
                    Duration = vodInfoResponse.Info.Duration,
                    DurationSecs = vodInfoResponse.Info.DurationSecs,
                    StreamType = vodInfoResponse.Info.StreamType,
                    StreamId = vodInfoResponse.Info.StreamId,
                    ContainerExtension = vodInfoResponse.Info.ContainerExtension
                }
            };

            await _cacheService.SetAsync(cacheKey, vodInfo, CacheExpiration);
            return Ok(vodInfo);
        }

        private async Task<IActionResult> GetSeriesCategories()
        {
            var cacheKey = "xtream:series_categories";
            
            var cached = await _cacheService.GetAsync<List<CategoryDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var categoriesResponse = await _streamingService.GetSeriesCategoriesAsync();
            var categories = categoriesResponse.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentId = c.ParentId
            }).ToList();

            await _cacheService.SetAsync(cacheKey, categories, CacheExpiration);
            return Ok(categories);
        }

        private async Task<IActionResult> GetSeries(string? categoryId)
        {
            var cacheKey = $"xtream:series:{categoryId ?? "all"}";
            
            var cached = await _cacheService.GetAsync<List<SeriesDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var seriesResponse = await _streamingService.GetSeriesAsync(categoryId);
            var series = seriesResponse.Select((s, index) => new SeriesDto
            {
                Num = index + 1,
                Name = s.Name,
                SeriesId = s.SeriesId,
                Cover = $"{Request.Scheme}://{Request.Host}{s.Cover}",
                Plot = s.Plot,
                Cast = s.Cast,
                Director = s.Director,
                Genre = s.Genre,
                ReleaseDate = s.ReleaseDate,
                Rating = s.Rating,
                Rating5Based = s.Rating5Based,
                CategoryId = s.CategoryId
            }).ToList();

            await _cacheService.SetAsync(cacheKey, series, CacheExpiration);
            return Ok(series);
        }

        private async Task<IActionResult> GetSeriesInfo(string? seriesId)
        {
            if (string.IsNullOrEmpty(seriesId))
            {
                return BadRequest(new { error = "series_id parameter is required" });
            }

            var cacheKey = $"xtream:series_info:{seriesId}";
            
            var cached = await _cacheService.GetAsync<SeriesInfoDto>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var seriesInfoResponse = await _streamingService.GetSeriesInfoAsync(seriesId);
            if (seriesInfoResponse == null)
            {
                return NotFound(new { error = "Series not found" });
            }

            var seriesInfo = new SeriesInfoDto
            {
                Info = new SeriesInfoDetailDto
                {
                    Name = seriesInfoResponse.Info.Name,
                    Cover = $"{Request.Scheme}://{Request.Host}{seriesInfoResponse.Info.Cover}",
                    Plot = seriesInfoResponse.Info.Plot,
                    Cast = seriesInfoResponse.Info.Cast,
                    Director = seriesInfoResponse.Info.Director,
                    Genre = seriesInfoResponse.Info.Genre,
                    ReleaseDate = seriesInfoResponse.Info.ReleaseDate,
                    Rating = seriesInfoResponse.Info.Rating,
                    Rating5Based = seriesInfoResponse.Info.Rating5Based,
                    Duration = seriesInfoResponse.Info.Duration,
                    DurationSecs = seriesInfoResponse.Info.DurationSecs,
                    StreamType = seriesInfoResponse.Info.StreamType,
                    SeriesId = seriesInfoResponse.Info.SeriesId
                },
                Episodes = seriesInfoResponse.Episodes.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(e => new EpisodeDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        ContainerExtension = e.ContainerExtension,
                        Info = new EpisodeInfoDto
                        {
                            Plot = e.Info.Plot,
                            Duration = e.Info.Duration,
                            DurationSecs = e.Info.DurationSecs,
                            MovieImage = $"{Request.Scheme}://{Request.Host}{e.Info.MovieImage}",
                            Released = e.Info.Released
                        }
                    }).ToList()
                )
            };

            await _cacheService.SetAsync(cacheKey, seriesInfo, CacheExpiration);
            return Ok(seriesInfo);
        }

        private async Task<IActionResult> GetEpg(string? categoryId)
        {
            var cacheKey = $"xtream:epg:{categoryId ?? "all"}";
            
            var cached = await _cacheService.GetAsync<Dictionary<string, List<EpgDto>>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var epgResponse = await _streamingService.GetEpgAsync(categoryId);
            var epg = epgResponse.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(e => new EpgDto
                {
                    Title = e.Title,
                    Lang = e.Lang,
                    Start = e.Start,
                    End = e.End,
                    Description = e.Description,
                    ChannelId = e.ChannelId,
                    StartTimestamp = e.StartTimestamp,
                    StopTimestamp = e.StopTimestamp
                }).ToList()
            );

            await _cacheService.SetAsync(cacheKey, epg, CacheExpiration);
            return Ok(epg);
        }

        private async Task<IActionResult> GetShortEpg(string? streamId, string? limit)
        {
            if (string.IsNullOrEmpty(streamId))
            {
                return BadRequest(new { error = "stream_id parameter is required" });
            }

            var limitValue = int.TryParse(limit, out var l) ? l : 20;
            var cacheKey = $"xtream:short_epg:{streamId}:{limitValue}";
            
            var cached = await _cacheService.GetAsync<List<ShortEpgDto>>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var shortEpgResponse = await _streamingService.GetShortEpgAsync(streamId, limitValue);
            var shortEpg = shortEpgResponse.Select(se => new ShortEpgDto
            {
                Id = se.Id,
                EpgList = se.EpgList.Select(e => new EpgDto
                {
                    Title = e.Title,
                    Lang = e.Lang,
                    Start = e.Start,
                    End = e.End,
                    Description = e.Description,
                    ChannelId = e.ChannelId,
                    StartTimestamp = e.StartTimestamp,
                    StopTimestamp = e.StopTimestamp
                }).ToList()
            }).ToList();

            await _cacheService.SetAsync(cacheKey, shortEpg, CacheExpiration);
            return Ok(shortEpg);
        }

        private async Task<IActionResult> GetSimpleDataTable(string? streamId)
        {
            if (string.IsNullOrEmpty(streamId))
            {
                return BadRequest(new { error = "stream_id parameter is required" });
            }

            var cacheKey = $"xtream:simple_data_table:{streamId}";
            
            var cached = await _cacheService.GetAsync<SimpleDataTableDto>(cacheKey);
            if (cached != null)
            {
                return Ok(cached);
            }

            // Use StreamingService
            var dataTableResponse = await _streamingService.GetSimpleDataTableAsync(streamId);
            var dataTable = new SimpleDataTableDto
            {
                EpgListings = dataTableResponse.EpgListings.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(e => new EpgDto
                    {
                        Title = e.Title,
                        Lang = e.Lang,
                        Start = e.Start,
                        End = e.End,
                        Description = e.Description,
                        ChannelId = e.ChannelId,
                        StartTimestamp = e.StartTimestamp,
                        StopTimestamp = e.StopTimestamp
                    }).ToList()
                )
            };

            await _cacheService.SetAsync(cacheKey, dataTable, CacheExpiration);
            return Ok(dataTable);
        }
    }

    // DTOs for Xtream API responses
    public class CategoryDto
    {
        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = string.Empty;

        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }
    }

    public class StreamDto
    {
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; } = string.Empty;

        [JsonPropertyName("stream_id")]
        public int StreamId { get; set; }

        [JsonPropertyName("stream_icon")]
        public string StreamIcon { get; set; } = string.Empty;

        [JsonPropertyName("epg_channel_id")]
        public string EpgChannelId { get; set; } = string.Empty;

        [JsonPropertyName("added")]
        public string Added { get; set; } = string.Empty;

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonPropertyName("custom_sid")]
        public string CustomSid { get; set; } = string.Empty;

        [JsonPropertyName("tv_archive")]
        public int TvArchive { get; set; }

        [JsonPropertyName("direct_source")]
        public string DirectSource { get; set; } = string.Empty;

        [JsonPropertyName("tv_archive_duration")]
        public int TvArchiveDuration { get; set; }
    }

    public class VodStreamDto
    {
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; } = string.Empty;

        [JsonPropertyName("stream_id")]
        public int StreamId { get; set; }

        [JsonPropertyName("stream_icon")]
        public string StreamIcon { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("rating_5based")]
        public double Rating5Based { get; set; }

        [JsonPropertyName("added")]
        public string Added { get; set; } = string.Empty;

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonPropertyName("container_extension")]
        public string ContainerExtension { get; set; } = string.Empty;

        [JsonPropertyName("custom_sid")]
        public string CustomSid { get; set; } = string.Empty;

        [JsonPropertyName("direct_source")]
        public string DirectSource { get; set; } = string.Empty;
    }

    public class SeriesDto
    {
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("series_id")]
        public int SeriesId { get; set; }

        [JsonPropertyName("cover")]
        public string Cover { get; set; } = string.Empty;

        [JsonPropertyName("plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("cast")]
        public string Cast { get; set; } = string.Empty;

        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("rating_5based")]
        public double Rating5Based { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;
    }

    public class UserInfoDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("auth")]
        public int Auth { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("exp_date")]
        public string ExpDate { get; set; } = string.Empty;

        [JsonPropertyName("is_trial")]
        public string IsTrial { get; set; } = string.Empty;

        [JsonPropertyName("active_cons")]
        public string ActiveCons { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("max_connections")]
        public string MaxConnections { get; set; } = string.Empty;

        [JsonPropertyName("allowed_output_formats")]
        public string[] AllowedOutputFormats { get; set; } = Array.Empty<string>();

        [JsonPropertyName("server_url")]
        public string ServerUrl { get; set; } = string.Empty;

        [JsonPropertyName("port")]
        public string Port { get; set; } = string.Empty;
    }

    public class VodInfoDto
    {
        [JsonPropertyName("info")]
        public VodInfoDetailDto Info { get; set; } = new();
    }

    public class VodInfoDetailDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cover")]
        public string Cover { get; set; } = string.Empty;

        [JsonPropertyName("plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("cast")]
        public string Cast { get; set; } = string.Empty;

        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("rating_5based")]
        public double Rating5Based { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("duration_secs")]
        public int DurationSecs { get; set; }

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; } = string.Empty;

        [JsonPropertyName("stream_id")]
        public int StreamId { get; set; }

        [JsonPropertyName("container_extension")]
        public string ContainerExtension { get; set; } = string.Empty;
    }

    public class SeriesInfoDto
    {
        [JsonPropertyName("info")]
        public SeriesInfoDetailDto Info { get; set; } = new();

        [JsonPropertyName("episodes")]
        public Dictionary<string, List<EpisodeDto>> Episodes { get; set; } = new();
    }

    public class SeriesInfoDetailDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cover")]
        public string Cover { get; set; } = string.Empty;

        [JsonPropertyName("plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("cast")]
        public string Cast { get; set; } = string.Empty;

        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("rating_5based")]
        public double Rating5Based { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("duration_secs")]
        public int DurationSecs { get; set; }

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; } = string.Empty;

        [JsonPropertyName("series_id")]
        public int SeriesId { get; set; }
    }

    public class EpisodeDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("container_extension")]
        public string ContainerExtension { get; set; } = string.Empty;

        [JsonPropertyName("info")]
        public EpisodeInfoDto Info { get; set; } = new();
    }

    public class EpisodeInfoDto
    {
        [JsonPropertyName("plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("duration_secs")]
        public int DurationSecs { get; set; }

        [JsonPropertyName("movie_image")]
        public string MovieImage { get; set; } = string.Empty;

        [JsonPropertyName("released")]
        public string Released { get; set; } = string.Empty;
    }

    public class EpgDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("lang")]
        public string Lang { get; set; } = string.Empty;

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = string.Empty;

        [JsonPropertyName("start_timestamp")]
        public long StartTimestamp { get; set; }

        [JsonPropertyName("stop_timestamp")]
        public long StopTimestamp { get; set; }
    }

    public class ShortEpgDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("epg_listings")]
        public List<EpgDto> EpgList { get; set; } = new();
    }

    public class SimpleDataTableDto
    {
        [JsonPropertyName("epg_listings")]
        public Dictionary<string, List<EpgDto>> EpgListings { get; set; } = new();
    }
}

