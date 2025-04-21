using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Migrations
{
    /// <inheritdoc />
    public partial class CodeReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 100,
                column: "ConcurrencyStamp",
                value: "2B31AE14-E8C2-42A5-BF1B-C4A1A6AEE7F7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 100,
                column: "ConcurrencyStamp",
                value: "95c4488c-1f9a-49f7-a4be-1e14f5d42e7d");
        }
    }
}
