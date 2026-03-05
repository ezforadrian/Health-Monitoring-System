using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _VEHRSv1.Models;

public partial class HswmsContext : DbContext
{
    public HswmsContext()
    {
    }

    public HswmsContext(DbContextOptions<HswmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AmeExcelRecord> AmeExcelRecords { get; set; }

    public virtual DbSet<Amedetail> Amedetails { get; set; }

    public virtual DbSet<Ameheader> Ameheaders { get; set; }

    public virtual DbSet<AppReference> AppReferences { get; set; }

    public virtual DbSet<AppReferenceGroup> AppReferenceGroups { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; } = null!;

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<ECUDetail> Ecudetails { get; set; }

    public virtual DbSet<ECUHeader> Ecuheaders { get; set; }

    public virtual DbSet<Mwractivity> Mwractivities { get; set; }

    public virtual DbSet<Mwrdate> Mwrdates { get; set; }

    public virtual DbSet<Mwrlist> Mwrlists { get; set; }


    public virtual DbSet<Pemedetail> Pemedetails { get; set; }

    public virtual DbSet<Pemeheader> Pemeheaders { get; set; }

    public virtual DbSet<Pemestatus> Pemestatuses { get; set; }

    public virtual DbSet<TestDetail> TestDetails { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AmeExcelRecord>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AmeExcelRecord");

            entity.Property(e => e.Age)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AgeGroup)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AmeDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AmeMonth)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AmeQuarter)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BirthMonth)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Branch)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DmIi).HasColumnName("DmII");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.PatientInfo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Patient_Info");
            entity.Property(e => e.Position)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Request)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sex)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Soa)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Amedetail>(entity =>
        {
            entity.ToTable("AMEDetails");

            entity.Property(e => e.AmedetailId).HasColumnName("AMEDetailID");
            entity.Property(e => e.AmeheaderId).HasColumnName("AMEHeaderID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.TestId).HasColumnName("TestID");

            entity.HasOne(d => d.Ameheader).WithMany(p => p.Amedetails)
                .HasForeignKey(d => d.AmeheaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AMEDetails_AMEHeader");

            entity.HasOne(d => d.Test).WithMany(p => p.Amedetails)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AMEDetails_AMETest");
        });

        modelBuilder.Entity<Ameheader>(entity =>
        {
            entity.ToTable("AMEHeader");

            entity.Property(e => e.AmeheaderId).HasColumnName("AMEHeaderID");
            entity.Property(e => e.Amedate)
                .HasColumnType("date")
                .HasColumnName("AMEDate");
            entity.Property(e => e.Amemonth).HasColumnName("AMEMonth");
            entity.Property(e => e.Amequarter).HasColumnName("AMEQuarter");
            entity.Property(e => e.Branch)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Request)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RunDate).HasColumnType("date");
            entity.Property(e => e.Soa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SOA");
            entity.Property(e => e.Remarks)
                .HasMaxLength(350)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AppReference>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AppReference");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceDescription)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceId).ValueGeneratedOnAdd();
            entity.Property(e => e.ReferenceName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AppReferenceGroup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AppReferenceGroup");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupDescription)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.GroupName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceGroupId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => new { e.Id });

            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RoleId).HasMaxLength(450);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => new { e.Id });

            entity.Property(e => e.Department).HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.LastLoginDateTime).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PayClass).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(150);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.Property(e => e.RoleId).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<ECUDetail>(entity =>
        {
            entity.ToTable("ECUDetail");

            entity.HasIndex(e => e.EcuheaderId, "IX_ECUDetail_ECUHeaderID");

            entity.Property(e => e.EcudetailId).HasColumnName("ECUDetailID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.EcuheaderId).HasColumnName("ECUHeaderID");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.TestId).HasColumnName("TestID");

            entity.HasOne(d => d.Ecuheader).WithMany(p => p.Ecudetails)
                .HasForeignKey(d => d.EcuheaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ECUDetail_ECUHeader");

            entity.HasOne(d => d.Test).WithMany(p => p.Ecudetails)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ECUDetail_TestDetail");
        });

        modelBuilder.Entity<ECUHeader>(entity =>
        {
            entity.ToTable("ECUHeader");

            entity.HasIndex(e => e.Branch, "IX_ECUHeader_Branch");

            entity.HasIndex(e => e.Ecudate, "IX_ECUHeader_ECUDate");

            entity.HasIndex(e => e.Idnumber, "IX_ECUHeader_IDNumber");

            entity.Property(e => e.EcuheaderId).HasColumnName("ECUHeaderID");
            entity.Property(e => e.Branch)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Ecudate).HasColumnName("ECUDate");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Mwractivity>(entity =>
        {
            entity.HasKey(e => e.MwractId);

            entity.ToTable("MWRActivities");

            entity.Property(e => e.MwractId).HasColumnName("MWRActID");
            entity.Property(e => e.ActDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.MwrlistId).HasColumnName("MWRListID");

            entity.HasOne(d => d.Mwrlist).WithMany(p => p.Mwractivities)
                .HasForeignKey(d => d.MwrlistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MWRActivities_MWRList");
        });

        modelBuilder.Entity<Mwrdate>(entity =>
        {
            entity.ToTable("Mwrdate");

            entity.Property(e => e.MwrdateId).HasColumnName("MWRDateId");
            entity.Property(e => e.MwractDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MwrlistId).HasColumnName("MWRListID");
        });


        modelBuilder.Entity<Mwrlist>(entity =>
        {
            entity.ToTable("MWRList");

            entity.Property(e => e.MwrlistId).HasColumnName("MWRListID");
            entity.Property(e => e.ActivityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Pemedetail>(entity =>
        {
            entity.HasKey(e => e.DetailId);

            entity.ToTable("PEMEDetails");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExamDate).HasColumnType("datetime");
            entity.Property(e => e.Pemeid).HasColumnName("PEMEID");
            entity.Property(e => e.Remarks)
                .HasMaxLength(350)
                .IsUnicode(false);

            entity.HasOne(d => d.Peme).WithMany(p => p.Pemedetails)
                .HasForeignKey(d => d.Pemeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PEMEDetails_PEMEHeader");
        });

        modelBuilder.Entity<Pemeheader>(entity =>
        {
            entity.HasKey(e => e.Pemeid);

            entity.ToTable("PEMEHeader");

            entity.Property(e => e.Pemeid).HasColumnName("PEMEID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.PositionRef)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Status).WithMany(p => p.Pemeheaders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PEMEHeader_PEMEStatus");
        });

        modelBuilder.Entity<Pemestatus>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("PEMEStatus");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TestDetail>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK_AMETest");

            entity.ToTable("TestDetail");

            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.TestCategory)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TestName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
