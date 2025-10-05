using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConnectFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Avatar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DateNumberFormat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DefaultCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeactivatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PaymentProviderProductId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentProviderPriceId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: false),
                    MaxUsers = table.Column<int>(type: "integer", nullable: false),
                    MaxChannels = table.Column<int>(type: "integer", nullable: false),
                    MaxWhatsAppChannels = table.Column<int>(type: "integer", nullable: false),
                    MaxFacebookChannels = table.Column<int>(type: "integer", nullable: false),
                    MaxInstagramChannels = table.Column<int>(type: "integer", nullable: false),
                    MaxTelegramChannels = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plans_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Plans_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Domain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PaymentProviderCustomerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Settings = table.Column<string>(type: "jsonb", nullable: true),
                    DeactivatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenants_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tenants_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    ChangeType = table.Column<string>(type: "text", nullable: false),
                    PropertyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PropertyDisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OldValue = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NewValue = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Context = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeLogs_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChangeLogs_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChangeLogs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ProviderAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Contact = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SettingsJson = table.Column<string>(type: "jsonb", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelAccounts_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelAccounts_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelAccounts_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChannelAccounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityDocuments_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityDocuments_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityDocuments_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityDocuments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityFiles_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityFiles_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityFiles_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityFiles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    AltText = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityImages_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityImages_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityImages_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityImages_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DirectCost = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPrices_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityPrices_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityPrices_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Labels_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Labels_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductCategories_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBoards_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectBoards_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectBoards_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectBoards_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoringProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TargetEntityType = table.Column<string>(type: "text", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    MinScore = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoringProfiles_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringProfiles_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringProfiles_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringProfiles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentProviderSubscriptionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CurrentPeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CanceledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelAtPeriodEnd = table.Column<bool>(type: "boolean", nullable: false),
                    CancellationRequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsInGracePeriod = table.Column<bool>(type: "boolean", nullable: false),
                    GracePeriodEndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FirstPaymentFailureAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastPaymentFailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentRetryCount = table.Column<int>(type: "integer", nullable: false),
                    HasReachedMaxRetries = table.Column<bool>(type: "boolean", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Subscriptions_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LeftAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InvitedBy = table.Column<int>(type: "integer", nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DateNumberFormat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DefaultCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Settings = table.Column<string>(type: "jsonb", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUsers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUsers_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantUsers_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ProjectBoardId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_ProjectBoards_ProjectBoardId",
                        column: x => x.ProjectBoardId,
                        principalTable: "ProjectBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pipelines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DealsProbabilityEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ScoringProfileId = table.Column<int>(type: "integer", nullable: true),
                    ScoringEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pipelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pipelines_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pipelines_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pipelines_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pipelines_ScoringProfiles_ScoringProfileId",
                        column: x => x.ScoringProfileId,
                        principalTable: "ScoringProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pipelines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoringGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ScoringProfileId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoringGroups_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringGroups_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringGroups_ScoringProfiles_ScoringProfileId",
                        column: x => x.ScoringProfileId,
                        principalTable: "ScoringProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoringGroups_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    TriggerEvent = table.Column<string>(type: "text", nullable: false),
                    AssignToUserId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeactivatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastExecutedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentRules_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRules_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRules_TenantUsers_AssignToUserId",
                        column: x => x.AssignToUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentRules_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityComments_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityComments_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityComments_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityComments_TenantUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityComments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    LabelId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityLabels_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityLabels_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityLabels_TenantUsers_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    PinOrder = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityNotes_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityNotes_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityNotes_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityNotes_TenantUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityNotes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LinkedIn = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NumberOfEmployees = table.Column<int>(type: "integer", nullable: true),
                    AnnualRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Organizations_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Organizations_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Organizations_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organizations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TaxPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BillingFrequency = table.Column<string>(type: "text", nullable: false),
                    RenewUntilCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    RecurringCycleCount = table.Column<int>(type: "integer", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedulers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    DefaultDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    AvailableFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AvailableTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BeforeEventBufferTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    AfterEventBufferTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    StartingIntervalMinutes = table.Column<string>(type: "text", nullable: false),
                    MinimumNoticeToBook = table.Column<string>(type: "text", nullable: false),
                    FurthestNoticeToBookInFutureDays = table.Column<string>(type: "text", nullable: false),
                    Timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DefaultSubject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    MeetingNote = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DefaultLocation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DefaultConferenceUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MeetingDescription = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    VisibleCompanyInfo = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsPhoneNumberRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CustomBookingFormFields = table.Column<string>(type: "jsonb", nullable: true),
                    FooterNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    UrlSlug = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    ManageAvailabilityManually = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedulers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedulers_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Schedulers_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Schedulers_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedulers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TargetType = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Settings = table.Column<string>(type: "jsonb", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sequences_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sequences_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sequences_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sequences_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sequences_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AssignedBy = table.Column<int>(type: "integer", nullable: true),
                    TenantUserId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_AspNetUsers_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectPhaseId = table.Column<int>(type: "integer", nullable: true),
                    AssigneeId = table.Column<int>(type: "integer", nullable: true),
                    ParentTaskId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_ProjectPhases_ProjectPhaseId",
                        column: x => x.ProjectPhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_ProjectTasks_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "ProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_TenantUsers_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    PipelineId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineStages_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PipelineStages_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PipelineStages_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PipelineStages_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PipelineStages_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoringCriterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    LogicalOperator = table.Column<string>(type: "text", nullable: false),
                    ScoringGroupId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringCriterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoringCriterias_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringCriterias_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringCriterias_ScoringGroups_ScoringGroupId",
                        column: x => x.ScoringGroupId,
                        principalTable: "ScoringGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoringCriterias_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentRuleHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssignmentRuleId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityTitle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PreviousAssignedUserId = table.Column<int>(type: "integer", nullable: true),
                    NewAssignedUserId = table.Column<int>(type: "integer", nullable: true),
                    ExecutionResult = table.Column<string>(type: "text", nullable: false),
                    ExecutionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExecutionTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TriggeredByUserId = table.Column<int>(type: "integer", nullable: true),
                    TriggerEventSource = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentRuleHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_AssignmentRules_AssignmentRuleId",
                        column: x => x.AssignmentRuleId,
                        principalTable: "AssignmentRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_TenantUsers_NewAssignedUserId",
                        column: x => x.NewAssignedUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_TenantUsers_PreviousAssignedUserId",
                        column: x => x.PreviousAssignedUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_TenantUsers_TriggeredByUserId",
                        column: x => x.TriggeredByUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentRulesSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    LogicalOperator = table.Column<string>(type: "text", nullable: false),
                    AssignmentRuleId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentRulesSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentRulesSets_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRulesSets_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRulesSets_AssignmentRules_AssignmentRuleId",
                        column: x => x.AssignmentRuleId,
                        principalTable: "AssignmentRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentRulesSets_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoteId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteReactions_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NoteReactions_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NoteReactions_EntityNotes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "EntityNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteReactions_TenantUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimaryOrganizationId = table.Column<int>(type: "integer", nullable: false),
                    RelatedOrganizationId = table.Column<int>(type: "integer", nullable: false),
                    RelationshipType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationRelationships_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrganizationRelationships_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrganizationRelationships_Organizations_PrimaryOrganization~",
                        column: x => x.PrimaryOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationRelationships_Organizations_RelatedOrganization~",
                        column: x => x.RelatedOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationRelationships_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_People_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_People_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_People_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_People_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_People_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductVariants_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductVariants_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchedulerId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    SpecificDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AllowMultipleBookings = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulerAvailabilities_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerAvailabilities_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerAvailabilities_Schedulers_SchedulerId",
                        column: x => x.SchedulerId,
                        principalTable: "Schedulers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulerAvailabilities_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SequenceSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StepType = table.Column<string>(type: "text", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    DelayDays = table.Column<int>(type: "integer", nullable: false),
                    DelayMinutes = table.Column<int>(type: "integer", nullable: false),
                    IncludeWeekends = table.Column<bool>(type: "boolean", nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    ActivityOwnerId = table.Column<int>(type: "integer", nullable: false),
                    SequenceId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SequenceSteps_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SequenceSteps_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SequenceSteps_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SequenceSteps_Sequences_SequenceId",
                        column: x => x.SequenceId,
                        principalTable: "Sequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SequenceSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoringRuleConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScoringCriteriaId = table.Column<int>(type: "integer", nullable: false),
                    ConditionType = table.Column<string>(type: "text", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    ComparisonValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    LogicalOperator = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringRuleConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoringRuleConditions_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringRuleConditions_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScoringRuleConditions_ScoringCriterias_ScoringCriteriaId",
                        column: x => x.ScoringCriteriaId,
                        principalTable: "ScoringCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoringRuleConditions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentRuleConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    LogicalOperator = table.Column<int>(type: "integer", nullable: false),
                    AssignmentRulesSetId = table.Column<int>(type: "integer", nullable: false),
                    Field = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ValueTo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentRuleConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleConditions_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleConditions_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleConditions_AssignmentRulesSets_AssignmentRule~",
                        column: x => x.AssignmentRulesSetId,
                        principalTable: "AssignmentRulesSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentRuleConditions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    LeadId = table.Column<int>(type: "integer", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TaxType = table.Column<string>(type: "text", nullable: false),
                    Probability = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    ScorePercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    LastScoredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SourceOrigin = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SourceChannel = table.Column<string>(type: "text", nullable: false),
                    SourceChannelId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PipelineStageId = table.Column<int>(type: "integer", nullable: true),
                    PipelineId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    WonLossReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deals_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Deals_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Deals_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Deals_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Deals_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Deals_PipelineStages_PipelineStageId",
                        column: x => x.PipelineStageId,
                        principalTable: "PipelineStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityParticipants_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityParticipants_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityParticipants_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityParticipants_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailType = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonEmails_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PersonEmails_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PersonEmails_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonEmails_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Extension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PhoneType = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonPhones_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PersonPhones_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PersonPhones_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonPhones_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    PersonId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectBoardId = table.Column<int>(type: "integer", nullable: false),
                    ProjectPhaseId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectBoards_ProjectBoardId",
                        column: x => x.ProjectBoardId,
                        principalTable: "ProjectBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectPhases_ProjectPhaseId",
                        column: x => x.ProjectPhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SlotType = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AllowMultipleBookings = table.Column<bool>(type: "boolean", nullable: false),
                    SchedulerId = table.Column<int>(type: "integer", nullable: true),
                    AvailabilityId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulerSlots_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerSlots_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerSlots_SchedulerAvailabilities_AvailabilityId",
                        column: x => x.AvailabilityId,
                        principalTable: "SchedulerAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulerSlots_Schedulers_SchedulerId",
                        column: x => x.SchedulerId,
                        principalTable: "Schedulers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulerSlots_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitySequenceEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    CurrentStepId = table.Column<int>(type: "integer", nullable: true),
                    EnrolledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SequenceId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySequenceEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitySequenceEnrollments_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntitySequenceEnrollments_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntitySequenceEnrollments_SequenceSteps_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "SequenceSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntitySequenceEnrollments_Sequences_SequenceId",
                        column: x => x.SequenceId,
                        principalTable: "Sequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitySequenceEnrollments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealInstallments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BillingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealInstallments_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealInstallments_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealInstallments_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealInstallments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    BillingStartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DiscountType = table.Column<string>(type: "text", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TaxPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AdditionalDiscount = table.Column<string>(type: "jsonb", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealProducts_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealProducts_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealProducts_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealProducts_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealProducts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealStageHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    PipelineStageId = table.Column<int>(type: "integer", nullable: false),
                    PipelineId = table.Column<int>(type: "integer", nullable: false),
                    EnteredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExitedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PreviousStageId = table.Column<int>(type: "integer", nullable: true),
                    NextStageId = table.Column<int>(type: "integer", nullable: true),
                    IsCurrentStage = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealStageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_PipelineStages_NextStageId",
                        column: x => x.NextStageId,
                        principalTable: "PipelineStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_PipelineStages_PipelineStageId",
                        column: x => x.PipelineStageId,
                        principalTable: "PipelineStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_PipelineStages_PreviousStageId",
                        column: x => x.PreviousStageId,
                        principalTable: "PipelineStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealStageHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    DealId = table.Column<int>(type: "integer", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ExpectedCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SourceOrigin = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SourceChannel = table.Column<string>(type: "text", nullable: false),
                    SourceChannelId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    EntityStatus = table.Column<string>(type: "text", nullable: false),
                    SuspendedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResumedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leads_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leads_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leads_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leads_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leads_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leads_TenantUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDeals_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectDeals_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectDeals_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectDeals_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectDeals_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchedulerSlotId = table.Column<int>(type: "integer", nullable: true),
                    BookerEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BookerName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BookerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BookingFormData = table.Column<string>(type: "jsonb", nullable: true),
                    BookingSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReminderSent = table.Column<bool>(type: "boolean", nullable: false),
                    ReminderSentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelledById = table.Column<int>(type: "integer", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RescheduledFromBookingId = table.Column<int>(type: "integer", nullable: true),
                    RescheduledToBookingId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_SchedulerBookings_RescheduledFromBookingId",
                        column: x => x.RescheduledFromBookingId,
                        principalTable: "SchedulerBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_SchedulerBookings_RescheduledToBookingId",
                        column: x => x.RescheduledToBookingId,
                        principalTable: "SchedulerBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_SchedulerSlots_SchedulerSlotId",
                        column: x => x.SchedulerSlotId,
                        principalTable: "SchedulerSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_TenantUsers_CancelledById",
                        column: x => x.CancelledById,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchedulerBookings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Done = table.Column<bool>(type: "boolean", nullable: false),
                    StartAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ConferenceUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    VisibilityOnCalendar = table.Column<string>(type: "text", nullable: false),
                    AssignedById = table.Column<int>(type: "integer", nullable: true),
                    AssignedToId = table.Column<int>(type: "integer", nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    SequenceStepId = table.Column<int>(type: "integer", nullable: true),
                    SchedulerBookingId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    TenantUserId = table.Column<int>(type: "integer", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_AspNetUsers_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_SchedulerBookings_SchedulerBookingId",
                        column: x => x.SchedulerBookingId,
                        principalTable: "SchedulerBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_SequenceSteps_SequenceStepId",
                        column: x => x.SequenceStepId,
                        principalTable: "SequenceSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_TenantUsers_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_TenantUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activities_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityActivityParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: true),
                    GuestEmail = table.Column<string>(type: "text", nullable: true),
                    GuestName = table.Column<string>(type: "text", nullable: true),
                    GuestPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityActivityParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityActivityParticipants_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityActivityParticipants_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityActivityParticipants_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntityActivityParticipants_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsSystemRole", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, "Full system access across all tenants", true, "SuperAdmin", "SUPERADMIN" },
                    { 2, null, "Full access within assigned tenant", true, "TenantAdmin", "TENANTADMIN" },
                    { 3, null, "Regular user access within assigned tenant", true, "NonTenantAdmin", "NONTENANTADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "BillingCycle", "CreatedBy", "Currency", "Description", "IsActive", "LastModifiedBy", "MaxChannels", "MaxFacebookChannels", "MaxInstagramChannels", "MaxTelegramChannels", "MaxUsers", "MaxWhatsAppChannels", "Name", "PaymentProviderPriceId", "PaymentProviderProductId", "Price", "Type" },
                values: new object[,]
                {
                    { 1, "Monthly", null, "usd", "Basic plan with limited features", true, null, 1, 0, 0, 0, 2, 1, "Free", "price_free", "", 0m, "Free" },
                    { 2, "Monthly", null, "usd", "Starter plan with basic features", true, null, 3, 1, 1, 1, 5, 2, "Starter Plan - Monthly", "price_1S1lFgDVRyfs46JiBJyvA5eu", "prod_SxgcF8F4u3unNk", 29.99m, "Starter" },
                    { 3, "Yearly", null, "usd", "Starter plan with basic features", true, null, 3, 1, 1, 1, 5, 2, "Starter Plan - Yearly", "price_1S1lHtDVRyfs46JizuWqnOp2", "prod_Sxgf0MdFfTzUXR", 299.99m, "Starter" },
                    { 4, "Monthly", null, "usd", "Professional plan with advanced features", true, null, 10, 3, 3, 3, 25, 5, "Professional Plan - Monthly", "price_1S1lIXDVRyfs46Jirxqm0dz6", "prod_SxgfAcz4HHgFcY", 99.99m, "Pro" },
                    { 5, "Yearly", null, "usd", "Professional plan with advanced features", true, null, 10, 3, 3, 3, 25, 5, "Professional Plan - Yearly", "price_1S1lJ3DVRyfs46Ji40RP91Sk", "prod_SxggEc36SZchwA", 999.99m, "Pro" },
                    { 6, "Monthly", null, "usd", "Enterprise plan with all features", true, null, 50, 15, 15, 15, 100, 20, "Enterprise Plan - Monthly", "price_1S1lJgDVRyfs46JidlIn73va", "prod_Sxgh4Ucpw7IxSG", 299.99m, "Enterprise" },
                    { 7, "Yearly", null, "usd", "Enterprise plan with all features", true, null, 50, 15, 15, 15, 100, 20, "Enterprise Plan - Yearly", "price_1S1lKVDVRyfs46Ji1DJXRhHp", "prod_SxghGjm7I9Ugag", 2999.99m, "Enterprise" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CreatedBy",
                table: "Activities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DeletedBy",
                table: "Activities",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LastModifiedBy",
                table: "Activities",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PublicId",
                table: "Activities",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TenantUserId",
                table: "Activities",
                column: "TenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_AssignedById",
                table: "Activities",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_AssignedToId",
                table: "Activities",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_SchedulerBookingId",
                table: "Activities",
                column: "SchedulerBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_SequenceStepId",
                table: "Activities",
                column: "SequenceStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TenantId_AssignedToId_Done",
                table: "Activities",
                columns: new[] { "TenantId", "AssignedToId", "Done" });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TenantId_EndAt",
                table: "Activities",
                columns: new[] { "TenantId", "EndAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TenantId_EntityType_EntityId",
                table: "Activities",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TenantId_StartAt",
                table: "Activities",
                columns: new[] { "TenantId", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_TenantId_Type",
                table: "Activities",
                columns: new[] { "TenantId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivity_TenantId",
                table: "Activities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivity_TenantId_Created",
                table: "Activities",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivity_TenantId_IsDeleted",
                table: "Activities",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_Name",
                table: "AspNetRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PublicId",
                table: "AspNetUsers",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleCondition_TenantId",
                table: "AssignmentRuleConditions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleCondition_TenantId_Created",
                table: "AssignmentRuleConditions",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleConditions_AssignmentRulesSetId",
                table: "AssignmentRuleConditions",
                column: "AssignmentRulesSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleConditions_CreatedBy",
                table: "AssignmentRuleConditions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleConditions_LastModifiedBy",
                table: "AssignmentRuleConditions",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleConditions_PublicId",
                table: "AssignmentRuleConditions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_AssignmentRuleId",
                table: "AssignmentRuleHistories",
                column: "AssignmentRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_CreatedBy",
                table: "AssignmentRuleHistories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_LastModifiedBy",
                table: "AssignmentRuleHistories",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_NewAssignedUserId",
                table: "AssignmentRuleHistories",
                column: "NewAssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_PreviousAssignedUserId",
                table: "AssignmentRuleHistories",
                column: "PreviousAssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_PublicId",
                table: "AssignmentRuleHistories",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistories_TriggeredByUserId",
                table: "AssignmentRuleHistories",
                column: "TriggeredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistory_TenantId",
                table: "AssignmentRuleHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRuleHistory_TenantId_Created",
                table: "AssignmentRuleHistories",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRule_TenantId",
                table: "AssignmentRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRule_TenantId_Created",
                table: "AssignmentRules",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRules_AssignToUserId",
                table: "AssignmentRules",
                column: "AssignToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRules_CreatedBy",
                table: "AssignmentRules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRules_LastModifiedBy",
                table: "AssignmentRules",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRules_PublicId",
                table: "AssignmentRules",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRulesSet_TenantId",
                table: "AssignmentRulesSets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRulesSet_TenantId_Created",
                table: "AssignmentRulesSets",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRulesSets_AssignmentRuleId",
                table: "AssignmentRulesSets",
                column: "AssignmentRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRulesSets_CreatedBy",
                table: "AssignmentRulesSets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRulesSets_LastModifiedBy",
                table: "AssignmentRulesSets",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentRulesSets_PublicId",
                table: "AssignmentRulesSets",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_TenantId_EntityType_EntityId",
                table: "ChangeLogs",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLogs_CreatedBy",
                table: "ChangeLogs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLogs_LastModifiedBy",
                table: "ChangeLogs",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLogs_PublicId",
                table: "ChangeLogs",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityChangeLog_TenantId",
                table: "ChangeLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityChangeLog_TenantId_Created",
                table: "ChangeLogs",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccount_TenantId",
                table: "ChannelAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccount_TenantId_Created",
                table: "ChannelAccounts",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccount_TenantId_EntityStatus",
                table: "ChannelAccounts",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccount_TenantId_IsDeleted",
                table: "ChannelAccounts",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccount_TenantId_ProviderAccountId",
                table: "ChannelAccounts",
                columns: new[] { "TenantId", "ProviderAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccounts_CreatedBy",
                table: "ChannelAccounts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccounts_DeletedBy",
                table: "ChannelAccounts",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccounts_LastModifiedBy",
                table: "ChannelAccounts",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAccounts_PublicId",
                table: "ChannelAccounts",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DealInstallment_TenantId",
                table: "DealInstallments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DealInstallment_TenantId_Created",
                table: "DealInstallments",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_DealInstallments_CreatedBy",
                table: "DealInstallments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DealInstallments_DealId",
                table: "DealInstallments",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealInstallments_LastModifiedBy",
                table: "DealInstallments",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DealInstallments_PublicId",
                table: "DealInstallments",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DealProduct_DealId",
                table: "DealProducts",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealProduct_ProductId",
                table: "DealProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DealProduct_ProductVariantId",
                table: "DealProducts",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_DealProduct_TenantId",
                table: "DealProducts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DealProduct_TenantId_Created",
                table: "DealProducts",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_DealProduct_TenantId_ProductId",
                table: "DealProducts",
                columns: new[] { "TenantId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_DealProducts_CreatedBy",
                table: "DealProducts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DealProducts_LastModifiedBy",
                table: "DealProducts",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DealProducts_PublicId",
                table: "DealProducts",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deal_OrganizationId",
                table: "Deals",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_OwnerId",
                table: "Deals",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_PersonId",
                table: "Deals",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_PipelineId",
                table: "Deals",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_PipelineStageId",
                table: "Deals",
                column: "PipelineStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId",
                table: "Deals",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId_Created",
                table: "Deals",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId_EntityStatus",
                table: "Deals",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId_IsDeleted",
                table: "Deals",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId_OwnerId_Status",
                table: "Deals",
                columns: new[] { "TenantId", "OwnerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId_PipelineId_StageId",
                table: "Deals",
                columns: new[] { "TenantId", "PipelineId", "PipelineStageId" });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_TenantId_Status",
                table: "Deals",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_CreatedBy",
                table: "Deals",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_DeletedBy",
                table: "Deals",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_LastModifiedBy",
                table: "Deals",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_PublicId",
                table: "Deals",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistories_CreatedBy",
                table: "DealStageHistories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistories_LastModifiedBy",
                table: "DealStageHistories",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistories_PublicId",
                table: "DealStageHistories",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_DealId",
                table: "DealStageHistories",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_DealId_EnteredAt",
                table: "DealStageHistories",
                columns: new[] { "DealId", "EnteredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_EnteredAt",
                table: "DealStageHistories",
                column: "EnteredAt");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_NextStageId",
                table: "DealStageHistories",
                column: "NextStageId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_PipelineId",
                table: "DealStageHistories",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_PipelineStageId",
                table: "DealStageHistories",
                column: "PipelineStageId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_PreviousStageId",
                table: "DealStageHistories",
                column: "PreviousStageId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_TenantId",
                table: "DealStageHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStageHistory_TenantId_Created",
                table: "DealStageHistories",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivityParticipants_ActivityId",
                table: "EntityActivityParticipants",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivityParticipants_CreatedBy",
                table: "EntityActivityParticipants",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivityParticipants_LastModifiedBy",
                table: "EntityActivityParticipants",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivityParticipants_PersonId",
                table: "EntityActivityParticipants",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivityParticipants_PublicId",
                table: "EntityActivityParticipants",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TenantId_EntityType_EntityId",
                table: "EntityComments",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityComment_TenantId",
                table: "EntityComments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityComment_TenantId_Created",
                table: "EntityComments",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityComment_TenantId_IsDeleted",
                table: "EntityComments",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityComments_AuthorId",
                table: "EntityComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityComments_CreatedBy",
                table: "EntityComments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityComments_DeletedBy",
                table: "EntityComments",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityComments_LastModifiedBy",
                table: "EntityComments",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityComments_PublicId",
                table: "EntityComments",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Document_TenantId_EntityType_EntityId",
                table: "EntityDocuments",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocument_TenantId",
                table: "EntityDocuments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocument_TenantId_Created",
                table: "EntityDocuments",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocument_TenantId_IsDeleted",
                table: "EntityDocuments",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocuments_CreatedBy",
                table: "EntityDocuments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocuments_DeletedBy",
                table: "EntityDocuments",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocuments_LastModifiedBy",
                table: "EntityDocuments",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDocuments_PublicId",
                table: "EntityDocuments",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_TenantId",
                table: "EntityFiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_TenantId_Created",
                table: "EntityFiles",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_TenantId_IsDeleted",
                table: "EntityFiles",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityFiles_CreatedBy",
                table: "EntityFiles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFiles_DeletedBy",
                table: "EntityFiles",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFiles_LastModifiedBy",
                table: "EntityFiles",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFiles_PublicId",
                table: "EntityFiles",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_TenantId_EntityType_EntityId",
                table: "EntityFiles",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityImage_TenantId",
                table: "EntityImages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImage_TenantId_Created",
                table: "EntityImages",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityImage_TenantId_IsDeleted",
                table: "EntityImages",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_CreatedBy",
                table: "EntityImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_DeletedBy",
                table: "EntityImages",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_LastModifiedBy",
                table: "EntityImages",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityImages_PublicId",
                table: "EntityImages",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_TenantId_EntityType_EntityId",
                table: "EntityImages",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityLabel_EntityType_EntityId",
                table: "EntityLabels",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityLabels_AssignedBy",
                table: "EntityLabels",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLabels_CreatedBy",
                table: "EntityLabels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLabels_LabelId",
                table: "EntityLabels",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLabels_LastModifiedBy",
                table: "EntityLabels",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLabels_PublicId",
                table: "EntityLabels",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityNote_TenantId",
                table: "EntityNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityNote_TenantId_Created",
                table: "EntityNotes",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityNote_TenantId_IsDeleted",
                table: "EntityNotes",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityNotes_AuthorId",
                table: "EntityNotes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityNotes_CreatedBy",
                table: "EntityNotes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityNotes_DeletedBy",
                table: "EntityNotes",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityNotes_LastModifiedBy",
                table: "EntityNotes",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityNotes_PublicId",
                table: "EntityNotes",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_TenantId_EntityType_EntityId",
                table: "EntityNotes",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityParticipant_TenantId",
                table: "EntityParticipants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityParticipant_TenantId_Created",
                table: "EntityParticipants",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityParticipants_CreatedBy",
                table: "EntityParticipants",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityParticipants_LastModifiedBy",
                table: "EntityParticipants",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityParticipants_PersonId",
                table: "EntityParticipants",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityParticipants_PublicId",
                table: "EntityParticipants",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_TenantId_EntityType_EntityId",
                table: "EntityParticipants",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityPrice_TenantId",
                table: "EntityPrices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPrice_TenantId_Created",
                table: "EntityPrices",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityPrices_CreatedBy",
                table: "EntityPrices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPrices_LastModifiedBy",
                table: "EntityPrices",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPrices_PublicId",
                table: "EntityPrices",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Price_TenantId_EntityType_EntityId",
                table: "EntityPrices",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Entity_Sequence_Enrollment_TenantId_EntityType_EntityId",
                table: "EntitySequenceEnrollments",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollment_TenantId",
                table: "EntitySequenceEnrollments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollment_TenantId_Created",
                table: "EntitySequenceEnrollments",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollments_CreatedBy",
                table: "EntitySequenceEnrollments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollments_CurrentStepId",
                table: "EntitySequenceEnrollments",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollments_LastModifiedBy",
                table: "EntitySequenceEnrollments",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollments_PublicId",
                table: "EntitySequenceEnrollments",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntitySequenceEnrollments_SequenceId",
                table: "EntitySequenceEnrollments",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Label_TenantId",
                table: "Labels",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Label_TenantId_Created",
                table: "Labels",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedBy",
                table: "Labels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_LastModifiedBy",
                table: "Labels",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_PublicId",
                table: "Labels",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lead_OrganizationId",
                table: "Leads",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_OwnerId",
                table: "Leads",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_PersonId",
                table: "Leads",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId",
                table: "Leads",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId_Created",
                table: "Leads",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId_EntityStatus",
                table: "Leads",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId_IsDeleted",
                table: "Leads",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId_SourceChannel",
                table: "Leads",
                columns: new[] { "TenantId", "SourceChannel" });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CreatedBy",
                table: "Leads",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_DealId",
                table: "Leads",
                column: "DealId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_DeletedBy",
                table: "Leads",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_LastModifiedBy",
                table: "Leads",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_PublicId",
                table: "Leads",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoteReactions_CreatedBy",
                table: "NoteReactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NoteReactions_LastModifiedBy",
                table: "NoteReactions",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NoteReactions_NoteId",
                table: "NoteReactions",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteReactions_PublicId",
                table: "NoteReactions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoteReactions_UserId",
                table: "NoteReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationship_TenantId",
                table: "OrganizationRelationships",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationship_TenantId_Created",
                table: "OrganizationRelationships",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationships_CreatedBy",
                table: "OrganizationRelationships",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationships_LastModifiedBy",
                table: "OrganizationRelationships",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationships_PrimaryOrganizationId",
                table: "OrganizationRelationships",
                column: "PrimaryOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationships_PublicId",
                table: "OrganizationRelationships",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRelationships_RelatedOrganizationId",
                table: "OrganizationRelationships",
                column: "RelatedOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Industry",
                table: "Organizations",
                column: "Industry");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organizations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_NumberOfEmployees",
                table: "Organizations",
                column: "NumberOfEmployees");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_OwnerId",
                table: "Organizations",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TenantId",
                table: "Organizations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TenantId_Created",
                table: "Organizations",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TenantId_EntityStatus",
                table: "Organizations",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TenantId_IsDeleted",
                table: "Organizations",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreatedBy",
                table: "Organizations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_DeletedBy",
                table: "Organizations",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_LastModifiedBy",
                table: "Organizations",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_PublicId",
                table: "Organizations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_People_CreatedBy",
                table: "People",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_People_DeletedBy",
                table: "People",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_People_LastModifiedBy",
                table: "People",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_People_PublicId",
                table: "People",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_OrganizationId",
                table: "People",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_OwnerId",
                table: "People",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_TenantId",
                table: "People",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_TenantId_Created",
                table: "People",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Person_TenantId_EntityStatus",
                table: "People",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Person_TenantId_IsDeleted",
                table: "People",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Person_TenantId_Name",
                table: "People",
                columns: new[] { "TenantId", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmail_TenantId",
                table: "PersonEmails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmail_TenantId_Created",
                table: "PersonEmails",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_CreatedBy",
                table: "PersonEmails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_LastModifiedBy",
                table: "PersonEmails",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_PersonId",
                table: "PersonEmails",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_PublicId",
                table: "PersonEmails",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhone_TenantId",
                table: "PersonPhones",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhone_TenantId_Created",
                table: "PersonPhones",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_CreatedBy",
                table: "PersonPhones",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_LastModifiedBy",
                table: "PersonPhones",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_PersonId",
                table: "PersonPhones",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_PublicId",
                table: "PersonPhones",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_Name",
                table: "Pipelines",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_ScoringProfileId",
                table: "Pipelines",
                column: "ScoringProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_SortOrder",
                table: "Pipelines",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_TenantId",
                table: "Pipelines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_TenantId_Created",
                table: "Pipelines",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_TenantId_EntityStatus",
                table: "Pipelines",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Pipeline_TenantId_IsDeleted",
                table: "Pipelines",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_CreatedBy",
                table: "Pipelines",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_DeletedBy",
                table: "Pipelines",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_LastModifiedBy",
                table: "Pipelines",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_PublicId",
                table: "Pipelines",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_Name",
                table: "PipelineStages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_PipelineId",
                table: "PipelineStages",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_PipelineId_SortOrder",
                table: "PipelineStages",
                columns: new[] { "PipelineId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_SortOrder",
                table: "PipelineStages",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_TenantId",
                table: "PipelineStages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_TenantId_Created",
                table: "PipelineStages",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_TenantId_EntityStatus",
                table: "PipelineStages",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStage_TenantId_IsDeleted",
                table: "PipelineStages",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStages_CreatedBy",
                table: "PipelineStages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStages_DeletedBy",
                table: "PipelineStages",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStages_LastModifiedBy",
                table: "PipelineStages",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStages_PublicId",
                table: "PipelineStages",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_CreatedBy",
                table: "Plans",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_LastModifiedBy",
                table: "Plans",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_PublicId",
                table: "Plans",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CreatedBy",
                table: "ProductCategories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_LastModifiedBy",
                table: "ProductCategories",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_PublicId",
                table: "ProductCategories",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_Name",
                table: "ProductCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_SortOrder",
                table: "ProductCategories",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_TenantId",
                table: "ProductCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_TenantId_Created",
                table: "ProductCategories",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_BillingFrequency",
                table: "Products",
                column: "BillingFrequency");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Code",
                table: "Products",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Product_OwnerId",
                table: "Products",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TenantId_Created",
                table: "Products",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_TenantId_EntityStatus",
                table: "Products",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_TenantId_IsDeleted",
                table: "Products",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedBy",
                table: "Products",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DeletedBy",
                table: "Products",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LastModifiedBy",
                table: "Products",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PublicId",
                table: "Products",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_Name",
                table: "ProductVariants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductId_Name",
                table: "ProductVariants",
                columns: new[] { "ProductId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_TenantId",
                table: "ProductVariants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_TenantId_Created",
                table: "ProductVariants",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_TenantId_IsDeleted",
                table: "ProductVariants",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_CreatedBy",
                table: "ProductVariants",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_DeletedBy",
                table: "ProductVariants",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_LastModifiedBy",
                table: "ProductVariants",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_PublicId",
                table: "ProductVariants",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoard_Name",
                table: "ProjectBoards",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoard_SortOrder",
                table: "ProjectBoards",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoard_TenantId",
                table: "ProjectBoards",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoard_TenantId_Created",
                table: "ProjectBoards",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoard_TenantId_EntityStatus",
                table: "ProjectBoards",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoard_TenantId_IsDeleted",
                table: "ProjectBoards",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoards_CreatedBy",
                table: "ProjectBoards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoards_DeletedBy",
                table: "ProjectBoards",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoards_LastModifiedBy",
                table: "ProjectBoards",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoards_PublicId",
                table: "ProjectBoards",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeal_TenantId",
                table: "ProjectDeals",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeal_TenantId_Created",
                table: "ProjectDeals",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeals_CreatedBy",
                table: "ProjectDeals",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeals_DealId",
                table: "ProjectDeals",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeals_LastModifiedBy",
                table: "ProjectDeals",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeals_ProjectId",
                table: "ProjectDeals",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeals_PublicId",
                table: "ProjectDeals",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_Name",
                table: "ProjectPhases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_ProjectBoardId",
                table: "ProjectPhases",
                column: "ProjectBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_ProjectBoardId_SortOrder",
                table: "ProjectPhases",
                columns: new[] { "ProjectBoardId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_SortOrder",
                table: "ProjectPhases",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_TenantId",
                table: "ProjectPhases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_TenantId_Created",
                table: "ProjectPhases",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_TenantId_EntityStatus",
                table: "ProjectPhases",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhase_TenantId_IsDeleted",
                table: "ProjectPhases",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_CreatedBy",
                table: "ProjectPhases",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_DeletedBy",
                table: "ProjectPhases",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_LastModifiedBy",
                table: "ProjectPhases",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_PublicId",
                table: "ProjectPhases",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_OrganizationId",
                table: "Projects",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_PersonId",
                table: "Projects",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Priority",
                table: "Projects",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectBoardId",
                table: "Projects",
                column: "ProjectBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectPhaseId",
                table: "Projects",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Status",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Status_Priority",
                table: "Projects",
                columns: new[] { "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_TenantId",
                table: "Projects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_TenantId_Created",
                table: "Projects",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_TenantId_EntityStatus",
                table: "Projects",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_TenantId_IsDeleted",
                table: "Projects",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DeletedBy",
                table: "Projects",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastModifiedBy",
                table: "Projects",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PublicId",
                table: "Projects",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_AssigneeId",
                table: "ProjectTasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ParentTaskId",
                table: "ProjectTasks",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ProjectPhaseId",
                table: "ProjectTasks",
                column: "ProjectPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_TenantId",
                table: "ProjectTasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_TenantId_AssigneeId_Status",
                table: "ProjectTasks",
                columns: new[] { "TenantId", "AssigneeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_TenantId_Created",
                table: "ProjectTasks",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_TenantId_IsDeleted",
                table: "ProjectTasks",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_TenantId_Status",
                table: "ProjectTasks",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_CreatedBy",
                table: "ProjectTasks",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_DeletedBy",
                table: "ProjectTasks",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_LastModifiedBy",
                table: "ProjectTasks",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_PublicId",
                table: "ProjectTasks",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerAvailabilities_CreatedBy",
                table: "SchedulerAvailabilities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerAvailabilities_LastModifiedBy",
                table: "SchedulerAvailabilities",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerAvailabilities_PublicId",
                table: "SchedulerAvailabilities",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerAvailabilities_SchedulerId",
                table: "SchedulerAvailabilities",
                column: "SchedulerId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerAvailability_TenantId",
                table: "SchedulerAvailabilities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerAvailability_TenantId_Created",
                table: "SchedulerAvailabilities",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBooking_TenantId",
                table: "SchedulerBookings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBooking_TenantId_Created",
                table: "SchedulerBookings",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_CancelledById",
                table: "SchedulerBookings",
                column: "CancelledById");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_CreatedBy",
                table: "SchedulerBookings",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_LastModifiedBy",
                table: "SchedulerBookings",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_PublicId",
                table: "SchedulerBookings",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_RescheduledFromBookingId",
                table: "SchedulerBookings",
                column: "RescheduledFromBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_RescheduledToBookingId",
                table: "SchedulerBookings",
                column: "RescheduledToBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerBookings_SchedulerSlotId",
                table: "SchedulerBookings",
                column: "SchedulerSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Scheduler_TenantId",
                table: "Schedulers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Scheduler_TenantId_Created",
                table: "Schedulers",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Schedulers_CreatedBy",
                table: "Schedulers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Schedulers_LastModifiedBy",
                table: "Schedulers",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Schedulers_OwnerId",
                table: "Schedulers",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedulers_PublicId",
                table: "Schedulers",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlot_TenantId",
                table: "SchedulerSlots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlot_TenantId_Created",
                table: "SchedulerSlots",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlots_AvailabilityId",
                table: "SchedulerSlots",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlots_CreatedBy",
                table: "SchedulerSlots",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlots_LastModifiedBy",
                table: "SchedulerSlots",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlots_PublicId",
                table: "SchedulerSlots",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerSlots_SchedulerId",
                table: "SchedulerSlots",
                column: "SchedulerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringCriteria_TenantId",
                table: "ScoringCriterias",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringCriteria_TenantId_Created",
                table: "ScoringCriterias",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringCriterias_CreatedBy",
                table: "ScoringCriterias",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringCriterias_LastModifiedBy",
                table: "ScoringCriterias",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringCriterias_PublicId",
                table: "ScoringCriterias",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoringCriterias_ScoringGroupId",
                table: "ScoringCriterias",
                column: "ScoringGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroup_TenantId",
                table: "ScoringGroups",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroup_TenantId_Created",
                table: "ScoringGroups",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroup_TenantId_EntityStatus",
                table: "ScoringGroups",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroups_CreatedBy",
                table: "ScoringGroups",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroups_LastModifiedBy",
                table: "ScoringGroups",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroups_PublicId",
                table: "ScoringGroups",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoringGroups_ScoringProfileId",
                table: "ScoringGroups",
                column: "ScoringProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfile_TenantId",
                table: "ScoringProfiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfile_TenantId_Created",
                table: "ScoringProfiles",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfile_TenantId_EntityStatus",
                table: "ScoringProfiles",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfile_TenantId_IsDeleted",
                table: "ScoringProfiles",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfiles_CreatedBy",
                table: "ScoringProfiles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfiles_DeletedBy",
                table: "ScoringProfiles",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfiles_LastModifiedBy",
                table: "ScoringProfiles",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringProfiles_PublicId",
                table: "ScoringProfiles",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRuleCondition_TenantId",
                table: "ScoringRuleConditions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRuleCondition_TenantId_Created",
                table: "ScoringRuleConditions",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRuleConditions_CreatedBy",
                table: "ScoringRuleConditions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRuleConditions_LastModifiedBy",
                table: "ScoringRuleConditions",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRuleConditions_PublicId",
                table: "ScoringRuleConditions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRuleConditions_ScoringCriteriaId",
                table: "ScoringRuleConditions",
                column: "ScoringCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sequence_TenantId",
                table: "Sequences",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sequence_TenantId_Created",
                table: "Sequences",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_Sequence_TenantId_EntityStatus",
                table: "Sequences",
                columns: new[] { "TenantId", "EntityStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Sequence_TenantId_IsDeleted",
                table: "Sequences",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_CreatedBy",
                table: "Sequences",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_DeletedBy",
                table: "Sequences",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_LastModifiedBy",
                table: "Sequences",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_OwnerId",
                table: "Sequences",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_PublicId",
                table: "Sequences",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SequenceStep_TenantId",
                table: "SequenceSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceStep_TenantId_Created",
                table: "SequenceSteps",
                columns: new[] { "TenantId", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_SequenceStep_TenantId_IsDeleted",
                table: "SequenceSteps",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_SequenceSteps_CreatedBy",
                table: "SequenceSteps",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceSteps_DeletedBy",
                table: "SequenceSteps",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceSteps_LastModifiedBy",
                table: "SequenceSteps",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceSteps_PublicId",
                table: "SequenceSteps",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SequenceSteps_SequenceId",
                table: "SequenceSteps",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_CanceledAt",
                table: "Subscriptions",
                column: "CanceledAt");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_IsInGracePeriod",
                table: "Subscriptions",
                column: "IsInGracePeriod");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_PaymentProviderSubscriptionId",
                table: "Subscriptions",
                column: "PaymentProviderSubscriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_PlanId",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_Status_CurrentPeriodEnd",
                table: "Subscriptions",
                columns: new[] { "Status", "CurrentPeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_TenantId",
                table: "Subscriptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_TenantId_CurrentPeriodEnd",
                table: "Subscriptions",
                columns: new[] { "TenantId", "CurrentPeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_TenantId_Status",
                table: "Subscriptions",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CreatedBy",
                table: "Subscriptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_LastModifiedBy",
                table: "Subscriptions",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PublicId",
                table: "Subscriptions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CreatedBy",
                table: "Tenants",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_LastModifiedBy",
                table: "Tenants",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_PaymentProviderCustomerId",
                table: "Tenants",
                column: "PaymentProviderCustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_PublicId",
                table: "Tenants",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_AssignedBy",
                table: "TenantUserRoles",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_CreatedBy",
                table: "TenantUserRoles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_LastModifiedBy",
                table: "TenantUserRoles",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_PublicId",
                table: "TenantUserRoles",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantUserId_RoleName",
                table: "TenantUserRoles",
                columns: new[] { "TenantUserId", "RoleName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUser_EntityStatus",
                table: "TenantUsers",
                column: "EntityStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_ApplicationUserId",
                table: "TenantUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_CreatedBy",
                table: "TenantUsers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_LastModifiedBy",
                table: "TenantUsers",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_PublicId",
                table: "TenantUsers",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_TenantId",
                table: "TenantUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_TenantId_ApplicationUserId",
                table: "TenantUsers",
                columns: new[] { "TenantId", "ApplicationUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssignmentRuleConditions");

            migrationBuilder.DropTable(
                name: "AssignmentRuleHistories");

            migrationBuilder.DropTable(
                name: "ChangeLogs");

            migrationBuilder.DropTable(
                name: "ChannelAccounts");

            migrationBuilder.DropTable(
                name: "DealInstallments");

            migrationBuilder.DropTable(
                name: "DealProducts");

            migrationBuilder.DropTable(
                name: "DealStageHistories");

            migrationBuilder.DropTable(
                name: "EntityActivityParticipants");

            migrationBuilder.DropTable(
                name: "EntityComments");

            migrationBuilder.DropTable(
                name: "EntityDocuments");

            migrationBuilder.DropTable(
                name: "EntityFiles");

            migrationBuilder.DropTable(
                name: "EntityImages");

            migrationBuilder.DropTable(
                name: "EntityLabels");

            migrationBuilder.DropTable(
                name: "EntityParticipants");

            migrationBuilder.DropTable(
                name: "EntityPrices");

            migrationBuilder.DropTable(
                name: "EntitySequenceEnrollments");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "NoteReactions");

            migrationBuilder.DropTable(
                name: "OrganizationRelationships");

            migrationBuilder.DropTable(
                name: "PersonEmails");

            migrationBuilder.DropTable(
                name: "PersonPhones");

            migrationBuilder.DropTable(
                name: "ProjectDeals");

            migrationBuilder.DropTable(
                name: "ProjectTasks");

            migrationBuilder.DropTable(
                name: "ScoringRuleConditions");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "TenantUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AssignmentRulesSets");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "EntityNotes");

            migrationBuilder.DropTable(
                name: "Deals");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ScoringCriterias");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "AssignmentRules");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "SchedulerBookings");

            migrationBuilder.DropTable(
                name: "SequenceSteps");

            migrationBuilder.DropTable(
                name: "PipelineStages");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "ProjectPhases");

            migrationBuilder.DropTable(
                name: "ScoringGroups");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "SchedulerSlots");

            migrationBuilder.DropTable(
                name: "Sequences");

            migrationBuilder.DropTable(
                name: "Pipelines");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "ProjectBoards");

            migrationBuilder.DropTable(
                name: "SchedulerAvailabilities");

            migrationBuilder.DropTable(
                name: "ScoringProfiles");

            migrationBuilder.DropTable(
                name: "Schedulers");

            migrationBuilder.DropTable(
                name: "TenantUsers");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
