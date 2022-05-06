using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SV18T1021130.DomainModel;

namespace SV18T1021130.Web.Models
{
    public class SupplierPaginationResultModel : PaginationResultModel
    {
        public List<Supplier> Data { get; set; }
    }
}