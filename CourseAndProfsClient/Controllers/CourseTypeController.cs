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

  [Route("api/courseType")]
  [ApiController]
  public class CourseTypeController : BaseController<CourseTypeController>
  {
    public CourseTypeController(ILogger<CourseTypeController> logger, CaPDbContext ctx, IPureMapper mapper)
      : base(logger, ctx, mapper)
    {
    }

    /// <summary>
    /// Returns all courseTypes. You can pass parameters to handle page and result count.
    /// </summary>
    /// <param name="itemsPerPage">Define how many items shall be returned. </param>
    /// <param name="page">Choose which page of the results shall be returned.</param>
    /// <returns>Returns a list of CourseTypes.</returns>
    [HttpGet("")]
    public async Task<ActionResult<List<CourseTypeDto>>> GetCourseTypes(int itemsPerPage = 20, int page = 1)
    {
      var toSkip = itemsPerPage * (page - 1);

      var courseTypesQuery = Context.CourseTypes
        .TagWith("Retrieving all courseTypes")
        .OrderBy(x => x.Id);

      var totalCourseTypes = await courseTypesQuery.CountAsync();

      if (page > ((totalCourseTypes / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }

      var pagedCourseTypes = await courseTypesQuery
        .Skip(toSkip)
        .Take(itemsPerPage)
        .ToListAsync();

      var result = new PagedResult<CourseTypeDto>
      {
        Results = pagedCourseTypes.Select(x => Mapper.Map<CourseType, CourseTypeDto>(x)).ToList(),
        Page = page,
        TotalPages = (totalCourseTypes / itemsPerPage) + 1,
        TotalElements = totalCourseTypes,
      };

      return Ok(result);
    }

    /// <summary>
    /// Returns an courseType provided an ID.
    /// </summary>
    /// <param name="id">CourseType's ID.</param>
    /// <returns>One single CourseType.</returns>
    /// <response code="400">CourseType was not found.</response>
    [HttpGet("{id}")]
    public ActionResult<CourseTypeDto> GetCourseType(long id)
    {
      var courseType = Context.CourseTypes.SingleOrDefault(x => x.Id == id);

      if (courseType == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(CourseType), id);
        return NotFound($"No {nameof(CourseType)} with Id {id} found in database");
      }

      Logger.LogInformation(LogTemplates.RequestEntity, nameof(CourseType), id);

      return Ok(Mapper.Map<CourseType, CourseTypeDto>(courseType));
    }

    /// <summary>
    /// Adds an courseType provided the necessary information.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<ActionResult<CourseTypeDto>> AddCourseType([FromBody] AddCourseTypeDto dto)
    {
      var courseType = Mapper.Map<AddCourseTypeDto, CourseType>(dto);

      Context.CourseTypes.Add(courseType);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.CreatedEntity, nameof(CourseType), courseType);

      return CreatedAtAction(nameof(GetCourseType), new { id = courseType.Id }, Mapper.Map<CourseType, CourseTypeDto>(courseType));
    }

    /// <summary>
    /// We delete a user provided an ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("")]
    public async Task<ActionResult> DeleteCourseType(int id)
    {
      var courseType = Context.CourseTypes.SingleOrDefault(x => x.Id == id);

      if (courseType == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(CourseType), id);
        return NotFound("No courseType found in the database");
      }

      Context.CourseTypes.Remove(courseType);

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Deleted, nameof(CourseType), id);

      return NoContent();
    }

    /// <summary>
    /// We update an CourseType provided all the necessary information. Id is required.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<CourseTypeDto>> UpdateCourseType(int id, AddCourseTypeDto dto)
    {
      var courseType = Context.CourseTypes.SingleOrDefault(x => x.Id == id);

      if (courseType == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(CourseType), id);
        return NotFound($"No {nameof(CourseType)} with Id {id} found in database");
      }

      courseType.Name = dto.Name;

      await Context.SaveChangesAsync();
      Logger.LogInformation(LogTemplates.Updated, nameof(CourseType), courseType);

      return Ok(Mapper.Map<CourseType, CourseTypeDto>(courseType));
    }
  }
}

