namespace Exomine_API_api_ydndua.Models.DTOs;

public class GovernorHistoryDTO
{
    public int Id { get; set; }
    public int GovernorId { get; set; }
    public int ColonyId { get; set; }
    public bool PreviousStatus { get; set; }
    public bool NewStatus { get; set; }
    public DateTime Timestamp { get; set; }
}