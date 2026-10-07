namespace TaskManagerAPI.DTOs;

public class ServiceRecordCreateDto
{
    public DateOnly PerformedOn { get; set; }
    public int Mileage { get; set; }

    public List<ServiceRecordItemCreateDto> Items { get; set; } = new();
}

public class ServiceRecordItemCreateDto
{
    public required string Description { get; set; }
}

public class ServiceRecordDto
{
    public int Id { get; set; }
    public DateOnly PerformedOn { get; set; }
    public int Mileage { get; set; }

    public List<ServiceRecordItemDto> Items { get; set; } = new();
}

public class ServiceRecordItemDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
}