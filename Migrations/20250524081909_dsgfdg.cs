using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class dsgfdg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DSAnhs_SanPhams_MaSP",
                table: "DSAnhs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DSAnhs",
                table: "DSAnhs");

            migrationBuilder.RenameTable(
                name: "DSAnhs",
                newName: "DSAnh");

            migrationBuilder.RenameIndex(
                name: "IX_DSAnhs_MaSP",
                table: "DSAnh",
                newName: "IX_DSAnh_MaSP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DSAnh",
                table: "DSAnh",
                column: "MaAnh");

            migrationBuilder.AddForeignKey(
                name: "FK_DSAnh_SanPhams_MaSP",
                table: "DSAnh",
                column: "MaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DSAnh_SanPhams_MaSP",
                table: "DSAnh");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DSAnh",
                table: "DSAnh");

            migrationBuilder.RenameTable(
                name: "DSAnh",
                newName: "DSAnhs");

            migrationBuilder.RenameIndex(
                name: "IX_DSAnh_MaSP",
                table: "DSAnhs",
                newName: "IX_DSAnhs_MaSP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DSAnhs",
                table: "DSAnhs",
                column: "MaAnh");

            migrationBuilder.AddForeignKey(
                name: "FK_DSAnhs_SanPhams_MaSP",
                table: "DSAnhs",
                column: "MaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
