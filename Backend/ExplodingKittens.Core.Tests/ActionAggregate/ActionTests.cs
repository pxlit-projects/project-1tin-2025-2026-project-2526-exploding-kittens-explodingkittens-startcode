using System.Reflection;
using ExplodingKittens.Core.ActionAggregate;
using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.Tests.Extensions;
using Guts.Client.NUnit;

namespace ExplodingKittens.Core.Tests.ActionAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "Action",
    @"ExplodingKittens.Core\ActionAggregate\ActionBase.cs;ExplodingKittens.Core\ActionAggregate\AttackAction.cs;ExplodingKittens.Core\ActionAggregate\DefuseAction.cs;ExplodingKittens.Core\ActionAggregate\FavorAction.cs;ExplodingKittens.Core\ActionAggregate\SkipAction.cs;ExplodingKittens.Core\ActionAggregate\ShuffleAction.cs;ExplodingKittens.Core\ActionAggregate\SeeTheFutureAction.cs;ExplodingKittens.Core\ActionAggregate\StealRandomCardAction.cs;ExplodingKittens.Core\ActionAggregate\StealSpecificCardAction.cs")]
public class ActionTests
{
    [MonitoredTest]
    public void AllActionClasses_ShouldBeInternal_SoThatTheyCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(AttackAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'AttackAction'");
        Assert.That(typeof(DefuseAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'DefuseAction'");
        Assert.That(typeof(FavorAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'FavorAction'");
        Assert.That(typeof(SkipAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'SkipAction'");
        Assert.That(typeof(ShuffleAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'ShuffleAction'");
        Assert.That(typeof(SeeTheFutureAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'SeeTheFutureAction'");
        Assert.That(typeof(StealRandomCardAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'StealRandomCardAction'");
        Assert.That(typeof(StealSpecificCardAction).IsNotPublic, Is.True, "use 'internal class' instead of 'public class' for 'StealSpecificCardAction'");
    }

    [MonitoredTest]
    public void AbstractActionBaseClass_ShouldImplement_IAction()
    {
        Assert.That(typeof(ActionBase).IsAssignableTo(typeof(IAction)), Is.True, "use 'IAction' interface for 'ActionBase'");
    }

    [MonitoredTest]
    public void AbstractActionBaseClass_ShouldHaveProtectedAbstractExecuteMethod()
    {
        MethodInfo? executeMethod = typeof(ActionBase).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(m => m is { Name: "Execute", IsAbstract: true, IsFamily: true });
        Assert.That(executeMethod, Is.Not.Null, "ActionBase should have a protected abstract Execute method");
    }

    [MonitoredTest]
    public void IAction_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IAction);

        type.AssertInterfaceProperty(nameof(IAction.PlayerId), true, false);
        type.AssertInterfaceProperty(nameof(IAction.Cards), true, false);
        type.AssertInterfaceProperty(nameof(IAction.CanBeNoped), true, false);
        type.AssertInterfaceProperty(nameof(IAction.TargetPlayerId), true, false);
        type.AssertInterfaceProperty(nameof(IAction.TargetCard), true, true);
        type.AssertInterfaceProperty(nameof(IAction.DrawPileIndex), true, false);
        type.AssertInterfaceProperty(nameof(IAction.PlayerNopeDecisions), true, false);
        type.AssertInterfaceProperty(nameof(IAction.IsNoped), true, false);
        type.AssertInterfaceProperty(nameof(IAction.IsExecuted), true, false);

        type.AssertInterfaceMethod(nameof(IAction.Nope), typeof(void), typeof(Guid));
        type.AssertInterfaceMethod(nameof(IAction.ConfirmNotNoping), typeof(void), typeof(Guid));
    }

    [MonitoredTest]
    public void AllActionClasses_ShouldDeriveFromTheAbstractActionBaseClass()
    {
        Assert.That(typeof(ActionBase).IsAbstract, Is.True, "'ActionBase' must be an abstract class");

        Assert.That(typeof(AttackAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'AttackAction'");
        Assert.That(typeof(DefuseAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'DefuseAction'");
        Assert.That(typeof(FavorAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'FavorAction'");
        Assert.That(typeof(SkipAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'SkipAction'");
        Assert.That(typeof(ShuffleAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'ShuffleAction'");
        Assert.That(typeof(SeeTheFutureAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'SeeTheFutureAction'");
        Assert.That(typeof(StealRandomCardAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'StealRandomCardAction'");
        Assert.That(typeof(StealSpecificCardAction).IsSubclassOf(typeof(ActionBase)), Is.True, "use 'ActionBase' as base class for 'StealSpecificCardAction'");
    }
}