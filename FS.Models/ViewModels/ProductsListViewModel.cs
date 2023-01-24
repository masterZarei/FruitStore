using FS.Models.Models;
using FS.Models.Paging;
using System.Collections.Generic;

namespace FS.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public List<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
