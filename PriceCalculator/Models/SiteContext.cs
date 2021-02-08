using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Models
{
    public class SiteContext : IdentityDbContext
    {
        public SiteContext(DbContextOptions<SiteContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Material>()
                .HasMany(m => m.LayerThicknesses)
                .WithOne(lt => lt.Material)
                .IsRequired();

            modelBuilder.Entity<MaterialPostProcess>()
                .HasKey(mp => new { mp.MaterialID, mp.PostProcessID });
            modelBuilder.Entity<MaterialPostProcess>()
                .HasOne(mp => mp.Material)
                .WithMany(m => m.MaterialPostProcesses)
                .HasForeignKey(mp => mp.MaterialID);
            modelBuilder.Entity<MaterialPostProcess>()
                .HasOne(mp => mp.PostProcess)
                .WithMany(p => p.MaterialPostProcesses)
                .HasForeignKey(mp => mp.PostProcessID);

            modelBuilder.Entity<PrintModelPostProcess>()
                .HasKey(pmpp => new { pmpp.PrintModelID, pmpp.PostProcessID });
            modelBuilder.Entity<PrintModelPostProcess>()
                .HasOne(pmpp => pmpp.PrintModel)
                .WithMany(pm => pm.PrintModelPostProcesses)
                .HasForeignKey(pmpp => pmpp.PrintModelID);
            modelBuilder.Entity<PrintModelPostProcess>()
                .HasOne(pmpp => pmpp.PostProcess)
                .WithMany(pp => pp.PrintModelPostProcesses)
                .HasForeignKey(pmpp => pmpp.PostProcessID);

            modelBuilder.Entity<PrinterLayerThickness>()
                .HasKey(pl => new { pl.PrinterID, pl.LayerThicknessID });
            modelBuilder.Entity<PrinterLayerThickness>()
                .HasOne(pl => pl.Printer)
                .WithMany(p => p.PrinterLayerThicknesses)
                .HasForeignKey(pl => pl.PrinterID);
            modelBuilder.Entity<PrinterLayerThickness>()
                .HasOne(pl => pl.LayerThickness)
                .WithMany(l => l.PrinterLayerThicknesses)
                .HasForeignKey(pl => pl.LayerThicknessID);
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<LayerThickness> LayerThicknesses { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<PrintModel> PrintModels { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<PostProcess> PostProcesses { get; set; }
        public DbSet<MaterialPostProcess> MaterialPostProcesses { get; set; }
        public DbSet<PrintModelPostProcess> PrintModelPostProcesses { get; set; }
        public DbSet<PrinterLayerThickness> PrinterLayerThicknesses { get; set; }
        public DbSet<Industry> Industries { get; set; }
    }
}
