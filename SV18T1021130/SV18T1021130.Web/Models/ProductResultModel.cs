using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SV18T1021130.Web.Models
{
    public class ProductResultModel
    {
        public Product product { get; set; }
        public List<ProductPhoto> productPhotos { get; set; }
        public List<ProductAttribute> productAttributes { get; set; }
    }
}