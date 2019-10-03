using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;

namespace MyLeasing.Web.Controllers.API
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropertiesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PropertiesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostProperty([FromBody] PropertyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = await _dataContext.Owners.FindAsync(request.OwnerId);
            if (owner == null)
            {
                return BadRequest("Not valid owner.");
            }

            var propertyType = await _dataContext.PropertyTypes.FindAsync(request.PropertyTypeId);
            if (propertyType == null)
            {
                return BadRequest("Not valid property type.");
            }

            var property = new Property
            {
                Address = request.Address,
                HasParkingLot = request.HasParkingLot,
                IsAvailable = request.IsAvailable,
                Neighborhood = request.Neighborhood,
                Owner = owner,
                Price = request.Price,
                PropertyType = propertyType,
                Remarks = request.Remarks,
                Rooms = request.Rooms,
                SquareMeters = request.SquareMeters,
                Stratum = request.Stratum
            };

            _dataContext.Properties.Add(property);
            await _dataContext.SaveChangesAsync();
            return Ok(true);
        }

        [HttpPost]
        [Route("AddImageToProperty")]
        public async Task<IActionResult> AddImageToProperty([FromBody] ImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var property = await _dataContext.Properties.FindAsync(request.PropertyId);
            if (property == null)
            {
                return BadRequest("Not valid property.");
            }

            var imageUrl = string.Empty;
            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(request.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Properties";
                var fullPath = $"~/images/Properties/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            var propertyImage = new PropertyImage
            {
                ImageUrl = imageUrl,
                Property = property
            };

            _dataContext.PropertyImages.Add(propertyImage);
            await _dataContext.SaveChangesAsync();
            return Ok(true);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutProperty(
            [FromRoute] int id, 
            [FromBody] PropertyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest();
            }

            var oldProperty = await _dataContext.Properties.FindAsync(request.Id);
            if (oldProperty == null)
            {
                return BadRequest("Property doesn't exists.");
            }

            var propertyType = await _dataContext.PropertyTypes.FindAsync(request.PropertyTypeId);
            if (propertyType == null)
            {
                return BadRequest("Not valid property type.");
            }

            oldProperty.Address = request.Address;
            oldProperty.HasParkingLot = request.HasParkingLot;
            oldProperty.IsAvailable = request.IsAvailable;
            oldProperty.Neighborhood = request.Neighborhood;
            oldProperty.Price = request.Price;
            oldProperty.PropertyType = propertyType;
            oldProperty.Remarks = request.Remarks;
            oldProperty.Rooms = request.Rooms;
            oldProperty.SquareMeters = request.SquareMeters;
            oldProperty.Stratum = request.Stratum;

            _dataContext.Properties.Update(oldProperty);
            await _dataContext.SaveChangesAsync();
            return Ok(true);
        }

        [HttpPost]
        [Route("DeleteImageToProperty")]
        public async Task<IActionResult> DeleteImageToProperty([FromBody] ImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var propertyImage = await _dataContext.PropertyImages.FindAsync(request.Id);
            if (propertyImage == null)
            {
                return BadRequest("Property image doesn't exist.");
            }

            _dataContext.PropertyImages.Remove(propertyImage);
            await _dataContext.SaveChangesAsync();
            return Ok(propertyImage);
        }
    }
}
