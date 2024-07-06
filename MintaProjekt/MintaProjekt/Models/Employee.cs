namespace MintaProjekt.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly HireDate { get; set; }
        public string? JobTitle { get; set; }
        public int? DepartmentID { get; set; }

        // Computed property for full name
        public string FullName => $"{FirstName} {LastName}";

        public Employee(){}

        public Employee(int employeeID, string firstName, string lastName, string email, string phoneNumber, DateOnly hireDate, string jobTitle, int departmentID)
        {
            EmployeeID = employeeID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            HireDate = hireDate;
            JobTitle = jobTitle;
            DepartmentID = departmentID;
        }

        // Method to check for null values, must invoke before database insert
        public bool HasInvalidProperties()
        {
            return string.IsNullOrWhiteSpace(FirstName) ||
                   string.IsNullOrWhiteSpace(LastName) ||
                   string.IsNullOrWhiteSpace(Email) ||
                   string.IsNullOrWhiteSpace(PhoneNumber) ||
                   HireDate == DateOnly.MinValue ||
                   string.IsNullOrWhiteSpace(JobTitle) ||
                   !DepartmentID.HasValue;
        }
    }

}
