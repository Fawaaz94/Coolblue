using System.Net;
using Domain.Common;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Application.Unit.Tests.Mocks;

public static class MockHttpClient
{
      public static void SetupMockHttpClient(this Mock<IHttpClientFactory> mockHttpClientFactory, string baseAddress)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);

        mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(mockHttpClient);

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().EndsWith("/product_types")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new[]
                {
                    new { Id = 21, Name = CommonConstants.Laptops, CanBeInsured = true },
                    new { Id = 124, Name = CommonConstants.Smartphones, CanBeInsured = true },
                    new { Id = 35, Name = CommonConstants.DigitalCameras, CanBeInsured = true }
                }))
            });
        
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("/products/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new 
                {
                    Id = 837856, 
                    ProductTypeId = 21, 
                    SalesPrice = 300 
                }))
            });
    
        var client = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(baseAddress)
        };

        mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
    }
}