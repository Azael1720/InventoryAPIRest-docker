namespace InventoryAPIRest.Models.Enums
{
    public enum MovementType
    {
        StockIn = 1,   // Entrada de stock
        StockOut = 2,  // Salida / baja de stock (merma, ajuste, devolución al proveedor)
        Sale = 3       // Venta
    }
}
