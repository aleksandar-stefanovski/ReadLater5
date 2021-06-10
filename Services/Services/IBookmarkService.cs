using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBookmarkService
    {
        Task<Bookmark> CreateBookmark(Bookmark category);
        Task<List<Bookmark>> GetBookmarks();
        Task<List<Category>> GetCategories();
        Task<Bookmark> GetBookmark(int Id);
        Task<Bookmark> GetBookmark(string Name);
        Task UpdateBookmark(Bookmark category);
        Task<bool> BookmarkExists(int id);
        Task DeleteBookmark(Bookmark category);
    }
}
