﻿using FS.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Models.Models
{
    public class Factor:BaseClass
    {
        [Key]
        public int FactorId { get; set; }
        //
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        //
        public bool IsFinally { get; set; }
        //
        public string PurchaseNumber { get; set; }
        //
        public string Post_Type { get; set; }
        //
        public DateTime Send_Date { get; set; }
        //
        public DateTime WillDeliver_Date { get; set; }
        //
        public List<FactorDetail> FactorDetails { get; set; }
        //
        public bool isReadyToDeliver { get; set; }
        //
        public bool isCompleted { get; set; }
        //
        public string Description { get; set; }
    }
}