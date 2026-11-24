using OllamaSharp;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public class OllamaService
    {
        private readonly OllamaApiClient _client;

        public OllamaService(IConfiguration configuration)
        {
            // Ollama server URL and model come from configuration ("Ollama:Url", "Ollama:Model").
            // Defaults target a local Ollama install. A hosted deployment (e.g. Azure App Service)
            // has no local Ollama, so point "Ollama:Url" at a reachable server or AI features will return errors.
            var url = configuration["Ollama:Url"] ?? "http://localhost:11434";
            var model = configuration["Ollama:Model"] ?? "llama3.2";
            _client = new OllamaApiClient(new Uri(url))
            {
                SelectedModel = model
            };
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                // Call the GenerateAsync API and stream responses
                var sb = new StringBuilder();
                await foreach (var stream in _client.GenerateAsync(prompt))
                {
                    if (!string.IsNullOrEmpty(stream.Response))
                        sb.Append(stream.Response); // Append each streamed response token
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateResponseAsync: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<IEnumerable<string>> ListModelsAsync()
        {
            try
            {
                // Get the list of local models and extract their names
                var models = await _client.ListLocalModelsAsync();
                return models.Select(model => model.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ListModelsAsync: {ex.Message}");
                return new List<string> { $"Error: {ex.Message}" };
            }
        }

        public async Task<bool> PullModelAsync(string modelName)
        {
            try
            {
                // Pull a model and report progress
                await foreach (var status in _client.PullModelAsync(modelName))
                {
                    Console.WriteLine($"{status.Percent}% {status.Status}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PullModelAsync: {ex.Message}");
                return false;
            }
        }
    }
}
