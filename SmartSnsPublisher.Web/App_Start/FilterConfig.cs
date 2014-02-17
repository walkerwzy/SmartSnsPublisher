using System.Web;
using System.Web.Mvc;
using SmartSnsPublisher.Web.Filters;

namespace SmartSnsPublisher.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyExceptionHandlerAttribute());
        }
    }
}
