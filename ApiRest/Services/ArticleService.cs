using ApiRest.Dto;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRest.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;


        public ArticleService(IHttpClientFactory httpClientFactory, JsonSerializerOptions options)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<UserDto> GetById(int id)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient("api"))
                {
                    var httpResponse = await client.GetAsync($"/todos/{id}");
                    
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception($"/todos/{id} - Status code: {httpResponse.StatusCode}");
                    }

                    string responseContent = await httpResponse.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<UserDto>(responseContent, _options);

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
