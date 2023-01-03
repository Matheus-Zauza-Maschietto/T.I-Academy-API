using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext: DbContext{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {

    }

    public DbSet<Product> Products {get; set;}
    public DbSet<Category> Category { get; set; }
    public DbSet<Tag> Tag { get; set; }
    // protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer("Server=localhost;Database=Products;User Id=sa;Password=HaYaBuSa10022004@;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(120).IsRequired(true);
        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(20).IsRequired(true);
    }
}

