using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    tel_numbers = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "telefonní číslo"),
                    keycloak_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "ID uživatele v Keycloaku (claim sub)"),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true, comment: "E-mail uživatele (kopie z Keycloaku)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "users_email_id",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_keycloak_id_key",
                table: "users",
                column: "keycloak_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_telnumber_id",
                table: "users",
                column: "tel_numbers",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
