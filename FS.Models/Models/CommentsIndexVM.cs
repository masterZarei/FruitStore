using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.Models.Models
{
    public class CommentsIndexVM
    {
        public int productId { get; set; }
        public string Name { get; set; }
        public int CommentsCount { get; set; }
    }
}
