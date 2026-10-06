using InventoryAPIRest.Controllers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Controllers
{
    public class ControllerSecurityTests
    {
        [Theory]
        [InlineData(typeof(CategoriesController))]
        [InlineData(typeof(ProductsController))]
        [InlineData(typeof(InventoryMovementsController))]
        public void Controllers_RequireAuthorization(Type controllerType)
        {
            var attributes = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);

            Assert.NotEmpty(attributes);
        }
    }
}
