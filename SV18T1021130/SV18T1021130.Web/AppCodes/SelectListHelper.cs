using SV18T1021130.BusinessLayer;
using SV18T1021130.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SV18T1021130.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectListHelper
    {
        /// <summary>
        /// Danh sách các quốc gia
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Countries()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach(var item in CommonDataService.ListOfCountries())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CountryName,
                    Text = item.CountryName
                });
            }
            return list;
        }
    }
}