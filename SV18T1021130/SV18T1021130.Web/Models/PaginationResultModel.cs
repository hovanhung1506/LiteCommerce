using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SV18T1021130.Web.Models
{
    /// <summary>
    /// Lớp cơ sở cho model chứa dữ liệu dưới dạng phân trang
    /// </summary>
    public abstract class PaginationResultModel
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Số dòng trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Giá trị tìm kiếm
        /// </summary>
        public string SearchValue { get; set; }
        /// <summary>
        /// Tổng số dòng dữ liệu truy vấn được
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// Số trang
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int p = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    p += 1;
                return p;
            }
        }
    }
}