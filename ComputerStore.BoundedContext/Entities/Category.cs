//-----------------------------------------------------------------------
// <copyright file="Category.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ComputerStore.BoundedContext.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string TemplateMetaData { get; set; }
        public string TemplateSpecificData { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
