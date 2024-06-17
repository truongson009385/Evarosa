using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evarosa.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ShowHeader = table.Column<bool>(type: "bit", nullable: false),
                    ShowHome = table.Column<bool>(type: "bit", nullable: false),
                    ShowFooter = table.Column<bool>(type: "bit", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleCategories_ArticleCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ArticleCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slogan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ShipFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigSite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GoogleAnalytics = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GoogleMapLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WhatsApp = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Viber = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Youtube = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Twitter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Instagram = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Linkedin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Zalo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Hotline = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LiveChat = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContactInfo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Favicon = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FooterInfo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AboutTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AboutText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AboutImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AboutUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMap = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigSite", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailConfirmation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowHome = table.Column<bool>(type: "bit", nullable: false),
                    ShowMenu = table.Column<bool>(type: "bit", nullable: false),
                    ShowHeader = table.Column<bool>(type: "bit", nullable: false),
                    ShowOutstanding = table.Column<bool>(type: "bit", nullable: false),
                    Display = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleCategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleMeta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionMeta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ShowHome = table.Column<bool>(type: "bit", nullable: false),
                    ShowFooter = table.Column<bool>(type: "bit", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_ArticleCategories_ArticleCategoryId",
                        column: x => x.ArticleCategoryId,
                        principalTable: "ArticleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InStock = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceSale = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ShowOutstanding = table.Column<bool>(type: "bit", nullable: false),
                    ShowHome = table.Column<bool>(type: "bit", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DistrictID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictID",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItems",
                columns: table => new
                {
                    RecordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CartId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItems", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    OrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransportType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShipFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Prepayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategories_ParentCategoryId",
                table: "ArticleCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategories_Url",
                table: "ArticleCategories",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleCategoryId",
                table: "Articles",
                column: "ArticleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Url",
                table: "Articles",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId",
                table: "Districts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAddresses_CityId",
                table: "MemberAddresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAddresses_DistrictId",
                table: "MemberAddresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAddresses_MemberId",
                table: "MemberAddresses",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAddresses_WardId",
                table: "MemberAddresses",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CityId",
                table: "Orders",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DistrictId",
                table: "Orders",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MemberId",
                table: "Orders",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WardId",
                table: "Orders",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentCategoryId",
                table: "ProductCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Url",
                table: "ProductCategories",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Url",
                table: "Products",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ProductId",
                table: "ShoppingCartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictID",
                table: "Wards",
                column: "DistrictID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "ConfigSite");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "MemberAddresses");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ShoppingCartItems");

            migrationBuilder.DropTable(
                name: "ArticleCategories");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
