using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class VehicleServiceTests
{
    [Fact]
    public void Create_PersistsVehicle_AndNormalizesRegistration()
    {
        // Arrange
        using var database = new TestDatabase();

        var service = new VehicleService(database.Db);

        var dto = new CreateVehicleDto
        {
            RegistrationNumber = " wa12345 ",
            Make = "Toyota",
            Model = "Corolla"
        };

        // Act
        var result = service.Create(dto);

        // Assert
        Assert.NotNull(result);

        database.Db.ChangeTracker.Clear();

        var savedVehicle = Assert.Single(database.Db.Vehicles);

        Assert.True(result.Id > 0);
        Assert.Equal(savedVehicle.Id, result.Id);

        Assert.Equal("WA12345", savedVehicle.RegistrationNumber);
        Assert.Equal("WA12345", result.RegistrationNumber);

        Assert.Equal("Toyota", savedVehicle.Make);
        Assert.Equal("Toyota", result.Make);

        Assert.Equal("Corolla", savedVehicle.Model);
        Assert.Equal("Corolla", result.Model);
    }

    [Fact]
    public void GetById_ReturnsExistingVehicle()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345",
            Make = "Ford",
            Model = "Transit"
        };

        database.Db.Vehicles.Add(vehicle);
        database.Db.SaveChanges();

        var service = new VehicleService(database.Db);

        // Act
        var result = service.GetById(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.Id);
        Assert.Equal(vehicle.RegistrationNumber, result.RegistrationNumber);
        Assert.Equal(vehicle.Make, result.Make);
        Assert.Equal(vehicle.Model, result.Model);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenVehicleDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();

        var service = new VehicleService(database.Db);

        // Act
        var result = service.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Update_PersistsChangedFields()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345",
            Make = "Ford",
            Model = "Transit"
        };

        database.Db.Vehicles.Add(vehicle);
        database.Db.SaveChanges();

        var service = new VehicleService(database.Db);

        var dto = new UpdateVehicleDto
        {
            RegistrationNumber = " wb67890 ",
            Make = "Toyota",
            Model = "Corolla"
        };

        // Act
        var result = service.Update(vehicle.Id, dto);

        // Assert
        Assert.True(result);

        database.Db.ChangeTracker.Clear();

        var savedVehicle = Assert.Single(database.Db.Vehicles);

        Assert.Equal("WB67890", savedVehicle.RegistrationNumber);
        Assert.Equal("Toyota", savedVehicle.Make);
        Assert.Equal("Corolla", savedVehicle.Model);
    }

    [Fact]
    public void Create_ReturnsNull_WhenNormalizedRegistrationIsDuplicate()
    {
        // Arrange
        using var database = new TestDatabase();

        var existingVehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Db.Vehicles.Add(existingVehicle);
        database.Db.SaveChanges();

        var service = new VehicleService(database.Db);

        var dto = new CreateVehicleDto
        {
            RegistrationNumber = " wa12345 "
        };

        // Act
        var result = service.Create(dto);

        // Assert
        Assert.Null(result);
        Assert.Single(database.Db.Vehicles);
    }

    [Fact]
    public void Update_RejectsDuplicateRegistration_WithoutChangingVehicle()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WB67890",
            Make = "Ford",
            Model = "Transit"
        };

        var otherVehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Db.Vehicles.AddRange(vehicle, otherVehicle);
        database.Db.SaveChanges();

        var service = new VehicleService(database.Db);

        var dto = new UpdateVehicleDto
        {
            RegistrationNumber = " wa12345 ",
            Make = "Toyota",
            Model = "Corolla"
        };

        // Act
        var result = service.Update(vehicle.Id, dto);

        // Assert
        Assert.False(result);

        database.Db.ChangeTracker.Clear();

        var savedVehicle = database.Db.Vehicles.Single(
            v => v.Id == vehicle.Id
        );

        Assert.Equal("WB67890", savedVehicle.RegistrationNumber);
        Assert.Equal("Ford", savedVehicle.Make);
        Assert.Equal("Transit", savedVehicle.Model);
    }
}
