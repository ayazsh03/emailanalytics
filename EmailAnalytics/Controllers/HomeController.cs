using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EA.DataModel;
using EmailAnalytics.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using Excel = Microsoft.Office.Interop.Excel;

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

        private bool ValidateFileFormat(string path)
        {
            bool isValid = false;

            try
            {
                Excel.Application excelApp;
                Excel.Workbook excelWorkBook;
                Excel.Worksheet excelWorkSheet;
                Excel.Range range;

                int colCount = 0;
                string[] validColNames = new string[] { "Full Name", "Email", "Address", "City", "State", "Zip Code", "County", "Gender", "Birthday", "Ethnicity", "Desi" };

                excelApp = new Excel.Application();
                excelWorkBook = excelApp.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                excelWorkSheet = (Excel.Worksheet)excelWorkBook.Worksheets.get_Item(1);
                string excelSheetName = excelWorkSheet.Name;

                range = excelWorkSheet.UsedRange;

                if (excelSheetName == "Sheet")
                {
                    if (range.Columns.Count == validColNames.Length)
                    {
                        for (colCount = 1; colCount <= range.Columns.Count; colCount++)
                        {
                            string excelColName = (string)(range.Cells[colCount][1] as Excel.Range).Value2;
                            string colName = validColNames[colCount - 1];

                            if (excelColName == colName)
                            {
                                isValid = true;
                            }
                            else
                            {
                                isValid = false;

                                break;
                            }
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }
                else
                {
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.StackTrace);
            }

            return isValid;
        }

        [HttpPost]
        public ViewResult UploadPackage(HttpPostedFileBase file)
        {
            bool isSuccess = false;

            var files = Request.Files;

            try
            {
                isSuccess = CheckFileType(Path.GetExtension(file.FileName));

                if (isSuccess)
                {
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);

                            if (ValidateFileName(fileName))
                            {
                                var path = Path.Combine(Server.MapPath("~/ExcelFileUpload"), fileName);
                                file.SaveAs(path);

                                if (ValidateFileFormat(path))
                                {
                                    db.usp_InsertPackageDetails(fileName);
                                }
                                else
                                {
                                    System.IO.File.Delete(path);
                                    ViewData["Message"] = "File cannot be saved as file format is incorrect";

                                    return View("Failure");
                                }
                            }
                            else
                            {
                                ViewData["Message"] = "File cannot be saved as file name is invalid";

                                return View("Failure");
                            }
                        }
                    }
                }
                else
                {
                    ViewData["Message"] = "File cannot be saved as file format is not valid";

                    return View("Failure");
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.StackTrace);

                ViewData["Message"] = "File cannot be saved as error occured";

                return View("Failure");
            }

            ViewData["Message"] = "File saved successfully";

            return View("Success");
        }

        private bool CheckFileType(string fileName)
        {
            string[] fileType = new string[] { ".xlsx", ".xls", ".csv" };

            if (fileType.Contains(fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateFileName(string fileName)
        {
            bool isValid = false;
            char[] splitString = { '_' };
            string[] fileNameArray = fileName.Split(splitString);
            int arrayLength = fileNameArray.Length;

            string[] listOfValidSources = new string[] { "RawData", "Actel", "Mannual", "Adhoc" };
            string[] listOfValidNames = new string[] { "Wasim", "Mateen", "Rizwan", "Manmohan", "Sanjay" };

            if (arrayLength > 0 && (arrayLength >= 3 && arrayLength <= 4))
            {
                string source = Convert.ToString(fileNameArray[0]);
                string name = Convert.ToString(fileNameArray[1]);
                string date = String.Empty;

                if (arrayLength == 3)
                {
                    string[] dateArray = fileNameArray[2].Split('.');

                    date = Convert.ToString(dateArray[0]);
                }
                else
                {
                    date = Convert.ToString(fileNameArray[2]);
                }

                if (listOfValidSources.Contains(fileNameArray[0]))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                if (isValid)
                {
                    if (listOfValidNames.Contains(name))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }
                }

                int mm, dd, yy;

                if (isValid)
                {
                    if (Int32.TryParse(date.Substring(0, 2), out mm))
                    {
                        if (mm <= 12)
                        {
                            isValid = true;
                        }
                        else
                        {
                            isValid = false;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    if (Int32.TryParse(date.Substring(2, 2), out dd))
                    {
                        if (dd <= 31)
                        {
                            isValid = true;
                        }
                        else
                        {
                            isValid = false;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    if (Int32.TryParse(date.Substring(4, 2), out yy))
                    {
                        if (yy >= 16)
                        {
                            isValid = true;
                        }
                        else
                        {
                            isValid = false;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        public ViewResult Failure()
        {
            return View();
        }

        public ViewResult Success()
        {
            return View();
        }
    }
}