using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieVerse.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CastOrder",
                table: "TVShowActors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CastOrder",
                table: "EpisodeActors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CastOrder",
                table: "TVShowActors");

            migrationBuilder.DropColumn(
                name: "CastOrder",
                table: "EpisodeActors");
        }
    }
}
