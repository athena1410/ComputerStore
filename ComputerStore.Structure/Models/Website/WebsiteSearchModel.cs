using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ComputerStore.Structure.Models.Website
{
    public class WebsiteSearchModel
    {
        [MaxLength(100, ErrorMessage = "Search string only 100 characters")]
        public string Name { get; set; }
        public string UrlPath { get; set; }
        public int CompanyId { get; set; }
    }
}
