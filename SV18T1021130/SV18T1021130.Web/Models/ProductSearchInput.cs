using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SV18T1021130.Web.Models
{
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public List<Category> Categories { get; set; }
        public List<Supplier> Suppliers { get; set; }
    }
}