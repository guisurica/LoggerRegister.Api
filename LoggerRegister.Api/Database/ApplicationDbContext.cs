using LoggerRegister.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LoggerRegister.Api.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<LogModel> Logs { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
