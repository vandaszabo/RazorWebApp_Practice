namespace MintaProjekt.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int LeaderID { get; set; }

        public Department() { }

        public Department(int departmentID, string departmentName, int leaderID)
        {
            DepartmentID = departmentID;
            DepartmentName = departmentName;
            LeaderID = leaderID;
        }
    }
}
