using ExplodingKittens.Api.Controllers;
using Guts.Client.NUnit;

namespace ExplodingKittens.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "TablesIntegration",
    @"ExplodingKittens.Api\Controllers\TablesController.cs;ExplodingKittens.Api\Models\ModelMapper.cs")]
public class TablesControllerIntegrationTests : ControllerIntegrationTestsBase<TablesController>
{
    [MonitoredTest]
    public void HappyFlow_UserAStartsATableWith2Seats_UserBJoins_AGameIsStartedAutomatically()
    {
        StartANewGameForANewTable();
    }
}