using CommunityResourceSharing.Data;
using CommunityResourceSharing.DTOs;
using CommunityResourceSharing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ResourceController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResource()
        {
            var resources = await _context.Resources.ToListAsync();
            return Ok(resources);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetResourceById(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();

            var dto = new ResourceDto
            {
                Id = resource.Id,
                Title = resource.Title,
                Description = resource.Description,
                Category = resource.Category,
                Status = resource.Status,
                OwnerId = resource.OwnerId,
                CreatedAt = resource.CreatedAt
            };

            return Ok(dto);
        }
        [HttpPost]
        public async Task<ActionResult<ResourceDto>> AddResource(ResourceDto resourceDto)
        {
            var newResource = new Resource
            {
                Title = resourceDto.Title,
                Description = resourceDto.Description,
                Category = resourceDto.Category,
                Status = resourceDto.Status,
                OwnerId = resourceDto.OwnerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Resources.AddAsync(newResource);
            await _context.SaveChangesAsync();

            // Map back to DTO (so response includes Id)
            var createdDto = new ResourceDto
            {
                Id = newResource.Id,
                Title = newResource.Title,
                Description = newResource.Description,
                Category = newResource.Category,
                Status = newResource.Status,
                OwnerId = newResource.OwnerId,
                CreatedAt = newResource.CreatedAt
            };

            // Return 201 Created with location header
            return CreatedAtAction(nameof(GetResourceById),
                new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResourceDto>> UpdateResource(int id, ResourceDto resourceDto)
        {
            if (id != resourceDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var existingResource = await _context.Resources.FindAsync(id);
            if (existingResource == null)
            {
                return NotFound();
            }

            existingResource.Title = resourceDto.Title;
            existingResource.Description = resourceDto.Description;
            existingResource.Category = resourceDto.Category;
            existingResource.Status = resourceDto.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Resources.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw; 
                }
            }

            // Map back to DTO for response
            var updatedDto = new ResourceDto
            {
                Id = existingResource.Id,
                Title = existingResource.Title,
                Description = existingResource.Description,
                Category = existingResource.Category,
                Status = existingResource.Status,
                OwnerId = existingResource.OwnerId,
                CreatedAt = existingResource.CreatedAt
            };

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 response
        }


    }
}
