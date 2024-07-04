namespace MintaProjekt.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int? LeaderID { get; set; }
        public List<Employee> Employees { get; set; }

        public Department() {
            Employees = new List<Employee>();
        }

        public Department(int departmentID, string departmentName, int? leaderID)
        {
            DepartmentID = departmentID;
            DepartmentName = departmentName;
            LeaderID = leaderID;
            Employees = new List<Employee>();
        }
    }
}
