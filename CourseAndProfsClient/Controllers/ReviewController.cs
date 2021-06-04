using CourseAndProfsClient.Helpers;
using CourseAndProfsClientModels;
using CourseAndProfsClientModels.Dto;
using CourseAndProfsPersistence;
using CourseAndProfsPersistence.Identity;
using CourseAndProfsPersistence.Models;
using Kritikos.PureMap.Contracts;
using Kritikos.Extensions.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kritikos.PureMap;

namespace CourseAndProfsClient.Controllers
{
  public class ReviewController : BaseController<ReviewController>
  {
    private readonly UserManager<CaPUser> userManager;
    public ReviewController(ILogger<ReviewController> logger, CaPDbContext ctx, IPureMapper mapper, UserManager<CaPUser> userManager)
  : base(logger, ctx, mapper)
    {
      this.userManager = userManager;
    }

    /// <summary>
    /// Returns all professors with OrderByDescending. You can pass parameters to handle page and result count.
    /// </summary>
    /// <param name="itemsPerPage">Define how many items shall be returned. </param>
    /// <param name="page">Choose which page of the results shall be returned.</param>
    /// <returns>Returns a list of Professors.</returns>
    /// <response code="200">Results.</response>
    /// <response code="404">No rating was found.</response>
    [HttpGet("AllProfessorsReviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetAllProfessorsAvgReviews(int itemsPerPage = 20, int page = 1, CancellationToken token = default)
    {

      var reviewsQuery = Context.Professors.Where(x => x.AverageRating != -1).OrderByDescending(x => x.AverageRating);

      if (reviewsQuery == null)
      {
        return NotFound("No ratings yet");
      }

      var totalReviews = await reviewsQuery.CountAsync(token);
      
      var toSkip = itemsPerPage * (page - 1);

      if (page > ((totalReviews / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }

      var pagedReviews = await reviewsQuery
        .Skip(toSkip)
        .Take(itemsPerPage)
        .ToListAsync();

      var result = new PagedResult<ProfessorDto>
      {
        Results = pagedReviews.Select(x => Mapper.Map<Professor, ProfessorDto>(x)).ToList(),
        Page = page,
        TotalPages = (totalReviews / itemsPerPage) + 1,
        TotalElements = totalReviews,
      };

      return Ok(result);
    }

    /// <summary>
    /// Returns paged reviews for a specific ProfId.
    /// </summary>
    /// <param name="profId">Professor's ID.</param>
    /// <returns>Listed paged reviews.</returns>
    /// <response code="200">Reviews.</response>
    /// <response code="404">No rating was found.</response>
    [HttpGet("ProfessorsReviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetProfessorsReviews(long profId, int itemsPerPage = 20, int page = 1, CancellationToken token = default)
    {

      var reviews = Context.Reviews.Include(x => x.Professor).Where(x => x.Professor.Id == profId).OrderBy(x => x.CreatedAt);
      var totalReviews = await reviews.CountAsync(token);
      var pagedReviews = await reviews.Slice(page, itemsPerPage).Project<Review, ReviewDto>(Mapper).ToListAsync(token);

      if (totalReviews == 0)
      {
        return NotFound("No ratings found.");
      }

      if (page > ((totalReviews / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }
      var toSkip = itemsPerPage * (page - 1);

      var result = new PagedResult<ReviewDto>
      {
        Results = pagedReviews,
        Page = page,
        TotalPages = (totalReviews / itemsPerPage) + 1,
        TotalElements = totalReviews,
      };

      return Ok(result);
    }

    /// <summary>
    /// Returns paged reviews made by an AppsId.
    /// </summary>
    /// <param name="appsId">Professor's ID.</param>
    /// <returns>Listed paged reviews.</returns>
    /// <response code="200">Reviews.</response>
    /// <response code="404">No rating was found.</response>
    [HttpGet("StudentsReviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetStudentsReviews(long appsId, int itemsPerPage = 20, int page = 1, CancellationToken token = default)
    {

      var reviews = Context.Reviews.Include(x => x.Professor).Where(x => x.UserA.Appsid == appsId).OrderBy(x => x.CreatedAt);
      var totalReviews = await reviews.CountAsync(token);
      var pagedReviews = await reviews.Slice(page, itemsPerPage).Project<Review, ReviewDto>(Mapper).ToListAsync(token);

      if (totalReviews == 0)
      {
        return NotFound("No ratings found.");
      }

      if (page > ((totalReviews / itemsPerPage) + 1))
      {
        return BadRequest("Page doesn't exist");
      }
      var toSkip = itemsPerPage * (page - 1);

      var result = new PagedResult<ReviewDto>
      {
        Results = pagedReviews,
        Page = page,
        TotalPages = (totalReviews / itemsPerPage) + 1,
        TotalElements = totalReviews,
      };

      return Ok(result);
    }

    /// <summary>
    /// Adds a review given the specified params.
    /// </summary>
    /// <param name="dto">Professor's ID.</param>
    /// <returns>Listed paged reviews.</returns>
    /// <response code="200">Review was added successufully.</response>
    /// <response code="400">Unauthorized.</response>
    /// <response code="404">Not found profId.</response>
    /// <response code="404">Not found courseId.</response>
    /// <response code="409">User has already reviewed this professor and course.</response>
    [HttpPost("Add")]
    public async Task<ActionResult> AddReview(AddReviewDto dto, CancellationToken token = default)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState.Values.SelectMany(c => c.Errors));
      }

      var userAuth = await Context.UserAuths.Where(x => x.Appsid == dto.AppsId && x.Token.Equals(dto.Token)).SingleOrDefaultAsync(token);
      if (userAuth == null)
      {
        return BadRequest("Unauthorized");
      }

      var course = await Context.Courses.Where(x => x.Id == dto.CourseId ).FirstOrDefaultAsync();

      var professor = await Context.Professors.Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == dto.ProfessorId, token);

      if (professor == null)
      {
        return NotFound($"Could not find professor with id {string.Join(", ", professor.Id)}");
      }

      if (course == null)
      {
        Logger.LogWarning(LogTemplates.NotFound, nameof(Course), course.Id);
        return NotFound($"Could not find course with id {string.Join(", ", course.Id)}");
      }

      var rating = await Context.Reviews.SingleOrDefaultAsync(x => x.UserA.Appsid == dto.AppsId && x.Professor.Id == professor.Id && x.Course.Id == course.Id, token);

      if (rating != null)
      {
        return Conflict("User has already reviewd the professor and course.");
      }

      professor.AverageRating = (professor.Reviews.Sum(x => x.Rating) + dto.Rating) / (professor.Reviews.Count + 1);
      professor.TotalReviews += 1;

      var review = new Review
      {
        Course = course,
        Professor = professor,
        UserA = userAuth,
        UsersSubjectScore = dto.UsersSubjectScore,
        Rating = dto.Rating,
        Comments = dto.Comments,
      };

      Context.Reviews.Add(review);

      await Context.SaveChangesAsync(token);

      return Ok("Review was added successufully");
    }

    /// <summary>
    /// Removes  a review given the reviewId.
    /// </summary>
    /// <param name="reviewId">Professor's ID.</param>
    /// <returns>Listed paged reviews.</returns>
    /// <response code="204">Review was added successufully.</response>
    /// <response code="404">Not found reviewId.</response>
    [HttpDelete("delete")]
    public async Task<ActionResult> RemoveReview(long reviewId, CancellationToken token = default)
    {

      var review =
        await Context.Reviews
          .Include(x => x.Professor.Reviews)
          .SingleOrDefaultAsync(x => x.Id == reviewId); //&& x.User.Id == user.Id, token);
      if (review == null)
      {
        return NotFound("No review found.");
      }

      double result = (review.Professor.Reviews.Where(x => x.Id != review.Id).Sum(x => x.Rating)) / (review.Professor.Reviews.Count - 1);

      if (!double.IsFinite(result))
      {
        review.Professor.AverageRating = 0;

        review.Professor.TotalReviews = 0;
      }
      else
      {
        review.Professor.AverageRating = result;

        review.Professor.TotalReviews -= 1;
      }

      Context.Reviews.Remove(review);

      await Context.SaveChangesAsync(token);
      return NoContent();
    }
  }
}
