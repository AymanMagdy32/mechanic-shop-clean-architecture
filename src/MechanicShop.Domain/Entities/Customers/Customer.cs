using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Entities.Customers.Vehicles;

namespace MechanicShop.Domain.Entities.Customers
{
    public class Customer : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;

    private readonly List<Vehicle> _vehicles = [];
    public IEnumerable<Vehicle> Vehicles => _vehicles.AsReadOnly();
       
        // Constructor
        private Customer() 
        {}

        private Customer(Guid id, string name, string email, string phoneNumber) : base(id)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        // Factory Method

        public static Result<Customer> Create(Guid id, string name, string email, string phoneNumber)
        {
             if (string.IsNullOrWhiteSpace(name))
                return CustomerError.NameRequired;
            if (string.IsNullOrWhiteSpace(email))
                return CustomerError.EmailRequired;
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return CustomerError.PhoneNumberRequired;
 
               return new Customer(id, name, email, phoneNumber);
        }   

        public  Result<Updated> Update(string name , string email , string phoneNumber)
        {
             if (string.IsNullOrWhiteSpace(name))
                return CustomerError.NameRequired;
            if (string.IsNullOrWhiteSpace(email))
                return CustomerError.EmailRequired;
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return CustomerError.PhoneNumberRequired;
 
        
          Name = name;
          Email = email;
          PhoneNumber = phoneNumber; 

          return Result.Updated ;
        }

      
     


    }
}