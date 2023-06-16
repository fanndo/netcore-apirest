using System.Text.Json;

namespace ApiRest.Config
{
    public class JsonLowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLowerInvariant();
    }
}
