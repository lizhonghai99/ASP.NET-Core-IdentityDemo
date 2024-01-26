using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Data;
using AuthCenter.Entities;
using Microsoft.AspNetCore.Authorization;
using AuthCenter.Models.Dtos.Company;
using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AuthCenter.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin,User")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Company-List")]
        public async Task<IActionResult> Index()
        {
            var query = from company in _context.Company
                        join createdByUser in _context.Users on company.CreatedBy equals createdByUser.Id
                        join updatedByUser in _context.Users on company.UpdatedBy equals updatedByUser.Id into updatedUsers
                        from updatedUser in updatedUsers.DefaultIfEmpty()
                        select new CompanyViewModel
                        {
                            Id = company.Id,
                            Name = company.Name,
                            CreatedOn = company.CreatedOn,
                            CreatedBy = createdByUser.Email,
                            UpdatedOn = company.UpdatedOn,
                            UpdatedBy = updatedUser != null ? updatedUser.Email : null,
                        };

            var result = await query.ToListAsync();

            return View(result);
        }

        [Authorize(Policy = "Company-List")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = from company in _context.Company
                        join createdByUser in _context.Users on company.CreatedBy equals createdByUser.Id
                        join updatedByUser in _context.Users on company.UpdatedBy equals updatedByUser.Id into updatedUsers
                        from updatedUser in updatedUsers.DefaultIfEmpty()
                        where company.Id == id
                        select new CompanyViewModel
                        {
                            Id = company.Id,
                            Name = company.Name,
                            CreatedOn = company.CreatedOn,
                            CreatedBy = createdByUser.Email,
                            UpdatedOn = company.UpdatedOn,
                            UpdatedBy = updatedUser != null ? updatedUser.Email : null,
                        };
            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        [Authorize(Policy = "Company-Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "Company-Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] CreateCompanyViewModel input)
        {
            if (ModelState.IsValid)
            {
                var company = new Company();
                company.Name = input.Name;
                company.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                company.CreatedOn = DateTime.Now;
                _context.Company.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); ;
            }
            return View(input);
        }

        [Authorize(Policy = "Company-Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            var dto = new EditCompanyViewModel();
            dto.Id = id;
            dto.Name = company.Name;
            return View(dto);
        }

        [Authorize(Policy = "Company-Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] EditCompanyViewModel input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var comopany = await _context.Company.FindAsync(input.Id);
                    comopany.Name = input.Name;
                    comopany.UpdatedOn = DateTime.Now;
                    comopany.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.Company.Update(comopany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(input.Id))
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
            return View(input);
        }

        [Authorize(Policy = "Company-Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = from company in _context.Company
                        join createdByUser in _context.Users on company.CreatedBy equals createdByUser.Id
                        join updatedByUser in _context.Users on company.UpdatedBy equals updatedByUser.Id into updatedUsers
                        from updatedUser in updatedUsers.DefaultIfEmpty()
                        where company.Id == id
                        select new CompanyViewModel
                        {
                            Id = company.Id,
                            Name = company.Name,
                            CreatedOn = company.CreatedOn,
                            CreatedBy = createdByUser.Email,
                            UpdatedOn = company.UpdatedOn,
                            UpdatedBy = updatedUser != null ? updatedUser.Email : null,
                        };
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        [Authorize(Policy = "Company-Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                _context.Company.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(string id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
