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
