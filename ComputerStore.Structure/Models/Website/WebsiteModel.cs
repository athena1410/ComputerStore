using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ComputerStore.Structure.Models.Website
{
    public class WebsiteModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        public string SecretKey { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string UrlPath { get; set; }
        public string LogoUrl { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }

        public bool IsDisable { get; set; }
    }
}
