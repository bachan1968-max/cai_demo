using Moq.Protected;
using Moq;
using System.Net;
using cai.Service.HttpClients;
using cai.Service.B2bInteraction;
using Microsoft.Extensions.Logging;
using cai.Domain;
using Microsoft.Extensions.Options;

namespace cai.Service.Test.B2bInteraction
{
    public class B2bRepositoryTest
    {
        private readonly HttpClientSettings _httpClientSettingsMoq;
        private readonly IOptionsMonitor<HttpClientSettings> _settingsParamMoq;

        public B2bRepositoryTest()
        {
            _httpClientSettingsMoq = new HttpClientSettings()
            {
                B2bApiUrl = "https://ff7ca14f-4595-4d54-8fc2-3008652beb22.mock.pstmn.io",
                Timeout = 1000
            };
            _settingsParamMoq = Mock.Of<IOptionsMonitor<HttpClientSettings>>(t => t.CurrentValue == _httpClientSettingsMoq);
        }

        [Fact]
        public async void GetFullStock_WhenReturnsBadJson_ShouldNull()
        {
            const string testContent = "test content"; //response Body
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testContent)
                });

            var httpClient = new HttpClient(mockMessageHandler.Object);
            var clientLoggerMock = Mock.Of<ILogger<B2bHttpClient>>();
            var b2bHttpClient = new B2bHttpClient(httpClient, _settingsParamMoq, clientLoggerMock);

            var loggerMock = Mock.Of<ILogger<B2bRepository>>();
            
            var b2bRepo = new B2bRepository(b2bHttpClient, loggerMock);
            var response = await b2bRepo.GetFullStock("test", "test", new CancellationToken());

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async void GetFullStock_WhenStatusNot200_ShouldNull()
        {
            const string testContent = "test content"; //response Body
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(testContent)
                });

            var httpClient = new HttpClient(mockMessageHandler.Object);
            var clientLoggerMock = Mock.Of<ILogger<B2bHttpClient>>();
            var b2bHttpClient = new B2bHttpClient(httpClient, _settingsParamMoq, clientLoggerMock);

            var loggerMock = Mock.Of<ILogger<B2bRepository>>();

            var b2bRepo = new B2bRepository(b2bHttpClient, loggerMock);
            var response = await b2bRepo.GetFullStock("test", "test", new CancellationToken());

            // Assert
            Assert.Null(response);
        }
    }
}
