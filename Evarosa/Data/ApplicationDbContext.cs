using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Evarosa.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleCategory> ArticleCategories { get; set; }
    public DbSet<ConfigSite> ConfigSite { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Member> Members { get; set; }

}
