using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        
        [HttpGet("getReportingStructureById/{employeeId}")]
        public IActionResult GetReportingStructureById(String employeeId)
        {
           
            _logger.LogDebug($"Received employee reporting structure request for '{employeeId}'");

            ReportingStructure employeeReportingStructure = new ReportingStructure();

            employeeReportingStructure = _employeeService.GetReportingStructureById(employeeId);
         
            if (employeeReportingStructure == null)
                return NotFound();

          
            return Ok(employeeReportingStructure);
        }

        [HttpPost("createCompensation")]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
       
            _logger.LogDebug($"Received compensation create request for '{compensation.Employee.FirstName} {compensation.Employee.FirstName}'");

            _employeeService.CreateCompensation(compensation);

            return CreatedAtRoute("getCompensationById", new { employeeId = compensation.Employee.EmployeeId }, compensation);
        }

        [HttpGet("getCompensationById/{employeeId}", Name = "getCompensationById")]
        public IActionResult GetCompensationById(String employeeId)
        {
            _logger.LogDebug($"Received compensation request for employee '{employeeId}'");

            var compensation = _employeeService.GetCompensationById(employeeId);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

    }
}
