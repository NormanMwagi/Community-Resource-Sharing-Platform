using AutoMapper;
using CommunityResourceSharing.Data;
using CommunityResourceSharing.DTOs;
using CommunityResourceSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public BorrowController(AppDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        [Authorize(Roles = "User, Admin")]
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<BorrowRequestDto>>> GetBorrowRequests()
        {
            var borrowReq = await _context.BorrowRequests
             .ToListAsync();

            return Ok(_mapper.Map<List<BorrowRequestDto>>(borrowReq));
        }
        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRequestDto>> GetBorrowRequests(int id)
        {
            var request = await _context.BorrowRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            var reqDto = _mapper.Map<BorrowRequestDto>(request);

            return Ok(reqDto);
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public async Task<ActionResult> AddBorrowRequest(BorrowRequestDto borrowRequestDto)
        {
            var req = _mapper.Map<BorrowRequest>(borrowRequestDto);
            await _context.BorrowRequests.AddAsync(req);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<BorrowRequestDto>(req);
            return CreatedAtAction(nameof(GetBorrowRequests),
                new { id = result.Id }, result);
        }
        [Authorize(Roles = "User, Admin")]
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
            _mapper.Map(borrowRequestDto, existingReq);

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
            var result = _mapper.Map<BorrowRequestDto>(existingReq);

            return NoContent();

        }
        [Authorize(Roles = "User, Admin")]
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
