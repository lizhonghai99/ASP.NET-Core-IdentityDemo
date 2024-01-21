using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Data;
using AuthCenter.Entities;
using AuthCenter.Models.Dtos.AccessToken;
using System.Security.Claims;
using AuthCenter.Services;
using Microsoft.AspNetCore.Authorization;

namespace AuthCenter.Controllers
{
    [Authorize]
    public class AccessTokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AccessTokenController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: AccessTokens
        public async Task<IActionResult> Index()
        {
            var query = from accessToken in _context.AccessToken.Include(t => t.Role)
                        join createdByUser in _context.Users on accessToken.CreatedBy equals createdByUser.Id
                        join updatedByUser in _context.Users on accessToken.UpdatedBy equals updatedByUser.Id into updatedUsers
                        from updatedUser in updatedUsers.DefaultIfEmpty()
                        select new AccessTokenViewModel
                        {
                            Id = accessToken.Id,
                            ApplicationName = accessToken.ApplicationName,
                            TokenName = accessToken.TokenName,
                            InActive = accessToken.InActive,
                            Role = accessToken.Role.Name,
                            CreatedOn = accessToken.CreatedOn,
                            CreatedBy = createdByUser.Email,
                            UpdatedOn = accessToken.UpdatedOn,
                            UpdatedBy = updatedUser != null ? updatedUser.Email : null
                        };
            var results = await query.ToListAsync();
            return View(results);
        }

        // GET: AccessTokens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = from accessToken in _context.AccessToken.Include(t => t.Role)
                        join createdByUser in _context.Users on accessToken.CreatedBy equals createdByUser.Id
                        join updatedByUser in _context.Users on accessToken.UpdatedBy equals updatedByUser.Id into updatedUsers
                        from updatedUser in updatedUsers.DefaultIfEmpty()
                        where accessToken.Id == id
                        select new AccessTokenDetailViewModel
                        {
                            Id = accessToken.Id,
                            ApplicationName = accessToken.ApplicationName,
                            TokenName = accessToken.TokenName,
                            InActive = accessToken.InActive,
                            Token = accessToken.Token,
                            Secret = accessToken.Secret,
                            Role = accessToken.Role.Name,
                            CreatedOn = accessToken.CreatedOn,
                            CreatedBy = createdByUser.Email,
                            UpdatedOn = accessToken.UpdatedOn,
                            UpdatedBy = updatedUser != null ? updatedUser.Email : null
                        };
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: AccessTokens/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: AccessTokens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationName,TokenName,InActive,RoleId")] CreateAccessTokenViewModel input)
        {
            if (ModelState.IsValid)
            {
                var tokenResult = await _authService.GenerateTokenByRole(roleId: input.RoleId);
                if (tokenResult.IsSuccess)
                {
                    var accessToken = new Entities.AccessToken();
                    accessToken.Id = tokenResult.TokenId;
                    accessToken.ApplicationName = input.ApplicationName;
                    accessToken.TokenName = input.TokenName;
                    accessToken.Token = tokenResult.Token;
                    accessToken.Secret = tokenResult.Secret;
                    accessToken.InActive = input.InActive;
                    accessToken.RoleId = input.RoleId;
                    accessToken.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    accessToken.CreatedOn = DateTime.Now;
                    _context.AccessToken.Add(accessToken);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", input.RoleId);
            return View(input);
        }

        // GET: AccessTokens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessToken = await _context.AccessToken.FindAsync(id);
            if (accessToken == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", accessToken.RoleId);
            var dto = new EditAccessTokenViewModel();
            dto.Id = accessToken.Id;
            dto.ApplicationName = accessToken.ApplicationName;
            dto.TokenName = accessToken.TokenName;
            dto.InActive = accessToken.InActive;
            dto.RoleId = accessToken.RoleId;
            return View(dto);
        }

        // POST: AccessTokens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ApplicationName,TokenName,InActive,RoleId")] EditAccessTokenViewModel input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var accessToken = await _context.AccessToken.FindAsync(input.Id);
                    accessToken.ApplicationName = input.ApplicationName;
                    accessToken.TokenName = input.TokenName;
                    accessToken.InActive = input.InActive;
                    accessToken.RoleId = input.RoleId;
                    accessToken.UpdatedOn = DateTime.Now;
                    accessToken.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.AccessToken.Update(accessToken);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccessTokenExists(input.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", input.RoleId);
            return View(input);
        }

        // GET: AccessTokens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = from accessToken in _context.AccessToken.Include(t => t.Role)
                        join createdByUser in _context.Users on accessToken.CreatedBy equals createdByUser.Id
                        join updatedByUser in _context.Users on accessToken.UpdatedBy equals updatedByUser.Id into updatedUsers
                        from updatedUser in updatedUsers.DefaultIfEmpty()
                        where accessToken.Id == id
                        select new AccessTokenDetailViewModel
                        {
                            Id = accessToken.Id,
                            ApplicationName = accessToken.ApplicationName,
                            TokenName = accessToken.TokenName,
                            InActive = accessToken.InActive,
                            Token = accessToken.Token,
                            Secret = accessToken.Secret,
                            Role = accessToken.Role.Name,
                            CreatedOn = accessToken.CreatedOn,
                            CreatedBy = createdByUser.Email,
                            UpdatedOn = accessToken.UpdatedOn,
                            UpdatedBy = updatedUser != null ? updatedUser.Email : null
                        };
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: AccessTokens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var accessToken = await _context.AccessToken.FindAsync(id);
            if (accessToken != null)
            {
                _context.AccessToken.Remove(accessToken);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccessTokenExists(string id)
        {
            return _context.AccessToken.Any(e => e.Id == id);
        }
    }
}
