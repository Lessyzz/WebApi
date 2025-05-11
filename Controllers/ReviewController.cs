using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[Controller]
public class ReviewController(IReviewService reviewService)
    : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost("Review/AddReview")]
    [Authorize]
    public async Task<IActionResult> AddReview(string productId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
        {
            ModelState.AddModelError("Rating", "Rating must be between 1 and 5");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            ModelState.AddModelError("Comment", "Comment is required");
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Product", new { id = productId });
        }
        
        var userId = User.Claims.First(c => c.Type == "sid").Value;
        
        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            Comment = comment,
            ReviewDate = DateTime.Now,
            // You may want to auto-approve reviews or require admin approval
            IsApproved = false
        };

        var result = await reviewService.AddReviewAsync(review);
        if (result)
        {
            TempData["SuccessMessage"] = "Your review has been submitted and is pending approval.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to submit your review. Please try again.";
        }

        return RedirectToAction("Product", "", new { id = productId });
    }

    [HttpPost("Review/DeleteReview")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int reviewId, int productId)
    {
        var result = await reviewService.DeleteReviewAsync(reviewId);
        if (result)
        {
            TempData["SuccessMessage"] = "Review deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete the review. Please try again.";
        }

        return RedirectToAction("Details", "Product", new { id = productId });
    }

    [HttpPost("Review/ApproveReview")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveReview(int reviewId, int productId)
    {
        var result = await reviewService.ApproveReviewAsync(reviewId);
        if (result)
        {
            TempData["SuccessMessage"] = "Review approved successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to approve the review. Please try again.";
        }

        return RedirectToAction("Details", "Product", new { id = productId });
    }
}