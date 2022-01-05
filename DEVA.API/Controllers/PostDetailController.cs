using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEVA.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace DEVA.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PostDetailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDetail>>> GetPostDetails()
        {
            return await _context.PostDetails.ToListAsync();
        }

        // GET: api/PostDetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDetail>> GetPostDetail(int id)
        {
            var postDetail = await _context.PostDetails.FindAsync(id);

            if (postDetail == null)
            {
                return NotFound();
            }

            return postDetail;
        }

        // PUT: api/PostDetail/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostDetail(int id, PostDetail postDetail)
        {
            if (id != postDetail.PostId)
            {
                return BadRequest();
            }

            _context.Entry(postDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostDetailExists(id))
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

        // POST: api/PostDetail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostDetail>> PostPostDetail(PostDetail postDetail)
        {
            _context.PostDetails.Add(postDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostDetail", new { id = postDetail.PostId }, postDetail);
        }

        // DELETE: api/PostDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostDetail(int id)
        {
            var postDetail = await _context.PostDetails.FindAsync(id);
            if (postDetail == null)
            {
                return NotFound();
            }

            _context.PostDetails.Remove(postDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostDetailExists(int id)
        {
            return _context.PostDetails.Any(e => e.PostId == id);
        }
    }
}