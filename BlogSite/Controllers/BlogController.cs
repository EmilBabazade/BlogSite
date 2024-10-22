using BlogSite.Data;
using BlogSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogSite.Controllers;

[Authorize]
public class BlogController : Controller
{
    private readonly BlogContext _context;

    public BlogController(BlogContext context)
    {
        _context = context;
    }

    // GET: BlogPosts
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await _context.BlogPosts.ToListAsync());
    }

    // GET: BlogPosts/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var blogPost = await _context.BlogPosts
            .FirstOrDefaultAsync(m => m.Id == id);
        if (blogPost == null)
        {
            return NotFound();
        }

        return View(blogPost);
    }

    // GET: BlogPosts/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: BlogPosts/Create
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt")] BlogPost blogPost)
    {
        if (ModelState.IsValid)
        {
            blogPost.CreatedAt = DateTime.Now; // Set created date
            _context.Add(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(blogPost);
    }

    // GET: BlogPosts/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return NotFound();
        }
        return View(blogPost);
    }

    // POST: BlogPosts/Edit/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt")] BlogPost blogPost)
    {
        if (id != blogPost.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(blogPost.Id))
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
        return View(blogPost);
    }

    // GET: BlogPosts/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var blogPost = await _context.BlogPosts
            .FirstOrDefaultAsync(m => m.Id == id);
        if (blogPost == null)
        {
            return NotFound();
        }

        return View(blogPost);
    }

    // POST: BlogPosts/Delete/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BlogPostExists(int id)
    {
        return _context.BlogPosts.Any(e => e.Id == id);
    }
}
