using System.Text;

namespace App.Helper;

public static class CreateCode
{
    public static string GenerateCodeFromTicks(string prefix = null)
    {
        string ticks = DateTime.UtcNow.Ticks.ToString();

        int lengthTick = ticks.Length;

        int startSubs = lengthTick - 14;

        string tickRandom = ticks.Substring(startSubs);

        var random = new Random();

        string code = $"{prefix}{tickRandom}{random.Next(10)}";

        return code;
    }
}

public static class PrefixCode
{
    public const string FACTORY = "NM";
    public const string SANPHAM = "SP"; 
}