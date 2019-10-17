using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Common.Models;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;

namespace MyLeasing.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OwnersController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public OwnersController(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        [HttpPost]
        [Route("GetOwnerByEmail")]
        public async Task<IActionResult> GetOwner(EmailRequest emailRequest)
        {
            try
            {
                var user = await _userHelper.GetUserByEmailAsync(emailRequest.Email);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                if (await _userHelper.IsUserInRoleAsync(user, "Owner"))
                {
                    return await GetOwnerAsync(emailRequest);
                }
                else
                {
                    return await GetLesseeAsync(emailRequest);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private async Task<IActionResult> GetLesseeAsync(EmailRequest emailRequest)
        {
            var lessee = await _dataContext.Lessees
                .Include(o => o.User)
                .Include(o => o.Contracts)
                .ThenInclude(c => c.Owner)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower().Equals(emailRequest.Email.ToLower()));

            var properties = await _dataContext.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyImages)
                .Where(p => p.IsAvailable)
                .ToListAsync();

            var response = new OwnerResponse
            {
                RoleId = 2,
                Id = lessee.Id,
                FirstName = lessee.User.FirstName,
                LastName = lessee.User.LastName,
                Address = lessee.User.Address,
                Document = lessee.User.Document,
                Email = lessee.User.Email,
                PhoneNumber = lessee.User.PhoneNumber,
                Properties = properties?.Select(p => new PropertyResponse
                {
                    Address = p.Address,
                    HasParkingLot = p.HasParkingLot,
                    Id = p.Id,
                    IsAvailable = p.IsAvailable,
                    Neighborhood = p.Neighborhood,
                    Price = p.Price,
                    PropertyImages = p.PropertyImages?.Select(pi => new PropertyImageResponse
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImageFullPath
                    }).ToList(),
                    PropertyType = p.PropertyType.Name,
                    Remarks = p.Remarks,
                    Rooms = p.Rooms,
                    SquareMeters = p.SquareMeters,
                    Stratum = p.Stratum
                }).ToList(),
                Contracts = lessee.Contracts?.Select(c => new ContractResponse
                {
                    EndDate = c.EndDate,
                    Id = c.Id,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    Remarks = c.Remarks,
                    StartDate = c.StartDate
                }).ToList()
            };

            return Ok(response);
        }

        private async Task<IActionResult> GetOwnerAsync(EmailRequest emailRequest)
        {
            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Properties)
                .ThenInclude(p => p.PropertyType)
                .Include(o => o.Properties)
                .ThenInclude(p => p.PropertyImages)
                .Include(o => o.Contracts)
                .ThenInclude(c => c.Lessee)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower().Equals(emailRequest.Email.ToLower()));

            var response = new OwnerResponse
            {
                RoleId = 1,
                Id = owner.Id,
                FirstName = owner.User.FirstName,
                LastName = owner.User.LastName,
                Address = owner.User.Address,
                Document = owner.User.Document,
                Email = owner.User.Email,
                PhoneNumber = owner.User.PhoneNumber,
                Properties = owner.Properties?.Select(p => new PropertyResponse
                {
                    Address = p.Address,
                    Contracts = p.Contracts?.Select(c => new ContractResponse
                    {
                        EndDate = c.EndDate,
                        Id = c.Id,
                        IsActive = c.IsActive,
                        Lessee = ToLessesResponse(c.Lessee),
                        Price = c.Price,
                        Remarks = c.Remarks,
                        StartDate = c.StartDate
                    }).ToList(),
                    HasParkingLot = p.HasParkingLot,
                    Id = p.Id,
                    IsAvailable = p.IsAvailable,
                    Neighborhood = p.Neighborhood,
                    Price = p.Price,
                    PropertyImages = p.PropertyImages?.Select(pi => new PropertyImageResponse
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImageFullPath
                    }).ToList(),
                    PropertyType = p.PropertyType.Name,
                    Remarks = p.Remarks,
                    Rooms = p.Rooms,
                    SquareMeters = p.SquareMeters,
                    Stratum = p.Stratum
                }).ToList(),
                Contracts = owner.Contracts?.Select(c => new ContractResponse
                {
                    EndDate = c.EndDate,
                    Id = c.Id,
                    IsActive = c.IsActive,
                    Price = c.Price,
                    Remarks = c.Remarks,
                    StartDate = c.StartDate
                }).ToList()
            };

            return Ok(response);
        }

        private LesseeResponse ToLessesResponse(Lessee lessee)
        {
            return new LesseeResponse
            {
                Address = lessee.User.Address,
                Document = lessee.User.Document,
                Email = lessee.User.Email,
                FirstName = lessee.User.FirstName,
                LastName = lessee.User.LastName,
                PhoneNumber = lessee.User.PhoneNumber
            };
        }

        [HttpGet]
        [Route("GetAvailbleProperties")]
        public async Task<IActionResult> GetAvailbleProperties()
        {
            var properties = await _dataContext.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyImages)
                .Where(p => p.IsAvailable)
                .ToListAsync();

            var response = new List<PropertyResponse>(properties.Select(p => new PropertyResponse
            {
                Address = p.Address,
                HasParkingLot = p.HasParkingLot,
                Id = p.Id,
                IsAvailable = p.IsAvailable,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Neighborhood = p.Neighborhood,
                Price = p.Price,
                PropertyImages = new List<PropertyImageResponse>(p.PropertyImages.Select(pi => new PropertyImageResponse
                {
                    Id = pi.Id,
                    ImageUrl = pi.ImageFullPath
                }).ToList()),
                PropertyType = p.PropertyType.Name,
                Remarks = p.Remarks,
                Rooms = p.Rooms,
                SquareMeters = p.SquareMeters,
                Stratum = p.Stratum
            }).ToList());

            return Ok(response);
        }

    }
}
