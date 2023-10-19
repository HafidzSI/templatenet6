using System.Data.Common;
using System.Threading.Tasks;
using NetCa.Application.Common.Models;

namespace NetCa.Application.IntegrationTests;

public interface ITestDatabase
{
    Task InitialiseAsync(AppSetting appSetting);

    DbConnection GetConnection();

    Task ResetAsync();

    Task DisposeAsync();
}