using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadLater5.Models
{
    public class BookmarkViewModel
    {
        public string ID { get; set; }

        public string URL { get; set; }

        public string ShortDescription { get; set; }

        public string UserId { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
