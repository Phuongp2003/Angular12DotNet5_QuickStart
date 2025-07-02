using Microsoft.VisualBasic;

namespace MyCURDApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateAndTime DateOfBirth { get; set; }
        public string Avatar { get; set; }
        public int DepartmentId { get; set; }
    }
}
