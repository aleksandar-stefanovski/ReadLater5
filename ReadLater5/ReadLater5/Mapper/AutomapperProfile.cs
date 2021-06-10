using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
        using AutoMapper;
using Entity;
using ReadLater5.Models;

namespace ReadLater5.Mapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Bookmark, BookmarkViewModel>().ReverseMap();
        }
    }
}