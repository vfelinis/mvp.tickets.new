using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    /// <inheritdoc />
    public partial class CompanyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Phone",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatusRules_Name",
                schema: "dbo",
                table: "TicketStatusRules");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatuses_IsDefault",
                schema: "dbo",
                table: "TicketStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatuses_Name",
                schema: "dbo",
                table: "TicketStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TicketResponseTemplateTypes_CompanyId",
                schema: "dbo",
                table: "TicketResponseTemplateTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketResponseTemplateTypes_Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketResolutions_CompanyId",
                schema: "dbo",
                table: "TicketResolutions");

            migrationBuilder.DropIndex(
                name: "IX_TicketResolutions_Name",
                schema: "dbo",
                table: "TicketResolutions");

            migrationBuilder.DropIndex(
                name: "IX_TicketQueues_CompanyId",
                schema: "dbo",
                table: "TicketQueues");

            migrationBuilder.DropIndex(
                name: "IX_TicketQueues_IsDefault",
                schema: "dbo",
                table: "TicketQueues");

            migrationBuilder.DropIndex(
                name: "IX_TicketQueues_Name",
                schema: "dbo",
                table: "TicketQueues");

            migrationBuilder.DropIndex(
                name: "IX_TicketPriorities_CompanyId",
                schema: "dbo",
                table: "TicketPriorities");

            migrationBuilder.DropIndex(
                name: "IX_TicketPriorities_Name",
                schema: "dbo",
                table: "TicketPriorities");

            migrationBuilder.DropIndex(
                name: "IX_TicketCategories_CompanyId",
                schema: "dbo",
                table: "TicketCategories");

            migrationBuilder.DropIndex(
                name: "IX_TicketCategories_IsDefault",
                schema: "dbo",
                table: "TicketCategories");

            migrationBuilder.DropIndex(
                name: "IX_TicketCategories_Name",
                schema: "dbo",
                table: "TicketCategories");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ContactName",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsDefaultHost",
                schema: "dbo",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "ToTicketStatusId",
                schema: "dbo",
                table: "TicketStatusRules",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromTicketStatusId",
                schema: "dbo",
                table: "TicketStatusRules",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "dbo",
                table: "TicketStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId_Email",
                schema: "dbo",
                table: "Users",
                columns: new[] { "CompanyId", "Email" },
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId_Phone",
                schema: "dbo",
                table: "Users",
                columns: new[] { "CompanyId", "Phone" },
                unique: true,
                filter: "\"Phone\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatuses_CompanyId_IsDefault",
                schema: "dbo",
                table: "TicketStatuses",
                columns: new[] { "CompanyId", "IsDefault" },
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatuses_CompanyId_Name",
                schema: "dbo",
                table: "TicketStatuses",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketResponseTemplateTypes_CompanyId_Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketResolutions_CompanyId_Name",
                schema: "dbo",
                table: "TicketResolutions",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueues_CompanyId_IsDefault",
                schema: "dbo",
                table: "TicketQueues",
                columns: new[] { "CompanyId", "IsDefault" },
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueues_CompanyId_Name",
                schema: "dbo",
                table: "TicketQueues",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketPriorities_CompanyId_Name",
                schema: "dbo",
                table: "TicketPriorities",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_CompanyId_IsDefault",
                schema: "dbo",
                table: "TicketCategories",
                columns: new[] { "CompanyId", "IsDefault" },
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_CompanyId_Name",
                schema: "dbo",
                table: "TicketCategories",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketStatuses_Companies_CompanyId",
                schema: "dbo",
                table: "TicketStatuses",
                column: "CompanyId",
                principalSchema: "dbo",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketStatuses_Companies_CompanyId",
                schema: "dbo",
                table: "TicketStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId_Email",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId_Phone",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatuses_CompanyId_IsDefault",
                schema: "dbo",
                table: "TicketStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatuses_CompanyId_Name",
                schema: "dbo",
                table: "TicketStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TicketResponseTemplateTypes_CompanyId_Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketResolutions_CompanyId_Name",
                schema: "dbo",
                table: "TicketResolutions");

            migrationBuilder.DropIndex(
                name: "IX_TicketQueues_CompanyId_IsDefault",
                schema: "dbo",
                table: "TicketQueues");

            migrationBuilder.DropIndex(
                name: "IX_TicketQueues_CompanyId_Name",
                schema: "dbo",
                table: "TicketQueues");

            migrationBuilder.DropIndex(
                name: "IX_TicketPriorities_CompanyId_Name",
                schema: "dbo",
                table: "TicketPriorities");

            migrationBuilder.DropIndex(
                name: "IX_TicketCategories_CompanyId_IsDefault",
                schema: "dbo",
                table: "TicketCategories");

            migrationBuilder.DropIndex(
                name: "IX_TicketCategories_CompanyId_Name",
                schema: "dbo",
                table: "TicketCategories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "dbo",
                table: "TicketStatuses");

            migrationBuilder.AlterColumn<int>(
                name: "ToTicketStatusId",
                schema: "dbo",
                table: "TicketStatusRules",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FromTicketStatusId",
                schema: "dbo",
                table: "TicketStatusRules",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                schema: "dbo",
                table: "Companies",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                schema: "dbo",
                table: "Companies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                schema: "dbo",
                table: "Companies",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultHost",
                schema: "dbo",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                schema: "dbo",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "dbo",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                schema: "dbo",
                table: "Users",
                column: "Phone",
                unique: true,
                filter: "\"Phone\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusRules_Name",
                schema: "dbo",
                table: "TicketStatusRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatuses_IsDefault",
                schema: "dbo",
                table: "TicketStatuses",
                column: "IsDefault",
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatuses_Name",
                schema: "dbo",
                table: "TicketStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketResponseTemplateTypes_CompanyId",
                schema: "dbo",
                table: "TicketResponseTemplateTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketResponseTemplateTypes_Name",
                schema: "dbo",
                table: "TicketResponseTemplateTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketResolutions_CompanyId",
                schema: "dbo",
                table: "TicketResolutions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketResolutions_Name",
                schema: "dbo",
                table: "TicketResolutions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueues_CompanyId",
                schema: "dbo",
                table: "TicketQueues",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueues_IsDefault",
                schema: "dbo",
                table: "TicketQueues",
                column: "IsDefault",
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueues_Name",
                schema: "dbo",
                table: "TicketQueues",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketPriorities_CompanyId",
                schema: "dbo",
                table: "TicketPriorities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPriorities_Name",
                schema: "dbo",
                table: "TicketPriorities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_CompanyId",
                schema: "dbo",
                table: "TicketCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_IsDefault",
                schema: "dbo",
                table: "TicketCategories",
                column: "IsDefault",
                unique: true,
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_Name",
                schema: "dbo",
                table: "TicketCategories",
                column: "Name",
                unique: true);
        }
    }
}
