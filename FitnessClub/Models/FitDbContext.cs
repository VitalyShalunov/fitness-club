using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FitnessClub.Models
{
    public class FitDbContext : DbContext
    {
        public FitDbContext() : base("DbConnectionString")
        {

        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Coach> Coach { get; set; }
        public DbSet<SeasonTicket> SeasonTicket { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Visiting> Visiting { get; set; }

    }

    
}