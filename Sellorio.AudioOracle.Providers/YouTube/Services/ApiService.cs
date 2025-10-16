using Sellorio.AudioOracle.Library.ApiTools;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class ApiService(HttpClient httpClient) : IApiService
{
    private const string _contextJson = "{\"context\":{\"client\":{\"hl\":\"en\",\"gl\":\"AU\",\"remoteHost\":\"1.43.18.152\",\"deviceMake\":\"\",\"deviceModel\":\"\",\"visitorData\":\"Cgt3TTQwaDYyeV9zbyj_7MXHBjIKCgJBVRIEGgAgXA%3D%3D\",\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:144.0) Gecko/20100101 Firefox/144.0,gzip(gfe)\",\"clientName\":\"WEB_REMIX\",\"clientVersion\":\"1.20251015.03.00\",\"osName\":\"Windows\",\"osVersion\":\"10.0\",\"originalUrl\":\"https://music.youtube.com/watch?v=0t2WobVXC74\",\"platform\":\"DESKTOP\",\"clientFormFactor\":\"UNKNOWN_FORM_FACTOR\",\"configInfo\":{\"appInstallData\":\"CP_sxccGENqF0BwQuPbPHBDni9AcENqK0BwQ0bGAExDnj9AcEMCP0BwQ3rzOHBDEgtAcEJmYsQUQ9quwBRC3yc8cEK7WzxwQyPfPHBDukdAcELfq_hIQi_fPHBCBlNAcELnZzhwQltvPHBCU8s8cENr3zhwQtquAExCqnc8cEL2KsAUQxcPPHBDM364FEJSD0BwQr5bQHBC85rAFEJT-sAUQlbGAExDiuLAFEPv_zxwQ8MzPHBDL688cELfkzxwQibDOHBCYuc8cELjkzhwQgffPHBCV988cEIeszhwQyfevBRDx6M8cEM-N0BwQvZmwBRDe6c8cEJ3QsAUQ8ZywBRCbiNAcEL22rgUQ0-GvBRCIh7AFEIWR0BwQmY2xBRDhjtAcEJjyzhwQndfPHBCM6c8cEIHNzhwQ_LLOHBDg6c8cELvZzhwQre_PHBCxsIATKkhDQU1TTUJVci1acS1ETWVVRXJjT3lvZnlDNnJJQlRLZ3JBUUR6ZjhGa0ZPSk5LSXVwR0tKTVp0SDZ5ZjNEN0toNFI0ZEJ3PT0wAA%3D%3D\",\"coldConfigData\":\"CP_sxccGGjJBT2pGb3gydFY1Skp5aHRNandWdnJkQUF6RVh4ZzE3NnV1d3RkMmtRV1lwd2pwbDNjdyIyQU9qRm94Mk1tWTdSR0lDUVNwNEstdGp3eGVGdkRMTjBJX2s4WmhkNGUxem54UlFmWlE%3D\",\"coldHashData\":\"CP_sxccGEhM4MzcyMjg4Nzg1MDY2MDg0NzkyGP_sxccGMjJBT2pGb3gydFY1Skp5aHRNandWdnJkQUF6RVh4ZzE3NnV1d3RkMmtRV1lwd2pwbDNjdzoyQU9qRm94Mk1tWTdSR0lDUVNwNEstdGp3eGVGdkRMTjBJX2s4WmhkNGUxem54UlFmWlE%3D\",\"hotHashData\":\"CP_sxccGEhQxMjg3MzUxNDc0ODI2OTU1ODg0NRj_7MXHBjIyQU9qRm94MnRWNUpKeWh0TWp3VnZyZEFBekVYeGcxNzZ1dXd0ZDJrUVdZcHdqcGwzY3c6MkFPakZveDJNbVk3UkdJQ1FTcDRLLXRqd3hlRnZETE4wSV9rOFpoZDRlMXpueFJRZlpR\"},\"userInterfaceTheme\":\"USER_INTERFACE_THEME_DARK\",\"timeZone\":\"Australia/Sydney\",\"browserName\":\"Firefox\",\"browserVersion\":\"144.0\",\"acceptHeader\":\"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\",\"deviceExperimentId\":\"ChxOelUyTVRrMU5UVXpPVEkwTVRjd01UUTJOZz09EP_sxccGGP_sxccG\",\"rolloutToken\":\"CKaH5NDp7qGlzQEQ-u6i5tT5iwMYg-jjq9qnkAM%3D\",\"screenWidthPoints\":2560,\"screenHeightPoints\":1355,\"screenPixelDensity\":1,\"screenDensityFloat\":1,\"utcOffsetMinutes\":660,\"musicAppInfo\":{\"pwaInstallabilityStatus\":\"PWA_INSTALLABILITY_STATUS_UNKNOWN\",\"webDisplayMode\":\"WEB_DISPLAY_MODE_BROWSER\",\"storeDigitalGoodsApiSupportStatus\":{\"playStoreDigitalGoodsApiSupportStatus\":\"DIGITAL_GOODS_API_SUPPORT_STATUS_UNSUPPORTED\"}}},\"user\":{\"lockedSafetyMode\":false},\"request\":{\"useSsl\":true,\"internalExperimentFlags\":[],\"consistencyTokenJars\":[],\"innertubeTokenJar\":{\"appTokens\":[{\"type\":2,\"value\":\"EicKI1FQX1NEa3d5XzE2c0QzRlc2cUJkRWhpRGtDc3hEeGpYdW1xEAA=\",\"maxAgeSeconds\":86400,\"creationTimeUsec\":\"1760654976173873\"}]}},\"clickTracking\":{\"clickTrackingParams\":\"CAoQ_V0YASITCNzIprDnqZADFSSySwUdST4QjMoBBDCu6Go=\"},\"adSignalsInfo\":{\"params\":[{\"key\":\"dt\",\"value\":\"1760654975557\"},{\"key\":\"flash\",\"value\":\"0\"},{\"key\":\"frm\",\"value\":\"0\"},{\"key\":\"u_tz\",\"value\":\"660\"},{\"key\":\"u_his\",\"value\":\"6\"},{\"key\":\"u_h\",\"value\":\"1440\"},{\"key\":\"u_w\",\"value\":\"2560\"},{\"key\":\"u_ah\",\"value\":\"1440\"},{\"key\":\"u_aw\",\"value\":\"2560\"},{\"key\":\"u_cd\",\"value\":\"24\"},{\"key\":\"bc\",\"value\":\"31\"},{\"key\":\"bih\",\"value\":\"1355\"},{\"key\":\"biw\",\"value\":\"2560\"},{\"key\":\"brdim\",\"value\":\"-8,-8,-8,-8,2560,0,2576,1456,2560,1355\"},{\"key\":\"vis\",\"value\":\"1\"},{\"key\":\"wgl\",\"value\":\"true\"},{\"key\":\"ca_type\",\"value\":\"image\"}]},\"activePlayers\":[{\"playerContextParams\":\"Q0FFU0FnZ0I=\"}]}}";

    public async Task<JsonNavigator> PostWithContextAsync(string url, object requestParameters)
    {
        var requestParametersJson = JsonSerializer.Serialize(requestParameters);
        var requestJson = _contextJson[..^1] + ',' + requestParametersJson[1..^1] + '}';
        using var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        HttpResponseMessage? responseMessage = null;
        await RateLimiters.YouTubeApi.WithRateLimit(async () => responseMessage = await httpClient.PostAsync(url.TrimStart('/'), stringContent));

        if (!responseMessage!.IsSuccessStatusCode)
        {
            var responseBody = await responseMessage.Content.ReadAsStringAsync();
            throw new InvalidOperationException("YouTube API request failed:\r\n" + responseBody);
        }

        var responseText = await responseMessage.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseText);
        return new(jsonDocument);
    }
}
