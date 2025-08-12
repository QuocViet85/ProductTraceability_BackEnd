using Microsoft.AspNetCore.Authorization;

namespace App.Areas.NhaMay.Authorization;

public class ThemDoanhNghiepVaoNhaMayRequirement : IAuthorizationRequirement
{
    public Guid DN_Id { set; get; }
    public ThemDoanhNghiepVaoNhaMayRequirement(Guid dn_id)
    {
        DN_Id = dn_id;
    }
}