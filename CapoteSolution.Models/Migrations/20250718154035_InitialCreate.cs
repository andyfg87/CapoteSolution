using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapoteSolution.Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Toners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TonerModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TonerYield = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Brand_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Toner_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineModels_Brands_Brand_FK",
                        column: x => x.Brand_FK,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MachineModels_Toners_Toner_FK",
                        column: x => x.Toner_FK,
                        principalTable: "Toners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Copiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Plan_BW = table.Column<int>(type: "int", nullable: false),
                    Plan_Color = table.Column<int>(type: "int", nullable: false),
                    Extra_BW = table.Column<int>(type: "int", nullable: false),
                    Extra_Color = table.Column<int>(type: "int", nullable: false),
                    Monthly_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Machine_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IP_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChargeExtra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineModel_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Customer_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Copiers_Brands_Brand_FK",
                        column: x => x.Brand_FK,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Copiers_MachineModels_MachineModel_FK",
                        column: x => x.MachineModel_FK,
                        principalTable: "MachineModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Copier_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Copiers_Copier_FK",
                        column: x => x.Copier_FK,
                        principalTable: "Copiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlackCounter = table.Column<int>(type: "int", nullable: false),
                    ColorCounter = table.Column<int>(type: "int", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BlackTonerQty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tech = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceReason_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Copier_FK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Copiers_Copier_FK",
                        column: x => x.Copier_FK,
                        principalTable: "Copiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Services_ServiceReasons_ServiceReason_FK",
                        column: x => x.ServiceReason_FK,
                        principalTable: "ServiceReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Copiers_Brand_FK",
                table: "Copiers",
                column: "Brand_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Copiers_MachineModel_FK",
                table: "Copiers",
                column: "MachineModel_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Copier_FK",
                table: "Customers",
                column: "Copier_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineModels_Brand_FK",
                table: "MachineModels",
                column: "Brand_FK");

            migrationBuilder.CreateIndex(
                name: "IX_MachineModels_Toner_FK",
                table: "MachineModels",
                column: "Toner_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Copier_FK",
                table: "Services",
                column: "Copier_FK");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceReason_FK",
                table: "Services",
                column: "ServiceReason_FK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Copiers");

            migrationBuilder.DropTable(
                name: "ServiceReasons");

            migrationBuilder.DropTable(
                name: "MachineModels");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Toners");
        }
    }
}
