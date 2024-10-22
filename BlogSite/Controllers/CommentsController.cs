using BlogSite.Data;
using BlogSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogSite.Controllers;
public class CommentsController : Controller
{
    private readonly BlogContext _context;

    public CommentsController(BlogContext context)
    {
        _context = context;
    }

    // POST: Comments/AddComment
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(string text, int blogPostId)
    {
        var comment = new Comment
        {
            Text = text,
            CreatedAt = DateTime.Now,
            CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
            BlogPostId = blogPostId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Blog", new { id = blogPostId });
    }

    // GET: Comments/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null || comment.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid(); // or NotFound()
        }

        return View(comment);
    }

    // POST: Comments/Edit/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Comment comment)
    {
        var commentInDb = await _context.Comments.AsNoTracking().FirstAsync(x => x.Id == id);
        comment.CreatedBy = commentInDb.CreatedBy;
        comment.BlogPostId = commentInDb.BlogPostId;
        // Remove validation errors related to CreatedBy and BlogPostId, if any
        ModelState["CreatedBy"].Errors.Clear();
        ModelState["CreatedBy"].ValidationState = ModelValidationState.Valid;
        ModelState["BlogPost"].Errors.Clear();
        ModelState["BlogPost"].ValidationState = ModelValidationState.Valid;
        if (id != comment.Id || comment.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid(); // or NotFound()
        }
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(comment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
        }

        return View(comment);
    }

    // GET: Comments/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null || comment.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid(); // or NotFound()
        }

        return View(comment);
    }

    // POST: Comments/Delete/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null || comment.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return Forbid(); // or NotFound()
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
    }

    private bool CommentExists(int id)
    {
        return _context.Comments.Any(e => e.Id == id);
    }
}
