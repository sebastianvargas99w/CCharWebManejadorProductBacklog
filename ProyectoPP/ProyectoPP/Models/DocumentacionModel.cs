using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProyectoPP.Models
{
    public class DocumentacionModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "ContentType")]
        public string ContentType { get; set; }

        [Display(Name = "Data")]
        public byte[] Data { get; set; }

        [Display(Name = "HUId")]
        public String HUid { get; set; }
    }
}