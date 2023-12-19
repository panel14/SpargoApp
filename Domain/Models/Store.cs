using System.ComponentModel.DataAnnotations;

namespace SpargoApp.Domain.Models
{
    public class Store
    {
        public int Id { get; set; }

        public int PharmacyId { get; set; }

        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        public override string ToString()
        {
            return $"Id: {Id}; PharmacyId: {PharmacyId}; Name: {Name}; Address: {Address}";
        }
    }
}
