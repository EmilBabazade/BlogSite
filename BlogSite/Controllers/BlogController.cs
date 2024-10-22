using BlogSite.Data;
using BlogSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            .Include(b => b.Comments)
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
    public async Task<IActionResult> Create([Bind("Id,Title,Content")] BlogPost blogPost)
    {
        if (ModelState.ContainsKey("CreatedBy"))
        {
            // Remove validation errors related to CreatedBy, if any
            ModelState["CreatedBy"].Errors.Clear();
            ModelState["CreatedBy"].ValidationState = ModelValidationState.Valid;
        }
        if (ModelState.IsValid)
        {
            // Set the CreatedAt and CreatedBy properties
            blogPost.CreatedAt = DateTime.Now;
            blogPost.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

            _context.Add(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(blogPost);
    }

    // POST: BlogPosts/Edit/5
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

        // Check if the current user is the creator of the post
        if (blogPost.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid(); // Return 403 Forbidden if the user is not the creator
        }

        return View(blogPost);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt")] BlogPost blogPost)
    {
        if (id != blogPost.Id)
        {
            return NotFound();
        }
        var postCreatedBy = (await _context.BlogPosts.AsNoTracking().FirstAsync(x => x.Id == id)).CreatedBy;
        // Check if the current user is the creator of the post
        if (postCreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid();
        }
        blogPost.CreatedBy = postCreatedBy;

        if (ModelState.ContainsKey("CreatedBy"))
        {
            // Remove validation errors related to CreatedBy, if any
            ModelState["CreatedBy"].Errors.Clear();
            ModelState["CreatedBy"].ValidationState = ModelValidationState.Valid;
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

        // Check if the current user is the creator of the post
        if (blogPost.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid();
        }

        return View(blogPost);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);

        // Check if the current user is the creator of the post
        if (blogPost.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid();
        }

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    private bool BlogPostExists(int id)
    {
        return _context.BlogPosts.Any(e => e.Id == id);
    }
}
