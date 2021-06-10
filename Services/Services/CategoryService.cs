using Data;
using Entity;
using Services.IdentityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private ReadLaterDataContext _ReadLaterDataContext;
        private readonly IIdentityService _identityService;

        public CategoryService(ReadLaterDataContext readLaterDataContext, IIdentityService identityService) 
        {
            _ReadLaterDataContext = readLaterDataContext;
            _identityService = identityService;
        }

        public void UpdateCategory(Category category)
        {
            category.UserId = _identityService.GetUserId();

            _ReadLaterDataContext.Update(category);
            _ReadLaterDataContext.SaveChanges();
        }

        public List<Category> GetCategories()
        {
            return _ReadLaterDataContext.Categories.Where(c => c.UserId.Equals(_identityService.GetUserId())).ToList();

        }

        public Category GetCategory(int Id)
        {
            return _ReadLaterDataContext.Categories.Where(c => c.ID == Id && c.UserId.Equals(_identityService.GetUserId())).FirstOrDefault();
        }

        public Category GetCategory(string Name)
        {
            return _ReadLaterDataContext.Categories.Where(c => c.Name == Name && c.UserId.Equals(_identityService.GetUserId())).FirstOrDefault();
        }

        public void DeleteCategory(Category category)
        {
            var categoryInBookmarks = _ReadLaterDataContext.Bookmark.Where(x => x.CategoryId != null && x.CategoryId.Equals(category.ID)).ToList();

            for (int i = 0; i < categoryInBookmarks.Count; i++)
            {
                categoryInBookmarks[i].CategoryId = null;
                categoryInBookmarks[i].Category = null;
            }
                
            _ReadLaterDataContext.Categories.Remove(category);
            _ReadLaterDataContext.SaveChanges();
        }

        public Category CreateCategory(Category category)
        {
            category.UserId = _identityService.GetUserId();

            var existing = _ReadLaterDataContext.Categories
                .FirstOrDefault(x => x.Name.Equals(category.Name) && x.UserId.Equals(category.UserId));
            if (existing != null)
                return existing;

            _ReadLaterDataContext.Add(category);
            _ReadLaterDataContext.SaveChanges();
            return category;
        }
    }
}
