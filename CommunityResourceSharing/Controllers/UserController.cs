using AutoMapper;
using CommunityResourceSharing.Data;
using CommunityResourceSharing.DTOs;
using CommunityResourceSharing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ✅ GET all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .ToListAsync();

            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        // ✅ GET user by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var dto = _mapper.Map<UserDto>(user);

            return Ok(dto);
        }

        // ✅ POST create user
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            var newUser = _mapper.Map<Users>(dto);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<UserDto>(newUser);

            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }

        // ✅ PUT update user
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, CreateUserDto dto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            _mapper.Map(dto, existingUser);
            existingUser.Id = id; // Ensure ID remains unchanged
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var updated = _mapper.Map<CreateUserDto>(existingUser);

            return Ok(updated);
        }

        // ✅ DELETE user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
