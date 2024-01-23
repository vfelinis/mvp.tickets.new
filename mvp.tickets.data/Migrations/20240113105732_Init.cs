using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace mvp.tickets.data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsRoot = table.Column<bool>(type: "boolean", nullable: false),
                    ContactName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Host = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsDefaultHost = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<byte[]>(type: "bytea", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Company = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateSent = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatuses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompletion = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsRoot = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCategories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketCategories_TicketCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalSchema: "dbo",
                        principalTable: "TicketCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketPriorities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketPriorities_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketQueues",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketQueues_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketResolutions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketResolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketResolutions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketResponseTemplateTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketResponseTemplateTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketResponseTemplateTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Permissions = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatusRules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FromTicketStatusId = table.Column<int>(type: "integer", nullable: true),
                    ToTicketStatusId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketStatusRules_TicketStatuses_FromTicketStatusId",
                        column: x => x.FromTicketStatusId,
                        principalSchema: "dbo",
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusRules_TicketStatuses_ToTicketStatusId",
                        column: x => x.ToTicketStatusId,
                        principalSchema: "dbo",
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketResponseTemplates",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketResponseTemplateTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketResponseTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketResponseTemplates_TicketResponseTemplateTypes_TicketR~",
                        column: x => x.TicketResponseTemplateTypeId,
                        principalSchema: "dbo",
                        principalTable: "TicketResponseTemplateTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Token = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<int>(type: "integer", nullable: false),
                    AssigneeId = table.Column<int>(type: "integer", nullable: true),
                    TicketPriorityId = table.Column<int>(type: "integer", nullable: true),
                    TicketQueueId = table.Column<int>(type: "integer", nullable: false),
                    TicketResolutionId = table.Column<int>(type: "integer", nullable: true),
                    TicketStatusId = table.Column<int>(type: "integer", nullable: false),
                    TicketCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "dbo",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketCategories_TicketCategoryId",
                        column: x => x.TicketCategoryId,
                        principalSchema: "dbo",
                        principalTable: "TicketCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketPriorities_TicketPriorityId",
                        column: x => x.TicketPriorityId,
                        principalSchema: "dbo",
                        principalTable: "TicketPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketQueues_TicketQueueId",
                        column: x => x.TicketQueueId,
                        principalSchema: "dbo",
                        principalTable: "TicketQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketResolutions_TicketResolutionId",
                        column: x => x.TicketResolutionId,
                        principalSchema: "dbo",
                        principalTable: "TicketResolutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketStatuses_TicketStatusId",
                        column: x => x.TicketStatusId,
                        principalSchema: "dbo",
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategoryHistories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    FromTicketCategoryId = table.Column<int>(type: "integer", nullable: false),
                    ToTicketCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategoryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCategoryHistories_TicketCategories_FromTicketCategory~",
                        column: x => x.FromTicketCategoryId,
                        principalSchema: "dbo",
                        principalTable: "TicketCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketCategoryHistories_TicketCategories_ToTicketCategoryId",
                        column: x => x.ToTicketCategoryId,
                        principalSchema: "dbo",
                        principalTable: "TicketCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketCategoryHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketCategoryHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketComments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsInternal = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketComments_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketComments_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketHistories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsInternal = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketObservation",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketObservation_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketObservation_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketPriorityHistories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    FromTicketPriorityId = table.Column<int>(type: "integer", nullable: true),
                    ToTicketPriorityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPriorityHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketPriorityHistories_TicketPriorities_FromTicketPriority~",
                        column: x => x.FromTicketPriorityId,
                        principalSchema: "dbo",
                        principalTable: "TicketPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketPriorityHistories_TicketPriorities_ToTicketPriorityId",
                        column: x => x.ToTicketPriorityId,
                        principalSchema: "dbo",
                        principalTable: "TicketPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketPriorityHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketPriorityHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketQueueHistories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    FromTicketQueueId = table.Column<int>(type: "integer", nullable: false),
                    ToTicketQueueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketQueueHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketQueueHistories_TicketQueues_FromTicketQueueId",
                        column: x => x.FromTicketQueueId,
                        principalSchema: "dbo",
                        principalTable: "TicketQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketQueueHistories_TicketQueues_ToTicketQueueId",
                        column: x => x.ToTicketQueueId,
                        principalSchema: "dbo",
                        principalTable: "TicketQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketQueueHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketQueueHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatusHistories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    FromTicketStatusId = table.Column<int>(type: "integer", nullable: false),
                    ToTicketStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistories_TicketStatuses_FromTicketStatusId",
                        column: x => x.FromTicketStatusId,
                        principalSchema: "dbo",
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistories_TicketStatuses_ToTicketStatusId",
                        column: x => x.ToTicketStatusId,
                        principalSchema: "dbo",
                        principalTable: "TicketStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "dbo",
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketCommentAttachments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TicketCommentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCommentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCommentAttachments_TicketComments_TicketCommentId",
                        column: x => x.TicketCommentId,
                        principalSchema: "dbo",
                        principalTable: "TicketComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Host",
                schema: "dbo",
                table: "Companies",
                column: "Host",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsRoot",
                schema: "dbo",
                table: "Companies",
                column: "IsRoot",
                unique: true,
                filter: "\"IsRoot\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                schema: "dbo",
                table: "Companies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Email_Company",
                schema: "dbo",
                table: "Invites",
                columns: new[] { "Email", "Company" },
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

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_ParentCategoryId",
                schema: "dbo",
                table: "TicketCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategoryHistories_FromTicketCategoryId",
                schema: "dbo",
                table: "TicketCategoryHistories",
                column: "FromTicketCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategoryHistories_TicketId",
                schema: "dbo",
                table: "TicketCategoryHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategoryHistories_ToTicketCategoryId",
                schema: "dbo",
                table: "TicketCategoryHistories",
                column: "ToTicketCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategoryHistories_UserId",
                schema: "dbo",
                table: "TicketCategoryHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentAttachments_FileName",
                schema: "dbo",
                table: "TicketCommentAttachments",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentAttachments_TicketCommentId",
                schema: "dbo",
                table: "TicketCommentAttachments",
                column: "TicketCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_CreatorId",
                schema: "dbo",
                table: "TicketComments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketId",
                schema: "dbo",
                table: "TicketComments",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_TicketId",
                schema: "dbo",
                table: "TicketHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_UserId",
                schema: "dbo",
                table: "TicketHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketObservation_TicketId",
                schema: "dbo",
                table: "TicketObservation",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketObservation_UserId",
                schema: "dbo",
                table: "TicketObservation",
                column: "UserId");

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
                name: "IX_TicketPriorityHistories_FromTicketPriorityId",
                schema: "dbo",
                table: "TicketPriorityHistories",
                column: "FromTicketPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPriorityHistories_TicketId",
                schema: "dbo",
                table: "TicketPriorityHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPriorityHistories_ToTicketPriorityId",
                schema: "dbo",
                table: "TicketPriorityHistories",
                column: "ToTicketPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPriorityHistories_UserId",
                schema: "dbo",
                table: "TicketPriorityHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueueHistories_FromTicketQueueId",
                schema: "dbo",
                table: "TicketQueueHistories",
                column: "FromTicketQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueueHistories_TicketId",
                schema: "dbo",
                table: "TicketQueueHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueueHistories_ToTicketQueueId",
                schema: "dbo",
                table: "TicketQueueHistories",
                column: "ToTicketQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketQueueHistories_UserId",
                schema: "dbo",
                table: "TicketQueueHistories",
                column: "UserId");

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
                name: "IX_TicketResponseTemplates_TicketResponseTemplateTypeId_Name",
                schema: "dbo",
                table: "TicketResponseTemplates",
                columns: new[] { "TicketResponseTemplateTypeId", "Name" },
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
                name: "IX_Tickets_AssigneeId",
                schema: "dbo",
                table: "Tickets",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CompanyId",
                schema: "dbo",
                table: "Tickets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ReporterId",
                schema: "dbo",
                table: "Tickets",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketCategoryId",
                schema: "dbo",
                table: "Tickets",
                column: "TicketCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketPriorityId",
                schema: "dbo",
                table: "Tickets",
                column: "TicketPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketQueueId",
                schema: "dbo",
                table: "Tickets",
                column: "TicketQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketResolutionId",
                schema: "dbo",
                table: "Tickets",
                column: "TicketResolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketStatusId",
                schema: "dbo",
                table: "Tickets",
                column: "TicketStatusId");

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
                name: "IX_TicketStatusHistories_FromTicketStatusId",
                schema: "dbo",
                table: "TicketStatusHistories",
                column: "FromTicketStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistories_TicketId",
                schema: "dbo",
                table: "TicketStatusHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistories_ToTicketStatusId",
                schema: "dbo",
                table: "TicketStatusHistories",
                column: "ToTicketStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistories_UserId",
                schema: "dbo",
                table: "TicketStatusHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusRules_FromTicketStatusId_ToTicketStatusId",
                schema: "dbo",
                table: "TicketStatusRules",
                columns: new[] { "FromTicketStatusId", "ToTicketStatusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusRules_Name",
                schema: "dbo",
                table: "TicketStatusRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusRules_ToTicketStatusId",
                schema: "dbo",
                table: "TicketStatusRules",
                column: "ToTicketStatusId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Invites",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketCategoryHistories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketCommentAttachments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketHistories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketObservation",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketPriorityHistories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketQueueHistories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketResponseTemplates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketStatusHistories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketStatusRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketComments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketResponseTemplateTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tickets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketCategories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketPriorities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketQueues",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketResolutions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TicketStatuses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "dbo");
        }
    }
}
