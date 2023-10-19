using System.Threading.Tasks;
using NetCa.Application.Common.Models;

namespace NetCa.Application.IntegrationTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync(AppSetting appSetting)
    {
        var database = new Testing();

        await database.InitialiseAsync(appSetting);

        return database;
    }
}