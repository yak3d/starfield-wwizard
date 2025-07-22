namespace StarfieldWwizard.Core.Helpers;

public static class StringExtensions
{
    public static string CaptureUntil(this string str, char c)
    {
        if (!String.IsNullOrEmpty(str))
        {
            int index = str.IndexOf(c);

            if (index > 0)
            {
                return str.Substring(0, index);
            }
        }

        return str;
    }
}