using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Migrations
{
    /// <inheritdoc />
    public partial class addingIsActiveColToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "ConcurrencyStamp", "IsActive", "PasswordHash" },
                values: new object[] { "84101219-602f-4dc0-b8ab-e19a1d75f1f6", true, "AQAAAAIAAYagAAAAENm5c37e6xLhOoAtpMJYfEnOYDSMrRBPcr0VJ959T+pLvoFXoz+YlZWmQ9qP/6L/Dg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c30f48b3-faaf-4939-9ebf-3a44c9c099b6", "AQAAAAIAAYagAAAAEFvq3xmuVbLso6vFRyP1TXkY59HxfUqqxq9tapOOwdqHGvkR2SGhDCBzbgGlVCsGFQ==" });
        }
    }
}
