using Microsoft.AspNetCore.Http;
using Moq;
using OnlineShopWebApp.Tests.Helpers;
using System.Collections.Generic;
using System.Security.Claims;

namespace OnlineShopWebApp.Tests.TestModels
{
    public class HttpContextAccessorMock : Mock<IHttpContextAccessor>
    {
        public HttpContextAccessorMock(FakerProvider fakerProvider)
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, fakerProvider.UserId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Setup(_ => _.HttpContext.User).Returns(claimsPrincipal);
        }
    }
}
