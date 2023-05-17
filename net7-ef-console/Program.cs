using Microsoft.EntityFrameworkCore;

var context = new MyDbContext();

var users = context.Users
    .Where(u => u.IsActive)
    .OrderBy(u => u.Username);

foreach (var user in users)
{
    Console.WriteLine(user.Username);
}

var newUser = new User() { Username = "johndoe", IsActive = true };
context.Users.Add(newUser);
await context.SaveChangesAsync();

public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=.;Database=Mydatabase;Trusted_Connection=True;Encrypt=False");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class User
{
    public int id { get; set; }
    public string? Username { get; set; }
    public bool IsActive { get; set; }
}