using Imms.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imms.Mes.Data
{
    public class MesModelBuilder : ICustomModelBuilder
    {
        public void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");
            
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorklineConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.OrgConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorkshopConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorkstationConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.OperatorConfigure());

            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorkorderConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorkorderActualConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.ActiveWorkorderConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.LineProductSumaryDateSpanConfigure());            

            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.DefectConfigure());

            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorkshiftConfigure());
            modelBuilder.ApplyConfiguration(new Imms.Mes.Data.Domain.WorkshiftSpanConfigure());            
        }
    }
}
