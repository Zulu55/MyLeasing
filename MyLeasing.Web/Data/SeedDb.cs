using MyLeasing.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckPropertyTypesAsync();
            await CheckOwnersAsync();
            await CheckLesseesAsync();
            await CheckPropertiesAsync();
        }

        private async Task CheckPropertyTypesAsync()
        {
            if (!_context.PropertyTypes.Any())
            {
                _context.PropertyTypes.Add(new Entities.PropertyType { Name = "Apartamento" });
                _context.PropertyTypes.Add(new Entities.PropertyType { Name = "Casa" });
                _context.PropertyTypes.Add(new Entities.PropertyType { Name = "Negocio" });
                await _context.SaveChangesAsync();
            }
        }
        private async Task CheckLesseesAsync()
        {
            if (!_context.Lessees.Any())
            {
                AddLessee("876543", "Ramon", "Gamboa", "234 3232", "310 322 3221", "Calle Luna Calle Sol");
                AddLessee("654565", "Julian", "Martinez", "343 3226", "300 322 3221", "Calle 77 #22 21");
                AddLessee("214231", "Carmenza", "Ruis", "450 4332", "350 322 3221", "Carrera 56 #22 21");
                await _context.SaveChangesAsync();
            }
        }

        private void AddLessee(string document, string firstName, string lastName, string fixedPhone, string cellPhone, string address)
        {
            _context.Lessees.Add(new Lessee
            {
                Address = address,
                CellPhone = cellPhone,
                Document = document,
                FirstName = firstName,
                FixedPhone = fixedPhone,
                LastName = lastName
            });
        }

        private async Task CheckPropertiesAsync()
        {
            var owner = _context.Owners.FirstOrDefault();
            var propertyType = _context.PropertyTypes.FirstOrDefault();
            if (!_context.Properties.Any())
            {
                AddProperty("Calle 43 #23 32", "Poblado", owner, propertyType, 800000M, 2, 72, 4);
                AddProperty("Calle 12 Sur #2 34", "Envigado", owner, propertyType, 950000M, 3, 81, 3);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckOwnersAsync()
        {
            if (!_context.Owners.Any())
            {
                AddOwner("8989898", "Juan", "Zuluaga", "234 3232", "310 322 3221", "Calle Luna Calle Sol");
                AddOwner("7655544", "Jose", "Cardona", "343 3226", "300 322 3221", "Calle 77 #22 21");
                AddOwner("6565555", "Maria", "López", "450 4332", "350 322 3221", "Carrera 56 #22 21");
                await _context.SaveChangesAsync();
            }
        }

        private void AddOwner(
            string document, 
            string firstName,
            string lastName, 
            string fixedPhone, 
            string cellPhone, 
            string address)
        {
            _context.Owners.Add(new Owner
            {
                Address = address,
                CellPhone = cellPhone,
                Document = document,
                FirstName = firstName,
                FixedPhone = fixedPhone,
                LastName = lastName
            });
        }

        private void AddProperty(
            string address, 
            string neighborhood, 
            Owner owner, 
            PropertyType propertyType, 
            decimal price, 
            int rooms, 
            int squareMeters, 
            int stratum)
        {
            _context.Properties.Add(new Property
            {
                Address = address,
                HasParkingLot = true,
                IsAvailable = true,
                Neighborhood = neighborhood,
                Owner = owner,
                Price = price,
                PropertyType = propertyType,
                Rooms = rooms,
                SquareMeters = squareMeters,
                Stratum = stratum
            });
        }
    }
}
