using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Data;
using AuthCenter.Entities;
using Microsoft.AspNetCore.Identity;
using AuthCenter.Models.Dtos.RoleClaim;
using System.Security.Claims;

namespace AuthCenter.Controllers
{
    public class RoleClaimController : Controller
    {


        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IRoleClaimStore<IdentityRole> _roleClaimStore;

        public RoleClaimController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: RoleClaim
        public async Task<IActionResult> Index()
        {

            var list = new List<RoleClaimDto>();
            var roles = _roleManager.Roles.ToList();

            foreach (var role in roles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in roleClaims)
                {
                    list.Add(new RoleClaimDto { Role = role.Name, ClaimType = claim.Type, ClaimValue = claim.Value });
                }
            }
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,ClaimType,ClaimValue")] CreateRoleClaim input)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(input.RoleId);
                if (role != null)
                {

                    var claim = new Claim(input.ClaimType, input.ClaimValue);
                    await _roleManager.AddClaimAsync(role, claim);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }


    }
}
