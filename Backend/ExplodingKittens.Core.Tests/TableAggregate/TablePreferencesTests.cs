using ExplodingKittens.Core.TableAggregate;
using Guts.Client.NUnit;

namespace ExplodingKittens.Core.Tests.TableAggregate;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "TablePreferences",
    @"ExplodingKittens.Core\TableAggregate\TablePreferences.cs")]
public class TablePreferencesTests
{
    private TablePreferences _tablePreferences = null!;

    [SetUp]
    public void SetUp()
    {
        _tablePreferences = new TablePreferences
        {
            NumberOfPlayers = 2,
            NumberOfArtificialPlayers = 0
        };
    }

    [MonitoredTest]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        var tablePreferences = new TablePreferences();

        // Assert
        Assert.That(tablePreferences.NumberOfPlayers, Is.EqualTo(2), "NumberOfPlayers should be initialized to 2.");
        Assert.That(tablePreferences.NumberOfArtificialPlayers, Is.EqualTo(0), "NumberOfArtificialPlayers should be initialized to 0.");
    }

    [MonitoredTest]
    public void Equals_ShouldReturnTrueForEqualPreferences()
    {
        // Arrange
        var otherPreferences = new TablePreferences
        {
            NumberOfPlayers = 2,
            NumberOfArtificialPlayers = 0
        };

        // Act & Assert
        Assert.That(_tablePreferences.Equals(otherPreferences), Is.True, "Equals should return true for equal preferences.");
    }

    [MonitoredTest]
    public void Equals_ShouldReturnFalseForDifferentPreferences()
    {
        // Arrange
        var otherPreferences = new TablePreferences
        {
            NumberOfPlayers = 3,
            NumberOfArtificialPlayers = 1
        };

        // Act & Assert
        Assert.That(_tablePreferences.Equals(otherPreferences), Is.False, "Equals should return false for different preferences.");
    }

    [MonitoredTest]
    public void GetHashCode_ShouldReturnHashCodeBasedOnNumberOfPlayersAndNumberOfArtificialPlayers()
    {
        // Arrange & Act
        int hashCode = _tablePreferences.GetHashCode();

        // Assert
        Assert.That(hashCode,
            Is.EqualTo(HashCode.Combine(_tablePreferences.NumberOfPlayers,
                _tablePreferences.NumberOfArtificialPlayers)),
            "GetHashCode should return the hash code based on NumberOfPlayers and NumberOfArtificialPlayers.");
    }
}