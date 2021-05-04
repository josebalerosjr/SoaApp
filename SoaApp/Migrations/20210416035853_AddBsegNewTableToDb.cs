using Microsoft.EntityFrameworkCore.Migrations;

namespace SoaApp.Migrations
{
    public partial class AddBsegNewTableToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BSEGNews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BUKRS = table.Column<int>(nullable: false),
                    BELNR = table.Column<int>(nullable: false),
                    GJAHR = table.Column<int>(nullable: false),
                    KOART = table.Column<string>(nullable: true),
                    BUZEI = table.Column<string>(nullable: true),
                    HKONT = table.Column<int>(nullable: false),
                    SGTXT = table.Column<string>(nullable: true),
                    SHKZG = table.Column<string>(nullable: true),
                    DMBTR = table.Column<double>(nullable: false),
                    WRBTR = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BSEGNews", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BSEGNews");
        }
    }
}
