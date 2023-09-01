using DAL_ACCOUNTING.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL_ACCOUNTING.Data;

public partial class DatabaseContext : DbContext
{
    private readonly string connectionString;

    public virtual DbSet<BalanceSheet> BalanceSheets { get; set; }

    public virtual DbSet<ChartOfAccount> ChartOfAccounts { get; set; }

    public virtual DbSet<IncomeStatement> IncomeStatements { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public DatabaseContext(string connectionString)
    {
        this.connectionString = connectionString;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    //    => optionsBuilder.UseSqlServer("Data Source=DESKTOP-LOMK63U;Initial Catalog=ACCOUNTING;Persist Security Info=True;User ID=sa;Password=pass@word1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BalanceSheet>(entity =>
        {
            entity.ToView("BalanceSheet");
        });

        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.Property(e => e.AcctType).IsFixedLength();
        });

        modelBuilder.Entity<IncomeStatement>(entity =>
        {
            entity.ToView("IncomeStatement");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.Property(e => e.Dc).IsFixedLength();
            entity.Property(e => e.JrnlType).IsFixedLength();
            entity.Property(e => e.Posted)
                .HasDefaultValueSql("('N')")
                .IsFixedLength();
            entity.Property(e => e.TransDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Account).WithMany(p => p.Journals).HasConstraintName("FK_Chart");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
