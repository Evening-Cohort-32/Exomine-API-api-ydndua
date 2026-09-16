namespace Exomine_API_api_ydndua.Models;

public class Transaction
{
    public int Id { get; set; }
    public int GovernorId { get; set; }
    public int ColonyId { get; set; }
    public int FacilityId { get; set; }
    public int MineralId { get; set; }
    public int Quantity { get; set; }
    public DateTime Timestamp { get; set; }
}