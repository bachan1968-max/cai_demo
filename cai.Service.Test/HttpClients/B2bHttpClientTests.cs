using cai.Domain;
using cai.Service.HttpClients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace cai.Service.Test.HttpClients
{
    public class B2bHttpClientTests
    {
        private readonly HttpClientSettings _httpClientSettingsMoq;
        private readonly IOptionsMonitor<HttpClientSettings> _settingsParamMoq;

        public B2bHttpClientTests()
        {
            _httpClientSettingsMoq = new HttpClientSettings()
            {
                B2bApiUrl = "https://ff7ca14f-4595-4d54-8fc2-3008652beb22.mock.pstmn.io",
                Timeout = 1000
            };
            _settingsParamMoq = Mock.Of<IOptionsMonitor<HttpClientSettings>>(t => t.CurrentValue == _httpClientSettingsMoq);
        }

        [Fact]
        public async Task GetFullStock_WhenRequestIsValid_ReturnsNotNull()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>();
            string jsonString = "{ \"Header\": { \"Code\": \"0\",\"Message\": \"Test\" }, \"Body\": { \"CategoryItem\": \"Test\"} }";
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(JObject.Parse(jsonString)))
                });
            var loggerMock = Mock.Of<ILogger<B2bHttpClient>>();
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock
                .Setup(e => e.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new B2bHttpClient(httpClient, _settingsParamMoq, loggerMock);
            var result = await service.GetFullStock("Test", "Test", new CancellationToken());
            Assert.NotNull(result);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                          && req.RequestUri == new Uri("https://ff7ca14f-4595-4d54-8fc2-3008652beb22.mock.pstmn.io/GetFullStock")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetFullStock_WhenCategoryItemIsNull_ReturnsNull()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });
            var loggerMock = Mock.Of<ILogger<B2bHttpClient>>();
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock
                .Setup(e => e.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new B2bHttpClient(httpClient, _settingsParamMoq, loggerMock);
            var result = await service.GetFullStock("Test", "Test", new CancellationToken());

            Assert.NotNull(result);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                          && req.RequestUri == new Uri("https://ff7ca14f-4595-4d54-8fc2-3008652beb22.mock.pstmn.io/GetFullStock")),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
