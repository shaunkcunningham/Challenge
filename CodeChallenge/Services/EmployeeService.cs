using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;
        int directReportsRunningTotal;
       
        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

     
        public ReportingStructure GetReportingStructureById(String employeeId)
        {
            ReportingStructure employeeReportingStructure = new ReportingStructure();

            //Get employee rercord
            Employee employee = GetById(employeeId);

            if (employee != null)
            {
                //If it is the first time the object is being populated, add the name of the employee
                if (String.IsNullOrEmpty(employeeReportingStructure.Employee))
                {
                    employeeReportingStructure.Employee = employee.FirstName + " " + employee.LastName;

                }


                if (employee.DirectReports != null)
                {
                    //If directreports are more than 0, add the total to directReportsRunningTotal
                    if (employee.DirectReports.Count() > 0)
                    {
                        directReportsRunningTotal += employee.DirectReports.Count();

                        //Create a recursive call to continue counting directreports in the tree
                        foreach (Employee employee1 in employee.DirectReports)
                        {
                            GetReportingStructureById(employee1.EmployeeId);
                        }
                    }
                }

                employeeReportingStructure.NumberOfReports = directReportsRunningTotal;
            }

            // If record is not found, return null
            if (directReportsRunningTotal==0 && employeeReportingStructure.Employee == null)
            {
               return null;
            }
       
            return employeeReportingStructure;
        }

        public Compensation CreateCompensation(Compensation compensation)
        {   try
            {
                if (compensation != null)
                {
                    //Remove any existing compensation records for the employee so that only one record exists at any given time.
                    Compensation existingCompensation = GetCompensationById(compensation.Employee.EmployeeId);
                    if (existingCompensation != null)
                    {
                        _employeeRepository.RemoveCompensation(existingCompensation);
                        _employeeRepository.SaveAsync().Wait();
                    }

                    //Remove any existing employee records so that duplicate employee records are not created
                    Employee existingEmployee = GetById(compensation.Employee.EmployeeId);
                    if (existingEmployee != null)
                    {
                        _employeeRepository.Remove(existingEmployee);
                        _employeeRepository.SaveAsync().Wait();
                    }

                    //Create the employee and compensation records
                    _employeeRepository.AddCompensation(compensation);
                    _employeeRepository.SaveAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                compensation= null;
       
            }

            return compensation;
        }

        public Compensation GetCompensationById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetCompensationById(id);
            }

            return null;
        }

    }
}
