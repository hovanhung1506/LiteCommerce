using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SV18T1021130.DomainModel;

namespace SV18T1021130.Web.Models
{
    /// <summary>
    /// Kết quả tìm kiếm và lấy dữ liệu khách hàng dưới dạng phân trang
    /// </summary>
    public class CustomerPaginationResultModel : PaginationResultModel
    {
        public List<Customer> Data { get; set; }
    }
}