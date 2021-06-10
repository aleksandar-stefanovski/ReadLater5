using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Entity;
using Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ReadLater5.Models;
using AutoMapper;

namespace ReadLater5.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public BookmarksController(IBookmarkService bookmarkService, IMapper mapper, ICategoryService categoryService)
        {
            _bookmarkService = bookmarkService;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        // GET: Bookmarks
        public async Task<IActionResult> Index()
        {
            List<Bookmark> model = await _bookmarkService.GetBookmarks();
            return View(model);
        }

        // GET: Bookmarks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);

            var bookmark = await _bookmarkService.GetBookmark((int)id);

            if (bookmark == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            return View(bookmark);
        }

        // GET: Bookmarks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bookmarks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookmarkViewModel request)
        {
            var bookmark = _mapper.Map<Bookmark>(request);

            if (ModelState.IsValid)
            {
                if (request.CategoryName != null)
                {
                    var category = _categoryService.CreateCategory(new Category
                    {
                        Name = request.CategoryName
                    });

                    bookmark.Category = category;
                    await _bookmarkService.CreateBookmark(bookmark);
                }

                return RedirectToAction("Index");
            }

            return View(bookmark);
        }

        // GET: Bookmarks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);

            var request = await _bookmarkService.GetBookmark((int)id);
            var bookmark = _mapper.Map<BookmarkViewModel>(request);

            if (bookmark == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            return View(bookmark);
        }

        // POST: Bookmarks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookmarkViewModel request)
        {
            var bookmark = _mapper.Map<Bookmark>(request);

            var exists = await _bookmarkService.BookmarkExists(bookmark.ID);
            if(!exists)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            if (ModelState.IsValid)
            {
                if (request.CategoryName != null)
                {
                     var category = _categoryService.CreateCategory(new Category
                    {
                        Name = request.CategoryName
                    });

                    bookmark.Category = category;
                    bookmark.CategoryId = category.ID;
                }
                await _bookmarkService.UpdateBookmark(bookmark);

                return RedirectToAction("Index");
            }

            return View(bookmark);
        }

        // GET: Bookmarks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            var bookmark = await _bookmarkService.GetBookmark((int)id);

            if (bookmark == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            return View(bookmark);
        }

        // POST: Bookmarks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);

            var bookmark = await _bookmarkService.GetBookmark((int)id);

            if (bookmark == null)
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);

            await _bookmarkService.DeleteBookmark(bookmark);

            return RedirectToAction("Index");
        }
    }
}
