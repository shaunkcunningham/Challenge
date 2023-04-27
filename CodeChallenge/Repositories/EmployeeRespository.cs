using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.Include(i => i.DirectReports)
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();

            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetCompensationById(string id)
        {
            return _employeeContext.Compensations.Include(i => i.Employee)
                .SingleOrDefault(e => e.Employee.EmployeeId == id);
        }

        public Compensation RemoveCompensation(Compensation compensation)
        {
            return _employeeContext.Remove(compensation).Entity;
        }


    }
}
