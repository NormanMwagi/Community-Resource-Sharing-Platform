using AutoMapper;
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
        private readonly IMapper _mapper;
        public ResourceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResource()
        {
            var resources = await _context.Resources
                .ToListAsync();
            return Ok(_mapper.Map<List<ResourceDto>>(resources));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetResourceById(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();

            return Ok(_mapper.Map<ResourceDto>(resource));
        }
        [HttpPost]
        public async Task<ActionResult<ResourceDto>> AddResource(ResourceDto resourceDto)
        {
            var newResource = _mapper.Map<Resource>(resourceDto);
            newResource.CreatedAt = DateTime.UtcNow;


            await _context.Resources.AddAsync(newResource);
            await _context.SaveChangesAsync();
            // Map back to DTO (so response includes Id)
         
            var createdDto = _mapper.Map<ResourceDto>(newResource);

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

            _mapper.Map(resourceDto, existingResource);
            resourceDto.CreatedAt = existingResource.CreatedAt;

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
            _mapper.Map<ResourceDto>(existingResource);

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
