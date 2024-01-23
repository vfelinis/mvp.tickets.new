using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invites_Email_Company",
                schema: "dbo",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Email",
                schema: "dbo",
                table: "Invites",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invites_Email",
                schema: "dbo",
                table: "Invites");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Email_Company",
                schema: "dbo",
                table: "Invites",
                columns: new[] { "Email", "Company" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                schema: "dbo",
                table: "Companies",
                column: "Name",
                unique: true);
        }
    }
}
