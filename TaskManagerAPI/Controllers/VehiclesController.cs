using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers;

[Authorize(Roles = "Admin,Employee,Viewer")]
[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleService _service;

    public VehiclesController(VehicleService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var vehicles = _service.GetAll();

        return Ok(vehicles);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var vehicle = _service.GetById(id);

        if (vehicle == null)
            return NotFound();

        return Ok(vehicle);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public IActionResult Create(CreateVehicleDto dto)
    {
        var vehicle = _service.Create(dto);

        if (vehicle == null)
            return Conflict("Vehicle with this registration number already exists.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = vehicle.Id },
            vehicle);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Employee")]
    public IActionResult Update(int id, UpdateVehicleDto dto)
    {
        var updated = _service.Update(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var deleted = _service.Delete(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}