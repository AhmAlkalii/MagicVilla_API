using MagicVilla_VillaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        //we need to get the connection string here too
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }

        //We create a DbSET to refernce the model and the name Villas is that will be giving to the table on sql server
        public DbSet<Villa> Villas{ get; set; } 
    }
}
