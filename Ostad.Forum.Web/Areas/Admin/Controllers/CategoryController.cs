using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ostad.Forum.BLL.Interfaces;
using Ostad.Forum.Contract;
using Ostad.Forum.Web.Areas.Admin.Models;

namespace Ostad.Forum.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController(ICategoryService categoryService) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateCategoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _categoryService.CreateAsync(new CreateCategoryDto
        {
            Name = model.Name,
            Slug = model.Slug,
            Description = model.Description
        });
        return RedirectToAction(nameof(Index));
    }
}
