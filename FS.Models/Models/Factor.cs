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
        public string Payment_Type { get; set; }
        //
        public string Deliver_Date { get; set; }
        //
        public string Deliver_Time { get; set; }
        //
        public List<FactorDetail> FactorDetails { get; set; }
        // 1=در حال آماده سازی
        // 2= اماده ارسال
        // 3= ارسال شده
        public byte DeliverState { get; set; }
        //
        public bool isCompleted { get; set; }
        //
        public string Description { get; set; }
    }
}
