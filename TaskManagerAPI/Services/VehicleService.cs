using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services;

public class VehicleService
{
    private readonly AppDbContext _db;

    public VehicleService(AppDbContext db)
    {
        _db = db;
    }

    public List<VehicleDto> GetAll()
    {
        return _db.Vehicles
            .Select(vehicle => new VehicleDto
            {
                Id = vehicle.Id,
                RegistrationNumber = vehicle.RegistrationNumber,
                Make = vehicle.Make,
                Model = vehicle.Model
            })
            .ToList();
    }

    public VehicleDto? GetById(int id)
    {
        return _db.Vehicles
            .Where(vehicle => vehicle.Id == id)
            .Select(vehicle => new VehicleDto
            {
                Id = vehicle.Id,
                RegistrationNumber = vehicle.RegistrationNumber,
                Make = vehicle.Make,
                Model = vehicle.Model
            })
            .FirstOrDefault();
    }

    public VehicleDto? Create(CreateVehicleDto dto)
    {
        var registrationNumber = dto.RegistrationNumber.Trim().ToUpper();

        var alreadyExists = _db.Vehicles
            .Any(vehicle => vehicle.RegistrationNumber == registrationNumber);

        if (alreadyExists)
            return null;

        var vehicle = new Vehicle
        {
            RegistrationNumber = registrationNumber,
            Make = dto.Make,
            Model = dto.Model
        };

        _db.Vehicles.Add(vehicle);
        _db.SaveChanges();

        return new VehicleDto
        {
            Id = vehicle.Id,
            RegistrationNumber = vehicle.RegistrationNumber,
            Make = vehicle.Make,
            Model = vehicle.Model
        };
    }

    public bool Update(int id, UpdateVehicleDto dto)
    {
        var vehicle = _db.Vehicles
            .FirstOrDefault(vehicle => vehicle.Id == id);

        if (vehicle == null)
            return false;

        var registrationNumber = dto.RegistrationNumber.Trim().ToUpper();

        var registrationTaken = _db.Vehicles
            .Any(otherVehicle =>
                otherVehicle.Id != id &&
                otherVehicle.RegistrationNumber == registrationNumber);

        if (registrationTaken)
            return false;

        vehicle.RegistrationNumber = registrationNumber;
        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;

        _db.SaveChanges();

        return true;
    }

    public bool Delete(int id)
    {
        var vehicle = _db.Vehicles
            .FirstOrDefault(vehicle => vehicle.Id == id);

        if (vehicle == null)
            return false;

        _db.Vehicles.Remove(vehicle);
        _db.SaveChanges();

        return true;
    }
}