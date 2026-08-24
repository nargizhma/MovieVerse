using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieVerse.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Children",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeathDate",
                table: "DirectorDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeathPlace",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherWorks",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parents",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Relatives",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Spouse",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Trademark",
                table: "DirectorDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BirthPlace",
                table: "ActorDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlternativeName",
                table: "ActorDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeathDate",
                table: "ActorDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeathPlace",
                table: "ActorDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Children",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "DeathDate",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "DeathPlace",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "OtherWorks",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "Parents",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "Relatives",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "Spouse",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "Trademark",
                table: "DirectorDetails");

            migrationBuilder.DropColumn(
                name: "DeathDate",
                table: "ActorDetails");

            migrationBuilder.DropColumn(
                name: "DeathPlace",
                table: "ActorDetails");

            migrationBuilder.AlterColumn<string>(
                name: "BirthPlace",
                table: "ActorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlternativeName",
                table: "ActorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
