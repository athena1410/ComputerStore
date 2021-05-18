//-----------------------------------------------------------------------
// <copyright file="DataSeeder.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Entities;
using ComputerStore.Structure.Helper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ComputerStore.BoundedContext.Data.DataSeed
{
    public class DataSeeder : IDataSeeder
    {
        public void SeedData(ModelBuilder modelBuilder)
        {
            #region Company
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "Fsoft",
                    CreatedDate = DateTime.UtcNow
                },
                new Company
                {
                    Id = 2,
                    Name = "CMC",
                    CreatedDate = DateTime.UtcNow
                },
                new Company
                {
                    Id = 3,
                    Name = "SamSung",
                    CreatedDate = DateTime.UtcNow
                }
            );

            #endregion

            #region Role
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "SuperAdmin"
                },
                new Role
                {
                    Id = 2,
                    Name = "Administrator"
                },
                new Role
                {
                    Id = 3,
                    Name = "User"
                }
            );
            #endregion

            #region Website
            modelBuilder.Entity<Website>().HasData(
               new Website
               {
                   Id = 1,
                   CompanyId = 1,
                   Name = "Fsoft Website",
                   UrlPath = "fsoft",
                   SecretKey = Guid.NewGuid().ToString(),
                   CreatedDate = DateTime.UtcNow
               },
               new Website
               {
                   Id = 2,
                   CompanyId = 2,
                   Name = "CMC Website",
                   UrlPath = "cmc",
                   SecretKey = Guid.NewGuid().ToString(),
                   CreatedDate = DateTime.UtcNow
               },
               new Website
               {
                   Id = 3,
                   CompanyId = 3,
                   Name = "SamSung Website",
                   UrlPath = "samsung",
                   SecretKey = Guid.NewGuid().ToString(),
                   CreatedDate = DateTime.UtcNow
               }
           );
            #endregion

            #region User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "sa@abc.com",
                    Password = Security.CreateHashPassword("sa@abc.com", "123456"),
                    RoleId = 1,
                    WebsiteId = null,
                    FirstName = "Administrator",
                    LastName = "",
                    Address = "Hà Nội",
                    Phone = "088666332"
                },
                new User
                {
                    Id = 2,
                    Email = "admin@gmail.com",
                    Password = Security.CreateHashPassword("admin@gmail.com", "123456"),
                    RoleId = 2,
                    WebsiteId = 1,
                    FirstName = "Fshop",
                    LastName = "Admin",
                    Address = "17 Duy Tân, Cầu Giấy, Hà Nội",
                    Phone = "0123456789"
                },
                new User
                {
                    Id = 3,
                    Email = "admin@cmc.com",
                    Password = Security.CreateHashPassword("admin@cmc.com", "123456"),
                    RoleId = 2,
                    WebsiteId = 2,
                    FirstName = "Administrator",
                    LastName = "CMC",
                    Address = "19 Duy Tân, Cầu Giấy, Hà Nội",
                    Phone = "0987654321"
                },
                new User
                {
                    Id = 4,
                    Email = "admin@samsung.com",
                    Password = Security.CreateHashPassword("admin@samsung.com", "123456"),
                    RoleId = 2,
                    WebsiteId = 3,
                    FirstName = "Administrator",
                    LastName = "",
                    Address = "Thái Nguyên, Việt Nam",
                    Phone = "0456789123"
                },
                new User
                {
                    Id = 5,
                    Email = "binhhoang@gmail.com",
                    Password = Security.CreateHashPassword("binhhoang@gmail.com", "123456"),
                    RoleId = 1,
                    WebsiteId = null,
                    FirstName = "Bình",
                    LastName = "Hoàng Thị Vũ",
                    Address = "Hà Nội",
                    Phone = "0886663345"
                },
                new User
                {
                    Id = 6,
                    Email = "nguyentuan@gmail.com",
                    Password = Security.CreateHashPassword("nguyentuan@gmail.com", "123456"),
                    RoleId = 3,
                    WebsiteId = 1,
                    FirstName = "Tuấn",
                    LastName = "Nguyễn",
                    Address = "Hà Nội",
                    Phone = "0886663346"
                },
                new User
                {
                    Id = 7,
                    Email = "buidung@gmail.com",
                    Password = Security.CreateHashPassword("buidung@gmail.com", "123456"),
                    RoleId = 3,
                    WebsiteId = 2,
                    FirstName = "Dũng",
                    LastName = "Bùi Doãn",
                    Address = "Hà Nội",
                    Phone = "0886663347"
                },
                new User
                {
                    Id = 8,
                    Email = "quangnguyen@gmail.com",
                    Password = Security.CreateHashPassword("quangnguyen@gmail.com", "123456"),
                    RoleId = 3,
                    WebsiteId = 2,
                    FirstName = "Quang",
                    LastName = "Nguyễn Văn",
                    Address = "Hà Nội",
                    Phone = "0886663348"
                },
                new User
                {
                    Id = 9,
                    Email = "toan@gmail.com",
                    Password = Security.CreateHashPassword("toan@gmail.com", "123456"),
                    RoleId = 3,
                    WebsiteId = 3,
                    FirstName = "Toàn",
                    LastName = "Hà Duy",
                    Address = "Hà Nội",
                    Phone = "0886663349"
                }
            );
            #endregion

            #region Category
            modelBuilder.Entity<Category>().HasData(
               new Category
               {
                   Id = 1,
                   WebsiteId = 1,
                   ParentId = null,
                   Name = "Dell",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Xeon", RAM = "16GB", SSD = "120GB", HDD = "2TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Black, White", VGA = "15.6-inch FHD (1920 x 1080) Anti-glare LED Backlit Non-touch Narrow Border WVA Display" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 2,
                   WebsiteId = 1,
                   ParentId = null,
                   Name = "Asus",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Core i7", RAM = "32GB", SSD = "280GB", HDD = "1TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Black, Yellow", VGA = "Intel UHD Graphics 620", Pin = "3-Cell Battery, 51 Whr" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 3,
                   WebsiteId = 1,
                   ParentId = 1,
                   Name = "Dell Vostro",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Core i3", RAM = "16GB", SSD = "120GB", HDD = "500GB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Black", VGA = "Intel UHD 620", PIN = "3Cell (42Whr)" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 4,
                   WebsiteId = 1,
                   ParentId = 1,
                   Name = "Dell Inspiron",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Core i7", RAM = "16GB", SSD = "120GB", HDD = "1TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "White", VGA = "Intel UHD Graphics 620" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 5,
                   WebsiteId = 1,
                   ParentId = null,
                   Name = "HP",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i7 10510U", RAM = "8GB", SSD = "120GB", HDD = "1TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Black, White", VGA = "Intel HD Graphics" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 6,
                   WebsiteId = 1,
                   ParentId = 5,
                   Name = "HP Pavilion",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i3 10110U", RAM = "4GB", SSD = "256GB", HDD = "1TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Yellow", VGA = "Intel HD Graphics" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 7,
                   WebsiteId = 1,
                   ParentId = 5,
                   Name = "HP Elitebook",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i7 10510U", RAM = "16GB DDR4 2666MHz", SSD = "512GB", HDD = "1TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Black, White", VGA = "NVIDIA GeForce GTX1050 4G DDR5" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 8,
                   WebsiteId = 1,
                   ParentId = null,
                   Name = "Apple",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i5 1.4Ghz", RAM = "8GB", SSD = "256GB", HDD = "2TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Gray, Black, White, Gold", VGA = "Intel Iris Plus 645" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 9,
                   WebsiteId = 1,
                   ParentId = 8,
                   Name = "Apple Macbook Air",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i5 1.6Ghz", RAM = "8GB", SSD = "128GB", HDD = "2TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Rose Gold", VGA = "Intel UHD 617" }),
                   CreatedDate = DateTime.UtcNow
               },
               new Category
               {
                   Id = 10,
                   WebsiteId = 1,
                   ParentId = 8,
                   Name = "Apple Macbook Pro",
                   TemplateMetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i5 1.4Ghz", RAM = "8GB", SSD = "128GB", HDD = "1TB" }),
                   TemplateSpecificData = JsonConvert.SerializeObject(new { Color = "Grey, White", VGA = "Intel Iris Plus 645" }),
                   CreatedDate = DateTime.UtcNow
               }
           );
            #endregion

            #region Product
            modelBuilder.Entity<Product>().HasData(
               new Product
               {
                   Id = 1,
                   WebsiteId = 1,
                   Name = "Dell Vostro 5537",
                   ProductCode = "LAP001",
                   Description = "Máy tính Dell vostro 5537",
                   Discount = 5,
                   Warranty = 12,
                   Price = 12500000,
                   Quantity = 100,
                   CategoryId = 3,
                   ViewCount = 0,
                   MetaData = JsonConvert.SerializeObject(new { Cpu = "Core I7", Ram = "4G", Ssd = "120G", Hdd = "500G" }),
                   SpecificData = JsonConvert.SerializeObject(JObject.Parse(@"{'Part number':'2N1R82','Bộ vi xử lý':'Intel Core i5-10210U (1.6Ghz/6MB cache)','Chipset':'Intel','Bộ nhớ trong':'8GB DDR4 2666Mhz (8GB *1)','Số khe cắm':'2','VGA':'AMD Radeon 610 2G DDR5','Ổ cứng':'256GB PCIe NVMe SSD','Màn hình':'14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit','Webcam':'HD','Audio':'Yes','Giao tiếp mạng':'Gigabit','Giao tiếp không dây':'802.11ac ,Bluetooth 4.1','Card Reader':'SD'}")),
                   CreatedDate = DateTime.UtcNow
               },
               new Product
               {
                   Id = 2,
                   WebsiteId = 1,
                   Name = "Dell Vostro 6666",
                   ProductCode = "LAP002",
                   Description = "Máy tính Dell vostro 5537",
                   Discount = 7,
                   Warranty = 12,
                   Price = 15500000,
                   Quantity = 200,
                   CategoryId = 3,
                   ViewCount = 0,
                   MetaData = JsonConvert.SerializeObject(new { Cpu = "Core I7", Ram = "8G", Ssd = "120G", Hdd = "250G" }),
                   SpecificData = JsonConvert.SerializeObject(JObject.Parse(@"{'Part number':'2N1R82','Bộ vi xử lý':'Intel Core i5-10210U (1.6Ghz/6MB cache)','Chipset':'Intel','Bộ nhớ trong':'8GB DDR4 2666Mhz (8GB *1)','Số khe cắm':'2','VGA':'AMD Radeon 610 2G DDR5','Ổ cứng':'256GB PCIe NVMe SSD','Màn hình':'14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit','Webcam':'HD','Audio':'Yes','Giao tiếp mạng':'Gigabit','Giao tiếp không dây':'802.11ac ,Bluetooth 4.1','Card Reader':'SD'}")),
                   CreatedDate = DateTime.UtcNow
               },
               new Product
               {
                   Id = 3,
                   WebsiteId = 1,
                   Name = "Dell Vostro 5537",
                   ProductCode = "LAP003",
                   Description = "Máy tính Dell vostro 5537",
                   Discount = 0,
                   Warranty = 24,
                   Price = 11500000,
                   Quantity = 250,
                   CategoryId = 2,
                   ViewCount = 0,
                   MetaData = JsonConvert.SerializeObject(new { Cpu = "Core I7", Ram = "16G", Ssd = "120G", Hdd = "500G" }),
                   SpecificData = JsonConvert.SerializeObject(JObject.Parse(@"{'Part number':'2N1R82','Bộ vi xử lý':'Intel Core i5-10210U (1.6Ghz/6MB cache)','Chipset':'Intel','Bộ nhớ trong':'8GB DDR4 2666Mhz (8GB *1)','Số khe cắm':'2','VGA':'AMD Radeon 610 2G DDR5','Ổ cứng':'256GB PCIe NVMe SSD','Màn hình':'14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit','Webcam':'HD','Audio':'Yes','Giao tiếp mạng':'Gigabit','Giao tiếp không dây':'802.11ac ,Bluetooth 4.1','Card Reader':'SD'}")),
                   CreatedDate = DateTime.UtcNow
               },
               new Product
               {
                   Id = 4,
                   WebsiteId = 1,
                   Name = "Laptop Dell Vostro 3490 (2N1R82) (i5 10210U/8GB Ram /256GBSSD/ 610 2G/14.0 inch FHD/Win 10/Đen)",
                   ProductCode = "LAP004",
                   Description = @"<h2>Đặc điểm nổi bật của Laptop Dell Vostro 3490 (2N1R82) (i5 10210U/8GB Ram /256GBSSD/ 610 2G/14.0 inch FHD/Win 10/Đen)</h2><h3>Màn hình</h3><p>Laptop Dell Vostro 3490 2N1R82 có màn hình 14.0 inch Full HD giúp bạn cho hình ảnh sắc nét, chi tiết ở mọi góc độ.Cùng tận hưởng hình ảnh sắc nét với công nghệ chống chói lóa giúp bạn thỏa sức trải nghiệm ngay cả khi làm việc, học tập trong môi trường nhiều ánh sáng.</p><h3> Bàn phím </h3><p> Bàn phím cho cảm giác gõ thoải mái nhờ độ nảy tốt,khoảng cách các phím được sắp xếp khoa học.Phần touchpad rộng rãi với khả năng hỗ trợ đa điểm giúp các thao tác cuộn trang, thu phóng… được thực hiện dễ dàng và chính xác.Máy hỗ trợ nhận dạng vân tay giúp máy tính của bạn được bảo mật một cách tối đa, chỉ có bạn mới có thể mở được chiếc laptop này.</p></p><h3>Cổng kết nối</h3><p> Laptop Dell Vostro 3490 2N1R82 được trang bị những chuẩn kết nối thông dụng có mặt hầu hết trên tất cả những dòng phổ thông hiện nay như cổng USB 2.0, USB 3.0, VGACard Reader,… điều này giúp tăng sự lựa chọn cho bạn với nhiều nhu cầu kết nối khác nhau và cho phép kết nối máy với nhiều thiết bị ngoại vi dễ dàng hơn.</p><p>Máy có cổng mạng LAN, hỗ trợ kết nối Wifi giúp bạn truy cập Internet nhanh chóng, phục vụ đắc lực cho công việc, học tập</p>",
                   Discount = 0,
                   Warranty = 24,
                   Price = 16289000,
                   Quantity = 250,
                   CategoryId = 2,
                   ViewCount = 0,
                   MetaData = JsonConvert.SerializeObject(new { CPU = "Intel Core i5 10210U", RAM = "8GB", SSD = "256GB" }),
                   SpecificData = JsonConvert.SerializeObject(JObject.Parse(@"{'Part number':'2N1R82','Bộ vi xử lý':'Intel Core i5-10210U (1.6Ghz/6MB cache)','Chipset':'Intel','Bộ nhớ trong':'8GB DDR4 2666Mhz (8GB *1)','Số khe cắm':'2','VGA':'AMD Radeon 610 2G DDR5','Ổ cứng':'256GB PCIe NVMe SSD','Màn hình':'14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit','Webcam':'HD','Audio':'Yes','Giao tiếp mạng':'Gigabit','Giao tiếp không dây':'802.11ac ,Bluetooth 4.1','Card Reader':'SD'}")),
                   CreatedDate = DateTime.UtcNow
               },
                new Product
               {
                   Id = 5,
                   WebsiteId = 2,
                   Name = "Asus K4444",
                   ProductCode = "LAP005",
                   Description = "Máy tính Asus K4444",
                   Discount = 0,
                   Warranty = 24,
                   Price = 12500000,
                   Quantity = 250,
                   CategoryId = 2,
                   ViewCount = 0,
                   MetaData = JsonConvert.SerializeObject(new { Cpu = "Core I7", Ram = "16G", Ssd = "120G", Hdd = "500G" }),
                   SpecificData = JsonConvert.SerializeObject(JObject.Parse(@"{'Part number':'2N1R82','Bộ vi xử lý':'Intel Core i5-10210U (1.6Ghz/6MB cache)','Chipset':'Intel','Bộ nhớ trong':'8GB DDR4 2666Mhz (8GB *1)','Số khe cắm':'2','VGA':'AMD Radeon 610 2G DDR5','Ổ cứng':'256GB PCIe NVMe SSD','Màn hình':'14.0 inch FHD (1920 x 1080) Anti-Glare LED Backlit','Webcam':'HD','Audio':'Yes','Giao tiếp mạng':'Gigabit','Giao tiếp không dây':'802.11ac ,Bluetooth 4.1','Card Reader':'SD'}")),
                    CreatedDate = DateTime.UtcNow
               }
           );
            #endregion

            #region Product Image
            modelBuilder.Entity<ProductImage>().HasData(
               new ProductImage
               {
                   Id = 1,
                   ProductId = 1,
                   ImageUrl = "assets/productimages/LAP001/LAP001_01.jpg",
                   CreatedDate = DateTime.UtcNow
               },
               new ProductImage
               {
                   Id = 2,
                   ProductId = 1,
                   ImageUrl = "assets/productimages/LAP001/LAP001_02.jpg",
                   CreatedDate = DateTime.UtcNow
               },
               new ProductImage
               {
                   Id = 3,
                   ProductId = 2,
                   ImageUrl = "assets/productimages/LAP002/LAP002_01.jpg",
                   CreatedDate = DateTime.UtcNow
               },
               new ProductImage
               {
                   Id = 4,
                   ProductId = 3,
                   ImageUrl = "assets/productimages/LAP003/LAP003_01.jpg",
                   CreatedDate = DateTime.UtcNow
               },
               new ProductImage
               {
                   Id = 5,
                   ProductId = 4,
                   ImageUrl = "assets/productimages/LAP004/LAP004_01.jpg",
                   CreatedDate = DateTime.UtcNow
               },
               new ProductImage
               {
                   Id = 6,
                   ProductId = 5,
                   ImageUrl = "assets/productimages/LAP005/LAP005_01.jpg",
                   CreatedDate = DateTime.UtcNow
               }
           );
            #endregion

            #region Order
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = 6,
                    WebsiteId = 1,
                    ShipAddress = "Ha Noi",
                    Phone = "0886663346",
                    Total = 40500000,
                    CreatedDate = DateTime.UtcNow
                },
                new Order
                {
                    Id = 2,
                    UserId = 7,
                    WebsiteId = 2,
                    ShipAddress = "Ha Noi",
                    Phone = "0886663346",
                    Total = 15500000,
                    CreatedDate = DateTime.UtcNow
                }
            );
            #endregion

            #region OrderDetail

            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 2,
                    Price = 12500000,
                    Discount = 0
                },
                new OrderDetail
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 2,
                    Quantity = 1,
                    Price = 15500000,
                    Discount = 0
                },
                new OrderDetail
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 4,
                    Quantity = 1,
                    Price = 15500000,
                    Discount = 0
                }
            );
            #endregion
        }
    }
}
