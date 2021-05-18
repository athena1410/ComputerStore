//-----------------------------------------------------------------------
// <copyright file="Company.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;

namespace ComputerStore.BoundedContext.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }

        public virtual Website Website { get; set; }
    }
}
