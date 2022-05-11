using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SV18T1021130.Web.Models
{
    public class ProductCreate
    {
        public Product product { get; set; }
        public List<Category> categories { get; set; }
        public List<Supplier> suppliers { get; set; }
    }
}