using BDJ.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDJ.Data
{
    public class BDJContext : IdentityDbContext<User>
    {
        public BDJContext(DbContextOptions<BDJContext> options) : base(options) { }

        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Train> Trains { get; set; } 
        public virtual DbSet<Line> Lines { get; set; }
    }
}
