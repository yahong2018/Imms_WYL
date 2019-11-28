using System.Linq;
using Imms.Security.Data.Domain;

namespace Imms.WebManager.Models
{
    public class ProgramWithPrivilgeViewModel : BaseProgram
    {
        public object[] Children { get; set; }
        public string DataType { get; set; }
        public bool Checked { get; set; }
        public long ProgramPrivilegeId { get; set; }
        public bool Expanded{get;set;}

        public ProgramWithPrivilgeViewModel() { }
        public ProgramWithPrivilgeViewModel(SystemProgram program)
        {
            this.Assign(program);
        }

        public override void Assign(BaseProgram other)
        {
            base.Assign(other);
            if (!(other is SystemProgram))
            {
                return;
            }

            SystemProgram program = other as SystemProgram;
            this.ProgramPrivilegeId = program.Privielges.Where(x => x.PrivilegeCode == ProgramPrivilege.PRIVILEGE_RUN).Single().RecordId;

            this.Children = new ProgramWithPrivilgeViewModel[program.Children.Count];
            this.DataType = "app.model.admin.SystemMenuTreeModel";

            for (int i = 0; i < this.Children.Length; i++)
            {
                ProgramWithPrivilgeViewModel childMenu = new ProgramWithPrivilgeViewModel();
                SystemProgram childProgram = program.Children[i];
                childMenu.Assign(childProgram);
                this.Children[i] = childMenu;
            }

            if (this.Children.Length == 0)
            {
                this.Children = (from item in program.Privielges
                                 where item.PrivilegeCode != ProgramPrivilege.PRIVILEGE_RUN
                                 select new
                                 {
                                     item.RecordId,
                                     item.ProgramId,
                                     item.PrivilegeCode,
                                     item.PrivilegeName,
                                     Checked = false,
                                     DataType = "app.model.admin.ProgramPrivilegeModel"
                                 }).ToArray();
            }

            if(this.Children.Length>0){
                this.Expanded=true;
            }
        }
    }
}