using Microsoft.EntityFrameworkCore;
using Evarosa.Models;

namespace Evarosa.Data
{
    public class UnitOfWork
    {
        private ApplicationDbContext _db;

        public Repository<Admin> Admin { get; private set; }
        public Repository<ConfigSite> ConfigSite { get; private set; }
        public Repository<ArticleCategory> ArticleCategory { get; set; }
        public Repository<Article> Article { get; set; }
        public Repository<Banner> Banner { get; set; }
        public Repository<Contact> Contact { get; set; }
        public Repository<Member> Member { get; set; }
        public Repository<ProductCategory> ProductCategory { get; set; }
        public Repository<Product> Product { get; set; }
        public Repository<CartItem> CartItem { get; set; }
        public Repository<Order> Order { get; set; }
        public Repository<OrderDetail> OrderDetail { get; set; }
        public Repository<City> City { get; set; }
        public Repository<District> District { get; set; }
        public Repository<Ward> Ward { get; set; }
        public Repository<MemberAddress> MemberAddress { get; set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Admin = new Repository<Admin>(_db);
            ConfigSite = new Repository<ConfigSite>(_db);
            ArticleCategory = new Repository<ArticleCategory>(_db);
            Article = new Repository<Article>(_db);
            Banner = new Repository<Banner>(_db);
            Contact = new Repository<Contact>(_db);
            Member = new Repository<Member>(_db);
            ProductCategory = new Repository<ProductCategory>(_db);
            Product = new Repository<Product>(_db);
            CartItem = new Repository<CartItem>(_db);
            Order = new Repository<Order>(_db);
            OrderDetail = new Repository<OrderDetail>(_db);
            City = new Repository<City>(_db);
            District = new Repository<District>(_db);
            Ward = new Repository<Ward>(_db);
            MemberAddress = new Repository<MemberAddress>(_db);

        }

        public void Commit()
            => _db.SaveChanges();

        public async Task CommitAsync()
            => await _db.SaveChangesAsync();

        public void Rollback()
            => _db.Dispose();

        public async Task RollbackAsync()
            => await _db.DisposeAsync();
    }
}
