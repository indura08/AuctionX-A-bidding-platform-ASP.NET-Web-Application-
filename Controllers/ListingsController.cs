﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionX.Data;
using AuctionX.Models;
using AuctionX.Data.Service;
using System.Security.Claims;

namespace AuctionX.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IListings _listingService;
        private readonly IBidsService _bidService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommentsService _commentsService;

        public ListingsController(ICommentsService commentsService , IListings listingService, IWebHostEnvironment webHostEnvironment, IBidsService ibidsService)  
        {
            _listingService = listingService;
            _webHostEnvironment = webHostEnvironment;
            _bidService = ibidsService;
            _commentsService = commentsService;
        }

        // GET: Listings
        public async Task<IActionResult> Index(int? pageNumber, string searchString)      //int? pageNumber: An optional integer for the page number. The ? means it can be null.
        {
            var applicationDbContext = _listingService.Getall();

            int pageSize = 3;
            if (!string.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(a => a.Titile.Contains(searchString));
                return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.IsSold == false).AsNoTracking(), pageNumber ?? 1, pageSize));        //.AsNoTracking(): Improves performance by telling Entity Framework not to track changes for these entities.

            }
            return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.IsSold == false).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyListings(int? pageNumber)
        {
            var applicationDbContext = _listingService.Getall();

            int pageSize = 3;
            
            return View("Index", await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).AsNoTracking(), pageNumber ?? 1, pageSize));      //methandi me complex process ekk wage krla thiynne log wela inna usermada me api gattu listings list eke aithi user kiyla check krla blna eka, ehma nm withri listings tiks denne 
        }

        public async Task<IActionResult> MyBids(int? pageNumber)
        {
            var applicationDbContext = _bidService.GetAll();

            int pageSize = 3;

            return View(await PaginatedList<Bid>.CreateAsync(applicationDbContext.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).AsNoTracking(), pageNumber ?? 1, pageSize));      //methandi me complex process ekk wage krla thiynne log wela inna usermada me api gattu listings list eke aithi user kiyla check krla blna eka, ehma nm withri listings tiks denne 
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _listingService.GetById(id);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // GET: Listings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingVM listing)
        {
            if (listing.Image != null) {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string fileName = listing.Image.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create)) { 
                    
                    listing.Image.CopyTo(fileStream);
                
                }
                var listObj = new Listing
                {
                    Titile = listing.Titile,
                    Description = listing.Description,
                    Price = listing.Price,
                    IdentityUserId = listing.IdentityUserId,
                    ImagePath = fileName,
                };

                await _listingService.Add(listObj);
                return RedirectToAction("Index");
            }
            return View(listing);
        }

        [HttpPost]
        public async Task<ActionResult> AddBid([Bind("Id , Price , ListingId, IdentityUserId")] Bid bid)
        {
            //if (ModelState.IsValid)
            //{
            //    await _bidService.Add(bid);
            //}  //me if statement ekn error ekk awa eka podk blnna 

            await _bidService.Add(bid);

            var listing = await _listingService.GetById(bid.ListingId);

            listing.Price = bid.Price;
            await _listingService.SaveChnages();

            return View("Details", listing);

        }

        public async Task<ActionResult> CloseBidding(int id)
        {
            var listing = await _listingService.GetById(id);

            listing.IsSold = true;
            await _listingService.SaveChnages();

            return View("Details" , listing);
        }

        [HttpPost]
        public async Task<ActionResult> AddComment([Bind("Id,Content,ListingId,IdentityUserId")] Comment comment)
        {
            //if (ModelState.IsValid)
            //{
            //    await _commentsService.Add(comment);
            //}

            await _commentsService.Add(comment);

            var lsiting = await _listingService.GetById(comment.ListingId);
            return View("Details", lsiting);

        }


        //// GET: Listings/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var listing = await _context.Listings.FindAsync(id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", listing.IdentityUserId);
        //    return View(listing);
        //}

        //// POST: Listings/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Titile,Description,Price,ImagePath,IsSold,IdentityUserId")] Listing listing)
        //{
        //    if (id != listing.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(listing);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ListingExists(listing.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", listing.IdentityUserId);
        //    return View(listing);
        //}

        //// GET: Listings/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var listing = await _context.Listings
        //        .Include(l => l.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(listing);
        //}

        //// POST: Listings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var listing = await _context.Listings.FindAsync(id);
        //    if (listing != null)
        //    {
        //        _context.Listings.Remove(listing);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ListingExists(int id)
        //{
        //    return _context.Listings.Any(e => e.Id == id);
        //}
    }
}
