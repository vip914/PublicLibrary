using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PublicLibrary.Domain.Models.Secirity;

namespace PublicLibrary.Web.Models
{
    public abstract class BaseViewPage : WebViewPage
    {
        public virtual new Principal User
        {
            get { return base.User as Principal; }
        }
    }
    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new Principal User
        {
            get { return base.User as Principal; }
        }
    }
}