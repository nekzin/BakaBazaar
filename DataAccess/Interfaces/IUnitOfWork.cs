using BBC.DataAccess.Interfaces;
namespace BBC.DataAccess.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IReviewRepository Reviews { get; }
        IWishlistRepository Wishlist { get; }
        ICartRepository Carts { get; }
        Task CompleteAsync();
    }
}