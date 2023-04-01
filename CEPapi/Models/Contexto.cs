using Microsoft.EntityFrameworkCore;

namespace CEPapi.Models

{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) 
        {
        
        }

        public DbSet<CEP> CEP { get; set; }

    }
}
