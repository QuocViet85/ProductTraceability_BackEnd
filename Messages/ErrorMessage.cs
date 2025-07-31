using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Messages;

public static class ErrorMessage
{
    public static Dictionary<string, List<string>> DTO(ModelStateDictionary modelState)
    {
        var errors = new Dictionary<string, List<string>>();

        foreach (var key in modelState.Keys)
        {
            var vals = modelState[key];

            if (vals != null)
            {
                var listErr = new List<string>();
                foreach (var error in vals.Errors)
                {
                    var errorString = error.ToString();

                    if (!string.IsNullOrEmpty(errorString))
                    {
                        Console.WriteLine($"{key}: {errorString}");
                        listErr.Add(errorString);
                    }
                }
                errors.Add(key, listErr);
            }
        }
        return errors;
    }

    public static string AuthFailReason(IEnumerable<AuthorizationFailureReason> authFails)
    {
        var authFailStringBuilder = new StringBuilder();
        foreach (var authFail in authFails)
        {
            authFailStringBuilder.Append(authFail + "\n");
        }

        return authFailStringBuilder.ToString();   
    }

    public const string Required = "Vui lòng nhập {0}";
    public const string PhoneFormat = "Số điện thoại không đúng định dạng";
}