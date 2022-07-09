using ApiTest.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IDistributedCache _cache;

        public UserController(ILogger<UserController> logger, IDistributedCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] string userName)
        {
            try
            {
                string key = $"user:{userName}";

                var response = await _cache.GetAsync(key).ConfigureAwait(false);

                if (response == null)
                {
                    return NotFound($"User {userName} not found");
                }
                var serialized = Encoding.UTF8.GetString(response);
                var user = JsonSerializer.Deserialize<User>(serialized);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetUser: {ex}");
                throw;
            }
        }

        [HttpPost("SetUser")]
        public async Task<IActionResult> SetUser([FromBody] User user)
        {
            try
            {
                string key = $"user:{user.Name}";
                var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                var entry = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user, options));

                await _cache.SetAsync(key, entry, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMonths(6)
                });

                return Ok("Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in SetUser: {ex}");
                throw;
            }
        }
    }
}