using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PublicLibrary.Domain.Models.Secirity;

namespace PublicLibrary.Web.Controllers
{
    public class BaseController : Controller
    {
        protected virtual new Principal User
        {
            get { return HttpContext.User as Principal; }
        }
    }
}