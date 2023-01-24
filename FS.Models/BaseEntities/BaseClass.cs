using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.Models.BaseEntities
{
    public class BaseClass
    {
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
