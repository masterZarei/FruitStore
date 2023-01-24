using System;

namespace FS.Models.ViewModels
{
    public class AdminIndexOrderVM
    {
        public string FullName { get; set; }
        public int OrderId { get; set; }
        public int OrderCount { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public string ID { get; set; }
        public bool ReadyToDeliver { get; set; }
    }
}
