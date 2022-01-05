using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DEVA.API.Models
{
    public class PostDetail
    {
        [Key]
        public int PostId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string OwnerUsername { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(600)")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(22)")]
        public string UploadingDate { get; set; }
    }
}