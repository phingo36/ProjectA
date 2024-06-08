using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ProjectA.Helper;
using ProjectA.Models;

namespace ProjectA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly ProjectAContext _context;
        public INotyfService _notifyfService { get; }

        public AdminCategoriesController(ProjectAContext context, INotyfService notifyfService)
        {
            _context = context;
            _notifyfService = notifyfService;
        }

        // GET: Admin/AdminCategories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var lsCategorys = _context.Categories
                .AsNoTracking()
                .OrderByDescending(x => x.CatId);

            PagedList<Category> models = new PagedList<Category>(lsCategorys, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Description,ParentId,Levels,Ordering,Published,Thumbnail,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    if (!string.IsNullOrEmpty(category.CatName))
                    {
                        string imageName = Utilities.SEOUrl(category.CatName) + extension;
                        category.Thumbnail = await Utilities.UploadFile(fThumb, @"categories", imageName.ToLower());
                    }
                }
                if (string.IsNullOrEmpty(category.Thumbnail)) category.Thumbnail = "default.jpg";
                if (!string.IsNullOrEmpty(category.CatName))
                {
                    category.Alias = Utilities.SEOUrl(category.CatName);
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                _notifyfService.Success("Thêm danh mục thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Description,ParentId,Levels,Ordering,Published,Thumbnail,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (fThumb == null)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    _notifyfService.Success("Cập nhật thành công");
                }
                catch
                {
                    if (!CategoryExists(category.CatId))
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
                            if (!string.IsNullOrEmpty(category.CatName))
                            {
                                string imageName = Utilities.SEOUrl(category.CatName) + extension;
                                category.Thumbnail = await Utilities.UploadFile(fThumb, @"categories", imageName.ToLower());
                            }
                        }
                        if (string.IsNullOrEmpty(category.Thumbnail)) category.Thumbnail = "default.jpg";
                        if (!string.IsNullOrEmpty(category.CatName))
                        {
                            category.Alias = Utilities.SEOUrl(category.CatName);
                        }

                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        _notifyfService.Success("Cập nhật thành công");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.CatId))
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

            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            _notifyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
