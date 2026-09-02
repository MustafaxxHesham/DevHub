using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
namespace Blog.Platform.EFCore.Interceptors;
public class PostInterceptor : DbCommandInterceptor
{
    public override async ValueTask<InterceptionResult<DbDataReader>>
        ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (command.CommandText.Trim().Equals(postReadingQuery))
        {
            Console.WriteLine(int.Parse(command.Parameters[0].Value.ToString())!);
            command.CommandText += "UPDATE Posts SET ViewsCount = ViewsCount + 1 WHERE Id = " + int.Parse(command.Parameters[0].Value.ToString())!;
        }
//        Console.WriteLine(command.CommandText);

        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    // WHEN query fails
    public override async Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("SQL ERROR:");
        Console.WriteLine(command.CommandText);

        Console.WriteLine(eventData.Exception.Message);

        await base.CommandFailedAsync(
            command,
            eventData,
            cancellationToken);
    }

    private string postReadingQuery = @"SELECT [s].[c], [s].[Title], [s].[Slug], [s].[PublishedAt], [s].[c0], [s].[ViewsCount], [s].[ProfileImageUrl], [s].[c1], [s].[Name], [s].[Id], [s].[Id0], [s].[Id1], [s0].[Name], [s0].[TagId], [s0].[PostId], [s0].[Id], [s].[MainImageUrl]
FROM (
    SELECT TOP(2) CONVERT(varchar(11), [p].[Id]) AS [c], [p].[Title], [p].[Slug], [p].[PublishedAt], COALESCE([u].[FirstName], N'') + N' ' + COALESCE([u].[LastName], N'') AS [c0], [p].[ViewsCount], [u].[ProfileImageUrl], (
        SELECT COUNT(*)
        FROM [Reactions] AS [r]
        WHERE [p].[Id] = [r].[PostId]) AS [c1], [c].[Name], [c].[Id], [p].[MainImageUrl], [p].[Id] AS [Id0], [u].[Id] AS [Id1]
    FROM [Posts] AS [p]
    LEFT JOIN [Users] AS [u] ON [p].[AuthorId] = [u].[Id]
    INNER JOIN [Categories] AS [c] ON [p].[CategoryId] = [c].[Id]
    WHERE [p].[Id] = @id
) AS [s]
LEFT JOIN (
    SELECT [t].[Name], [p0].[TagId], [p0].[PostId], [t].[Id]
    FROM [PostTag] AS [p0]
    INNER JOIN [Tags] AS [t] ON [p0].[TagId] = [t].[Id]
) AS [s0] ON [s].[Id0] = [s0].[PostId]
ORDER BY [s].[Id0], [s].[Id1], [s].[Id], [s0].[TagId], [s0].[PostId]";

    private string xx = @"SELECT [s].[c], [s].[Title], [s].[Slug], [s].[PublishedAt], [s].[c0], [s].[ViewsCount], [s].[ProfileImageUrl], [s].[c1], [s].[Name], [s].[Id], [s].[Id0], [s].[Id1], [s0].[Name], [s0].[TagId], [s0].[PostId], [s0].[Id], [s].[MainImageUrl]
      FROM (
          SELECT TOP(2) CONVERT(varchar(11), [p].[Id]) AS [c], [p].[Title], [p].[Slug], [p].[PublishedAt], COALESCE([u].[FirstName], N'') + N' ' + COALESCE([u].[LastName], N'') AS [c0], [p].[ViewsCount], [u].[ProfileImageUrl], (
              SELECT COUNT(*)
              FROM [Reactions] AS [r]
              WHERE [p].[Id] = [r].[PostId]) AS [c1], [c].[Name], [c].[Id], [p].[MainImageUrl], [p].[Id] AS [Id0], [u].[Id] AS [Id1]
          FROM [Posts] AS [p]
          LEFT JOIN [Users] AS [u] ON [p].[AuthorId] = [u].[Id]
          INNER JOIN [Categories] AS [c] ON [p].[CategoryId] = [c].[Id]
          WHERE [p].[Id] = @id
      ) AS [s]
      LEFT JOIN (
          SELECT [t].[Name], [p0].[TagId], [p0].[PostId], [t].[Id]
          FROM [PostTag] AS [p0]
          INNER JOIN [Tags] AS [t] ON [p0].[TagId] = [t].[Id]
      ) AS [s0] ON [s].[Id0] = [s0].[PostId]
      ORDER BY [s].[Id0], [s].[Id1], [s].[Id], [s0].[TagId], [s0].[PostId]";

}