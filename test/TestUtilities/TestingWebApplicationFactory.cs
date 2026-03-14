using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RentingPrototype.TestUtilities;

public class TestingWebApplicationFactory : WebApplicationFactory<Program>
{
    private bool _contentRootDeleted;

    public TestingWebApplicationFactory()
    {
        ContentRootPath = CreateTestingContentRoot();
    }

    public string ContentRootPath { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseContentRoot(ContentRootPath);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            DeleteContentRoot();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        DeleteContentRoot();
    }

    private void DeleteContentRoot()
    {
        if (_contentRootDeleted || !Directory.Exists(ContentRootPath))
        {
            return;
        }

        Directory.Delete(ContentRootPath, recursive: true);
        _contentRootDeleted = true;
    }

    private static string CreateTestingContentRoot()
    {
        var contentRootPath = Path.Combine(
            Path.GetTempPath(),
            $"rentingprototype-tests-{Guid.NewGuid():N}");

        var schemaDirectory = Path.Combine(contentRootPath, "data", "schema");
        Directory.CreateDirectory(schemaDirectory);

        var schemaTargetPath = Path.Combine(schemaDirectory, "rentingprototype-schema.sql");
        File.Copy(FindSchemaSourcePath(), schemaTargetPath);

        return contentRootPath;
    }

    private static string FindSchemaSourcePath()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null)
        {
            var solutionPath = Path.Combine(currentDirectory.FullName, "RentingPrototype.sln");
            if (File.Exists(solutionPath))
            {
                var schemaPath = Path.Combine(
                    currentDirectory.FullName,
                    "src",
                    "RentingPrototype.Infrastructure",
                    "Persistence",
                    "Schema",
                    "rentingprototype-schema.sql");

                if (File.Exists(schemaPath))
                {
                    return schemaPath;
                }
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new FileNotFoundException("Unable to locate rentingprototype-schema.sql for test host setup.");
    }
}
