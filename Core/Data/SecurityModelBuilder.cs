using Imms.Data;
using Imms.Security.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Imms.Security.Data{


    public class SecurityModelBuilder:ICustomModelBuilder{
        public  void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProgramPrivilegeConfigure());
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfigure());
            modelBuilder.ApplyConfiguration(new RoleUserConfigure());
            modelBuilder.ApplyConfiguration(new SystemProgramConfigure());
            modelBuilder.ApplyConfiguration(new SystemRoleConfigure());
            modelBuilder.ApplyConfiguration(new SystemUserConfigure());          
            
        }    
    }
}