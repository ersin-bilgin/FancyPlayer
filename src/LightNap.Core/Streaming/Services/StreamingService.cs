using LightNap.Core.Data;
using LightNap.Core.Streaming.Dto.Response;
using LightNap.Core.Streaming.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightNap.Core.Streaming.Services
{
    /// <summary>
    /// Service for managing streaming content (Live TV, VOD, Series, EPG).
    /// </summary>
    public class StreamingService : IStreamingService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<StreamingService> _logger;

        public StreamingService(ApplicationDbContext db, ILogger<StreamingService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<CategoryResponseDto>> GetLiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _db.LiveCategories
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.Id.ToString(),
                    CategoryName = c.Name,
                    ParentId = 0
                })
                .ToListAsync(cancellationToken);

            return categories;
        }

        public async Task<List<StreamResponseDto>> GetLiveStreamsAsync(string? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _db.LiveStreams
                .Where(ls => ls.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out var catId))
            {
                query = query.Where(ls => ls.CategoryId == catId);
            }

            var streams = await query
                .OrderBy(ls => ls.SortOrder)
                .ThenBy(ls => ls.StreamName)
                .Select(ls => new StreamResponseDto
                {
                    Num = 0, // Will be set by controller
                    Name = ls.StreamName,
                    StreamType = "live",
                    StreamId = ls.Id,
                    StreamIcon = ls.Logo ?? string.Empty,
                    EpgChannelId = ls.EpgChannel != null ? ls.EpgChannel.EpgId : string.Empty,
                    Added = "0",
                    CategoryId = ls.CategoryId.HasValue ? ls.CategoryId.Value.ToString() : string.Empty,
                    CustomSid = string.Empty,
                    TvArchive = 0,
                    DirectSource = string.Empty,
                    TvArchiveDuration = 0
                })
                .ToListAsync(cancellationToken);

            return streams;
        }

        public async Task<List<CategoryResponseDto>> GetVodCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _db.VodCategories
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.Id.ToString(),
                    CategoryName = c.Name,
                    ParentId = 0
                })
                .ToListAsync(cancellationToken);

            return categories;
        }

        public async Task<List<VodStreamResponseDto>> GetVodStreamsAsync(string? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _db.VodMovies
                .Where(vm => vm.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out var catId))
            {
                query = query.Where(vm => vm.CategoryId == catId);
            }

            var streams = await query
                .OrderBy(vm => vm.Title)
                .Select(vm => new VodStreamResponseDto
                {
                    Num = 0, // Will be set by controller
                    Name = vm.Title,
                    StreamType = "movie",
                    StreamId = vm.Id,
                    StreamIcon = vm.CoverUrl ?? string.Empty,
                    Rating = vm.Rating.HasValue ? vm.Rating.Value.ToString("F1") : "0",
                    Rating5Based = vm.Rating.HasValue ? (double)(vm.Rating.Value / 2.0m) : 0,
                    Added = "0",
                    CategoryId = vm.CategoryId.HasValue ? vm.CategoryId.Value.ToString() : string.Empty,
                    ContainerExtension = "mp4",
                    CustomSid = string.Empty,
                    DirectSource = string.Empty
                })
                .ToListAsync(cancellationToken);

            return streams;
        }

        public async Task<VodInfoResponseDto?> GetVodInfoAsync(string vodId, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(vodId, out var id))
            {
                return null;
            }

            var movie = await _db.VodMovies
                .FirstOrDefaultAsync(vm => vm.Id == id && vm.IsActive, cancellationToken);

            if (movie == null)
            {
                return null;
            }

            return new VodInfoResponseDto
            {
                Info = new VodInfoDetailDto
                {
                    Name = movie.Title,
                    Cover = movie.CoverUrl ?? string.Empty,
                    Plot = movie.Description ?? string.Empty,
                    Cast = string.Empty,
                    Director = string.Empty,
                    Genre = string.Empty,
                    ReleaseDate = movie.ReleaseDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                    Rating = movie.Rating.HasValue ? movie.Rating.Value.ToString("F1") : "0",
                    Rating5Based = movie.Rating.HasValue ? (double)(movie.Rating.Value / 2.0m) : 0,
                    Duration = movie.Duration.HasValue ? movie.Duration.Value.ToString() : "0",
                    DurationSecs = movie.Duration ?? 0,
                    StreamType = "movie",
                    StreamId = movie.Id,
                    ContainerExtension = "mp4"
                }
            };
        }

        public async Task<List<CategoryResponseDto>> GetSeriesCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _db.SeriesCategories
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.Id.ToString(),
                    CategoryName = c.Name,
                    ParentId = 0
                })
                .ToListAsync(cancellationToken);

            return categories;
        }

        public async Task<List<SeriesResponseDto>> GetSeriesAsync(string? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _db.Series.AsQueryable();

            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out var catId))
            {
                query = query.Where(s => s.CategoryId == catId);
            }

            var series = await query
                .OrderBy(s => s.Title)
                .Select(s => new SeriesResponseDto
                {
                    Num = 0, // Will be set by controller
                    Name = s.Title,
                    SeriesId = s.Id,
                    Cover = s.CoverUrl ?? string.Empty,
                    Plot = s.Description ?? string.Empty,
                    Cast = string.Empty,
                    Director = string.Empty,
                    Genre = string.Empty,
                    ReleaseDate = string.Empty,
                    Rating = s.Rating.HasValue ? s.Rating.Value.ToString("F1") : "0",
                    Rating5Based = s.Rating.HasValue ? (double)(s.Rating.Value / 2.0m) : 0,
                    CategoryId = s.CategoryId.HasValue ? s.CategoryId.Value.ToString() : string.Empty
                })
                .ToListAsync(cancellationToken);

            return series;
        }

        public async Task<SeriesInfoResponseDto?> GetSeriesInfoAsync(string seriesId, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(seriesId, out var id))
            {
                return null;
            }

            var series = await _db.Series
                .Include(s => s.Episodes.OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (series == null)
            {
                return null;
            }

            var episodesBySeason = series.Episodes
                .GroupBy(e => e.SeasonNumber)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Select(e => new EpisodeDto
                    {
                        Id = e.Id.ToString(),
                        Title = e.Title ?? $"Episode {e.EpisodeNumber}",
                        ContainerExtension = "mp4",
                        Info = new EpisodeInfoDto
                        {
                            Plot = e.Description ?? string.Empty,
                            Duration = e.Duration.HasValue ? e.Duration.Value.ToString() : "0",
                            DurationSecs = e.Duration ?? 0,
                            MovieImage = string.Empty,
                            Released = e.ReleaseDate?.ToString("yyyy-MM-dd") ?? string.Empty
                        }
                    }).ToList()
                );

            return new SeriesInfoResponseDto
            {
                Info = new SeriesInfoDetailDto
                {
                    Name = series.Title,
                    Cover = series.CoverUrl ?? string.Empty,
                    Plot = series.Description ?? string.Empty,
                    Cast = string.Empty,
                    Director = string.Empty,
                    Genre = string.Empty,
                    ReleaseDate = string.Empty,
                    Rating = series.Rating.HasValue ? series.Rating.Value.ToString("F1") : "0",
                    Rating5Based = series.Rating.HasValue ? (double)(series.Rating.Value / 2.0m) : 0,
                    Duration = "45",
                    DurationSecs = 2700,
                    StreamType = "series",
                    SeriesId = series.Id
                },
                Episodes = episodesBySeason
            };
        }

        public async Task<Dictionary<string, List<EpgResponseDto>>> GetEpgAsync(string? categoryId = null, CancellationToken cancellationToken = default)
        {
            var query = _db.EpgProgrammes
                .Include(ep => ep.EpgChannel)
                .ThenInclude(ec => ec.Channel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out var catId))
            {
                query = query.Where(ep => ep.EpgChannel.Channel.CategoryId == catId);
            }

            var programmes = await query
                .Where(ep => ep.EndTime >= DateTime.UtcNow)
                .OrderBy(ep => ep.StartTime)
                .ToListAsync(cancellationToken);

            var epgByChannel = programmes
                .GroupBy(ep => ep.EpgChannel.EpgId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ep => new EpgResponseDto
                    {
                        Title = ep.Title ?? string.Empty,
                        Lang = "tr",
                        Start = ep.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        End = ep.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Description = ep.Description ?? string.Empty,
                        ChannelId = ep.EpgChannel.EpgId,
                        StartTimestamp = ((DateTimeOffset)ep.StartTime).ToUnixTimeSeconds(),
                        StopTimestamp = ((DateTimeOffset)ep.EndTime).ToUnixTimeSeconds()
                    }).ToList()
                );

            return epgByChannel;
        }

        public async Task<List<ShortEpgResponseDto>> GetShortEpgAsync(string streamId, int limit = 20, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(streamId, out var id))
            {
                return new List<ShortEpgResponseDto>();
            }

            var epgChannel = await _db.EpgChannels
                .Include(ec => ec.Programmes)
                .FirstOrDefaultAsync(ec => ec.ChannelId == id, cancellationToken);

            if (epgChannel == null)
            {
                return new List<ShortEpgResponseDto>();
            }

            var programmes = epgChannel.Programmes
                .Where(ep => ep.EndTime >= DateTime.UtcNow)
                .OrderBy(ep => ep.StartTime)
                .Take(limit)
                .Select(ep => new EpgResponseDto
                {
                    Title = ep.Title ?? string.Empty,
                    Lang = "tr",
                    Start = ep.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = ep.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = ep.Description ?? string.Empty,
                    ChannelId = epgChannel.EpgId,
                    StartTimestamp = ((DateTimeOffset)ep.StartTime).ToUnixTimeSeconds(),
                    StopTimestamp = ((DateTimeOffset)ep.EndTime).ToUnixTimeSeconds()
                })
                .ToList();

            return new List<ShortEpgResponseDto>
            {
                new()
                {
                    Id = streamId,
                    EpgList = programmes
                }
            };
        }

        public async Task<SimpleDataTableResponseDto> GetSimpleDataTableAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(streamId, out var id))
            {
                return new SimpleDataTableResponseDto();
            }

            var epgChannel = await _db.EpgChannels
                .Include(ec => ec.Programmes)
                .FirstOrDefaultAsync(ec => ec.ChannelId == id, cancellationToken);

            if (epgChannel == null)
            {
                return new SimpleDataTableResponseDto();
            }

            var programmes = epgChannel.Programmes
                .Where(ep => ep.EndTime >= DateTime.UtcNow)
                .OrderBy(ep => ep.StartTime)
                .Select(ep => new EpgResponseDto
                {
                    Title = ep.Title ?? string.Empty,
                    Lang = "tr",
                    Start = ep.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    End = ep.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = ep.Description ?? string.Empty,
                    ChannelId = epgChannel.EpgId,
                    StartTimestamp = ((DateTimeOffset)ep.StartTime).ToUnixTimeSeconds(),
                    StopTimestamp = ((DateTimeOffset)ep.EndTime).ToUnixTimeSeconds()
                })
                .ToList();

            return new SimpleDataTableResponseDto
            {
                EpgListings = new Dictionary<string, List<EpgResponseDto>>
                {
                    { streamId, programmes }
                }
            };
        }
    }
}
