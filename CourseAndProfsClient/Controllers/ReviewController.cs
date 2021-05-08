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

    [HttpGet("AllProfessorsReviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetAllProfessorsAvgReviews(int itemsPerPage = 20, int page = 1, CancellationToken token = default)
    {
      var userId = RetrieveUserId().ToString();
      //var user = await userManager.FindByIdAsync(userId);

      //if (user == null)
      //{
      //  return BadRequest("Something went wrong.");
      //}

      var reviewsQuery = Context.Professors.Where(x => x.AverageRating != -1).OrderBy(x => x.AverageRating);

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

    [HttpGet("ProfessorsReviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetProfessorsReviews(long profId, int itemsPerPage = 20, int page = 1, CancellationToken token = default)
    {
      var userid = RetrieveUserId().ToString();
      //var user = await userManager.FindByIdAsync(userId);

      //if (user == null)
      //{
      //  return BadRequest("Something went wrong.");
      //}

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


    [HttpPost("Add")]
    public async Task<ActionResult> AddReview(AddReviewDto dto, CancellationToken token = default)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState.Values.SelectMany(c => c.Errors));
      }
      var userId = RetrieveUserId().ToString();
      //if (userId == "00000000-0000-0000-0000-000000000000")
      //{
      //  return BadRequest("Something went wrong.");
      //}

      var user = await userManager.FindByIdAsync(userId);
      //if (user == null)
      //{
      //  return BadRequest("Something went wrong.");
      //}

      var course = await Context.Courses.Where(x => x.Id == dto.CourseId ).FirstOrDefaultAsync();
      //var professor = Context.Professors.Where(x => x.Id == dto.ProfessorId ).SingleOrDefault();
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

      //var rating = await Context.Reviews.SingleOrDefaultAsync(x => x.User == user && x.Professor == professor && x.Course == course, token);

      //if (rating != null)
      //{
      //  return Conflict("User has already rated the movie.");
      //}

      professor.AverageRating = (professor.Reviews.Sum(x => x.Rating) + dto.Rating) / (professor.Reviews.Count + 1);
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

      await Context.SaveChangesAsync(token);

      return Ok("Review was added successufully");
    }
  }
}
