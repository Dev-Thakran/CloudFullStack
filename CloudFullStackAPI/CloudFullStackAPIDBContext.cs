using CloudFullStackAPI.Models;
using Microsoft.EntityFrameworkCore;

public class CloudFullStackAPIDBContext(DbContextOptions<CloudFullStackAPIDBContext> options) : DbContext(options)
{
    public DbSet<ParkingLot> ParkingLot { get; set; } = default!;
    public DbSet<Bay> Bay { get; set; } = default!;
    public DbSet<Booking> Booking { get; set; } = default!;
    public DbSet<Review> Review { get; set; } = default!;
    public DbSet<Vehicle> Vehicle { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParkingLot>().Property(l => l.Latitude).HasPrecision(9, 6);
        modelBuilder.Entity<ParkingLot>().Property(l => l.Longitude).HasPrecision(9, 6);
    }
}
