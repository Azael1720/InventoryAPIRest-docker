namespace InventoryAPIRest.Exceptions
{
    public abstract class DomainExceptions : Exception
    {
        protected DomainExceptions(string message) : base(message) { }
    }
    public abstract class NotFoundException : DomainExceptions
    {
        protected NotFoundException(string message) : base(message) { }
    }
    public abstract class ConflictException : DomainExceptions
    {
        protected ConflictException(string message) : base(message) { }
    }

    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(int id)
            : base($"El producto con Id {id} no existe.") { }
    }
    public class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(int id)
            : base($"La categoría con Id {id} no existe.") { }
    }
    public class DuplicateProductCodeException : ConflictException
    {
        public DuplicateProductCodeException(string code)
            : base($"Ya existe un producto con el código '{code}'.") { }
    }
    public class CategoryHasProductsException : ConflictException
    {
        public CategoryHasProductsException(int id)
            : base("No se puede eliminar una categoría que tiene productos asociados.") { }
    }
    public class InactiveProductException : ConflictException
    {
        public InactiveProductException(int id)
            : base("No se puede vender un producto inactivo.") { }
    }
    public class InsufficientStockException : ConflictException
    {
        public int Available { get; }
        public int Requested { get; }

        public InsufficientStockException(int available, int requested)
            : base($"Stock insuficiente. Disponible: {available}, solicitado: {requested}.")
        {
            Available = available;
            Requested = requested;
        }
    }
    public class StockConcurrencyException : ConflictException
    {
        public StockConcurrencyException(int productId)
            : base("El stock cambió mientras se procesaba la operación. Intenta de nuevo.") { }
    }
}
