using Bogus;

namespace Test.Pages.Data;

public class EmployeeService
{
    private static readonly Faker<Employee> _faker = new Faker<Employee>()
        .RuleFor(e => e.Id, f => f.IndexFaker + 1)
        .RuleFor(e => e.FirstName, f => f.Name.FirstName())
        .RuleFor(e => e.LastName, f => f.Name.LastName())
        .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName))
        .RuleFor(e => e.Department, f => f.Commerce.Department())
        .RuleFor(e => e.JobTitle, f => f.Name.JobTitle())
        .RuleFor(e => e.Salary, f => f.Finance.Amount(30_000, 200_000, 2))
        .RuleFor(e => e.HireDate, f => DateOnly.FromDateTime(f.Date.Past(10)))
        .RuleFor(e => e.IsActive, f => f.Random.Bool(0.85f))
        .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber());

    public Task<Employee[]> GetEmployeesAsync(int count = 1_000)
        => Task.Run(() => _faker.Generate(count).ToArray());

    public Task<HashSet<Employee>> GetEmployeesHashSetAsync(int count = 1_000_000)
        => Task.Run(() => _faker.Generate(count).ToHashSet());
}
