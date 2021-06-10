using AutoMapper;
using Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadLater5.Models;
using Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace ReadLater5.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/bookmarks")]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public BookmarksController(IBookmarkService bookmarkService, IMapper mapper, ICategoryService categoryService, UserManager<IdentityUser> userManager)
        {
            _bookmarkService = bookmarkService;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> QueryBookmarks()
        {
            var bookmarks = await _bookmarkService.GetBookmarks();
            return Ok(bookmarks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> QueryBookmark([FromRoute] int? id)
        {
            if (id == null)
                return BadRequest();

            var bookmark = await _bookmarkService.GetBookmark((int)id);

            if (bookmark == null)
                NotFound();  

            return Ok(bookmark);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookmark([FromBody] Bookmark request)
        {
            if (ModelState.IsValid)
            {
                if (request.Category.Name != null)
                {
                    var category = _categoryService.CreateCategory(new Category
                    {
                        Name = request.Category.Name
                    });

                    request.Category = category;
                    await _bookmarkService.CreateBookmark(request);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Created(new Uri($"{Request.Path}/{request.ID}", UriKind.Relative), request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookmark([FromRoute] int? id, [FromBody]Bookmark request)
        {
            var exists = await _bookmarkService.BookmarkExists((int)id);
            if (!exists)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            if (ModelState.IsValid)
            {
                try
                {
                    if (request.Category.Name != null)
                    {
                        var category = _categoryService.CreateCategory(new Category
                        {
                            Name = request.Category.Name
                        });

                        request.Category = category;
                        await _bookmarkService.UpdateBookmark(request);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookmarkService.BookmarkExists(request.ID))
                        return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

                    else
                        throw;
                }
            }

            return Created(new Uri($"{Request.Path}", UriKind.Relative), _mapper.Map<BookmarkViewModel>(request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark([FromRoute] int? id)
        {
            if (id == null)
                return BadRequest();

            var bookmark = await _bookmarkService.GetBookmark((int)id);

            if (bookmark == null)
                NotFound();

            await _bookmarkService.DeleteBookmark(bookmark);

            return NoContent();
        }
    }
}
