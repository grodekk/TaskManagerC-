using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerAPI.Controllers;

[Authorize(Roles = "Admin,Employee,Viewer")]
[ApiController]
[Route("api/vehicles/{vehicleId}/service-records")]
public class ServiceRecordsController : ControllerBase
{
    private readonly ServiceRecordService _service;

    public ServiceRecordsController(ServiceRecordService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    public async Task<ActionResult<ServiceRecordDto>> Create(int vehicleId, ServiceRecordCreateDto dto)
    {
        var serviceRecord = await _service.AddAsync(vehicleId, dto);

        if (serviceRecord is null)
            return NotFound();

        return StatusCode(
            StatusCodes.Status201Created,
            serviceRecord);
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceRecordDto>>> GetForVehicle(int vehicleId)
    {
        var serviceRecords = await _service.GetForVehicleAsync(vehicleId);

        if (serviceRecords is null)
            return NotFound();

        return Ok(serviceRecords);
    }
}