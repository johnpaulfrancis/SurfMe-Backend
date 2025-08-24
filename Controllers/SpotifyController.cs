using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SurfMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SpotifyController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        /// <summary>
        /// Search Spotify tracks
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("search-track")]
        public async Task<IActionResult> SearchSpotifyTracks([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required.");
            }
            var token = await GetSpotifyAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var url = $"https://api.spotify.com/v1/search?q={query}&limit=20&type=track&offset=0";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        /// <summary>
        /// To generate Spotify access token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetSpotifyAccessToken()
        {
            var clientId = _configuration["SpotifyAuth:ClientId"]; 
            var clientSecret = _configuration["SpotifyAuth:ClientSecret"];
            var tokeUri = "https://accounts.spotify.com/api/token";

            var byteArray = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };
            var response = await _httpClient.PostAsync(tokeUri, new FormUrlEncodedContent(requestBody));
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            return tokenData.GetProperty("access_token").GetString() ?? string.Empty;
        }
    }
}
