using Allup_Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.UI.Dtos
{
    public class AboutDto
    {
        public List<Brand> Brands { get; set; } = null!;
        public List<FeaturesBanner> FeaturesBanners { get; set; } = null!;
        public List<About> Abouts { get; set; } = null!;

    }
}
