using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CountryStateJwtAuthenticationWebApi.Migrations
{
    /// <inheritdoc />
    public partial class CachDB10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetCoreRegistration_Subhajit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetCoreRegistration_Subhajit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country_AspSubhajit",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country_AspSubhajit", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "MyApp_Roles_AspSubhajit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyApp_Roles_AspSubhajit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "State_AspSubhajit",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State_AspSubhajit", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "MyApp_UserClaims_AspSubhajit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyApp_UserClaims_AspSubhajit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyApp_UserClaims_AspSubhajit_AspNetCoreRegistration_Subhajit_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetCoreRegistration_Subhajit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyApp_UserLogins_AspSubhajit",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyApp_UserLogins_AspSubhajit", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_MyApp_UserLogins_AspSubhajit_AspNetCoreRegistration_Subhajit_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetCoreRegistration_Subhajit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyApp_UserTokens_AspSubhajit",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyApp_UserTokens_AspSubhajit", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_MyApp_UserTokens_AspSubhajit_AspNetCoreRegistration_Subhajit_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetCoreRegistration_Subhajit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyApp_RoleClaims_AspSubhajit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyApp_RoleClaims_AspSubhajit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyApp_RoleClaims_AspSubhajit_MyApp_Roles_AspSubhajit_RoleId",
                        column: x => x.RoleId,
                        principalTable: "MyApp_Roles_AspSubhajit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MyApp_UserRoles_AspSubhajit",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyApp_UserRoles_AspSubhajit", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_MyApp_UserRoles_AspSubhajit_AspNetCoreRegistration_Subhajit_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetCoreRegistration_Subhajit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MyApp_UserRoles_AspSubhajit_MyApp_Roles_AspSubhajit_RoleId",
                        column: x => x.RoleId,
                        principalTable: "MyApp_Roles_AspSubhajit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetCoreRegistration_Subhajit",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetCoreRegistration_Subhajit",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MyApp_RoleClaims_AspSubhajit_RoleId",
                table: "MyApp_RoleClaims_AspSubhajit",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "MyApp_Roles_AspSubhajit",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MyApp_UserClaims_AspSubhajit_UserId",
                table: "MyApp_UserClaims_AspSubhajit",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MyApp_UserLogins_AspSubhajit_UserId",
                table: "MyApp_UserLogins_AspSubhajit",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MyApp_UserRoles_AspSubhajit_RoleId",
                table: "MyApp_UserRoles_AspSubhajit",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Country_AspSubhajit");

            migrationBuilder.DropTable(
                name: "MyApp_RoleClaims_AspSubhajit");

            migrationBuilder.DropTable(
                name: "MyApp_UserClaims_AspSubhajit");

            migrationBuilder.DropTable(
                name: "MyApp_UserLogins_AspSubhajit");

            migrationBuilder.DropTable(
                name: "MyApp_UserRoles_AspSubhajit");

            migrationBuilder.DropTable(
                name: "MyApp_UserTokens_AspSubhajit");

            migrationBuilder.DropTable(
                name: "State_AspSubhajit");

            migrationBuilder.DropTable(
                name: "MyApp_Roles_AspSubhajit");

            migrationBuilder.DropTable(
                name: "AspNetCoreRegistration_Subhajit");
        }
    }
}
