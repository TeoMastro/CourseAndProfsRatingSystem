using CourseAndProfsClient.Helpers;
using CourseAndProfsClientModels;
using CourseAndProfsClientModels.Dto;
using CourseAndProfsPersistence;
using CourseAndProfsPersistence.Models;
using Kritikos.PureMap.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseAndProfsClient.Controllers
{
  public class ReviewController : BaseController<ReviewController>
  {
    public ReviewController(ILogger<ReviewController> logger, CaPDbContext ctx, IPureMapper mapper)
  : base(logger, ctx, mapper)
    {
    }

    [HttpGet("All")]
    public async Task<ActionResult<List<ReviewDto>>> GetReviews(int itemsPerPage = 20, int page = 1)
    {
      var toSkip = itemsPerPage * (page - 1);

      var reviewsQuery = Context.Reviews
        .TagWith("Retrieving all reviews")
        .OrderBy(x => x.Id);

      var totalReviews = await reviewsQuery.CountAsync();

      if (page > ((totalReviews / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }

      var pagedReviews = await reviewsQuery
        .Skip(toSkip)
        .Take(itemsPerPage)
        .ToListAsync();

      var result = new PagedResult<ReviewDto>
      {
        Results = pagedReviews.Select(x => Mapper.Map<Review, ReviewDto>(x)).ToList(),
        Page = page,
        TotalPages = (totalReviews / itemsPerPage) + 1,
        TotalElements = totalReviews,
      };

      return Ok(result);
    }


    [HttpPost("Add")]
    public async Task<ActionResult> AddReview([FromBody] ReviewDto dto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState.Values.SelectMany(c => c.Errors));
      }

      var course = Context.Courses.Where(x => x.Id == dto.CourseId ).SingleOrDefault();
      var professor = Context.Professors.Where(x => x.Id == dto.ProfessorId ).SingleOrDefault();
      var user = Context.Users.Where(x => x.Id == dto.UserId).SingleOrDefault();

      if (course.Equals(null))
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Course), course.Id);
        return NotFound($"Could not find course with id {string.Join(", ", course.Id)}");
      }
      if (professor.Equals(null))
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Professor), professor.Id);
        return NotFound($"Could not find professor with id {string.Join(", ", professor.Id)}");
      }

      var review = new Review
      {
        Course = course,
        Professor = professor,
        User = user,
        UsersSubjectScore = dto.UsersSubjectScore,
        Rating = dto.Rating,
        Comments = dto.Comments,
      };

      Context.Reviews.Add(review);

      await Context.SaveChangesAsync();

      return Ok();
    }
  }
}
