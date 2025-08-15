namespace Api.Features.Departments;

public record class Api(string Url, string Description)
{
    public const string Tag = "Departments";
    public const string BaseUrlSingular = $"{ApiBase.Path}/department";
    public const string BaseUrlPlural = $"{ApiBase.Path}/departments";

    public static readonly Api GetAll = new(
        Url: BaseUrlPlural,
        Description: "Gets all departments"
    );

    public static readonly Api GetById = new(
        Url: $"{BaseUrlSingular}/{{departmentId}}",
        Description: "Gets a department by ID"
    );

    public static readonly Api Add = new(
        Url: BaseUrlSingular,
        Description: "Adds a new department"
    );

    public static readonly Api Update = new(
        Url: $"{BaseUrlSingular}/{{departmentId}}",
        Description: "Updates a department by ID"
    );

    public static readonly Api Delete = new(
        Url: $"{BaseUrlSingular}/{{departmentId}}",
        Description: "Deletes a department by ID"
    );

    public static string GetByIdUrl(long departmentId) => $"{BaseUrlSingular}/{departmentId}";
    public static string UpdateUrl(long departmentId) => $"{BaseUrlSingular}/{departmentId}";
    public static string DeleteUrl(long departmentId) => $"{BaseUrlSingular}/{departmentId}";
}
