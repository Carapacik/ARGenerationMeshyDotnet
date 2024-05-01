using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ARModelGeneration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly FileStorageService _fileStorageService;

        public GenerationController(FileStorageService fileStorageService)
        {
            _httpClient = new();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("msy_WFLOivfdvY9LSjzGBJM2rrxOJZxQeidekatR");
            _fileStorageService = fileStorageService;
        }

        [HttpGet("text-2-result")]
        public async Task<string> TextTo3DResult(string generatePrompt)
        {
            using StringContent jsonContent = new(
                // realistic: Realistic style
                // cartoon: Cartoon style
                // low - poly: Low Poly style
                JsonSerializer.Serialize(new
                {
                    prompt = generatePrompt,
                    mode = "preview",
                    negative_prompt = "low quality, low resolution, low poly, ugly",
                    art_style = "low-poly"
                }),
                Encoding.UTF8,
                "application/json");
            var result = await _httpClient.PostAsync(string.Format("https://api.meshy.ai/v2/text-to-3d"), jsonContent);
            var responseString = await result.Content.ReadAsStringAsync();
            if (!responseString.Contains("result"))
            {
                return responseString;
            }
            string resultId = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString)["result"];
            return resultId;
        }

        [HttpGet("result-2-model")]
        public async Task<string> Result2Model(string resultId)
        {
            var result = await _httpClient.GetAsync(string.Format($"https://api.meshy.ai/v2/text-to-3d/{resultId}"));
            var responseString = await result.Content.ReadAsStringAsync();
            return responseString;
        }

        [HttpGet("temp-model/{filePath}")]
        public async Task<IActionResult> GetImage(string filePath)
        {
            if (filePath.Contains('\\'))
                return BadRequest();

            var result = await _fileStorageService.GetFile(filePath);
            return File(result.Content, "application/octet-stream", filePath);
        }
    }
}
