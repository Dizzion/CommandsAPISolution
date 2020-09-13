using Microsoft.EntityFrameworkCore;

namespace CommandAPI.Model
{
    public class CommandContext : DbContext
    {
        public CommandContext(DbContextOptions<CommandContext> options) : base(options)
        {
            
        }

        public DbSet<Command> CommandItems { get; set; }
    }
}