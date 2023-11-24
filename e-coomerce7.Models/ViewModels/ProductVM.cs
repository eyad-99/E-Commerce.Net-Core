using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace e_coomerce7.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { set; get; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { set; get;}
    }
}
