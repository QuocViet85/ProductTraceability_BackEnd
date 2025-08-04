using App.Areas.Factories.DTO;
using App.Areas.Products.DTO;
using Areas.Auth.DTO;

namespace App.Areas.Batches.DTO;

public class BatchDTO
{
    public Guid? Id { set; get; }
    public string? BatchCode { set; get; }
    public Guid? ProductId { set; get; }
    public Guid? FactoryId { set; get; }
    public string? Name { set; get; }
    public DateTime? ManufactureDate { set; get; }
    public DateTime? ExpireDate { set; get; }
    public int? Quantity { set; get; }
    public string? Status { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public ProductDTO? Product { set; get; }
    public FactoryDTO? Factory { set; get; }
    public UserDTO? CreatedUser { set; get; }
    public UserDTO? UpdatedUser { set; get; }
}