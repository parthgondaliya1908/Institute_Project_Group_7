// TODO: Remove if not needed
namespace Api.Common.Constants;

public static class FileTypes
{
    public static readonly string PdfFile = "pdf";

    public static string? AsMimeType(this string fileType)
    {
        if (fileType == PdfFile)
            return MimeTypes.PdfFile;

        return null;         
    }
}

public static class MimeTypes
{
    public static readonly string PdfFile = "application/pdf";
}
