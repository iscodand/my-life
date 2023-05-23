using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLifeApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPkForAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileAnalytics_Profiles_ProfileId",
                table: "ProfileAnalytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_User_UserId",
                table: "Profiles");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Profiles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProfileId",
                table: "ProfileAnalytics",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProfileAnalytics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PostAnalytics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileAnalytics",
                table: "ProfileAnalytics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostAnalytics",
                table: "PostAnalytics",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileAnalytics_Profiles_ProfileId",
                table: "ProfileAnalytics",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_User_UserId",
                table: "Profiles",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileAnalytics_Profiles_ProfileId",
                table: "ProfileAnalytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_User_UserId",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileAnalytics",
                table: "ProfileAnalytics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostAnalytics",
                table: "PostAnalytics");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProfileAnalytics");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PostAnalytics");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Profiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProfileId",
                table: "ProfileAnalytics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileAnalytics_Profiles_ProfileId",
                table: "ProfileAnalytics",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_User_UserId",
                table: "Profiles",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
