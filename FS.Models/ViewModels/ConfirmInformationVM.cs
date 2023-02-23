using FS.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.ViewModels
{
    public class ConfirmInformationVM
    {

        public User ApplicationUser { get; set; }

        public string Description { get; set; }

        #region Payment Types
        public SelectList PaymentTypes { get; set; }

        [Required(ErrorMessage = "لطفا نوع پرداخت را تعیین کنید!")]
        public string SelectedPaymentType { get; set; }
        #endregion
        #region Deliver
        public SelectList DeliverDate { get; set; }
        public SelectList DeliverTime { get; set; }

        public string SelectedDeliverDate { get; set; }
        public string SelectedDeliverTime { get; set; }
        #endregion

    }
}
