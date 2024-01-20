using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    /// <inheritdoc />
    public partial class User_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeEmail",
                schema: "dbo",
                table: "Tickets",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterEmail",
                schema: "dbo",
                table: "Tickets",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeEmail",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReporterEmail",
                schema: "dbo",
                table: "Tickets");
        }
    }
}
