namespace CourseAndProfsClient.Controllers
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using CourseAndProfsPersistence;
  using CourseAndProfsPersistence.Helpers;
  using CourseAndProfsPersistence.Models;
  using CourseAndProfsPersistence.Joins;
  using Helpers;
  using CourseAndProfsClientModels;
  using CourseAndProfsClientModels.Dto;

  using Kritikos.PureMap.Contracts;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  [Route("api/professor")]
  [ApiController]
  public class ProfessorsController : BaseController<ProfessorsController>
  {
    public ProfessorsController(ILogger<ProfessorsController> logger, CaPDbContext ctx, IPureMapper mapper)
      : base(logger, ctx, mapper)
    {
    }

    /// <summary>
    /// Returns all professors. You can pass parameters to handle page and result count.
    /// </summary>
    /// <param name="itemsPerPage">Define how many items shall be returned. </param>
    /// <param name="page">Choose which page of the results shall be returned.</param>
    /// <returns>Returns a list of Professors.</returns>
    /// <response code="200">Ok results.</response>
    [HttpGet("")]
    public async Task<ActionResult<List<ProfessorDto>>> GetProfessors(int itemsPerPage = 20, int page = 1)
    {
      var toSkip = itemsPerPage * (page - 1);

      var professorsQuery = Context.Professors
        .TagWith("Retrieving all professors")
        .OrderBy(x => x.Id);

      var totalProfessors = await professorsQuery.CountAsync();

      if (page > ((totalProfessors / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }

      var pagedProfessors = await professorsQuery
        .Skip(toSkip)
        .Take(itemsPerPage)
        .ToListAsync();

      var result = new PagedResult<ProfessorDto>
      {
        Results = pagedProfessors.Select(x => Mapper.Map<Professor, ProfessorDto>(x)).ToList(),
        Page = page,
        TotalPages = (totalProfessors / itemsPerPage) + 1,
        TotalElements = totalProfessors,
      };

      return Ok(result);
    }

    /// <summary>
    /// Returns an professor provided an ID.
    /// </summary>
    /// <param name="id">Professor's ID.</param>
    /// <returns>One single Professor.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Professor was not found.</response>
    [HttpGet("{id}")]
    public ActionResult<ProfessorDto> GetProfessor(long id)
    {
      var professor = Context.Professors.SingleOrDefault(x => x.Id == id);

      if (professor == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Professor), id);
        return NotFound($"No {nameof(Professor)} with Id {id} found in database");
      }

      Logger.LogInformation(LogTemplates.RequestEntity, nameof(Professor), id);

      return Ok(Mapper.Map<Professor, ProfessorDto>(professor));
    }

    /// <summary>
    /// Returns all joined courses for a ProfId.
    /// </summary>
    /// <param name="id">Professor's ID.</param>
    /// <returns>One Professor's Courses.</returns>
    /// <response code="200">Ok results.</response>
    /// <response code="404">Professor was not found.</response>
    /// <response code="404">Courses were not found.</response>
    [HttpGet("professorscourses/{id}")]
    public ActionResult<ProfessorDto> GetProfessorsCourses(long id)
    {
      var professor = Context.Professors.SingleOrDefault(x => x.Id == id);

      if (professor == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Professor), id);
        return NotFound($"No {nameof(Professor)} with Id {id} found in database");
      }

      var profcourses = Context.ProfessorCourses
        .Include(p => p.Professor)
        .Include(c => c.Course)
        .Where(pr => pr.Professor.Id == professor.Id)
        .Select(pc => pc.Course);

      if (profcourses == null)
      {
        return NotFound($"No Courses found for profId {id} in database");
      }

      Logger.LogInformation(LogTemplates.RequestEntity, nameof(Professor), id);

      return Ok(profcourses);
    }

    /// <summary>
    /// Adds an professor provided the necessary information.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<ActionResult<ProfessorDto>> AddProfessor([FromBody] AddProfessorDto dto)
    {
      var professor = Mapper.Map<AddProfessorDto, Professor>(dto);

      Context.Professors.Add(professor);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.CreatedEntity, nameof(Professor), professor);

      return CreatedAtAction(nameof(GetProfessor), new { id = professor.Id }, Mapper.Map<Professor, ProfessorDto>(professor));
    }

    /// <summary>
    /// Adds an professor-course Join.
    /// </summary>
    /// <param name="profId"></param>
    /// <param name="courseId"></param> W
    /// <response code="200">Added successfully.</response>
    /// <response code="404">Professor was not found.</response>
    /// <response code="404">Course was not found.</response>
    [HttpPost("join")]
    public async Task<ActionResult<ProfessorDto>> AddProfessorCourseJoin(long profId, long courseId)
    {
      var professor = Context.Professors.SingleOrDefault(x => x.Id == profId);
      var course = Context.Courses.SingleOrDefault(x => x.Id == courseId);
      if (professor == null)
      {
        return NotFound("Professor doesnt exist");
      }
      if (course == null)
      {
        return NotFound("Course doesnt exist");
      }
      ProfessorCourse professorCourse = new ProfessorCourse { Professor = professor, Course = course, };
      Context.ProfessorCourses.Add(professorCourse);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.CreatedEntity, nameof(Professor), professor);

      return Ok("Added successfully");
    }

    /// <summary>
    /// We delete a Professor provided an ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content.</returns>
    /// <response code="204">Deleted successfully.</response>
    /// <response code="404">Professor was not found.</response>
    [HttpDelete("")]
    public async Task<ActionResult> DeleteProfessor(int id)
    {
      var professor = Context.Professors.SingleOrDefault(x => x.Id == id);

      if (professor == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Professor), id);
        return NotFound("No professor found in the database");
      }

      Context.Professors.Remove(professor);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Deleted, nameof(Professor), id);

      return NoContent();
    }

    /// <summary>
    /// We update an Professor provided all the necessary information. Id is required.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>Ok professor</returns>
    /// <response code="200">Ok.</response>
    /// <response code="404">Professor was not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProfessorDto>> UpdateProfessor(int id, AddProfessorDto dto)
    {
      var professor = Context.Professors.SingleOrDefault(x => x.Id == id);

      if (professor == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Professor), id);
        return NotFound($"No {nameof(Professor)} with Id {id} found in database");
      }

      professor.FullName = dto.FullName;
      professor.Mail = dto.Mail;
      professor.Phone = dto.Phone;
      professor.Office = dto.Office;
      professor.EOffice = dto.EOffice;
      professor.Department = dto.Department;

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Updated, nameof(Professor), professor);

      return Ok(Mapper.Map<Professor, ProfessorDto>(professor));
    }
  }
}

