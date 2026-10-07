namespace TaskManagerAPI.Models;

public class ServiceRecordItem
{
    public int Id { get; set; }

    public int ServiceRecordId { get; set; }
    public ServiceRecord ServiceRecord { get; set; } = null!;
        
    public required string Description { get; set; }
}