namespace Exomine_API_api_ydndua.Models.DTOs;

public class FacilityInventoryDTO
{
    public int Id { get; set; }
    public int FacilityId { get; set; }
    public int MineralId { get; set; }
    public string MineralName { get; set; }
    public int Quantity { get; set; }
}