using Data;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.IdentityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookmarkService : IBookmarkService
    {
        private ReadLaterDataContext _ReadLaterDataContext;
        private readonly IIdentityService _identityService;
        private readonly ICategoryService _categoryService;

        public BookmarkService(ReadLaterDataContext readLaterDataContext, IIdentityService identityService, ICategoryService categoryService)
        {
            _ReadLaterDataContext = readLaterDataContext;
            _identityService = identityService;
            _categoryService = categoryService;
        }

        public async Task<Bookmark> CreateBookmark(Bookmark bookmark)
        {
            bookmark.CreateDate = DateTime.Now;
            bookmark.UserId = _identityService.GetUserId();

            await _ReadLaterDataContext.AddAsync(bookmark);
            await _ReadLaterDataContext.SaveChangesAsync();

            return bookmark;
        }

        public async Task UpdateBookmark(Bookmark bookmark)
        {
            bookmark.CreateDate = DateTime.Now;

            UpdateExistingCategory(bookmark);
            _ReadLaterDataContext.Update(bookmark);
            await _ReadLaterDataContext.SaveChangesAsync();
        }

        public async Task<List<Bookmark>> GetBookmarks()
        {
            return await _ReadLaterDataContext.Bookmark.Include(x => x.Category).Where(x => x.UserId.Equals(_identityService.GetUserId())).ToListAsync();
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _ReadLaterDataContext.Categories.ToListAsync();
        }

        public async Task<Bookmark> GetBookmark(int Id)
        {
            return await _ReadLaterDataContext.Bookmark.Where(x => x.ID == Id && x.UserId.Equals(_identityService.GetUserId())).Include(x => x.Category).FirstOrDefaultAsync();
        }

        public async Task<Bookmark> GetBookmark(string Url)
        {
            return await _ReadLaterDataContext.Bookmark.Where(x => x.URL == Url && x.UserId.Equals(_identityService.GetUserId())).Include(x => x.Category).FirstOrDefaultAsync();
        }
        public async Task<bool> BookmarkExists(int id)
        {
            var exists = await _ReadLaterDataContext.Bookmark.AnyAsync(x => x.ID == id);

            return exists ? true : false;
        }

        public async Task DeleteBookmark(Bookmark bookmark)
        {
            _ReadLaterDataContext.Bookmark.Remove(bookmark);
            await _ReadLaterDataContext.SaveChangesAsync();
        }

        protected void UpdateExistingCategory(Bookmark bookmark)
        {
            if (bookmark.Category.Name != null)
            {
                var existing = _categoryService.GetCategory(bookmark.Category.Name);

                if (existing != null)
                    bookmark.Category = existing;
                else
                    _categoryService.CreateCategory(
                       new Category()
                       {
                           Name = bookmark.Category.Name
                       });
            }
            else
                bookmark.Category = null;
        }
    }
}
