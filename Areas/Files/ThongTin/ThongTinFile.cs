using System.Text;

namespace App.Areas.Files.ThongTin;

public static class ThongTinFile
{
    public static string[] FILE_EXTENSIONS = {".jpg", ".png", ".jpeg", ".gif", "svg"};

    public const int MAX_SIZE = 5000000;

    public static string LayDuoiFileChoPhep()
    {
        StringBuilder extensionStringBuilder = new StringBuilder();
        foreach (var extension in FILE_EXTENSIONS)
        {
            extensionStringBuilder.Append(extension + ", ");
        }

        return extensionStringBuilder.ToString();
    }

    public static class KieuTaiNguyen
    {
        public const string SAN_PHAM = "SP";
        public const string USER = "USER";
        public const string NHA_MAY = "NM";
        public const string DOANH_NGHIEP = "DN";
    }
    public static class KieuFile
    {
        public const string IMAGE = "image";
        public const string AVATAR = "avatar";
    }
}