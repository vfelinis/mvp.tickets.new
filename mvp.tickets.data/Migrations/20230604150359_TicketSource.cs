using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    public partial class TicketSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                schema: "dbo",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                schema: "dbo",
                table: "Tickets",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "dbo",
                table: "TicketCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_IsDefault",
                schema: "dbo",
                table: "TicketCategories",
                column: "IsDefault",
                unique: true,
                filter: "[IsDefault] = 1 AND [IsActive] = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketCategories_IsDefault",
                schema: "dbo",
                table: "TicketCategories");

            migrationBuilder.DropColumn(
                name: "Source",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Token",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "dbo",
                table: "TicketCategories");
        }
    }
}
