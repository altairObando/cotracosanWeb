namespace Cotracosan.Models.Cotracosan
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Context : DbContext
    {
        public Context()
            : base("name=Context")
        {
        }

        public virtual DbSet<Abonos> Abonos { get; set; }
        public virtual DbSet<Articulos> Articulos { get; set; }
        public virtual DbSet<Carreras> Carreras { get; set; }
        public virtual DbSet<Conductores> Conductores { get; set; }
        public virtual DbSet<Creditos> Creditos { get; set; }
        public virtual DbSet<DetallesDeCreditos> DetallesDeCreditos { get; set; }
        public virtual DbSet<LugaresFinalesDelosRecorridos> LugaresFinalesDelosRecorridos { get; set; }
        public virtual DbSet<Penalizaciones> Penalizaciones { get; set; }
        public virtual DbSet<Socios> Socios { get; set; }
        public virtual DbSet<Turnos> Turnos { get; set; }
        public virtual DbSet<Vehiculos> Vehiculos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Abonos>()
                .Property(e => e.MontoDeAbono)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Articulos>()
                .Property(e => e.DescripcionDeArticulo)
                .IsUnicode(false);

            modelBuilder.Entity<Articulos>()
                .Property(e => e.Precio)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Articulos>()
                .HasMany(e => e.DetallesDeCreditos)
                .WithRequired(e => e.Articulos)
                .HasForeignKey(e => e.ArticuloId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Carreras>()
                .Property(e => e.MontoRecaudado)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Carreras>()
                .Property(e => e.Multa)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Conductores>()
                .Property(e => e.Nombres)
                .IsUnicode(false);

            modelBuilder.Entity<Conductores>()
                .Property(e => e.Apellido1Conductor)
                .IsUnicode(false);

            modelBuilder.Entity<Conductores>()
                .Property(e => e.Apellido2Conductor)
                .IsUnicode(false);

            modelBuilder.Entity<Conductores>()
                .HasMany(e => e.Carreras)
                .WithRequired(e => e.Conductores)
                .HasForeignKey(e => e.ConductorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Creditos>()
                .Property(e => e.MontoTotal)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Creditos>()
                .HasMany(e => e.Abonos)
                .WithRequired(e => e.Creditos)
                .HasForeignKey(e => e.CreditoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Creditos>()
                .HasMany(e => e.DetallesDeCreditos)
                .WithRequired(e => e.Creditos)
                .HasForeignKey(e => e.CreditoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LugaresFinalesDelosRecorridos>()
                .Property(e => e.NombreDeLugar)
                .IsUnicode(false);

            modelBuilder.Entity<LugaresFinalesDelosRecorridos>()
                .HasMany(e => e.Carreras)
                .WithRequired(e => e.LugaresFinalesDelosRecorridos)
                .HasForeignKey(e => e.LugarFinalDeRecorridoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Penalizaciones>()
                .Property(e => e.Cantidad)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Penalizaciones>()
                .HasMany(e => e.Carreras)
                .WithRequired(e => e.Penalizaciones)
                .HasForeignKey(e => e.PenalizacionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Socios>()
                .Property(e => e.Nombres)
                .IsUnicode(false);

            modelBuilder.Entity<Socios>()
                .Property(e => e.Apellido1Socio)
                .IsUnicode(false);

            modelBuilder.Entity<Socios>()
                .Property(e => e.Apellido2Socio)
                .IsUnicode(false);

            modelBuilder.Entity<Socios>()
                .HasMany(e => e.Vehiculos)
                .WithRequired(e => e.Socios)
                .HasForeignKey(e => e.SocioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Turnos>()
                .HasMany(e => e.Carreras)
                .WithRequired(e => e.Turnos)
                .HasForeignKey(e => e.TurnoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vehiculos>()
                .HasMany(e => e.Carreras)
                .WithRequired(e => e.Vehiculos)
                .HasForeignKey(e => e.VehiculoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vehiculos>()
                .HasMany(e => e.Creditos)
                .WithRequired(e => e.Vehiculos)
                .HasForeignKey(e => e.VehiculoId)
                .WillCascadeOnDelete(false);
        }
    }
}
