namespace Exomine_API_api_ydndua.Models.DTOs;

public class ColonyInventoryDTO
{
    public int Id { get; set; }
    public int ColonyId { get; set; }
    public int MineralId { get; set; }
    public string MineralName { get; set; }
    public int Quantity { get; set; }
}