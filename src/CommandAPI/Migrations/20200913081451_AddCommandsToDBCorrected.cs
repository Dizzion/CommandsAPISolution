using Microsoft.EntityFrameworkCore.Migrations;

namespace CommandAPI.Migrations
{
    public partial class AddCommandsToDBCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Command",
                table: "Command");

            migrationBuilder.RenameTable(
                name: "Command",
                newName: "CommandItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommandItems",
                table: "CommandItems",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommandItems",
                table: "CommandItems");

            migrationBuilder.RenameTable(
                name: "CommandItems",
                newName: "Command");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Command",
                table: "Command",
                column: "Id");
        }
    }
}
