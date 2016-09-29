using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EA.DataModel;

namespace EmailAnalytics.Models
{
    public class PackageViewModel
    {
        public string FileName { get; set; }
        public string UploadDate { get; set; }
        public string ProcessedDate { get; set; }

        public PackageViewModel()
        { }

        public PackageViewModel(usp_SearchPackageDetails_Result package)
        {
            FileName = package.FileName;
            UploadDate = Convert.ToString(package.UploadDate);

            if (Convert.ToString(package.ProcessedDate) == null || Convert.ToString(package.ProcessedDate).Trim().Length <= 0)
            {
                ProcessedDate = "Not Processed";
            }
            else
            {
                ProcessedDate = Convert.ToString(package.ProcessedDate);
            }
        }
    }
}