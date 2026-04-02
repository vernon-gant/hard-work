using Microsoft.EntityFrameworkCore;

namespace EntityBenchmark;

public class BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options) : DbContext(options)
{
    // UserEvent benchmark
    public DbSet<BenchmarkUser> Users { get; set; } = null!;
    public DbSet<BenchmarkUserEvent> UserEvents { get; set; } = null!;

    // Appointment benchmark
    public DbSet<BenchmarkParticipant> Participants { get; set; } = null!;
    public DbSet<BenchmarkGroupEvent> GroupEvents { get; set; } = null!;
    public DbSet<BenchmarkAppointmentLocation> AppointmentLocations { get; set; } = null!;
    public DbSet<BenchmarkAppointment> Appointments { get; set; } = null!;
    public DbSet<BenchmarkAppointmentUser> AppointmentUsers { get; set; } = null!;
    public DbSet<BenchmarkAppointmentParticipant> AppointmentParticipants { get; set; } = null!;
    
    public DbSet<BenchmarkProject> Projects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BenchmarkUser>(e => e.ToTable("AspNetUsers"));

        modelBuilder.Entity<BenchmarkUserEvent>(e =>
        {
            e.ToTable("UserEvents");
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.Type).HasConversion<int>();
            e.Property(x => x.DaysOfWeekGermanFlagEnum).HasConversion<int>();
        });

        modelBuilder.Entity<BenchmarkParticipant>(e => e.ToTable("Participants"));

        modelBuilder.Entity<BenchmarkGroupEvent>(e =>
        {
            e.ToTable("GroupEvents");
            e.Property(x => x.GroupEventType).HasConversion<int?>();
        });

        modelBuilder.Entity<BenchmarkAppointmentLocation>(e => e.ToTable("AppointmentLocations"));

        modelBuilder.Entity<BenchmarkAppointment>(e =>
        {
            e.ToTable("Appointments");
            e.Property(x => x.Type).HasConversion<int>();
            e.Property(x => x.AppointmentState).HasConversion<int>();
            e.HasOne(x => x.GroupEvent).WithMany(g => g.Appointments).HasForeignKey(x => x.GroupEventId);
            e.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId);
        });

        modelBuilder.Entity<BenchmarkAppointmentUser>(e =>
        {
            e.ToTable("AppointmentUsers");
            e.Property(x => x.OverrideAppointmentUserState).HasConversion<int>();
            e.HasOne(x => x.Appointment).WithMany(a => a.AppointmentUsers).HasForeignKey(x => x.AppointmentId);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<BenchmarkAppointmentParticipant>(e =>
        {
            e.ToTable("AppointmentParticipants");
            e.Property(x => x.AppointmentParticipantParticipationState).HasConversion<int>();
            e.HasOne(x => x.Appointment).WithMany(a => a.AppointmentParticipants).HasForeignKey(x => x.AppointmentId);
            e.HasOne(x => x.Participant).WithMany().HasForeignKey(x => x.ParticipantId);
        });
        
        modelBuilder.Entity<BenchmarkProject>(e => e.ToTable("Projects"));
    }
}