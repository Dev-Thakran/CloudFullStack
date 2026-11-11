using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFullStackAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isElectric",
                table: "Vehicle",
                newName: "IsElectric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsElectric",
                table: "Vehicle",
                newName: "isElectric");
        }
    }
}
