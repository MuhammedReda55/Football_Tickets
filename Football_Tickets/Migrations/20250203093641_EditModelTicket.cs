using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football_Tickets.Migrations
{
    /// <inheritdoc />
    public partial class EditModelTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sections",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sections",
                table: "Tickets");
        }
    }
}
