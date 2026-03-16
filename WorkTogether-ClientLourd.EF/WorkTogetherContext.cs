using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorkTogether_ClientLourd.EF.Entities;

namespace WorkTogether_ClientLourd.EF;

public partial class WorkTogetherContext : DbContext
{
    public WorkTogetherContext()
    {
    }

    public WorkTogetherContext(DbContextOptions<WorkTogetherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accountant> Accountants { get; set; }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Bay> Bays { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Intervention> Interventions { get; set; }

    public virtual DbSet<Notice> Notices { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<Particular> Particulars { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Support> Supports { get; set; }

    public virtual DbSet<Technician> Technicians { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=127.0.0.1;Port=3306;User ID=root;Password=root;Database=WorkTogether;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accountant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("accountant");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Accountant)
                .HasForeignKey<Accountant>(d => d.Id)
                .HasConstraintName("FK_E7681183BF396750");
        });

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("administrator");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Administrator)
                .HasForeignKey<Administrator>(d => d.Id)
                .HasConstraintName("FK_58DF0651BF396750");
        });

        modelBuilder.Entity<Bay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bay");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CapacityUnit).HasColumnName("capacity_unit");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("company");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name");
            entity.Property(e => e.Siret)
                .HasMaxLength(14)
                .HasColumnName("siret");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Company)
                .HasForeignKey<Company>(d => d.Id)
                .HasConstraintName("FK_4FBF094FBF396750");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adress)
                .HasMaxLength(200)
                .HasColumnName("adress");
            entity.Property(e => e.InvoiceAdress)
                .HasMaxLength(200)
                .HasColumnName("invoice_adress");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.Id)
                .HasConstraintName("FK_81398E09BF396750");
        });

        modelBuilder.Entity<Intervention>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("intervention");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BeginDate)
                .HasColumnType("datetime")
                .HasColumnName("begin_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FinishDate)
                .HasColumnType("datetime")
                .HasColumnName("finish_date");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasMany(d => d.Units).WithMany(p => p.Interventions)
                .UsingEntity<Dictionary<string, object>>(
                    "InterventionUnit",
                    r => r.HasOne<Unit>().WithMany()
                        .HasForeignKey("UnitId")
                        .HasConstraintName("FK_D3041491F8BD700D"),
                    l => l.HasOne<Intervention>().WithMany()
                        .HasForeignKey("InterventionId")
                        .HasConstraintName("FK_D30414918EAE3863"),
                    j =>
                    {
                        j.HasKey("InterventionId", "UnitId").HasName("PRIMARY");
                        j.ToTable("intervention_unit");
                        j.HasIndex(new[] { "InterventionId" }, "IDX_D30414918EAE3863");
                        j.HasIndex(new[] { "UnitId" }, "IDX_D3041491F8BD700D");
                        j.IndexerProperty<int>("InterventionId").HasColumnName("intervention_id");
                        j.IndexerProperty<int>("UnitId").HasColumnName("unit_id");
                    });
        });

        modelBuilder.Entity<Notice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notice");

            entity.HasIndex(e => e.UserId, "IDX_480D45C2A76ED395");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_480D45C2A76ED395");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("offer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NbUnit).HasColumnName("nb_unit");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Version).HasColumnName("version");
        });

        modelBuilder.Entity<Particular>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("particular");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gender)
                .HasMaxLength(14)
                .HasColumnName("gender");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Particular)
                .HasForeignKey<Particular>(d => d.Id)
                .HasConstraintName("FK_862161CFBF396750");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.ReservationOffer, "IDX_42C849556AA95799");

            entity.HasIndex(e => e.CustomerId, "IDX_42C849559395C3F3");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BeginDate)
                .HasColumnType("datetime")
                .HasColumnName("begin_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FinishDate)
                .HasColumnType("datetime")
                .HasColumnName("finish_date");
            entity.Property(e => e.ReservationOffer).HasColumnName("reservation_offer");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_42C849559395C3F3");

            entity.HasOne(d => d.ReservationOfferNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReservationOffer)
                .HasConstraintName("FK_42C849556AA95799");

            entity.HasMany(d => d.Units).WithMany(p => p.Reservations)
                .UsingEntity<Dictionary<string, object>>(
                    "ReservationUnit",
                    r => r.HasOne<Unit>().WithMany()
                        .HasForeignKey("UnitId")
                        .HasConstraintName("FK_CA82C13BF8BD700D"),
                    l => l.HasOne<Reservation>().WithMany()
                        .HasForeignKey("ReservationId")
                        .HasConstraintName("FK_CA82C13BB83297E7"),
                    j =>
                    {
                        j.HasKey("ReservationId", "UnitId").HasName("PRIMARY");
                        j.ToTable("reservation_unit");
                        j.HasIndex(new[] { "ReservationId" }, "IDX_CA82C13BB83297E7");
                        j.HasIndex(new[] { "UnitId" }, "IDX_CA82C13BF8BD700D");
                        j.IndexerProperty<int>("ReservationId").HasColumnName("reservation_id");
                        j.IndexerProperty<int>("UnitId").HasColumnName("unit_id");
                    });
        });

        modelBuilder.Entity<Support>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("support");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Support)
                .HasForeignKey<Support>(d => d.Id)
                .HasConstraintName("FK_8004EBA5BF396750");
        });

        modelBuilder.Entity<Technician>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("technician");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Phone)
                .HasMaxLength(14)
                .HasColumnName("phone");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Technician)
                .HasForeignKey<Technician>(d => d.Id)
                .HasConstraintName("FK_F244E948BF396750");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ticket");

            entity.HasIndex(e => e.SupportId, "IDX_97A0ADA3315B405");

            entity.HasIndex(e => e.CustomerId, "IDX_97A0ADA39395C3F3");

            entity.HasIndex(e => e.TechniciansId, "IDX_97A0ADA3FEFB4E80");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CloseDate)
                .HasColumnType("datetime")
                .HasColumnName("close_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.OpenDate)
                .HasColumnType("datetime")
                .HasColumnName("open_date");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(50)
                .HasColumnName("subject");
            entity.Property(e => e.SupportId).HasColumnName("support_id");
            entity.Property(e => e.TechniciansId).HasColumnName("technicians_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_97A0ADA39395C3F3");

            entity.HasOne(d => d.Support).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SupportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_97A0ADA3315B405");

            entity.HasOne(d => d.Technicians).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TechniciansId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_97A0ADA3FEFB4E80");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("unit");

            entity.HasIndex(e => e.BayId, "IDX_DCBB0C53DF9BA23B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BayId).HasColumnName("bay_id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.NetworkThroughput).HasColumnName("network_throughput");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.PowerConsumption).HasColumnName("power_consumption");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Temperature).HasColumnName("temperature");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Bay).WithMany(p => p.Units)
                .HasForeignKey(d => d.BayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DCBB0C53DF9BA23B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "UNIQ_8D93D649E7927C74").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Discr)
                .HasMaxLength(255)
                .HasColumnName("discr");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Roles).HasColumnName("roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
