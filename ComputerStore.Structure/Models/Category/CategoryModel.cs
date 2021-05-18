//-----------------------------------------------------------------------
// <copyright file="CategoryModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Category
{
    public class CategoryModel
    {
        public CategoryModel()
        {
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public int? ParentId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string TemplateMetaData { get; set; }
        [MaxLength(4000)]
        public string TemplateSpecificData { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<CategoryModel> Categories { get; set; }

        public override bool Equals(object obj)
        {
            return obj is CategoryModel model &&
                   Id == model.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
