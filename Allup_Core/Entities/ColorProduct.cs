using Allup_Core.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Core.Entities
{
    public class ColorProduct:BaseEntity
    {
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public Product Product { get; set; } 
        public Color Color { get; set; } 
    }
}
