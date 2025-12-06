using LightNap.Core.Api;
using LightNap.Core.Streaming.Dto.Response;
using LightNap.Core.Streaming.Interfaces;
using LightNap.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Modern API controller for streaming content (Live TV, VOD, Series, EPG).
    /// All endpoints require JWT token authentication.
    /// </summary>
    [ApiController]
    [Route("api/v1/streaming")]
    [Authorize]
    public class StreamingController : ControllerBase
    {
        private readonly IStreamingService _streamingService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<StreamingController> _logger;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        public StreamingController(
            IStreamingService streamingService,
            ICacheService cacheService,
            ILogger<StreamingController> logger)
        {
            _streamingService = streamingService;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Gets live TV categories.
        /// </summary>
        [HttpGet("live/categories")]
        [ProducesResponseType(typeof(ApiResponseDto<List<CategoryResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<CategoryResponseDto>>> GetLiveCategories(CancellationToken cancellationToken)
        {
            var cacheKey = "streaming:live_categories";
            var categories = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetLiveCategoriesAsync(cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<CategoryResponseDto>>(categories);
        }

        /// <summary>
        /// Gets live TV streams, optionally filtered by category.
        /// </summary>
        [HttpGet("live/streams")]
        [ProducesResponseType(typeof(ApiResponseDto<List<StreamResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<StreamResponseDto>>> GetLiveStreams(
            [FromQuery] string? categoryId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:live_streams:{categoryId ?? "all"}";
            var streams = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetLiveStreamsAsync(categoryId, cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<StreamResponseDto>>(streams);
        }

        /// <summary>
        /// Gets VOD categories.
        /// </summary>
        [HttpGet("vod/categories")]
        [ProducesResponseType(typeof(ApiResponseDto<List<CategoryResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<CategoryResponseDto>>> GetVodCategories(CancellationToken cancellationToken)
        {
            var cacheKey = "streaming:vod_categories";
            var categories = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetVodCategoriesAsync(cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<CategoryResponseDto>>(categories);
        }

        /// <summary>
        /// Gets VOD streams, optionally filtered by category.
        /// </summary>
        [HttpGet("vod/streams")]
        [ProducesResponseType(typeof(ApiResponseDto<List<VodStreamResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<VodStreamResponseDto>>> GetVodStreams(
            [FromQuery] string? categoryId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:vod_streams:{categoryId ?? "all"}";
            var streams = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetVodStreamsAsync(categoryId, cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<VodStreamResponseDto>>(streams);
        }

        /// <summary>
        /// Gets VOD information by ID.
        /// </summary>
        [HttpGet("vod/{vodId}")]
        [ProducesResponseType(typeof(ApiResponseDto<VodInfoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponseDto<VodInfoResponseDto>>> GetVodInfo(
            string vodId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:vod_info:{vodId}";
            var vodInfo = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetVodInfoAsync(vodId, cancellationToken),
                CacheExpiration,
                cancellationToken);

            if (vodInfo == null)
            {
                return NotFound(new ApiResponseDto<VodInfoResponseDto> { ErrorMessages = new[] { "VOD not found" }, Type = ApiResponseType.Error });
            }

            return new ApiResponseDto<VodInfoResponseDto>(vodInfo);
        }

        /// <summary>
        /// Gets series categories.
        /// </summary>
        [HttpGet("series/categories")]
        [ProducesResponseType(typeof(ApiResponseDto<List<CategoryResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<CategoryResponseDto>>> GetSeriesCategories(CancellationToken cancellationToken)
        {
            var cacheKey = "streaming:series_categories";
            var categories = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetSeriesCategoriesAsync(cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<CategoryResponseDto>>(categories);
        }

        /// <summary>
        /// Gets series, optionally filtered by category.
        /// </summary>
        [HttpGet("series")]
        [ProducesResponseType(typeof(ApiResponseDto<List<SeriesResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<SeriesResponseDto>>> GetSeries(
            [FromQuery] string? categoryId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:series:{categoryId ?? "all"}";
            var series = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetSeriesAsync(categoryId, cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<SeriesResponseDto>>(series);
        }

        /// <summary>
        /// Gets series information by ID.
        /// </summary>
        [HttpGet("series/{seriesId}")]
        [ProducesResponseType(typeof(ApiResponseDto<SeriesInfoResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponseDto<SeriesInfoResponseDto>>> GetSeriesInfo(
            string seriesId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:series_info:{seriesId}";
            var seriesInfo = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetSeriesInfoAsync(seriesId, cancellationToken),
                CacheExpiration,
                cancellationToken);

            if (seriesInfo == null)
            {
                return NotFound(new ApiResponseDto<SeriesInfoResponseDto> { ErrorMessages = new[] { "Series not found" }, Type = ApiResponseType.Error });
            }

            return new ApiResponseDto<SeriesInfoResponseDto>(seriesInfo);
        }

        /// <summary>
        /// Gets EPG data, optionally filtered by category.
        /// </summary>
        [HttpGet("epg")]
        [ProducesResponseType(typeof(ApiResponseDto<Dictionary<string, List<EpgResponseDto>>>), 200)]
        public async Task<ApiResponseDto<Dictionary<string, List<EpgResponseDto>>>> GetEpg(
            [FromQuery] string? categoryId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:epg:{categoryId ?? "all"}";
            var epg = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetEpgAsync(categoryId, cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<Dictionary<string, List<EpgResponseDto>>>(epg);
        }

        /// <summary>
        /// Gets short EPG for a specific stream.
        /// </summary>
        [HttpGet("epg/short/{streamId}")]
        [ProducesResponseType(typeof(ApiResponseDto<List<ShortEpgResponseDto>>), 200)]
        public async Task<ApiResponseDto<List<ShortEpgResponseDto>>> GetShortEpg(
            string streamId,
            CancellationToken cancellationToken,
            [FromQuery] int limit = 20)
        {
            var cacheKey = $"streaming:epg_short:{streamId}:{limit}";
            var shortEpg = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetShortEpgAsync(streamId, limit, cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<List<ShortEpgResponseDto>>(shortEpg);
        }

        /// <summary>
        /// Gets simple data table for EPG.
        /// </summary>
        [HttpGet("epg/table/{streamId}")]
        [ProducesResponseType(typeof(ApiResponseDto<SimpleDataTableResponseDto>), 200)]
        public async Task<ApiResponseDto<SimpleDataTableResponseDto>> GetSimpleDataTable(
            string streamId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"streaming:epg_table:{streamId}";
            var dataTable = await _cacheService.GetOrSetAsync(
                cacheKey,
                async () => await _streamingService.GetSimpleDataTableAsync(streamId, cancellationToken),
                CacheExpiration,
                cancellationToken);
            return new ApiResponseDto<SimpleDataTableResponseDto>(dataTable);
        }
    }
}

