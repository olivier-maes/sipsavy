using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SipSavy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCocktailIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "cocktails",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "cocktails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "cocktail_ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    CocktailId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cocktail_ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cocktail_ingredients_cocktails_CocktailId",
                        column: x => x.CocktailId,
                        principalTable: "cocktails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cocktails_VideoId",
                table: "cocktails",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_cocktail_ingredients_CocktailId",
                table: "cocktail_ingredients",
                column: "CocktailId");

            migrationBuilder.AddForeignKey(
                name: "FK_cocktails_videos_VideoId",
                table: "cocktails",
                column: "VideoId",
                principalTable: "videos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cocktails_videos_VideoId",
                table: "cocktails");

            migrationBuilder.DropTable(
                name: "cocktail_ingredients");

            migrationBuilder.DropIndex(
                name: "IX_cocktails_VideoId",
                table: "cocktails");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "cocktails");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "cocktails");
        }
    }
}
