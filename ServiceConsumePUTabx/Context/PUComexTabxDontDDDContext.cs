using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ServiceConsumePUTabx.Context
{
    public class PUComexTabxDontDDDContext : DbContext
    {
        public PUComexTabxDontDDDContext() : base("name=PUComexTabxDontDDD")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}