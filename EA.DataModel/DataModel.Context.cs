﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EA.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EmailAnalyticsEntities : DbContext
    {
        public EmailAnalyticsEntities()
            : base("name=EmailAnalyticsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Raw_Data> Raw_Data { get; set; }
    
        public virtual ObjectResult<usp_SearchPackageDetails_Result> usp_SearchPackageDetails(Nullable<System.DateTime> from, Nullable<System.DateTime> to)
        {
            var fromParameter = from.HasValue ?
                new ObjectParameter("From", from) :
                new ObjectParameter("From", typeof(System.DateTime));
    
            var toParameter = to.HasValue ?
                new ObjectParameter("To", to) :
                new ObjectParameter("To", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_SearchPackageDetails_Result>("usp_SearchPackageDetails", fromParameter, toParameter);
        }
    }
}
