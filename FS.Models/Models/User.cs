using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "لطفا نام خود را وارد کنید")]
        [Display(Name = "نام ")]
        [MaxLength(200)]
        public string Name { get; set; }
        //
        [Required(ErrorMessage = "لطفا نام خانوادگی خود را وارد کنید")]
        [Display(Name = "نام خانوادگی ")]
        [MaxLength(200)]
        public string LastName { get; set; }
        //

        [Display(Name = "استان")]
        //[Required(ErrorMessage = "لطفا نام شهر خود را وارد کنید")]
        [MaxLength(150)]
        public string State { get; set; }
        //
        [Display(Name = "شهر")]
        //[Required(ErrorMessage = "لطفا نام شهر خود را وارد کنید")]
        [MaxLength(100)]
        public string City { get; set; }
        //
        [Display(Name = "آدرس")]
        //[Required(ErrorMessage = "لطفا آدرس خود را وارد کنید")]
        [MaxLength(2500)]
        public string Address { get; set; }
        //
        [Display(Name = "کد پستی")]
        [MaxLength(200)]
        public string PostalCode { get; set; }
        //
        [Display(Name = "ایمیل")]
        public override string Email { get => base.Email; set => base.Email = value; }
        //
        [Required(ErrorMessage = "لطفا شماره همراه خود را وارد کنید")]
        [Display(Name = "شماره همراه")]
        [MaxLength(11)]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        //
        [Required(ErrorMessage = "لطفا کد تایید را وارد کنید")]
        [Display(Name = "کد تایید")]
        public string VerificationCode { get; set; }
        //
        [Display(Name = "تاریخ ثبت نام")]
        public DateTime reg_Date { get; set; } = DateTime.Now;
        //
        [Display(Name = "(حسابی با نام خودتان) شماره کارت")]
        [MaxLength(16)]
        public string CartNumber { get; set; }
        //
        public bool isDisabled { get; set; }
        //
        public bool isVerified { get; set; }
    }
}
