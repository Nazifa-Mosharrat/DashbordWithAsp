using AsianJP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
namespace AsianJP.Controllers
{
    public class HomeController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["NswCon"].ConnectionString;
        string query = string.Empty;
        SqlConnection Connection;
        //This is for All the Charts and table on the dashbord except barchart
        public ActionResult DashboardView()
        {

            int /*ClientWiseJob = 0,*/ TotalJob =0, JobDone=0, JobInProgress=0, TotalClients=0;
          

            if (Session["userName"] == null) return RedirectToAction("Index", "Login");
            Connection = new SqlConnection(conStr);
            query = 
                    "select count(jobNo) as TotalJob," +
                    "(select count(JobID) from[ProcessVw] where ProcessID != 28) as JobInProgress," +
                    "(select count(JobID) from[ProcessVw] where ProcessID = 28) as JobDone," +
                    "(SELECT count(DISTINCT clientId) FROM[jobVw])as TotalClients  from[jobVw] where eStatus = 0;";
            SqlCommand Command2 = new SqlCommand(query, Connection);
            Connection.Open();
            SqlDataReader Reader2 = Command2.ExecuteReader();
            Reader2.Read();
            if (Reader2.HasRows)
            {
               
                TotalJob     = Convert.ToInt32(Reader2["TotalJob"]);
                JobDone       = Convert.ToInt32(Reader2["JobDone"]);
                JobInProgress = Convert.ToInt32(Reader2["JobInProgress"]);
                TotalClients  = Convert.ToInt32(Reader2["TotalClients"]);

            }
            Connection.Close();
           

            ViewBag.TotalJob     = TotalJob;
            ViewBag.JobDone      = JobDone      ;
            ViewBag.JobInProgress= JobInProgress;
            ViewBag.TotalClients = TotalClients;
            return View();
        }
        //This is for the Bar Chart
        public JsonResult GetJobList()
        {

            DataTable dt = DB.rtnDT("select clientName, count(jobNo) as totalJob from JobVw where [jobDate] <= GetDate() and [jobDate] >= DATEADD(day,-7, GetDate()) group by clientName");
            List<JobVw> jobVw = new List<JobVw>();
            jobVw = (from DataRow dr in dt.Rows
                          select new Models.JobVw()
                          {
                              JobID = Convert.ToInt32(dr["totalJob"]),
                              ClientName = dr["clientName"].ToString(),
                             
                          }).ToList();

            return Json(jobVw, JsonRequestBehavior.AllowGet);
        }
            public ActionResult About()
        {
            if (Session["userName"] == null) return RedirectToAction("Index", "Login");
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            if (Session["userName"] == null) return RedirectToAction("Index", "Login");
            ViewBag.Message = "Your contact page.";
            return View();
        }



    }
}
