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
  using CourseAndProfsPersistence.Identity;

  [Route("api/user")]
  [ApiController]
  public class UserController : BaseController<UserController>
  {
    public UserController(ILogger<UserController> logger, CaPDbContext ctx, IPureMapper mapper)
      : base(logger, ctx, mapper)
    {
    }

    /// <summary>
    /// Inserts or Updates an AppsId with the given AppsToken.
    /// </summary>
    /// <param name="id">Professor's ID.</param>
    /// <param name="token">Professor's ID.</param>
    /// <response code="200"></response>
    [HttpPost("")]
    public async Task<ActionResult> AddUsersCredentials(long id, string token)
    {
      var user = await Context.UserAuths.Where(x => x.Appsid == id).SingleOrDefaultAsync();
      UserAuth userAuth = new UserAuth();
      userAuth.Appsid = id;
      userAuth.Token = token;
      if (user == null)
      {
        Context.Add(userAuth);
      }
      else {
        user.Token = token;
        Context.Update(user);
      }
      await Context.SaveChangesAsync();
      return Ok();
    }
  }
}

