using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ARModelGeneration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly FileStorageService _fileStorageService;

        public GenerationController(FileStorageService fileStorageService, IConfiguration configuration)
        {
            _httpClient = new();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(configuration.GetSection("MeshyToken").Value!);
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Generate result
        /// </summary>
        /// <remarks>
        /// Describe your desired art style of the object. 
        /// Default to "cartoon" if not specified.Available values:
        /// 
        /// realistic: Realistic style
        /// cartoon: Cartoon style
        /// low-poly: Low Poly style
        /// sculpture: Sculpture style
        /// pbr: PBR style
        /// </remarks>
        [HttpGet("text-2-result")]
        public async Task<string> Text2Result(string prompt, string artStyle = "realistic")
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    prompt,
                    mode = "preview",
                    art_style = artStyle,
                    negative_prompt = "low quality, low resolution, ugly",
                }),
                Encoding.UTF8,
                "application/json");
            var result = await _httpClient.PostAsync(string.Format("https://api.meshy.ai/v2/text-to-3d"), jsonContent);
            var responseString = await result.Content.ReadAsStringAsync();
            if (!responseString.Contains("result"))
            {
                return responseString;
            }
            string resultId = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString)!["result"];
            return resultId;
        }

        /// <summary>
        /// Generate model by resultId
        /// </summary>
        [HttpGet("result-2-model")]
        public async Task<string> Result2Model(string resultId)
        {
            var result = await _httpClient.GetAsync(string.Format($"https://api.meshy.ai/v2/text-to-3d/{resultId}"));
            var responseString = await result.Content.ReadAsStringAsync();
            return responseString;
        }

        /// <summary>
        /// Refine model by resultId
        /// </summary>
        [HttpGet("refine-model")]
        public async Task RefineModel(string resultId)
        {

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    mode = "refine",
                    preview_task_id = resultId,
                }),
                Encoding.UTF8,
                "application/json");
            var result = await _httpClient.PostAsync(string.Format("https://api.meshy.ai/v2/text-to-3d"), jsonContent);
        }
    }
}
