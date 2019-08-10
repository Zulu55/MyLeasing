using System;
using System.Linq;
using System.Threading.Tasks;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;

namespace MyLeasing.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRoles();
            var manager = await CheckUserAsync("1010", "Juan", "Zuluaga", "jzuluaga55@gmail.com", "350 634 2747", "Calle Luna Calle Sol", "Manager");
            var owner = await CheckUserAsync("2020", "Juan", "Zuluaga", "jzuluaga55@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", "Owner");
            var lessee = await CheckUserAsync("3030", "Juan", "Zuluaga", "carlos.zuluaga@globant.com", "350 634 2747", "Calle Luna Calle Sol", "Lessee");
            await CheckPropertyTypesAsync();
            await CheckManagerAsync(manager);
            await CheckOwnersAsync(owner);
            await CheckLesseesAsync(lessee);
            await CheckPropertiesAsync();
            await CheckContractsAsync();
        }

        private async Task CheckContractsAsync()
        {
            var owner = _context.Owners.FirstOrDefault();
            var lessee = _context.Lessees.FirstOrDefault();
            var property = _context.Properties.FirstOrDefault();
            if (!_context.Contracts.Any())
            {
                _context.Contracts.Add(new Contract
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddYears(1),
                    IsActive = true,
                    Lessee = lessee,
                    Owner = owner,
                    Price = 800000M,
                    Property = property,
                    Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                    "Mauris nec iaculis ex. Nullam gravida nunc eleifend, placerat tellus a, " +
                    "eleifend metus. Phasellus id suscipit magna. Orci varius natoque penatibus et " +
                    "magnis dis parturient montes, nascetur ridiculus mus. Nullam volutpat ultrices ex, " +
                    "sed cursus sem tincidunt ut. Nullam metus lorem, convallis quis dignissim quis, " +
                    "porttitor quis leo. In hac habitasse platea dictumst. Duis pharetra sed arcu ac " +
                    "viverra. Proin dapibus lobortis commodo. Vivamus non commodo est, ac vehicula augue. " +
                    "Nam enim felis, rutrum in tortor sit amet, efficitur hendrerit augue. Cras pellentesque " +
                    "nisl eu maximus tempor. Curabitur eu efficitur metus. Sed ultricies urna et auctor commodo."
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckManagerAsync(User user)
        {
            if (!_context.Managers.Any())
            {
                _context.Managers.Add(new Manager { User = user });
                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> CheckUserAsync(
            string document, 
            string firstName, 
            string lastName, 
            string email, 
            string phone, 
            string address, 
            string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Manager");
            await _userHelper.CheckRoleAsync("Owner");
            await _userHelper.CheckRoleAsync("Lessee");
        }

        private async Task CheckLesseesAsync(User user)
        {
            if (!_context.Lessees.Any())
            {
                _context.Lessees.Add(new Lessee { User = user });
                await _context.SaveChangesAsync();
            }
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

        private async Task CheckPropertyTypesAsync()
        {
            if (!_context.PropertyTypes.Any())
            {
                _context.PropertyTypes.Add(new PropertyType { Name = "Apartamento" });
                _context.PropertyTypes.Add(new PropertyType { Name = "Casa" });
                _context.PropertyTypes.Add(new PropertyType { Name = "Negocio" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckOwnersAsync(User user)
        {
            if (!_context.Owners.Any())
            {
                _context.Owners.Add(new Owner { User = user });
                await _context.SaveChangesAsync();
            }
        }

        private void AddProperty(string address, string neighborhood, Owner owner, PropertyType propertyType, decimal price, int rooms, int squareMeters, int stratum)
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
