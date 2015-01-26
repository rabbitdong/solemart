using System.Web.Mvc;

namespace Xxx.Web.Areas.Acknowledge {
    public class AcknowledgeAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Acknowledge";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Acknowledge_default",
                "Acknowledge/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
