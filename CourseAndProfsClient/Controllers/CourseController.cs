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

  [Route("api/course")]
  [ApiController]
  public class CourseController : BaseController<CourseController>
  {
    public CourseController(ILogger<CourseController> logger, CaPDbContext ctx, IPureMapper mapper)
      : base(logger, ctx, mapper)
    {
    }

    /// <summary>
    /// Returns all courses. You can pass parameters to handle page and result count.
    /// </summary>
    /// <param name="itemsPerPage">Define how many items shall be returned. </param>
    /// <param name="page">Choose which page of the results shall be returned.</param>
    /// <returns>Returns a list of Courses.</returns>
    [HttpGet("")]
    public async Task<ActionResult<List<CourseDto>>> GetCourses(int itemsPerPage = 20, int page = 1)
    {
      var toSkip = itemsPerPage * (page - 1);

      var coursesQuery = Context.Courses
        .TagWith("Retrieving all courses")
        .OrderBy(x => x.Id);

      var totalCourses = await coursesQuery.CountAsync();

      if (page > ((totalCourses / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }

      var pagedCourses = await coursesQuery
        .Skip(toSkip)
        .Take(itemsPerPage)
        .ToListAsync();

      var result = new PagedResult<CourseDto>
      {
        Results = pagedCourses.Select(x => Mapper.Map<Course, CourseDto>(x)).ToList(),
        Page = page,
        TotalPages = (totalCourses / itemsPerPage) + 1,
        TotalElements = totalCourses,
      };

      return Ok(result);
    }

    /// <summary>
    /// Returns an course provided an ID.
    /// </summary>
    /// <param name="id">Course's ID.</param>
    /// <returns>One single Course.</returns>
    /// <response code="400">Course was not found.</response>
    [HttpGet("{id}")]
    public ActionResult<CourseDto> GetCourse(long id)
    {
      var course = Context.Courses.SingleOrDefault(x => x.Id == id);

      if (course == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Course), id);
        return NotFound($"No {nameof(Course)} with Id {id} found in database");
      }

      Logger.LogInformation(LogTemplates.RequestEntity, nameof(Course), id);

      return Ok(Mapper.Map<Course, CourseDto>(course));
    }

    /// <summary>
    /// Adds an course provided the necessary information.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<ActionResult<CourseDto>> AddCourse([FromBody] AddCourseDto dto)
    {
      var course = Mapper.Map<AddCourseDto, Course>(dto);

      Context.Courses.Add(course);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.CreatedEntity, nameof(Course), course);

      return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, Mapper.Map<Course, CourseDto>(course));
    }

    /// <summary>
    /// We delete a user provided an ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("")]
    public async Task<ActionResult> DeleteCourse(int id)
    {
      var course = Context.Courses.SingleOrDefault(x => x.Id == id);

      if (course == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Course), id);
        return NotFound("No course found in the database");
      }

      Context.Courses.Remove(course);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Deleted, nameof(Course), id);

      return NoContent();
    }

    /// <summary>
    /// We update an Course provided all the necessary information. Id is required.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, AddCourseDto dto)
    {
      var course = Context.Courses.SingleOrDefault(x => x.Id == id);

      if (course == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Course), id);
        return NotFound($"No {nameof(Course)} with Id {id} found in database");
      }

      course.Name = dto.Name;
      course.Type = dto.Type;

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Updated, nameof(Course), course);

      return Ok(Mapper.Map<Course, CourseDto>(course));
    }
  }
}

