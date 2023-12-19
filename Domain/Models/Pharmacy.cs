namespace SpargoApp.Domain.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public override string ToString()
        {
            return $"Id: {Id}; Name: {Name}; Address: {Address}; Phone: {Phone}";
        }
    }
}
