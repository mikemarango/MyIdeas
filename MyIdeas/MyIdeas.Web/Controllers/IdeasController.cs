using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MyIdeas.Web.Models;
using Microsoft.AspNet.Identity;

namespace MyIdeas.Web.Controllers
{
    [Authorize]
    public class IdeasController : ApiController
    {
        private IdeasContext db = new IdeasContext();

        // GET: api/Ideas
        public IQueryable<Idea> GetIdeas()
        {
            return db.Ideas;
        }

        // GET: api/Ideas/CurrentUser
        [Route("api/Ideas/CurrentUser")]

        public IQueryable<Idea> GetIdeasForCurrentUser()
        {
            string userId = User.Identity.GetUserId();
            return db.Ideas.Where(idea => idea.UserId == userId);
        }

        // GET: api/Ideas/5
        [ResponseType(typeof(Idea))]
        public async Task<IHttpActionResult> GetIdea(int id)
        {
            Idea idea = await db.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }

            return Ok(idea);
        }

        // PUT: api/Ideas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutIdea(int id, Idea idea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != idea.Id)
            {
                return BadRequest();
            }

            string userId = User.Identity.GetUserId();

            if (userId != idea.UserId)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            db.Entry(idea).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IdeaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Ideas
        [ResponseType(typeof(Idea))]
        public async Task<IHttpActionResult> PostIdea(Idea idea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Identity.GetUserId();

            idea.UserId = userId;

            db.Ideas.Add(idea);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = idea.Id }, idea);
        }

        // DELETE: api/Ideas/5
        [ResponseType(typeof(Idea))]
        public async Task<IHttpActionResult> DeleteIdea(int id)
        {
            Idea idea = await db.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }

            db.Ideas.Remove(idea);
            await db.SaveChangesAsync();

            return Ok(idea);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IdeaExists(int id)
        {
            return db.Ideas.Count(e => e.Id == id) > 0;
        }
    }
}