using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProgramEngineering.Models;
using ProgramEngineering.DB;
using Microsoft.EntityFrameworkCore;

namespace ProgramEngineering.Controllers
{
    [Route("api/v1")]
    public class HomeController : Controller
    {
        private readonly ApiDbContext _dbContext;
        private readonly S3Repository _s3Repository;
        public HomeController(ApiDbContext dbContext, S3Repository s3Repository)
        {
            _dbContext = dbContext;
            _s3Repository = s3Repository;
        }

        [HttpGet("profile/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfile(long id)
        {
            //TODO: Get Info

            return ConvertToJsonResponse(new Profile 
            { 
                UserName = $"roma_{id}",
                BoughtPicturesCount = 7,
                Email = "mymail@gmail.com",
            });
        }

        [HttpGet("pictures")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPictures()
        {
            var picturesList = await _dbContext.Pictures
                .Select(b => new Pictures
                {
                    Id = b.Id,
                    Author = b.Author.Name,
                    PublicationDate = b.PublicationDate,
                    Title = b.Title
                })
                .ToListAsync();
            return ConvertToJsonResponse(picturesList);
        }

        [HttpGet("pictures/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindPictures(string search)
        {
            var picturesList = await _dbContext.Pictures
                .Where(b=>b.Title.Contains(search))
                .Select(b => new Pictures
                {
                    Id = b.Id,
                    Author = b.Author.Name,
                    PublicationDate = b.PublicationDate,
                    Title = b.Title
                })
                .ToListAsync();

            return ConvertToJsonResponse(picturesList);
        }

        [HttpGet("pictures/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPicture(long id)
        {
            var picture = await _dbContext.Pictures
                 .Where(b=>b.Id == id)
                 .Select(b => new Pictures
                 {
                     Id = b.Id,
                     Author = b.Author.Name,
                     PublicationDate = b.PublicationDate,
                     Title = b.Title
                 })
                 .FirstOrDefaultAsync();


            return ConvertToJsonResponse(picture);
        }

        [HttpPost("pictures")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPicture([FromBody] Pictures picture)
        {
            if (!picture.IsValid())
            {
                return BadRequest(picture);
            }

            var authorId = await _dbContext.Authors
                .Where(a => a.Name == picture.Author)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (authorId <= 0)
            {
                var authorDb = new DB.Models.Author
                {
                    Name = picture.Author
                };
                _dbContext.Add(authorDb);
                _dbContext.SaveChanges();
                authorId = authorDb.Id;
            }
            //Write to DB
            var pictureDb = new DB.Models.Picture
            {
                PublicationDate = picture.PublicationDate,
                Title = picture.Title,
                AuthorId = authorId
            };
            _dbContext.Add(pictureDb);
            _dbContext.SaveChanges();
            return ConvertToJsonResponse(picture);
        }

        [HttpPut("files/upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDocumentToS3(IFormFile file)
        {
            if (file is null || file.Length <= 0)
                return BadRequest();

            using (var stream = file.OpenReadStream())
            {
                await _s3Repository.PutFile(stream, file.FileName);
            }
            return Ok();
        }

        [HttpGet("files/get/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentsFromS3()
        {
            var files = await _s3Repository.GetFiles();
            return ConvertToJsonResponse(files);
        }

        [HttpGet("files/get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentFromS3(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var document = await _s3Repository.GetFile(name);

            return File(document, "application/octet-stream", name);
        }


        private IActionResult ConvertToJsonResponse(object obj)
        {
            if (obj is null)
            {
                return NotFound(obj);
            }

            return Ok(JsonConvert.SerializeObject(obj));
        }
    }
}
