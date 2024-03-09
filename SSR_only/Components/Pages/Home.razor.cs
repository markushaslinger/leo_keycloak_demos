using LeoAuth;
using UserInfo = (string Name, string Value);

namespace LeoUserInfo.Components.Pages;

public sealed partial class Home
{
    protected override void OnInitialized()
    {
        try
        {
            _loading = true;
            var user = HttpContext.User;
            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                _isLoggedIn = false;

                return;
            }

            _isLoggedIn = true;
            var userInfo = user.GetLeoUserInformation();
            userInfo.Switch(leoUser => _user = leoUser,
                            _ => _noData = true);
        }
        finally
        {
            _loading = false;
        }
    }

    private static List<UserInfo> GetUserInfo(LeoUser user)
    {
        const string NameKey = "Name";
        const string TypeKey = "Type";

        List<UserInfo> data = [];
        user.Username.Switch(username => data.Add(("Username", username)),
                             _ => { });
        user.Name.Switch(fullName => data.Add((NameKey, $"{fullName.FirstName}, {fullName.LastName}")),
                         firstNameOnly => data.Add((NameKey, firstNameOnly.FirstName)),
                         lastNameOnly => data.Add((NameKey, lastNameOnly.LastName)),
                         _ => { });
        user.Department.Switch(department => data.Add(("Department", department.Name)),
                               _ => { });
        if (user.IsStudent)
        {
            data.Add((TypeKey, "Student"));
        }
        else if (user.IsTeacher)
        {
            data.Add((TypeKey, "Teacher"));
        }
        else if (user.IsTestUser)
        {
            data.Add((TypeKey, "Test User"));
        }

        return data;
    }
}
