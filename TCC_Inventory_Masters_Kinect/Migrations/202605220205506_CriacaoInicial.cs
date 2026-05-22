namespace TCC_Inventory_Masters_Kinect.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedicaoVolumes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VolumeCm3 = c.Double(nullable: false),
                        DataHora = c.DateTime(nullable: false),
                        KinectLigado = c.Boolean(nullable: false),
                        Calibrado = c.Boolean(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MedicaoVolumes");
        }
    }
}
