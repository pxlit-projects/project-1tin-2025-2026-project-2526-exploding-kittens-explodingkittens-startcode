using ExplodingKittens.Core.PlayerAggregate;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.Tests.Extensions;
using Guts.Client.NUnit;

namespace ExplodingKittens.Core.Tests.PlayerAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "Player",
    @"ExplodingKittens.Core\PlayerAggregate\PlayerBase.cs;ExplodingKittens.Core\PlayerAggregate\HumanPlayer.cs")]
public class PlayerTests
{
    [MonitoredTest]
    public void HumanPlayer_Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(HumanPlayer).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void HumanPlayer_Class_ShouldInheritFromPlayerBase()
    {
        Assert.That(typeof(HumanPlayer).IsAssignableTo(typeof(PlayerBase)), Is.True);
    }

    [MonitoredTest]
    public void PlayerBase_Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(PlayerBase).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void PlayerBase_Class_ShouldImplement_IPlayer()
    {
        Assert.That(typeof(PlayerBase).IsAssignableTo(typeof(IPlayer)), Is.True);
    }

    [MonitoredTest]
    public void IPlayer_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IPlayer);
        type.AssertInterfaceProperty(nameof(IPlayer.Id), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.Name), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.BirthDate), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.Hand), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.Eliminated), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.FutureCards), shouldHaveGetter: true, shouldHaveSetter: true);
    }

    [MonitoredTest]
    public void PlayerBase_Constructor_ShouldInitializeProperties()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        string name = "John Doe";
        DateOnly birthDate = new DateOnly(2003, 1, 1);

        //Act
        IPlayer? testPlayer = new TestPlayer(userId, name, birthDate) as IPlayer;

        //Assert
        Assert.That(testPlayer, Is.Not.Null, "PlayerBase should implement IPlayer");
        Assert.That(testPlayer.Id, Is.EqualTo(userId), "'Id' is not set properly");
        Assert.That(testPlayer.Name, Is.EqualTo(name), "'Name' is not set properly");
        Assert.That(testPlayer.BirthDate, Is.EqualTo(birthDate), "'BirthDate' is not set properly");
        Assert.That(testPlayer.Hand, Is.Not.Null, "'Hand' is not set properly");
        Assert.That(testPlayer.FutureCards, Is.Not.Null, "'FutureCards' is not set properly");
    }

    private class TestPlayer(Guid id, string name, DateOnly birthDate)
        : PlayerBase(id, name, birthDate);
}