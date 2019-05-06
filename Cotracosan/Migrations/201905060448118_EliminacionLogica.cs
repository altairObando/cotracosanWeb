namespace Cotracosan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EliminacionLogica : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Abonos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaDeAbono = c.DateTime(nullable: false),
                        CodigoAbono = c.String(nullable: false, maxLength: 25),
                        MontoDeAbono = c.Decimal(nullable: false, precision: 18, scale: 0),
                        CreditoId = c.Int(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Creditos", t => t.CreditoId)
                .Index(t => t.CreditoId);
            
            CreateTable(
                "dbo.Creditos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoCredito = c.String(nullable: false, maxLength: 20),
                        FechaDeCredito = c.DateTime(nullable: false),
                        MontoTotal = c.Decimal(nullable: false, precision: 18, scale: 0),
                        EstadoDeCredito = c.Boolean(nullable: false),
                        CreditoAnulado = c.String(nullable: false),
                        VehiculoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vehiculos", t => t.VehiculoId)
                .Index(t => t.VehiculoId);
            
            CreateTable(
                "dbo.DetallesDeCreditos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cantidad = c.Int(nullable: false),
                        CreditoId = c.Int(nullable: false),
                        ArticuloId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articulos", t => t.ArticuloId)
                .ForeignKey("dbo.Creditos", t => t.CreditoId)
                .Index(t => t.CreditoId)
                .Index(t => t.ArticuloId);
            
            CreateTable(
                "dbo.Articulos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoDeArticulo = c.String(nullable: false, maxLength: 6),
                        DescripcionDeArticulo = c.String(nullable: false, maxLength: 50, unicode: false),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Vehiculos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Placa = c.String(nullable: false, maxLength: 15),
                        SocioId = c.Int(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Socios", t => t.SocioId)
                .Index(t => t.SocioId);
            
            CreateTable(
                "dbo.Carreras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoCarrera = c.String(nullable: false),
                        FechaDeCarrera = c.DateTime(nullable: false),
                        HoraRealDeLlegada = c.Time(nullable: false, precision: 7),
                        CarreraAnulada = c.Boolean(nullable: false),
                        MontoRecaudado = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Multa = c.Decimal(nullable: false, precision: 18, scale: 0),
                        VehiculoId = c.Int(nullable: false),
                        ConductorId = c.Int(nullable: false),
                        PenalizacionId = c.Int(nullable: false),
                        TurnoId = c.Int(nullable: false),
                        LugarFinalDeRecorridoId = c.Int(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conductores", t => t.ConductorId)
                .ForeignKey("dbo.LugaresFinalesDelosRecorridos", t => t.LugarFinalDeRecorridoId)
                .ForeignKey("dbo.Penalizaciones", t => t.PenalizacionId)
                .ForeignKey("dbo.Turnos", t => t.TurnoId)
                .ForeignKey("dbo.Vehiculos", t => t.VehiculoId)
                .Index(t => t.VehiculoId)
                .Index(t => t.ConductorId)
                .Index(t => t.PenalizacionId)
                .Index(t => t.TurnoId)
                .Index(t => t.LugarFinalDeRecorridoId);
            
            CreateTable(
                "dbo.Conductores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Licencia = c.String(nullable: false, maxLength: 9),
                        Nombres = c.String(nullable: false, maxLength: 50, unicode: false),
                        Apellido1Conductor = c.String(nullable: false, maxLength: 20, unicode: false),
                        Apellido2Conductor = c.String(nullable: false, maxLength: 20, unicode: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LugaresFinalesDelosRecorridos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoDeLugar = c.String(nullable: false, maxLength: 6),
                        NombreDeLugar = c.String(nullable: false, maxLength: 50, unicode: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Penalizaciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoPenalizacion = c.String(nullable: false),
                        Cantidad = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Turnos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoDeTurno = c.String(nullable: false, maxLength: 4),
                        HoraDeSalida = c.Time(nullable: false, precision: 7),
                        HoraDeLlegada = c.Time(nullable: false, precision: 7),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Socios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoSocio = c.String(nullable: false, maxLength: 5),
                        Nombres = c.String(nullable: false, maxLength: 50, unicode: false),
                        Apellido1Socio = c.String(nullable: false, maxLength: 20, unicode: false),
                        Apellido2Socio = c.String(nullable: false, maxLength: 20, unicode: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vehiculos", "SocioId", "dbo.Socios");
            DropForeignKey("dbo.Creditos", "VehiculoId", "dbo.Vehiculos");
            DropForeignKey("dbo.Carreras", "VehiculoId", "dbo.Vehiculos");
            DropForeignKey("dbo.Carreras", "TurnoId", "dbo.Turnos");
            DropForeignKey("dbo.Carreras", "PenalizacionId", "dbo.Penalizaciones");
            DropForeignKey("dbo.Carreras", "LugarFinalDeRecorridoId", "dbo.LugaresFinalesDelosRecorridos");
            DropForeignKey("dbo.Carreras", "ConductorId", "dbo.Conductores");
            DropForeignKey("dbo.DetallesDeCreditos", "CreditoId", "dbo.Creditos");
            DropForeignKey("dbo.DetallesDeCreditos", "ArticuloId", "dbo.Articulos");
            DropForeignKey("dbo.Abonos", "CreditoId", "dbo.Creditos");
            DropIndex("dbo.Carreras", new[] { "LugarFinalDeRecorridoId" });
            DropIndex("dbo.Carreras", new[] { "TurnoId" });
            DropIndex("dbo.Carreras", new[] { "PenalizacionId" });
            DropIndex("dbo.Carreras", new[] { "ConductorId" });
            DropIndex("dbo.Carreras", new[] { "VehiculoId" });
            DropIndex("dbo.Vehiculos", new[] { "SocioId" });
            DropIndex("dbo.DetallesDeCreditos", new[] { "AticuloId" });
            DropIndex("dbo.DetallesDeCreditos", new[] { "CreditoId" });
            DropIndex("dbo.Creditos", new[] { "VehiculoId" });
            DropIndex("dbo.Abonos", new[] { "CreditoId" });
            DropTable("dbo.Socios");
            DropTable("dbo.Turnos");
            DropTable("dbo.Penalizaciones");
            DropTable("dbo.LugaresFinalesDelosRecorridos");
            DropTable("dbo.Conductores");
            DropTable("dbo.Carreras");
            DropTable("dbo.Vehiculos");
            DropTable("dbo.Articulos");
            DropTable("dbo.DetallesDeCreditos");
            DropTable("dbo.Creditos");
            DropTable("dbo.Abonos");
        }
    }
}
