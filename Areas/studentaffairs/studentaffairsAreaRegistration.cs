using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Areas.studentaffairs
{
    public class studentaffairsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "studentaffairs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "studentaffairs_default",
                "studentaffairs/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}