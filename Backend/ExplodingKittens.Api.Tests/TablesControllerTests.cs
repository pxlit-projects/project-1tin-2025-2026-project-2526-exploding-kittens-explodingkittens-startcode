using System.Security.Claims;
using ExplodingKittens.Api.Controllers;
using ExplodingKittens.Api.Models;
using ExplodingKittens.Api.Models.Output;
using ExplodingKittens.Core.TableAggregate;
using ExplodingKittens.Core.TableAggregate.Contracts;
using ExplodingKittens.Core.Tests.Builders;
using ExplodingKittens.Core.UserAggregate;
using Guts.Client.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExplodingKittens.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "ExplodingKittens", "TablesController",
    @"ExplodingKittens.Api\Controllers\TablesController.cs;ExplodingKittens.Api\Models\ModelMapper.cs")]
public class TablesControllerTests
{
    private TablesController _controller = null!;
    private Mock<ITableManager> _tableManagerMock = null!;
    private Mock<ITableRepository> _tableRepositoryMock = null!;
    private Mock<UserManager<User>> _userManagerMock = null!;
    private Mock<IModelMapper> _mapperMock = null!;
    private User _loggedInUser = null!;
    private TablePreferences _tablePreferences = null!;

    [SetUp]
    public void Setup()
    {
        _tableManagerMock = new Mock<ITableManager>();
        _tableRepositoryMock = new Mock<ITableRepository>();
        _mapperMock = new Mock<IModelMapper>();

        var userStoreMock = new Mock<IUserStore<User>>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();
        var lookupNormalizerMock = new Mock<ILookupNormalizer>();
        var errorsMock = new Mock<IdentityErrorDescriber>();
        var loggerMock = new Mock<ILogger<UserManager<User>>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null,
            passwordHasherMock.Object,
            null,
            null,
            lookupNormalizerMock.Object,
            errorsMock.Object,
            null,
            loggerMock.Object);

        _controller = new TablesController(_tableManagerMock.Object, _tableRepositoryMock.Object, _mapperMock.Object, _userManagerMock.Object);

        _loggedInUser = new UserBuilder().Build();
        var userClaimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _loggedInUser.Id.ToString())
            })
        );
        var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.User = userClaimsPrincipal;
        _controller.ControllerContext = context;
        _userManagerMock.Setup(manager => manager.GetUserAsync(userClaimsPrincipal))
            .ReturnsAsync(_loggedInUser);

        _tablePreferences = new TablePreferencesBuilder().Build();
    }

    [MonitoredTest]
    public void GetTablesWithAvailableSeats_ShouldRetrieveTablesUsingRepository_ShouldReturnMappedModels()
    {
        //Arrange
        ITable table1 = new TableMockBuilder().Object;
        ITable table2 = new TableMockBuilder().Object;
        IList<ITable> tables = [table1, table2];

        _tableRepositoryMock.Setup(repository => repository.FindTablesWithAvailableSeats(_tablePreferences))
            .Returns(tables);

        var tableModel1 = new TableModel();
        var tableModel2 = new TableModel();
        _mapperMock.Setup(mapper => mapper.MapTable(table1)).Returns(tableModel1);
        _mapperMock.Setup(mapper => mapper.MapTable(table2)).Returns(tableModel2);

        //Act
        var result = _controller.GetTablesWithAvailableSeats(_tablePreferences) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _tableRepositoryMock.Verify(repository => repository.FindTablesWithAvailableSeats(_tablePreferences), Times.Once,
            "The repository method 'FindTablesWithAvailableSeats' is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapTable(table1), Times.Once,
            "The first table is not correctly mapped to a table model");
        _mapperMock.Verify(mapper => mapper.MapTable(table2), Times.Once,
            "The second table is not correctly mapped to a table model");

        var returnedModels = result!.Value as IList<TableModel>;
        Assert.That(returnedModels, Is.Not.Null, "The result value should be a list of TableModel");
        Assert.That(returnedModels!.Count, Is.EqualTo(2), "The returned list should contain 2 table models");
        Assert.That(returnedModels[0], Is.SameAs(tableModel1), "The first mapped table model should be in the result");
        Assert.That(returnedModels[1], Is.SameAs(tableModel2), "The second mapped table model should be in the result");
    }

    [MonitoredTest]
    public void GetTableById_ShouldRetrieveTableUsingRepository_ShouldReturnAModelOfIt()
    {
        //Arrange
        ITable table = new TableMockBuilder().Mock.Object;
        _tableRepositoryMock.Setup(repository => repository.Get(table.Id)).Returns(table);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.MapTable(It.IsAny<ITable>())).Returns(tableModel);

        //Act
        var result = _controller.GetTableById(table.Id) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
        _mapperMock.Verify(mapper => mapper.MapTable(table), Times.Once,
            "The table is not correctly mapped to a table model");
        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model is not in the OkObjectResult");
    }

    [MonitoredTest]
    public void Create_ShouldUseTheTableManagerToCreateATable_ShouldReturnTheCreatedTableModel()
    {
        //Arrange
        ITable createdTable = new TableMockBuilder().Object;
        _tableManagerMock.Setup(manager => manager.CreateTable(_loggedInUser, _tablePreferences))
            .Returns(createdTable);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.MapTable(createdTable)).Returns(tableModel);

        //Act
        var result = _controller.Create(_tablePreferences).Result as CreatedAtActionResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'CreatedAtActionResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.CreateTable(_loggedInUser, _tablePreferences), Times.Once,
            "The 'CreateTable' method of the table manager is not called correctly");

        _mapperMock.Verify(mapper => mapper.MapTable(createdTable), Times.Once,
            "The table is not correctly mapped to a table model");

        Assert.That(result!.ActionName, Is.EqualTo(nameof(TablesController.GetTableById)),
            "The action name should be 'GetTableById'");
        Assert.That(result.RouteValues!["id"], Is.EqualTo(createdTable.Id),
            "The route values should contain the table id");
        Assert.That(result.Value, Is.SameAs(tableModel), "The mapped table model should be in the CreatedAtActionResult");
    }

    [MonitoredTest]
    public void Join_TableStillHasAvailableSeats_ShouldUseTheTableManagerToJoinTheTable_ShouldNotStartGame()
    {
        //Arrange
        Guid tableId = Guid.NewGuid();
        var tableWithAvailableSeatsMock = new TableMockBuilder().Mock;
        tableWithAvailableSeatsMock.Setup(t => t.HasAvailableSeat).Returns(true);
        ITable tableWithAvailableSeats = tableWithAvailableSeatsMock.Object;

        _tableManagerMock.Setup(manager => manager.JoinTable(tableId, _loggedInUser))
            .Returns(tableWithAvailableSeats);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.MapTable(tableWithAvailableSeats)).Returns(tableModel);

        //Act
        var result = _controller.Join(tableId).Result as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.JoinTable(tableId, _loggedInUser), Times.Once,
            "The 'JoinTable' method of the table manager is not called correctly");

        _tableManagerMock.Verify(manager => manager.StartGameForTable(It.IsAny<Guid>()), Times.Never,
            "The 'StartGameForTable' method should not be called when the table still has available seats");

        _mapperMock.Verify(mapper => mapper.MapTable(tableWithAvailableSeats), Times.Once,
            "The table is not correctly mapped to a table model");

        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void Join_TableIsFull_ShouldUseTheTableManagerToJoinTheTable_ShouldStartTheGame()
    {
        //Arrange
        Guid tableId = Guid.NewGuid();
        var fullTableMock = new TableMockBuilder().Mock;
        fullTableMock.Setup(t => t.Id).Returns(tableId);
        fullTableMock.Setup(t => t.HasAvailableSeat).Returns(false);
        ITable fullTable = fullTableMock.Object;

        _tableManagerMock.Setup(manager => manager.JoinTable(tableId, _loggedInUser))
            .Returns(fullTable);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.MapTable(fullTable)).Returns(tableModel);

        //Act
        var result = _controller.Join(tableId).Result as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.JoinTable(tableId, _loggedInUser), Times.Once,
            "The 'JoinTable' method of the table manager is not called correctly");

        _tableManagerMock.Verify(manager => manager.StartGameForTable(tableId), Times.Once,
            "The 'StartGameForTable' method should be called when the table is full");

        _mapperMock.Verify(mapper => mapper.MapTable(fullTable), Times.Once,
            "The table is not correctly mapped to a table model");

        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model should be in the OkObjectResult");
    }

    [MonitoredTest]
    public void Leave_ShouldUseTheTableManagerToRemoveTheLoggedInUserFromTheTable()
    {
        //Arrange
        ITable existingTable = new TableMockBuilder().Object;

        //Act
        var result = _controller.Leave(existingTable.Id).Result as OkResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.LeaveTable(existingTable.Id, _loggedInUser), Times.Once,
            "The 'LeaveTable' method of the table manager is not called correctly");
    }
}

