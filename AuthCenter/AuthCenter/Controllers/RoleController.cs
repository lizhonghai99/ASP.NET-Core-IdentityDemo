using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using AuthCenter.Models.Dtos.Role;

namespace AuthCenter.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            
            _roleManager = roleManager;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var dtos = roles.Select(t => new RoleDto { Id = t.Id, Name = t.Name, NormalizedName = t.NormalizedName }).ToList();
            return View(dtos);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.Roles.Where(t => t.Id == id).FirstOrDefaultAsync();
            if (role == null)
            {
                return NotFound();
            }
            var dto = new RoleDto();
            dto.Id = role.Id;
            dto.Name = role.Name;
            dto.NormalizedName = role.NormalizedName;
            return View(dto);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,NormalizedName")] CreateRole input)
        {
            if (ModelState.IsValid)
            {
                var roleExist = await _roleManager.FindByNameAsync(input.Name);
                if (roleExist != null)
                    return BadRequest();
                var role = new IdentityRole { Name = input.Name, NormalizedName = input.NormalizedName, ConcurrencyStamp = Guid.NewGuid().ToString() };
                await _roleManager.CreateAsync(role);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var dto = new RoleDto();
            dto.Id = role.Id;
            dto.Name = role.Name;
            dto.NormalizedName = role.NormalizedName;
            return View(dto);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,NormalizedName")] RoleDto input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    if (role != null)
                    {
                        role.Name = input.Name;
                        role.NormalizedName = input.NormalizedName;
                        role.ConcurrencyStamp = Guid.NewGuid().ToString();
                        var result = await _roleManager.UpdateAsync(role);
                        Console.WriteLine();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await RoleExists(input.Id))
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

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RoleExists(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return role != null;
        }
    }
}
