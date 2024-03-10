using BlazorClient.Components;
using MudBlazor;

namespace BlazorClient.Pages;

public sealed partial class ApiTest
{
    private IEnumerable<CallData> _calls = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        const string LoginCheckCall = "Login";
        const string AtLeastStudentCheckCall = "At Least Student";
        const string IsTeacherCheckCall = "Is Teacher";
        const string AnonymousCheckCall = "Anonymous";
        const string TokenDataCall = "Token Data";
        _calls =
        [
            new CallData(LoginCheckCall, async () =>
            {
                var result = await BackendService.CheckLoggedIn();
                result.Switch(success => ShowResult(LoginCheckCall, true, success.Value),
                              _ => ShowResult(LoginCheckCall, false, null));
            }),
            new CallData(AtLeastStudentCheckCall, async () =>
            {
                var result = await BackendService.CheckAtLeastStudent();
                result.Switch(success => ShowResult(AtLeastStudentCheckCall, true, success.Value),
                              _ => ShowResult(AtLeastStudentCheckCall, false, null));
            }),
            new CallData(IsTeacherCheckCall, async () =>
            {
                var result = await BackendService.CheckIsTeacher();
                result.Switch(success => ShowResult(IsTeacherCheckCall, true, success.Value),
                              _ => ShowResult(IsTeacherCheckCall, false, null));
            }),
            new CallData(AnonymousCheckCall, async () =>
            {
                var result = await BackendService.CheckAnonymous();
                result.Switch(success => ShowResult(AnonymousCheckCall, true, success.Value),
                              _ => ShowResult(AnonymousCheckCall, false, null));
            }),
            new CallData(TokenDataCall, async () =>
            {
                var result = await BackendService.GetTokenData();
                result.Switch(success =>
                              {
                                  var text = string.Join(Environment.NewLine, success.Value);
                                  ShowResult(TokenDataCall, true, text);
                              },
                              _ => ShowResult(TokenDataCall, false, null));
            })
        ];
    }

    private async void ShowResult(string call, bool success, string? responseMessage)
    {
        var result = success ? "Success" : "Failure";
        var parameters = new DialogParameters
        {
            [nameof(InformationDialog.Content)] = $"{result}! {responseMessage ?? "No response"}"
        };
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            DisableBackdropClick = true,
            Position = DialogPosition.TopCenter
        };
        
        var reference = await DialogService
            .ShowAsync<InformationDialog>(call, parameters, options);
        await reference.Result;
    }

    private readonly record struct CallData(string Call, Func<Task> Handler);
}
