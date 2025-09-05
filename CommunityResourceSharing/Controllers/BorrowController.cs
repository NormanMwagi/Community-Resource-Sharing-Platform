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
    public class BorrowController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BorrowController(AppDbContext context) {
            _context = context;
        }
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<BorrowRequestDto>>> GetBorrowRequests()
        {
            var borrowReq = await _context.BorrowRequests.Select(x => new BorrowRequestDto
            {
                Id = x.Id,
                ResourceId = x.ResourceId,
                BorrowerId = x.BorrowerId,
                Status = x.Status
            })
             .ToListAsync();

            return Ok(borrowReq);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRequestDto>> GetBorrowRequests(int id)
        {
            var request = await _context.BorrowRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            var reqDto = new BorrowRequestDto
            {
                Id = request.Id,
                ResourceId = request.ResourceId,
                BorrowerId = request.BorrowerId,
                Status = request.Status
            };

            return Ok(reqDto);
        }
        [HttpPost]
        public async Task<ActionResult> AddBorrowRequest(BorrowRequestDto borrowRequestDto)
        {
            var req = new BorrowRequest
            {
                ResourceId = borrowRequestDto.ResourceId,
                BorrowerId = borrowRequestDto.BorrowerId,
                Status = borrowRequestDto.Status
            };
            await _context.BorrowRequests.AddAsync(req);
            await _context.SaveChangesAsync();
            var result = new BorrowRequestDto
            {
                Id = req.Id,
                ResourceId = req.ResourceId,
                BorrowerId = req.BorrowerId,
                Status = req.Status
            };
            return CreatedAtAction(nameof(GetBorrowRequests),
                new { id = result.Id }, result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBorrowRequest(int id, BorrowRequestDto borrowRequestDto)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var existingReq = await _context.BorrowRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (existingReq == null)
            {
                return NotFound();
            }
            existingReq.ResourceId = borrowRequestDto.ResourceId;
            existingReq.BorrowerId = borrowRequestDto.BorrowerId;
            existingReq.Status = borrowRequestDto.Status;

            try
            {
                await _context.SaveChangesAsync();
                

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BorrowRequests.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new BorrowRequestDto
            {
                ResourceId = existingReq.ResourceId,
                BorrowerId = existingReq.BorrowerId,
                Status = existingReq.Status
            };

            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBorrowRequest(int id)
        {
            var existingReq = await _context.BorrowRequests.FindAsync(id);

            if (existingReq == null)
            {
                return NotFound(); // No record found with that id
            }

            _context.BorrowRequests.Remove(existingReq);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 response (successful delete)
        }
    }
}
