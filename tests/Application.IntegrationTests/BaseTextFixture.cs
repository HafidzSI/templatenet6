using System.Threading.Tasks;
using NUnit.Framework;

namespace NetCa.Application.IntegrationTests;

using static Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetData();
    }
}