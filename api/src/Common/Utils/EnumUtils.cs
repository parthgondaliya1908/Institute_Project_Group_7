namespace Api.Common.Utils;

public static class EnumUtils
{
    public static string CommaSeparated<TEnum>() where TEnum: struct, Enum
    {
        TEnum[] enumValues = Enum.GetValues<TEnum>();
        return string.Join(", ", enumValues);
    }
}
