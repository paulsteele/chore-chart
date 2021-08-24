using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hub.Server.Migrations
{
    public partial class addtodocompletions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodosCompletions",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    TodoModelId = table.Column<byte[]>(type: "varbinary(16)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<string>(type: "varchar(85)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodosCompletions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodosCompletions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TodosCompletions_Todos_TodoModelId",
                        column: x => x.TodoModelId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodosCompletions_TodoModelId",
                table: "TodosCompletions",
                column: "TodoModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TodosCompletions_UserId",
                table: "TodosCompletions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodosCompletions");
        }
    }
}
