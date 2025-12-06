using LightNap.Core.Streaming.Dto.Response;

namespace LightNap.Core.Streaming.Interfaces
{
    /// <summary>
    /// Service for managing streaming content (Live TV, VOD, Series, EPG).
    /// </summary>
    public interface IStreamingService
    {
        /// <summary>
        /// Gets live TV categories.
        /// </summary>
        Task<List<CategoryResponseDto>> GetLiveCategoriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets live TV streams, optionally filtered by category.
        /// </summary>
        Task<List<StreamResponseDto>> GetLiveStreamsAsync(string? categoryId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets VOD categories.
        /// </summary>
        Task<List<CategoryResponseDto>> GetVodCategoriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets VOD streams, optionally filtered by category.
        /// </summary>
        Task<List<VodStreamResponseDto>> GetVodStreamsAsync(string? categoryId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets VOD information by ID.
        /// </summary>
        Task<VodInfoResponseDto?> GetVodInfoAsync(string vodId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets series categories.
        /// </summary>
        Task<List<CategoryResponseDto>> GetSeriesCategoriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets series, optionally filtered by category.
        /// </summary>
        Task<List<SeriesResponseDto>> GetSeriesAsync(string? categoryId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets series information by ID.
        /// </summary>
        Task<SeriesInfoResponseDto?> GetSeriesInfoAsync(string seriesId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets EPG data, optionally filtered by category.
        /// </summary>
        Task<Dictionary<string, List<EpgResponseDto>>> GetEpgAsync(string? categoryId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets short EPG for a specific stream.
        /// </summary>
        Task<List<ShortEpgResponseDto>> GetShortEpgAsync(string streamId, int limit = 20, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets simple data table for EPG.
        /// </summary>
        Task<SimpleDataTableResponseDto> GetSimpleDataTableAsync(string streamId, CancellationToken cancellationToken = default);
    }
}

