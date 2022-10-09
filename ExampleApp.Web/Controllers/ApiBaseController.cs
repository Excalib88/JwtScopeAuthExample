using ExampleApp.Web.Models.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Web.Controllers;

[ApiController]
public class ApiBaseController : ControllerBase
{
    protected UserInfo CurrentUser => new(HttpContext.User);
}