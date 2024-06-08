using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PagedList.Core;
using ProjectA.Helper;
using ProjectA.Models;

namespace ProjectA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminPostsController : Controller
    {
        private readonly ProjectAContext _context;
        public INotyfService _notifyfService { get; }

        public AdminPostsController(ProjectAContext context, INotyfService notifyfService)
        {
            _context = context;
            _notifyfService = notifyfService;
        }

        // GET: Admin/AdminPosts
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

        // GET: Admin/AdminPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/AdminPosts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            return View();
        }

        // POST: Admin/AdminPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,IsHot,IsNewfeed,MetaKey,MetaDesc,Views")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    if (!string.IsNullOrEmpty(post.Title))
                    {
                        string imageName = Utilities.SEOUrl(post.Title) + extension;
                        post.Thumb = await Utilities.UploadFile(fThumb, @"posts", imageName.ToLower());
                    }
                }
                if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";
                if (!string.IsNullOrEmpty(post.Title))
                {
                    post.Alias = Utilities.SEOUrl(post.Title);
                }
                post.CreatedDate = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            return View(post);
        }

        // GET: Admin/AdminPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            return View(post);
        }

        // POST: Admin/AdminPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,IsHot,IsNewfeed,MetaKey,MetaDesc,Views")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (fThumb == null)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    _notifyfService.Success("Cập nhật thành công");
                }
                catch
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (fThumb != null)
                        {
                            string extension = Path.GetExtension(fThumb.FileName);
                            if (!string.IsNullOrEmpty(post.Title))
                            {
                                string imageName = Utilities.SEOUrl(post.Title) + extension;
                                post.Thumb = await Utilities.UploadFile(fThumb, @"posts", imageName.ToLower());
                            }
                        }
                        if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";
                        if (!string.IsNullOrEmpty(post.Title))
                        {
                            post.Alias = Utilities.SEOUrl(post.Title);
                        }

                        _context.Update(post);
                        await _context.SaveChangesAsync();
                        _notifyfService.Success("Cập nhật thành công");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PostExists(post.PostId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            return View(post);
        }

        // GET: Admin/AdminPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/AdminPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            _notifyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
