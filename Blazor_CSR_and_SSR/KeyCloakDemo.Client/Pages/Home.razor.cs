using LeoAuth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using UserInfo = (string Name, string Value);

namespace KeyCloakDemo.Client.Pages;

public sealed partial class Home
{
    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;
            
            var userInformation = await AuthenticationStateProvider.GetLeoUserInformation();
            userInformation.Switch(user => _user = user,
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
    
    private void GoToLogin() => NavigationManager.NavigateTo("auth/login", forceLoad: true);
}
