using System.ComponentModel;

namespace Samovar.Grid.TestPages.Data;

public class Employee
{
    [DisplayName("ID")]
    public int Id { get; set; }

    [DisplayName("First Name")]
    public string FirstName { get; set; } = string.Empty;

    [DisplayName("Last Name")]
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    [DisplayName("Job Title")]
    public string JobTitle { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    [DisplayName("Hire Date")]
    public DateOnly HireDate { get; set; } = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);

    [DisplayName("Active")]
    public bool IsActive { get; set; }

    public string Phone { get; set; } = string.Empty;
}
