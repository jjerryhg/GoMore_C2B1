﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoMore_C2B1.Models
{
    [Table("FactoryModel", Schema = "public")]
    public class FactoryModel
    {
        [Key]
        public string ID { get; set; }
        public string FileName { get; set; }
        public string UploadTime { get; set; }
        public string Uploader { get; set; }
        public string FileType { get; set; }
    }
}