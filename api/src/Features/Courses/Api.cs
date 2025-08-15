namespace Api.Features.Courses;

public record class Api(string Url, string Description)
{
    public const string Tag = "Courses";
    public const string BaseUrlSingular = $"{ApiBase.Path}/course";
    public const string BaseUrlPlural = $"{ApiBase.Path}/courses";

    public static readonly Api Get = new(
        Url: $"{ApiBase.Path}/department/{{departmentId}}/courses",
        Description: "Gets all courses for the specified department"
    );

    public static readonly Api Add = new(
        Url: BaseUrlSingular,
        Description: "Adds a new course"
    );

    public static readonly Api Update = new(
        Url: $"{BaseUrlSingular}/{{courseId}}",
        Description: "Updates an existing course"
    );

    public static readonly Api Delete = new(
        Url: $"{BaseUrlSingular}/{{courseId}}",
        Description: "Deletes an existing course"
    );

    public static string GetUrl(long departmentId) => $"{ApiBase.Path}/department/{departmentId}/courses";
    public static string UpdateUrl(long courseId) => $"{BaseUrlSingular}/{courseId}";
    public static string DeleteUrl(long courseId) => $"{BaseUrlSingular}/{courseId}";
}
