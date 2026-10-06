using InventoryAPIRest.Abstractions;
using InventoryAPIRest.Data;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPIRest.Features.Inventory
{
    public record GetMovementsQuery : IQuery<IEnumerable<InventoryMovementDto>>;
    public record GetMovementsByProductQuery(int ProductId) : IQuery<IEnumerable<InventoryMovementDto>>;
    internal static class MovementQueryExtensions
    {
        public static async Task<IEnumerable<InventoryMovementDto>> ToDtosAsync(
            this IQueryable<InventoryMovement> query)
        {
            var rows = await query
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.ProductId,
                    ProductName = m.Product!.Name,
                    ProductCode = m.Product.Code,
                    m.Type,
                    m.Quantity,
                    m.StockBefore,
                    m.StockAfter,
                    m.UnitPrice,
                    m.Reason,
                    m.CreatedAt
                })
                .ToListAsync();

            return rows.Select(r => new InventoryMovementDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.ProductName,
                ProductCode = r.ProductCode,
                Type = r.Type.ToString(),
                Quantity = r.Quantity,
                StockBefore = r.StockBefore,
                StockAfter = r.StockAfter,
                UnitPrice = r.UnitPrice,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
    public class GetMovementsHandler : IQueryHandler<GetMovementsQuery, IEnumerable<InventoryMovementDto>>
    {
        private readonly AppDbContext _context;

        public GetMovementsHandler(AppDbContext context) => _context = context;

        public Task<IEnumerable<InventoryMovementDto>> HandleAsync(GetMovementsQuery query) =>
            _context.InventoryMovements.AsNoTracking().ToDtosAsync();
    }
    public class GetMovementsByProductHandler
        : IQueryHandler<GetMovementsByProductQuery, IEnumerable<InventoryMovementDto>>
    {
        private readonly AppDbContext _context;

        public GetMovementsByProductHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<InventoryMovementDto>> HandleAsync(GetMovementsByProductQuery query)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == query.ProductId))
                throw new ProductNotFoundException(query.ProductId);

            return await _context.InventoryMovements
                .AsNoTracking()
                .Where(m => m.ProductId == query.ProductId)
                .ToDtosAsync();
        }
    }
}
