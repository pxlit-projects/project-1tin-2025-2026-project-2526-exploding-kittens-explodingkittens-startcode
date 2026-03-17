using ExplodingKittens.Core.PlayerAggregate;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using Guts.Client.NUnit;

namespace ExplodingKittens.Core.Tests.PlayerAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "Hand",
    @"ExplodingKittens.Core\PlayerAggregate\Hand.cs")]
public class HandTests
{
    [MonitoredTest]
    public void Hand_Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(Hand).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Hand_Class_ShouldImplement_IHand()
    {
        Assert.That(typeof(Hand).IsAssignableTo(typeof(IHand)), Is.True);
    }

}