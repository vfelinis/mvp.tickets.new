using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    /// <inheritdoc />
    public partial class Kafka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "dbo",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                schema: "dbo",
                table: "Tickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                schema: "dbo",
                table: "TicketComments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UniqueId",
                schema: "dbo",
                table: "Tickets",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_UniqueId",
                schema: "dbo",
                table: "TicketComments",
                column: "UniqueId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_UniqueId",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_UniqueId",
                schema: "dbo",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                schema: "dbo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                schema: "dbo",
                table: "TicketComments");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "dbo",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
