using Microsoft.Extensions.Options;

namespace ApiRest.Config
{
    public class HealthcheckSettings : IOptions<HealthcheckSettings>
    {
        HealthcheckSettings IOptions<HealthcheckSettings>.Value => this;
    }
}
