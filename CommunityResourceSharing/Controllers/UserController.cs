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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .ToListAsync();

            return Ok(_mapper.Map<List<UserDto>>(users));
        }

    }
}
