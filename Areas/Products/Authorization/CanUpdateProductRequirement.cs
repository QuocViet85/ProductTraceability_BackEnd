using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Products.Authorization;

public class CanUpdateProductRequirement : IAuthorizationRequirement
{
    public string TraceCode { set; get; }

    public CanUpdateProductRequirement(string traceCode)
    {
        TraceCode = traceCode;
    }
}