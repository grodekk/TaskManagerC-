using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services;

public class ServiceRecordService
{
    private readonly AppDbContext _context;

    public ServiceRecordService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceRecordDto?> AddAsync(int vehicleId, ServiceRecordCreateDto dto)
    {
        var vehicleExists = await _context.Vehicles
            .AnyAsync(v => v.Id == vehicleId);

        if (!vehicleExists)
            return null;

        var record = new ServiceRecord
        {
            VehicleId = vehicleId,
            PerformedOn = dto.PerformedOn,
            Mileage = dto.Mileage,
            Items = dto.Items
                .Select(item => new ServiceRecordItem
                {
                    Description = item.Description
                })
                .ToList()
        };

        _context.ServiceRecords.Add(record);

        await _context.SaveChangesAsync();

        return new ServiceRecordDto
        {
            Id = record.Id,
            PerformedOn = record.PerformedOn,
            Mileage = record.Mileage,
            Items = record.Items
                .Select(item => new ServiceRecordItemDto
                {
                    Id = item.Id,
                    Description = item.Description
                })
                .ToList()
        };
    }

    public async Task<List<ServiceRecordDto>?> GetForVehicleAsync(int vehicleId)
    {
        var vehicleExists = await _context.Vehicles
            .AnyAsync(v => v.Id == vehicleId);

        if (!vehicleExists)
            return null;

        return await _context.ServiceRecords
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.PerformedOn)
            .Select(r => new ServiceRecordDto
            {
                Id = r.Id,
                PerformedOn = r.PerformedOn,
                Mileage = r.Mileage,
                Items = r.Items
                    .Select(item => new ServiceRecordItemDto
                    {
                        Id = item.Id,
                        Description = item.Description
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<ServiceRecordDto?> GetByIdAsync(int vehicleId, int serviceRecordId)
    {
        return await _context.ServiceRecords
            .Where(r =>
                r.VehicleId == vehicleId &&
                r.Id == serviceRecordId)
            .Select(r => new ServiceRecordDto
            {
                Id = r.Id,
                PerformedOn = r.PerformedOn,
                Mileage = r.Mileage,
                Items = r.Items
                    .Select(item => new ServiceRecordItemDto
                    {
                        Id = item.Id,
                        Description = item.Description
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }
}