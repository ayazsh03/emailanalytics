using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EA.DataModel;
using EmailAnalytics.Models;
using System.Web.Script.Serialization;
using System.IO;

namespace EmailAnalytics.Controllers
{
    public class HomeController : Controller
    {
        private EmailAnalyticsEntities db = new EmailAnalyticsEntities();

        public ActionResult SearchPackage()
        {
            return View();
        }

        private void LogError(string message, string stackTrace)
        {
            FileStream fs = new FileStream(Server.MapPath("~/LogError.txt"), FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs);

            writer.WriteLine(DateTime.Now + " / Error Message: " + message + " / Stack Trace: " + stackTrace);
            writer.Flush();
            writer.Close();
            fs.Close();
        }

        public ActionResult PackageDetails(FormCollection form)
        {
            try
            {
                string from = form["fromDate"].ToString();
                string to = form["toDate"].ToString();

                DateTime fromDate = DateTime.ParseExact(from, "MM-dd-yyyy", null);
                DateTime toDate = DateTime.ParseExact(to, "MM-dd-yyyy", null);

                TempData["FromDate"] = fromDate;
                TempData["ToDate"] = toDate;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.StackTrace);
            }

            return View();
        }

        public JsonResult GetPackageList()
        {
            List<PackageViewModel> packageViewModel = new List<PackageViewModel>();

            try
            {
                var result = db.usp_SearchPackageDetails((DateTime)TempData["FromDate"], (DateTime)TempData["ToDate"]);               

                if (result != null)
                {
                    foreach (usp_SearchPackageDetails_Result package in result)
                    {
                        packageViewModel.Add(new PackageViewModel(package));
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.StackTrace);
            }

            return Json(packageViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}