using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapoteSolution.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddrelationOneToManyCustomerCopier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Copiers_CustomerId",
                table: "Copiers");

            migrationBuilder.CreateIndex(
                name: "IX_Copiers_CustomerId",
                table: "Copiers",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Copiers_CustomerId",
                table: "Copiers");

            migrationBuilder.CreateIndex(
                name: "IX_Copiers_CustomerId",
                table: "Copiers",
                column: "CustomerId",
                unique: true);
        }
    }
}
