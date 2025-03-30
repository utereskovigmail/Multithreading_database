using FirstSp;
using FirstSp.Entities;


namespace FirstSp;

using Microsoft.EntityFrameworkCore;

public class Context : DbContext
{

    public DbSet<User> Users { get; set; }
    
    public Context() 
    {
        
    }
    public Context(DbContextOptions<Context> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=ep-broad-glitter-a8fxqbq7-pooler.eastus2.azure.neon.tech;Database=default;Username=neondb_owner;Password=npg_xapVfrWTDQ28");
    }
}