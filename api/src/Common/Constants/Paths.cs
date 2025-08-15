namespace Api.Common.Constants;

public static class Folders
{
    public readonly record struct Uploads(string Name)
    {
        public static readonly Uploads Directory = new("uploads");
        public static readonly Uploads Services = new($"{Directory}/services");

        public override string ToString() => Name;
    }
}
