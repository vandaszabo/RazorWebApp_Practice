using MintaProjekt.Enums;

namespace MintaProjekt.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public DepartmentName DepartmentName { get; set; }
        public List<Employee> Employees { get; set; }

        public List<Employee> Leaders { get; set; }

        public Department() {
            Employees = new List<Employee>();
            Leaders = new List<Employee>();
        }

        public Department(int departmentID, DepartmentName departmentName, List<Employee> employees, List<Employee> leaders)
        {
            DepartmentID = departmentID;
            DepartmentName = departmentName;
            Employees = employees;
            Leaders = leaders;
        }

    }
}
