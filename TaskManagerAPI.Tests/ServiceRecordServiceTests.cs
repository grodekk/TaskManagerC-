using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests;

public class ServiceRecordServiceTests
{
    [Fact]
    public async Task AddAsync_PersistsRecordAndItems_AndMapsResponse()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Db.Vehicles.Add(vehicle);
        await database.Db.SaveChangesAsync();

        var dto = new ServiceRecordCreateDto
        {
            PerformedOn = new DateOnly(2026, 9, 1),
            Mileage = 120000,
            Items = new()
            {
                new()
                {
                    Description = "Oil change"
                },
                new()
                {
                    Description = "Brake inspection"
                }
            }
        };

        var service = new ServiceRecordService(database.Db);

        // Act
        var result = await service.AddAsync(vehicle.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);

        database.Db.ChangeTracker.Clear();

        var savedRecord = await database.Db.ServiceRecords
            .Include(record => record.Items)
            .SingleAsync();

        Assert.Equal(result.Id, savedRecord.Id);
        Assert.Equal(vehicle.Id, savedRecord.VehicleId);

        Assert.Equal(dto.PerformedOn, savedRecord.PerformedOn);
        Assert.Equal(dto.PerformedOn, result.PerformedOn);

        Assert.Equal(dto.Mileage, savedRecord.Mileage);
        Assert.Equal(dto.Mileage, result.Mileage);

        Assert.Equal(2, savedRecord.Items.Count);
        Assert.Equal(2, result.Items.Count);

        foreach (var expectedItem in dto.Items)
        {
            var savedItem = Assert.Single(
                savedRecord.Items,
                item => item.Description == expectedItem.Description
            );

            Assert.True(savedItem.Id > 0);
            Assert.Equal(savedRecord.Id, savedItem.ServiceRecordId);

            var responseItem = Assert.Single(
                result.Items,
                item => item.Id == savedItem.Id
            );

            Assert.Equal(expectedItem.Description, responseItem.Description);
        }
    }

    [Fact]
    public async Task AddAsync_ReturnsNull_WhenVehicleDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();

        var service = new ServiceRecordService(database.Db);

        var dto = new ServiceRecordCreateDto
        {
            Items = new()
            {
                new()
                {
                    Description = "Oil change"
                }
            }
        };

        // Act
        var result = await service.AddAsync(999, dto);

        // Assert
        Assert.Null(result);
        Assert.Empty(database.Db.ServiceRecords);
        Assert.Empty(database.Db.ServiceRecordItems);
    }

    [Fact]
    public async Task GetForVehicleAsync_ReturnsOnlyRecordsForRequestedVehicle()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        var otherVehicle = new Vehicle
        {
            RegistrationNumber = "WB67890"
        };

        database.Db.Vehicles.AddRange(vehicle, otherVehicle);
        await database.Db.SaveChangesAsync();

        var vehicleRecord = new ServiceRecord
        {
            VehicleId = vehicle.Id,
            PerformedOn = new DateOnly(2026, 9, 1),
            Mileage = 120000
        };

        var otherVehicleRecord = new ServiceRecord
        {
            VehicleId = otherVehicle.Id,
            PerformedOn = new DateOnly(2026, 10, 1),
            Mileage = 50000
        };

        database.Db.ServiceRecords.AddRange(
            vehicleRecord,
            otherVehicleRecord
        );

        await database.Db.SaveChangesAsync();

        var expectedRecordId = vehicleRecord.Id;

        var service = new ServiceRecordService(database.Db);

        // Act
        var result = await service.GetForVehicleAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);

        var record = Assert.Single(result);

        Assert.Equal(expectedRecordId, record.Id);
    }

    [Fact]
    public async Task GetForVehicleAsync_ReturnsRecordsNewestFirst()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Db.Vehicles.Add(vehicle);
        await database.Db.SaveChangesAsync();

        var olderRecord = new ServiceRecord
        {
            VehicleId = vehicle.Id,
            PerformedOn = new DateOnly(2026, 8, 1),
            Mileage = 100000
        };

        var newerRecord = new ServiceRecord
        {
            VehicleId = vehicle.Id,
            PerformedOn = new DateOnly(2026, 9, 1),
            Mileage = 120000
        };

        database.Db.ServiceRecords.AddRange(
            olderRecord,
            newerRecord
        );

        await database.Db.SaveChangesAsync();

        var olderRecordId = olderRecord.Id;
        var newerRecordId = newerRecord.Id;

        var service = new ServiceRecordService(database.Db);

        // Act
        var result = await service.GetForVehicleAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(newerRecordId, result[0].Id);
        Assert.Equal(olderRecordId, result[1].Id);
    }

    [Fact]
    public async Task GetForVehicleAsync_ReturnsItemsForServiceRecord()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Db.Vehicles.Add(vehicle);
        await database.Db.SaveChangesAsync();

        var serviceRecord = new ServiceRecord
        {
            VehicleId = vehicle.Id,
            PerformedOn = new DateOnly(2026, 9, 1),
            Mileage = 120000,
            Items = new()
        {
            new()
            {
                Description = "Oil change"
            },
            new()
            {
                Description = "Brake inspection"
            }
        }
        };

        database.Db.ServiceRecords.Add(serviceRecord);
        await database.Db.SaveChangesAsync();

        var service = new ServiceRecordService(database.Db);

        // Act
        var result = await service.GetForVehicleAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);

        var record = Assert.Single(result);

        Assert.Equal(2, record.Items.Count);

        Assert.Contains(
            record.Items,
            item => item.Description == "Oil change"
        );

        Assert.Contains(
            record.Items,
            item => item.Description == "Brake inspection"
        );
    }

    [Fact]
    public async Task GetForVehicleAsync_ReturnsNull_WhenVehicleDoesNotExist()
    {
        // Arrange
        using var database = new TestDatabase();

        var service = new ServiceRecordService(database.Db);

        // Act
        var result = await service.GetForVehicleAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetForVehicleAsync_ReturnsEmptyHistory_WhenVehicleHasNoRecords()
    {
        // Arrange
        using var database = new TestDatabase();

        var vehicle = new Vehicle
        {
            RegistrationNumber = "WA12345"
        };

        database.Db.Vehicles.Add(vehicle);
        await database.Db.SaveChangesAsync();

        var service = new ServiceRecordService(database.Db);

        // Act
        var result = await service.GetForVehicleAsync(vehicle.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}