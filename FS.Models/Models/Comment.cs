using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Models.Models
{
    public class Comments : BaseClass
    {
        public int Id { get; set; }
        //
        public string User_Id { get; set; }
        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
        //
        public int Product_Id { get; set; }
        [ForeignKey("Product_Id")]
        public virtual Product Product { get; set; }
        //
        [Required(ErrorMessage = "لطفا نظرتان را وارد کنید")]
        [Display(Name = "متن نظر")]
        public string Text { get; set; }
        //
        public bool isVerified { get; set; }
        //
        public string Answer { get; set; }
        //
        public string Responder { get; set; }

    }
}
