namespace Api.Common.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder ProducesProblems(this RouteHandlerBuilder builder, params int[] errorCodes)
    {
        foreach (int errorCode in errorCodes)
        {
            builder.ProducesProblem(errorCode);
        }

        return builder;
    }
}
