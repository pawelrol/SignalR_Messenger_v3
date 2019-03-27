using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalApp.Data.Migrations
{
    public partial class mssages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SignalMessage",
                table: "SignalMessage");

            migrationBuilder.RenameTable(
                name: "SignalMessage",
                newName: "SignalMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SignalMessages",
                table: "SignalMessages",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SignalMessages",
                table: "SignalMessages");

            migrationBuilder.RenameTable(
                name: "SignalMessages",
                newName: "SignalMessage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SignalMessage",
                table: "SignalMessage",
                column: "Id");
        }
    }
}
