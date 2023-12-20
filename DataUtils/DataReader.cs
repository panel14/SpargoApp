using SpargoApp.Domain.Models;
using SpargoApp.Domain.Repository;
using SpargoApp.Properties;
using System.Text.RegularExpressions;

namespace SpargoApp.DataUtils
{
    internal static class DataReader
    {
        public static void GetIntValueFromUser(out int value, string message, string error, Predicate<int> predicate)
        {
            Console.WriteLine(message);
            while (!int.TryParse(Console.ReadLine(), out value) || !predicate(value))
            {
                Console.WriteLine(error);
            }
        }

        private static void GetDecimalValueFromUser(out decimal value, string message, string error, Predicate<decimal> predicate)
        {
            Console.WriteLine(message);
            while (!decimal.TryParse(Console.ReadLine(), out value) || !predicate(value))
            {
                Console.WriteLine(error);
            }
        }

        public static void GetStringValueFromUser(out string value, string message, string error, string pattern = @".+")
        {
            Regex regex = new(pattern);

            Console.WriteLine(message);
            while (true)
            {
                value = Console.ReadLine();
                if (value != null && regex.IsMatch(value))
                {
                    return;
                }
                Console.WriteLine(error);
            }
        }

        public static Product GetProductModelFromUser()
        {
            Product product = new();

            Console.WriteLine("Ввод продукта");

            GetStringValueFromUser(out string name, "Введите название продукта:", "Название не должно быть пустым");
            product.Name = name;

            GetDecimalValueFromUser(out decimal price, "Введите цену продукта:", "Цена должна быть больше 0 и содержит только цифры (разделитель - запятая)", i => i > 0);
            product.Price = price;

            return product;
        }

        public static Pharmacy GetPharmacyModelFromUser()
        {
            Pharmacy pharmacy = new();

            GetStringValueFromUser(out string name, "Введите название аптеки", "Поле не должно быть пустым");
            pharmacy.Name = name;

            GetStringValueFromUser(out string address, "Введите адрес аптеки", "Поле не должно быть пустым");
            pharmacy.Address = address;

            GetStringValueFromUser(out string phone, "Введите телефон аптеки", "Формат номера: +X (XXX) XXX-XX-XX", @"^\+[1-9]{1}\s\([0-9]{3}\)\s[0-9]{3}-[0-9]{2}-[0-9]{2}$");
            pharmacy.Phone = phone;

            return pharmacy;
        }

        public static Store GetStoreModelFromUser()
        {
            Store store = new();

            Console.WriteLine("Ввод склада");

            string connectionString = Resources.connectionString;
            using GenericRepository<Pharmacy> repository = new(connectionString);
            List<Pharmacy> ids = repository.GetAll(nameof(Pharmacy)).ToList();

            if (ids.Count == 0)
            {
                Console.WriteLine("На данный момент нет аптек для добавления в поле PharmacyId. Необходимо создать аптеку");
                store.Id = -1;
                return store;
            }

            Console.WriteLine("Введите ID аптеки из представленных ниже:");
            ids.ForEach(Console.WriteLine);

            GetIntValueFromUser(out int value, string.Empty, "Выберите ID аптеки из существующих:", ids.Select(p => p.Id).Contains);
            store.PharmacyId = value;

            GetStringValueFromUser(out string name, "Введите имя склада", "Имя не должно быть пустым");
            store.Name = name;

            GetStringValueFromUser(out string address, "Введите адрес склада", "Имя не должно быть пустым");
            store.Address = address;

            return store;
        }

        public static Shipment GetShipmentModelFromUser()
        {
            Shipment shipment = new();

            string connectionString = Resources.connectionString;
            using GenericRepository<Product> pRepos = new(connectionString);
            List<Product> productIds = pRepos.GetAll(nameof(Product)).ToList();

            if (productIds.Count == 0)
            {
                Console.WriteLine("На данный момент нет продуктов для добавления в поле ProductId. Необходимо создать продукт");
                shipment.Id = -1;
                return shipment;
            }

            Console.WriteLine("Введите ID продукта из представленных ниже:");
            productIds.ForEach(Console.WriteLine);

            GetIntValueFromUser(out int product, string.Empty, "Выберите ID продукта из существующих", productIds.Select(p => p.Id).Contains);
            shipment.ProductId = product;

            using GenericRepository<Store> sRepos = new(connectionString);
            List<Store> storeIds = sRepos.GetAll(nameof(Store)).ToList();

            if (storeIds.Count == 0)
            {
                Console.WriteLine("На данный момент нет складов для добавления в поле StoreId. Необходимо создать склад");
                shipment.Id = -1;
                return shipment;
            }

            Console.WriteLine("Введите ID склада из представленных ниже:");
            storeIds.ForEach(Console.WriteLine);

            GetIntValueFromUser(out int store, string.Empty, "Выберите ID склада из существующих", storeIds.Select(s => s.Id).Contains);
            shipment.StoreId = store;

            GetIntValueFromUser(out int count, "Введите количество продукта в партии", "Количество должно быть целым числом больше 0", i => i > 0);
            shipment.Count = count;

            return shipment;
        }
    }
}
