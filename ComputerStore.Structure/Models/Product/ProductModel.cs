using ComputerStore.Structure.Models.ProductImage;
using System;
using System.Collections.Generic;

namespace ComputerStore.Structure.Models.Product
{
   public class ProductModel
   {
      public int Id { get; set; }
      public int WebsiteId { get; set; }
      public string Name { get; set; }
      public string ProductCode { get; set; }
      public string Description { get; set; }
      public float Discount { get; set; }
      public int Warranty { get; set; }
      public float Price { get; set; }
      public int Quantity { get; set; }
      public int CategoryId { get; set; }
      public int? ViewCount { get; set; }
      public string MetaData { get; set; }
      public string SpecificData { get; set; }

      /// <summary>
      /// Gets or sets path images to save to Database when create a new Product
      /// </summary>
      public string[] PathImages { get; set; }

      public List<ProductImageModel> ProductImage { get; set; }
      public DateTime? CreatedDate { get; set; }
      public DateTime? UpdatedDate { get; set; }
      public int? Status { get; set; }
   }
}
