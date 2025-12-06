using LightNap.Core.Public.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LightNap.WebApi.Controllers
{
    /// <summary>
    /// Controller for handling publicly accessible data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PublicController(
        // Suppress CS9113 warning for unused parameter 'publicService'. Remove this if actually using the parameter.
#pragma warning disable CS9113
        IPublicService publicService
#pragma warning restore CS9113
        ) : ControllerBase
    {
        // Add Web API methods for data that is publicly accessible (user doesn't need to be logged in).
    }
}