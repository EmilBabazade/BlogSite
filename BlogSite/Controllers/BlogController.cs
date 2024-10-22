using BlogSite.Data;
using BlogSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogSite.Controllers;
public class BlogController : Controller
{
    private readonly BlogContext _dbContext;

    public BlogController(BlogContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IActionResult Index()
    {
        var posts = _dbContext.BlogPosts.ToList();
        return View(posts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(BlogPost blogPost)
    {
        if (ModelState.IsValid)
        {
            _dbContext.BlogPosts.Add(blogPost);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(blogPost);
    }
}
