using System.Web.Mvc;

namespace Xxx.Web.Areas.Manager.Controllers {
    public class ManagerAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Manager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Manager_default",
                "Manager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[]{"Xxx.Web.Areas.Manager.Controllers"}
            );
        }
    }
}
