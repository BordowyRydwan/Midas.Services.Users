using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class UserDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public UserDbContext() { }

    public UserDbContext(DbContextOptions options) : base(options) { }
}