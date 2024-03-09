using LeoAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemoApi.Controller;

//[Authorize]
[ApiController]
[Route("api/demo")]
public sealed class DemoController : ControllerBase
{
    [HttpGet]
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
