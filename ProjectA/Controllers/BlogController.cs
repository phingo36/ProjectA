using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ProjectA.Models;

namespace ProjectA.Controllers
{
    public class BlogController : Controller
    {
        private readonly ProjectAContext _context;
        public BlogController(ProjectAContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsPosts = _context.Posts
                .AsNoTracking()
                .OrderByDescending(x => x.PostId);

            PagedList<Post> models = new PagedList<Post>(lsPosts, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        public IActionResult Details(int id)
        {
            var tins = _context.Posts.AsNoTracking().SingleOrDefault(x=>x.PostId == id);
            if (tins == null)
            {
                return RedirectToAction("Index");
            }
            return View(tins);
        }
    }
}
