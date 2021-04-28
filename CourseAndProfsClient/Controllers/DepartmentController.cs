namespace CourseAndProfsClient.Controllers
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using CourseAndProfsPersistence;
  using CourseAndProfsPersistence.Helpers;
  using CourseAndProfsPersistence.Models;
  using Helpers;
  using CourseAndProfsClientModels;
  using CourseAndProfsClientModels.Dto;

  using Kritikos.PureMap.Contracts;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  [Route("api/department")]
  [ApiController]
  public class DepartmentController : BaseController<DepartmentController>
  {
    public DepartmentController(ILogger<DepartmentController> logger, CaPDbContext ctx, IPureMapper mapper)
      : base(logger, ctx, mapper)
    {
    }

    /// <summary>
    /// Returns all departments. You can pass parameters to handle page and result count.
    /// </summary>
    /// <param name="itemsPerPage">Define how many items shall be returned. </param>
    /// <param name="page">Choose which page of the results shall be returned.</param>
    /// <returns>Returns a list of Departments.</returns>
    [HttpGet("")]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments(int itemsPerPage = 20, int page = 1)
    {
      var toSkip = itemsPerPage * (page - 1);

      var departmentsQuery = Context.Departments
        .TagWith("Retrieving all departments")
        .OrderBy(x => x.Id);

      var totalDepartments = await departmentsQuery.CountAsync();

      if (page > ((totalDepartments / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }

      var pagedDepartments = await departmentsQuery
        .Skip(toSkip)
        .Take(itemsPerPage)
        .ToListAsync();

      var result = new PagedResult<DepartmentDto>
      {
        Results = pagedDepartments.Select(x => Mapper.Map<Department, DepartmentDto>(x)).ToList(),
        Page = page,
        TotalPages = (totalDepartments / itemsPerPage) + 1,
        TotalElements = totalDepartments,
      };

      return Ok(result);
    }

    /// <summary>
    /// Returns an department provided an ID.
    /// </summary>
    /// <param name="id">Department's ID.</param>
    /// <returns>One single Department.</returns>
    /// <response code="400">Department was not found.</response>
    [HttpGet("{id}")]
    public ActionResult<DepartmentDto> GetDepartment(long id)
    {
      var department = Context.Departments.SingleOrDefault(x => x.Id == id);

      if (department == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Department), id);
        return NotFound($"No {nameof(Department)} with Id {id} found in database");
      }

      Logger.LogInformation(LogTemplates.RequestEntity, nameof(Department), id);

      return Ok(Mapper.Map<Department, DepartmentDto>(department));
    }

    /// <summary>
    /// Adds an department provided the necessary information.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<ActionResult<DepartmentDto>> AddDepartment([FromBody] AddDepartmentDto dto)
    {
      var department = Mapper.Map<AddDepartmentDto, Department>(dto);

      Context.Departments.Add(department);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.CreatedEntity, nameof(Department), department);

      return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, Mapper.Map<Department, DepartmentDto>(department));
    }

    /// <summary>
    /// We delete a user provided an ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("")]
    public async Task<ActionResult> DeleteDepartment(int id)
    {
      var department = Context.Departments.SingleOrDefault(x => x.Id == id);

      if (department == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Department), id);
        return NotFound("No department found in the database");
      }

      Context.Departments.Remove(department);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Deleted, nameof(Department), id);

      return NoContent();
    }

    /// <summary>
    /// We update an Department provided all the necessary information. Id is required.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, AddDepartmentDto dto)
    {
      var department = Context.Departments.SingleOrDefault(x => x.Id == id);

      if (department == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Department), id);
        return NotFound($"No {nameof(Department)} with Id {id} found in database");
      }

      department.Name = dto.Name;

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Updated, nameof(Department), department);

      return Ok(Mapper.Map<Department, DepartmentDto>(department));
    }
  }
}

