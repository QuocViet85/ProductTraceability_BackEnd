using System.Text;

namespace App.Areas.Files;

public static class FileInformation
{
    public static string[] FILE_EXTENSIONS = {".jpg", ".png", ".jpeg", ".gif", "svg"};

    public const int MAX_SIZE = 5000000;

    public static string GetFileExtensionsAllowed()
    {
        StringBuilder extensionStringBuilder = new StringBuilder();
        foreach (var extension in FILE_EXTENSIONS)
        {
            extensionStringBuilder.Append(extension + ", ");
        }

        return extensionStringBuilder.ToString();
    }

    public static class EntityType
    {
        public const string PRODUCT = "product";
        public const string USER = "user";
        public const string FACTORY = "factory";
        public const string ENTERPRISE = "enterprise";
        public const string INDIVIDUAL_ENTERPRISE = "individualEnterprise";
    }
    public static class FileType
    {
        public const string IMAGE = "image";
        public const string AVATAR = "avatar";
    }
}