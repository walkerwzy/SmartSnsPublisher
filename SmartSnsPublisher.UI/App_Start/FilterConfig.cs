using System.Web;
using System.Web.Mvc;
using SmartSnsPublisher.UI.Filters;

namespace SmartSnsPublisher.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyExceptionHandlerAttribute());
        }
    }
}
