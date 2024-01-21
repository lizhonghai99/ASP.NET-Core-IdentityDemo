using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Data;
using AuthCenter.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthCenter.Models.Dtos.Company;
using System.Security.Claims;

namespace AuthCenter.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        [Authorize(Policy = "ViewCompany")]
        public async Task<ActionResult<IEnumerable<CompanyViewModel>>> GetCompany()
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
            return result;
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "ViewCompany")]
        public async Task<ActionResult<CompanyViewModel>> GetCompany(string id)
        {
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
            return result;
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "EditCompany")]
        public async Task<IActionResult> PutCompany(string id, EditCompanyViewModel input)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }
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
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Policy = "CreateCompany")]
        public async Task<ActionResult<Company>> PostCompany(CreateCompanyViewModel input)
        {
            var company = new Company();
            try
            {
                company.Name = input.Name;
                company.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                company.CreatedOn = DateTime.Now;
                _context.Company.Add(company);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompanyExists(company.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteCompany")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(string id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
