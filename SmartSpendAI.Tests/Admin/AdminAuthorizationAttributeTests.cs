using Microsoft.AspNetCore.Authorization;
using SmartSpendAI.Controllers;
using SmartSpendAI.Security;

namespace SmartSpendAI.Tests.Admin;

public sealed class AdminAuthorizationAttributeTests
{
    [Fact]
    public void AdminController_Uses_AdminOnlyPolicy()
    {
        var attribute = GetAuthorizeAttribute(typeof(AdminController));

        Assert.NotNull(attribute);
        Assert.Equal(AppPolicies.AdminOnly, attribute!.Policy);
    }

    [Fact]
    public void AdminCategoriesController_Uses_AdminOnlyPolicy()
    {
        var attribute = GetAuthorizeAttribute(typeof(AdminCategoriesController));

        Assert.NotNull(attribute);
        Assert.Equal(AppPolicies.AdminOnly, attribute!.Policy);
    }

    private static AuthorizeAttribute? GetAuthorizeAttribute(Type controllerType)
    {
        return controllerType
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();
    }
}
