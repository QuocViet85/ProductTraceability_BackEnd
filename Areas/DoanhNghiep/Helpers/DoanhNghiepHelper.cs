using App.Areas.DoanhNghiep.Models;

namespace App.Areas.DoanhNghiep.Helpers;

public static class DoanhNghiepHelper
{
    public static bool LaChuDoanhNghiepDuyNhat(DoanhNghiepModel doanhNghiep, Guid userId)
    {
        if (doanhNghiep == null)
        {
            return false;
        }

        bool laChuDoanhNghiep = doanhNghiep.DN_List_CDN.Any(dn => dn.CDN_ChuDN_Id == userId);

        if (!laChuDoanhNghiep)
        {
            return false;
        }

        bool laChuDoanhNghiepDuyNhat = doanhNghiep.DN_List_CDN.Count == 1;

        if (!laChuDoanhNghiepDuyNhat)
        {
            return false;
        }

        return true;
    }
}