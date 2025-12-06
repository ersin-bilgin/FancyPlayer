using Microsoft.Extensions.Logging;

namespace LightNap.MaintenanceService
{
    /// <summary>
    /// Represents the main service that runs maintenance tasks.
    /// </summary>
    internal class MainService(ILogger<MainService> logger, IEnumerable<IMaintenanceTask> tasks)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tasks">The collection of maintenance tasks to run.</param>
        public async Task RunAsync()
        {
            logger.LogInformation("Starting maintenance run");

            foreach (var task in tasks)
            {
                logger.LogInformation("Starting '{task}'", task.Name);

                try
                {
                    await task.RunAsync();
                    logger.LogInformation("Completed '{task}'", task.Name);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error occurred during '{task}': {e}", task.Name, e);
                }
            }

            logger.LogInformation("Completed maintenance run");
        }
    }
}
