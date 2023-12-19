namespace SpargoApp.Domain.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}; ProductId: {ProductId}; StoreId: {StoreId}; Count: {Count}";
        }
    }
}
