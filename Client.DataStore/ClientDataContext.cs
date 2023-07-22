using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Client.DataStore
{
    public class ClientDataContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public ClientDataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // in memory database used for simplicity,
            // change to a real db for production applications
            options.UseInMemoryDatabase("ClientDb");
        }

        public DbSet<ClientLog> Clients { get; set; }
    }
}
