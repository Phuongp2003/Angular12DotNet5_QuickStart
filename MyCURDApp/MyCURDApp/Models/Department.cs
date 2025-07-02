namespace MyCURDApp.Models
{
    public class Department
    {
        public Department(int _departmentId, string _departmentName, int _departmentSize)
        {
            this.DepartmentId = _departmentId;
            this.DepartmentName = _departmentName;
            this.DepartmentSize = _departmentSize;
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentSize { get; set; }
    }
}
