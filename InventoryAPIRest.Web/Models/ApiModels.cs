namespace InventoryAPIRest.Web.Models
{
    public sealed record CategoryModel(int Id, string Name, string? Description);
    public sealed record ProductModel(
        int Id, string Name, string Code, string? Description, decimal Price, int Stock,
        bool IsActive, int CategoryId, string? CategoryName, DateTime CreatedAt, DateTime? UpdatedAt);
    public sealed record MovementModel(
        int Id, int ProductId, string? ProductName, string? ProductCode, string Type, int Quantity,
        int StockBefore, int StockAfter, decimal? UnitPrice, string? Reason, DateTime CreatedAt)
    {
        public string TypeLabel => Type switch
        {
            "StockIn" => "Entrada",
            "StockOut" => "Salida",
            "Sale" => "Venta",
            _ => Type
        };
        public string TypeBadgeClass => Type switch
        {
            "StockIn" => "bg-success",
            "StockOut" => "bg-warning text-dark",
            "Sale" => "bg-primary",
            _ => "bg-secondary"
        };
    }
    public sealed record CategoryRequest(string Name, string? Description);
    public sealed record CreateProductRequest(
        string Name, string Code, string? Description, decimal Price, int Stock, int CategoryId);
    public sealed record UpdateProductRequest(
        string Name, string Code, string? Description, decimal Price, int CategoryId, bool IsActive);
    public sealed record StockRequest(int Quantity, string? Reason);
}
