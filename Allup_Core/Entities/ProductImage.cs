using Allup_Core.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Core.Entities
{
    public class ProductImage:BaseEntity
    {
        public int ProductId { get; set; }
        public bool IsCover { get; set; } //true = uz qabigi, false = Hower , null = detail sekilleri
        public string ImageUrl { get; set; }
        public Product Product { get; set; }
    }
}
