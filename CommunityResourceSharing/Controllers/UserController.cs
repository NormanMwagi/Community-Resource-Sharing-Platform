using CommunityResourceSharing.Data;
using CommunityResourceSharing.DTOs;
using CommunityResourceSharing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceSharing.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsAdmin = u.isAdmin,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ GET user by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var dto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsAdmin = user.isAdmin,
                CreatedAt = user.CreatedAt
            };

            return Ok(dto);
        }

        // ✅ POST create user
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            var newUser = new Users
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password, // TODO: hash before saving
                isAdmin = dto.IsAdmin,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                Id = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                IsAdmin = newUser.isAdmin,
                CreatedAt = newUser.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }

        // ✅ PUT update user
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, CreateUserDto dto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.FullName = dto.FullName;
            existingUser.Email = dto.Email;
            existingUser.Password = dto.Password; // again, hash ideally
            existingUser.isAdmin = dto.IsAdmin;

            await _context.SaveChangesAsync();

            var updated = new UserDto
            {
                Id = existingUser.Id,
                FullName = existingUser.FullName,
                Email = existingUser.Email,
                IsAdmin = existingUser.isAdmin,
                CreatedAt = existingUser.CreatedAt
            };

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
