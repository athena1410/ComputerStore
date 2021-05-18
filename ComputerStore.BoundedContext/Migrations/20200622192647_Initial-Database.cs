using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputerStore.BoundedContext.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebsiteId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    TemplateMetaData = table.Column<string>(nullable: true),
                    TemplateSpecificData = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Address = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Website",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    UrlPath = table.Column<string>(maxLength: 50, nullable: false),
                    LogoUrl = table.Column<string>(maxLength: 255, nullable: true),
                    Note = table.Column<string>(maxLength: 255, nullable: true),
                    SecretKey = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Website", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Website_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebsiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ProductCode = table.Column<string>(maxLength: 10, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Discount = table.Column<float>(nullable: false, defaultValueSql: "((0))"),
                    Warranty = table.Column<int>(nullable: false, defaultValueSql: "((12))"),
                    Price = table.Column<float>(nullable: false, defaultValueSql: "((0))"),
                    Quantity = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    CategoryId = table.Column<int>(nullable: false),
                    ViewCount = table.Column<int>(nullable: false, defaultValueSql: "((0))"),
                    MetaData = table.Column<string>(nullable: true),
                    SpecificData = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Website",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(maxLength: 150, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    WebsiteId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(maxLength: 50, nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Website",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnonymousCart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebsiteId = table.Column<int>(nullable: false),
                    IdentityCode = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousCart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnonymousCartt_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnonymousCart_User",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    WebsiteId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cart_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cart_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cart_Website",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    WebsiteId = table.Column<int>(nullable: true),
                    ShipAddress = table.Column<string>(maxLength: 255, nullable: true),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    PaymentState = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    OrderState = table.Column<int>(nullable: false, defaultValueSql: "((0))"),
                    Total = table.Column<float>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Website",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByIp = table.Column<string>(nullable: true),
                    Revoked = table.Column<DateTime>(type: "datetime", nullable: true),
                    RevokedByIp = table.Column<string>(nullable: true),
                    ReplacedByToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    Price = table.Column<float>(nullable: false),
                    Discount = table.Column<float>(nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedDate", "Name", "ParentId", "TemplateMetaData", "TemplateSpecificData", "UpdatedDate", "WebsiteId" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 6, 22, 19, 26, 47, 240, DateTimeKind.Utc).AddTicks(2070), "Dell", null, "{\"CPU\":\"Intel Xeon\",\"RAM\":\"16GB\",\"SSD\":\"120GB\",\"HDD\":\"2TB\"}", "{\"Color\":\"Black, White\",\"VGA\":\"15.6-inch FHD (1920 x 1080) Anti-glare LED Backlit Non-touch Narrow Border WVA Display\"}", null, 1 },
                    { 2, new DateTime(2020, 6, 22, 19, 26, 47, 242, DateTimeKind.Utc).AddTicks(2531), "Asus", null, "{\"CPU\":\"Core i7\",\"RAM\":\"32GB\",\"SSD\":\"280GB\",\"HDD\":\"1TB\"}", "{\"Color\":\"Black, Yellow\",\"VGA\":\"Intel UHD Graphics 620\",\"Pin\":\"3-Cell Battery, 51 Whr\"}", null, 1 },
                    { 3, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9017), "Dell Vostro", 1, "{\"CPU\":\"Core i3\",\"RAM\":\"16GB\",\"SSD\":\"120GB\",\"HDD\":\"500GB\"}", "{\"Color\":\"Black\",\"VGA\":\"Intel UHD 620\",\"PIN\":\"3Cell (42Whr)\"}", null, 1 },
                    { 4, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9165), "Dell Inspiron", 1, "{\"CPU\":\"Core i7\",\"RAM\":\"16GB\",\"SSD\":\"120GB\",\"HDD\":\"1TB\"}", "{\"Color\":\"White\",\"VGA\":\"Intel UHD Graphics 620\"}", null, 1 },
                    { 5, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9263), "HP", null, "{\"CPU\":\"Intel Core i7 10510U\",\"RAM\":\"8GB\",\"SSD\":\"120GB\",\"HDD\":\"1TB\"}", "{\"Color\":\"Black, White\",\"VGA\":\"Intel HD Graphics\"}", null, 1 },
                    { 6, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9306), "HP Pavilion", 5, "{\"CPU\":\"Intel Core i3 10110U\",\"RAM\":\"4GB\",\"SSD\":\"256GB\",\"HDD\":\"1TB\"}", "{\"Color\":\"Yellow\",\"VGA\":\"Intel HD Graphics\"}", null, 1 },
                    { 7, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9388), "HP Elitebook", 5, "{\"CPU\":\"Intel Core i7 10510U\",\"RAM\":\"16GB DDR4 2666MHz\",\"SSD\":\"512GB\",\"HDD\":\"1TB\"}", "{\"Color\":\"Black, White\",\"VGA\":\"NVIDIA GeForce GTX1050 4G DDR5\"}", null, 1 },
                    { 8, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9431), "Apple", null, "{\"CPU\":\"Intel Core i5 1.4Ghz\",\"RAM\":\"8GB\",\"SSD\":\"256GB\",\"HDD\":\"2TB\"}", "{\"Color\":\"Gray, Black, White, Gold\",\"VGA\":\"Intel Iris Plus 645\"}", null, 1 },
                    { 9, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9472), "Apple Macbook Air", 8, "{\"CPU\":\"Intel Core i5 1.6Ghz\",\"RAM\":\"8GB\",\"SSD\":\"128GB\",\"HDD\":\"2TB\"}", "{\"Color\":\"Rose Gold\",\"VGA\":\"Intel UHD 617\"}", null, 1 },
                    { 10, new DateTime(2020, 6, 22, 19, 26, 47, 243, DateTimeKind.Utc).AddTicks(9551), "Apple Macbook Pro", 8, "{\"CPU\":\"Intel Core i5 1.4Ghz\",\"RAM\":\"8GB\",\"SSD\":\"128GB\",\"HDD\":\"1TB\"}", "{\"Color\":\"Grey, White\",\"VGA\":\"Intel Iris Plus 645\"}", null, 1 }
                });

            migrationBuilder.InsertData(
                table: "Company",
                columns: new[] { "Id", "Address", "CreatedDate", "Name", "Phone", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2020, 6, 22, 19, 26, 47, 67, DateTimeKind.Utc).AddTicks(1396), "Fsoft", null, null },
                    { 2, null, new DateTime(2020, 6, 22, 19, 26, 47, 67, DateTimeKind.Utc).AddTicks(2363), "CMC", null, null },
                    { 3, null, new DateTime(2020, 6, 22, 19, 26, 47, 67, DateTimeKind.Utc).AddTicks(2376), "SamSung", null, null }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Administrator" },
                    { 3, "User" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "CreatedDate", "Email", "FirstName", "LastLogin", "LastName", "Password", "Phone", "RoleId", "UpdatedDate", "WebsiteId" },
                values: new object[,]
                {
                    { 1, "Hà Nội", null, "sa@abc.com", "Administrator", null, "", "bp/Xrgx7+49qHB3a9eC4ZF/xE0xVr61ABIhxZ0g3lXc=", "088666332", 1, null, null },
                    { 5, "Hà Nội", null, "binhhoang@gmail.com", "Bình", null, "Hoàng Thị Vũ", "2Pqovjl5wRuQmAqYeRbulK3oDJOzMBWTe9EYpyUQmA8=", "0886663345", 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "Website",
                columns: new[] { "Id", "CompanyId", "CreatedDate", "LogoUrl", "Name", "Note", "SecretKey", "UpdatedDate", "UrlPath" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2020, 6, 22, 19, 26, 47, 69, DateTimeKind.Utc).AddTicks(996), null, "Fsoft Website", null, "c8c60f47-cff4-4c01-a8ae-926dc59df43a", null, "fsoft" },
                    { 2, 2, new DateTime(2020, 6, 22, 19, 26, 47, 69, DateTimeKind.Utc).AddTicks(1670), null, "CMC Website", null, "c065c139-651f-4d20-9974-af831292d606", null, "cmc" },
                    { 3, 3, new DateTime(2020, 6, 22, 19, 26, 47, 69, DateTimeKind.Utc).AddTicks(1686), null, "SamSung Website", null, "61c81bd0-aa72-44f8-a337-b5441d0a1fc6", null, "samsung" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "Description", "Discount", "MetaData", "Name", "Price", "ProductCode", "Quantity", "SpecificData", "UpdatedDate", "Warranty", "WebsiteId" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2020, 6, 22, 19, 26, 47, 260, DateTimeKind.Utc).AddTicks(5287), "Máy tính Dell vostro 5537", 5f, "{\"Cpu\":\"Core I7\",\"Ram\":\"4G\",\"Ssd\":\"120G\",\"Hdd\":\"500G\"}", "Dell Vostro 5537", 12500000f, "LAP001", 100, "{\"Part number\":\"2N1R82\",\"Bộ vi xử lý\":\"Intel Core i5-10210U (1.6Ghz/6MB cache)\",\"Chipset\":\"Intel\",\"Bộ nhớ trong\":\"8GB DDR4 2666Mhz (8GB *1)\",\"Số khe cắm\":\"2\",\"VGA\":\"AMD Radeon 610 2G DDR5\",\"Ổ cứng\":\"256GB PCIe NVMe SSD\",\"Màn hình\":\"14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit\",\"Webcam\":\"HD\",\"Audio\":\"Yes\",\"Giao tiếp mạng\":\"Gigabit\",\"Giao tiếp không dây\":\"802.11ac ,Bluetooth 4.1\",\"Card Reader\":\"SD\"}", null, 12, 1 },
                    { 2, 3, new DateTime(2020, 6, 22, 19, 26, 47, 260, DateTimeKind.Utc).AddTicks(6772), "Máy tính Dell vostro 5537", 7f, "{\"Cpu\":\"Core I7\",\"Ram\":\"8G\",\"Ssd\":\"120G\",\"Hdd\":\"250G\"}", "Dell Vostro 6666", 15500000f, "LAP002", 200, "{\"Part number\":\"2N1R82\",\"Bộ vi xử lý\":\"Intel Core i5-10210U (1.6Ghz/6MB cache)\",\"Chipset\":\"Intel\",\"Bộ nhớ trong\":\"8GB DDR4 2666Mhz (8GB *1)\",\"Số khe cắm\":\"2\",\"VGA\":\"AMD Radeon 610 2G DDR5\",\"Ổ cứng\":\"256GB PCIe NVMe SSD\",\"Màn hình\":\"14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit\",\"Webcam\":\"HD\",\"Audio\":\"Yes\",\"Giao tiếp mạng\":\"Gigabit\",\"Giao tiếp không dây\":\"802.11ac ,Bluetooth 4.1\",\"Card Reader\":\"SD\"}", null, 12, 1 }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "Description", "MetaData", "Name", "Price", "ProductCode", "Quantity", "SpecificData", "UpdatedDate", "Warranty", "WebsiteId" },
                values: new object[,]
                {
                    { 3, 2, new DateTime(2020, 6, 22, 19, 26, 47, 260, DateTimeKind.Utc).AddTicks(7112), "Máy tính Dell vostro 5537", "{\"Cpu\":\"Core I7\",\"Ram\":\"16G\",\"Ssd\":\"120G\",\"Hdd\":\"500G\"}", "Dell Vostro 5537", 11500000f, "LAP003", 250, "{\"Part number\":\"2N1R82\",\"Bộ vi xử lý\":\"Intel Core i5-10210U (1.6Ghz/6MB cache)\",\"Chipset\":\"Intel\",\"Bộ nhớ trong\":\"8GB DDR4 2666Mhz (8GB *1)\",\"Số khe cắm\":\"2\",\"VGA\":\"AMD Radeon 610 2G DDR5\",\"Ổ cứng\":\"256GB PCIe NVMe SSD\",\"Màn hình\":\"14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit\",\"Webcam\":\"HD\",\"Audio\":\"Yes\",\"Giao tiếp mạng\":\"Gigabit\",\"Giao tiếp không dây\":\"802.11ac ,Bluetooth 4.1\",\"Card Reader\":\"SD\"}", null, 24, 1 },
                    { 4, 2, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(3669), "<h2>Đặc điểm nổi bật của Laptop Dell Vostro 3490 (2N1R82) (i5 10210U/8GB Ram /256GBSSD/ 610 2G/14.0 inch FHD/Win 10/Đen)</h2><h3>Màn hình</h3><p>Laptop Dell Vostro 3490 2N1R82 có màn hình 14.0 inch Full HD giúp bạn cho hình ảnh sắc nét, chi tiết ở mọi góc độ.Cùng tận hưởng hình ảnh sắc nét với công nghệ chống chói lóa giúp bạn thỏa sức trải nghiệm ngay cả khi làm việc, học tập trong môi trường nhiều ánh sáng.</p><h3> Bàn phím </h3><p> Bàn phím cho cảm giác gõ thoải mái nhờ độ nảy tốt,khoảng cách các phím được sắp xếp khoa học.Phần touchpad rộng rãi với khả năng hỗ trợ đa điểm giúp các thao tác cuộn trang, thu phóng… được thực hiện dễ dàng và chính xác.Máy hỗ trợ nhận dạng vân tay giúp máy tính của bạn được bảo mật một cách tối đa, chỉ có bạn mới có thể mở được chiếc laptop này.</p></p><h3>Cổng kết nối</h3><p> Laptop Dell Vostro 3490 2N1R82 được trang bị những chuẩn kết nối thông dụng có mặt hầu hết trên tất cả những dòng phổ thông hiện nay như cổng USB 2.0, USB 3.0, VGACard Reader,… điều này giúp tăng sự lựa chọn cho bạn với nhiều nhu cầu kết nối khác nhau và cho phép kết nối máy với nhiều thiết bị ngoại vi dễ dàng hơn.</p><p>Máy có cổng mạng LAN, hỗ trợ kết nối Wifi giúp bạn truy cập Internet nhanh chóng, phục vụ đắc lực cho công việc, học tập</p>", "{\"CPU\":\"Intel Core i5 10210U\",\"RAM\":\"8GB\",\"SSD\":\"256GB\"}", "Laptop Dell Vostro 3490 (2N1R82) (i5 10210U/8GB Ram /256GBSSD/ 610 2G/14.0 inch FHD/Win 10/Đen)", 16289000f, "LAP004", 250, "{\"Part number\":\"2N1R82\",\"Bộ vi xử lý\":\"Intel Core i5-10210U (1.6Ghz/6MB cache)\",\"Chipset\":\"Intel\",\"Bộ nhớ trong\":\"8GB DDR4 2666Mhz (8GB *1)\",\"Số khe cắm\":\"2\",\"VGA\":\"AMD Radeon 610 2G DDR5\",\"Ổ cứng\":\"256GB PCIe NVMe SSD\",\"Màn hình\":\"14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit\",\"Webcam\":\"HD\",\"Audio\":\"Yes\",\"Giao tiếp mạng\":\"Gigabit\",\"Giao tiếp không dây\":\"802.11ac ,Bluetooth 4.1\",\"Card Reader\":\"SD\"}", null, 24, 1 },
                    { 5, 2, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(4231), "Máy tính Asus K4444", "{\"Cpu\":\"Core I7\",\"Ram\":\"16G\",\"Ssd\":\"120G\",\"Hdd\":\"500G\"}", "Asus K4444", 12500000f, "LAP005", 250, "{\"Part number\":\"2N1R82\",\"Bộ vi xử lý\":\"Intel Core i5-10210U (1.6Ghz/6MB cache)\",\"Chipset\":\"Intel\",\"Bộ nhớ trong\":\"8GB DDR4 2666Mhz (8GB *1)\",\"Số khe cắm\":\"2\",\"VGA\":\"AMD Radeon 610 2G DDR5\",\"Ổ cứng\":\"256GB PCIe NVMe SSD\",\"Màn hình\":\"14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit\",\"Webcam\":\"HD\",\"Audio\":\"Yes\",\"Giao tiếp mạng\":\"Gigabit\",\"Giao tiếp không dây\":\"802.11ac ,Bluetooth 4.1\",\"Card Reader\":\"SD\"}", null, 24, 2 }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "CreatedDate", "Email", "FirstName", "LastLogin", "LastName", "Password", "Phone", "RoleId", "UpdatedDate", "WebsiteId" },
                values: new object[,]
                {
                    { 2, "17 Duy Tân, Cầu Giấy, Hà Nội", null, "admin@gmail.com", "Fshop", null, "Admin", "DZCBGrk+6Jx/uMpdH4cTIsJRy3Mu9I8ZzNLcD5KSMek=", "0123456789", 2, null, 1 },
                    { 6, "Hà Nội", null, "nguyentuan@gmail.com", "Tuấn", null, "Nguyễn", "zxe9I4uWD9TngXQ6OljmHJk1TZrHH6HlsICnFaIARCk=", "0886663346", 3, null, 1 },
                    { 3, "19 Duy Tân, Cầu Giấy, Hà Nội", null, "admin@cmc.com", "Administrator", null, "CMC", "kqbEJ7Pv5cxJFY2QI7TPX6sYO+i4Nqo2b4NldvfS0bw=", "0987654321", 2, null, 2 },
                    { 7, "Hà Nội", null, "buidung@gmail.com", "Dũng", null, "Bùi Doãn", "uT0pxCBWeR4gqh+gnKGw+H6xFgpQ5eP7big9G9VhBcU=", "0886663347", 3, null, 2 },
                    { 8, "Hà Nội", null, "quangnguyen@gmail.com", "Quang", null, "Nguyễn Văn", "5kkTZN74J/7uS0qMbRIOjy1c0sfBu1HvzL/0/US5EBc=", "0886663348", 3, null, 2 },
                    { 4, "Thái Nguyên, Việt Nam", null, "admin@samsung.com", "Administrator", null, "", "wGKmxP5wb870aBXCMC0WhnHK8cCgj5sRDhW8jhwDKJg=", "0456789123", 2, null, 3 },
                    { 9, "Hà Nội", null, "toan@gmail.com", "Toàn", null, "Hà Duy", "XeX3Vn8y/NmKu4/MXRohSrOfoMDSRr9r1OAKBdExRI0=", "0886663349", 3, null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "Id", "CreatedDate", "Note", "Phone", "ShipAddress", "Total", "UpdatedDate", "UserId", "WebsiteId" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 6, 22, 19, 26, 47, 263, DateTimeKind.Utc).AddTicks(1160), null, "0886663346", "Ha Noi", 40500000f, null, 6, 1 },
                    { 2, new DateTime(2020, 6, 22, 19, 26, 47, 263, DateTimeKind.Utc).AddTicks(1783), null, "0886663346", "Ha Noi", 15500000f, null, 7, 2 }
                });

            migrationBuilder.InsertData(
                table: "ProductImage",
                columns: new[] { "Id", "CreatedDate", "ImageUrl", "ProductId", "Status", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(6876), "assets/productimages/LAP001/LAP001_01.jpg", 1, null, null },
                    { 2, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(7521), "assets/productimages/LAP001/LAP001_02.jpg", 1, null, null },
                    { 3, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(7534), "assets/productimages/LAP002/LAP002_01.jpg", 2, null, null },
                    { 4, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(7536), "assets/productimages/LAP003/LAP003_01.jpg", 3, null, null },
                    { 5, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(7537), "assets/productimages/LAP004/LAP004_01.jpg", 4, null, null },
                    { 6, new DateTime(2020, 6, 22, 19, 26, 47, 262, DateTimeKind.Utc).AddTicks(7538), "assets/productimages/LAP005/LAP005_01.jpg", 5, null, null }
                });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "OrderId", "ProductId", "Id", "Price", "Quantity" },
                values: new object[] { 1, 1, 1, 12500000f, 2 });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "OrderId", "ProductId", "Id", "Price", "Quantity" },
                values: new object[] { 1, 2, 2, 15500000f, 1 });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "OrderId", "ProductId", "Id", "Price", "Quantity" },
                values: new object[] { 2, 4, 3, 15500000f, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousCart_ProductId",
                table: "AnonymousCart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousCart_WebsiteId",
                table: "AnonymousCart",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductId",
                table: "Cart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_WebsiteId",
                table: "Cart",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_WebsiteId",
                table: "Order",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_WebsiteId",
                table: "Product",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_User_WebsiteId",
                table: "User",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Website_CompanyId",
                table: "Website",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Website_Name",
                table: "Website",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Website_SecretKey",
                table: "Website",
                column: "SecretKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Website_UrlPath",
                table: "Website",
                column: "UrlPath",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymousCart");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Website");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
