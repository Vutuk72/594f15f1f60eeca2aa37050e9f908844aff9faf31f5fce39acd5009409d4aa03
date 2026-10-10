using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class PrejmenovaniIndexu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "users_telnumber_id",
                table: "users",
                newName: "users_telnumber_key");

            migrationBuilder.RenameIndex(
                name: "users_email_id",
                table: "users",
                newName: "users_email_key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "users_telnumber_key",
                table: "users",
                newName: "users_telnumber_id");

            migrationBuilder.RenameIndex(
                name: "users_email_key",
                table: "users",
                newName: "users_email_id");
        }
    }
}
