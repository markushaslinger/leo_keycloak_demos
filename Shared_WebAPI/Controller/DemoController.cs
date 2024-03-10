using LeoAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemoApi.Controller;

[Authorize]
[ApiController]
[Route("api/demo")]
public sealed class DemoController : ControllerBase
{
    // Note: route names do not follow rest conventions - demo purposes only
    
    [HttpGet]
    // the Authorize attribute of the controller is applied
    [Route("at-least-logged-in")]
    public IActionResult GetIfAtLeastLoggedIn() => Ok("You are at least logged in");

    [HttpGet]
    // Note: test users are treated like students
    [Authorize(nameof(LeoUserRole.Student))]
    [Route("at-least-student")]
    public IActionResult GetIfAtLeastStudent() => Ok("You are at least a student");
    
    [HttpGet]
    [Authorize(nameof(LeoUserRole.Teacher))]
    [Route("is-teacher")]
    public IActionResult GetIfTeacher() => Ok("You are a teacher");
    
    [HttpGet]
    [AllowAnonymous]
    [Route("everyone-allowed")]
    public IActionResult GetInAnyCase() => Ok("Everyone is allowed to see this");
    
    [HttpGet]
    // the Authorize attribute of the controller is applied
    [Route("token-data")]
    public ActionResult<string[]> GetTokenData()
    {
        var userInfo = HttpContext.User.GetLeoUserInformation();

        return userInfo.Match<ActionResult<string[]>>(user => Ok(GetUserInfo(user)),
                                                      _ => NotFound());
    }

    private static string[] GetUserInfo(LeoUser user)
    {
        List<string> data = [];
        user.Username.Switch(username => data.Add(username),
                             _ => { });
        user.Name.Switch(fullName => data.Add($"{fullName.FirstName}, {fullName.LastName}"),
                        firstNameOnly => data.Add(firstNameOnly.FirstName),
                        lastNameOnly => data.Add(lastNameOnly.LastName),
                        _ => { });
        user.Department.Switch(department => data.Add(department.Name),
                              _ => { });
        if (user.IsStudent)
        {
            data.Add("Student");
        }
        else if (user.IsTeacher)
        {
            data.Add("Teacher");
        }
        else if (user.IsTestUser)
        {
            data.Add("Test User");
        }

        return data.ToArray();
    }
}
