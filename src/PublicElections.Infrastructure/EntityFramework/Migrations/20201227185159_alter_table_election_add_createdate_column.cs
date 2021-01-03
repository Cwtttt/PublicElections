using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PublicElections.Infrastructure.EntityFramework.Migrations
{
    public partial class alter_table_election_add_createdate_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Elections",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Elections");
        }
    }
}
