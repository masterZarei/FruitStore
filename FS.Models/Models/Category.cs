using FS.Models.BaseEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class Category : BaseClass
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا نام دسته بندی را وارد کنید")]
        [Display(Name = "نام دسته بندی")]
        public string Name { get; set; }

        [Display(Name = "توضیحات دسته بندی")]
        public string Description { get; set; }
        public List<Product>? Products { get; set; }
    }
}
