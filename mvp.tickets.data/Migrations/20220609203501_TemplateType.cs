using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    public partial class TemplateType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketResponseTemplates_TicketResponseTemplateType_TicketResponseTemplateTypeId",
                schema: "dbo",
                table: "TicketResponseTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketResponseTemplateType",
                schema: "dbo",
                table: "TicketResponseTemplateType");

            migrationBuilder.RenameTable(
                name: "TicketResponseTemplateType",
                schema: "dbo",
                newName: "TicketResponseTemplateTypes",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketResponseTemplateTypes",
                schema: "dbo",
                table: "TicketResponseTemplateTypes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketResponseTemplateTypes_Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketResponseTemplates_TicketResponseTemplateTypes_TicketResponseTemplateTypeId",
                schema: "dbo",
                table: "TicketResponseTemplates",
                column: "TicketResponseTemplateTypeId",
                principalSchema: "dbo",
                principalTable: "TicketResponseTemplateTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketResponseTemplates_TicketResponseTemplateTypes_TicketResponseTemplateTypeId",
                schema: "dbo",
                table: "TicketResponseTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketResponseTemplateTypes",
                schema: "dbo",
                table: "TicketResponseTemplateTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketResponseTemplateTypes_Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes");

            migrationBuilder.RenameTable(
                name: "TicketResponseTemplateTypes",
                schema: "dbo",
                newName: "TicketResponseTemplateType",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "TicketResponseTemplateType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketResponseTemplateType",
                schema: "dbo",
                table: "TicketResponseTemplateType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketResponseTemplates_TicketResponseTemplateType_TicketResponseTemplateTypeId",
                schema: "dbo",
                table: "TicketResponseTemplates",
                column: "TicketResponseTemplateTypeId",
                principalSchema: "dbo",
                principalTable: "TicketResponseTemplateType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
