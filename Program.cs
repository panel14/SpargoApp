using SpargoApp.DataUtils;
using SpargoApp.Domain.Models;
using SpargoApp.Domain.Repository;
using SpargoApp.Properties;
using SpargoApp.Utils;
using System.Linq;

string helpCommand = "Список команд:\n" +
    "add [product, pharmacy, store, shipment] - добавить продукт, аптеку, магазин или склад\n" +
    "delete [product, pharmacy, store, shipment] - удалить продукт, аптеку, магазин или склад\n" +
    "help - вывести список доступных команд\n" +
    "show all - вывести все объекты базы данных\n" +
    "show task - вывести весь список товаров и его количество в выбранной аптеке\n" +
    "exit - выход из приложения";

Console.WriteLine($"Добро пожаловать. {helpCommand}");

Console.WriteLine("Введите команду");

string connectionString = Resources.connectionString;

Lazy<GenericRepository<Product>> productRepository = new(() => new(connectionString));
Lazy<GenericRepository<Pharmacy>> pharmacyRepository = new(() => new(connectionString));
Lazy<GenericRepository<Shipment>> shipmentRepository = new(() => new(connectionString));
Lazy<GenericRepository<Store>> storeRepository = new(() => new(connectionString));

string? command = Console.ReadLine();

while  (!command.Equals("exit"))
{
    string[] commandArray = command.Split(' ');

    switch(commandArray[0])
    {
        case "add":
            bool isInsertSuccess = false;
            switch (commandArray[1])
            {
                case "product":
                    Product product = DataReader.GetProductModelFromUser();
                    isInsertSuccess = productRepository.Value.Insert(nameof(Product), product);
                    break;
                case "pharmacy":
                    Pharmacy pharmacy = DataReader.GetPharmacyModelFromUser();
                    isInsertSuccess = pharmacyRepository.Value.Insert(nameof(Pharmacy), pharmacy);
                    break;
                case "store":
                    Store store = DataReader.GetStoreModelFromUser();
                    if (store.Id == -1)
                    {
                        isInsertSuccess = false;
                    }
                    else
                    {
                        isInsertSuccess = storeRepository.Value.Insert(nameof(Store), store);
                    }
                    break;
                case "shipment":
                    Shipment shipment = DataReader.GetShipmentModelFromUser();
                    if (shipment.Id == -1)
                    {
                        isInsertSuccess = false;
                    }
                    else
                    {
                        isInsertSuccess = shipmentRepository.Value.Insert(nameof(Shipment), shipment);
                    }
                    break;
                default:
                    ConsoleExtension.WriteColorLine("Неизвестная команда. Попробуйте ещё раз", ConsoleColor.Red);
                    break;
            }
            if (isInsertSuccess)
            {
                ConsoleExtension.WriteColorLine("Успешно.", ConsoleColor.Green);
            }
            else
            {
                ConsoleExtension.WriteColorLine("Ошибка при выполнении операции.", ConsoleColor.Red);
            }
            break;
        case "delete":

            bool isDeleteSuccess = false;
            switch (commandArray[1])
            {
                case "product":
                    List<Product> products = productRepository.Value.GetAll(nameof(Product)).ToList();
                    if (products.Count > 0)
                    {
                        Console.WriteLine("Введите id удаляемого продукта. Список продуктов:");
                        products.ForEach(Console.WriteLine);
                        DataReader.GetIntValueFromUser(out int productId, "Введите Id:", "Выберите Id из предложенных выше:", products.Select(p => p.Id).Contains);

                        List<Shipment> linkedShipments = shipmentRepository.Value.GetAll(nameof(Shipment), s => s.ProductId == productId).ToList();
                        if (linkedShipments.Count > 0)
                        {
                            ConsoleExtension.WriteColorLine("При удалении данного товара будут также удалены следующие партии:", ConsoleColor.Yellow);
                            linkedShipments.ForEach(Console.WriteLine);
                            DataReader.GetStringValueFromUser(out string answer, "Продолжить? [y/n]", "Продолжить? [y/n]", "^[yn]{1}$");
                            if (answer.Equals("n"))
                            {
                                Console.WriteLine("Операция прервана.");
                                isDeleteSuccess = false;
                                break;
                            }
                        }
                        isDeleteSuccess = productRepository.Value.Delete(nameof(Product), productId);
                    }
                    else
                    {
                        Console.WriteLine("Нет продуктов для удаления.");
                        isDeleteSuccess = false;
                    }
                    break;
                case "pharmacy":
                    List<Pharmacy> pharmacies = pharmacyRepository.Value.GetAll(nameof(Pharmacy)).ToList();
                    if (pharmacies.Count > 0)
                    {
                        Console.WriteLine("Введите id удаляемой аптеки. Список аптек:");
                        pharmacies.ForEach(Console.WriteLine);
                        DataReader.GetIntValueFromUser(out int pharmacyId, "Введите Id:", "Выберите Id из предложенных выше:", pharmacies.Select(p => p.Id).Contains);

                        List<Store> linkedStores = storeRepository.Value.GetAll(nameof(Store), s => s.PharmacyId == pharmacyId).ToList();
                        if (linkedStores.Count > 0)
                        {
                            ConsoleExtension.WriteColorLine("При удалении данной аптеки будут также удалены следующие склады:", ConsoleColor.Yellow);
                            linkedStores.ForEach(Console.WriteLine);
                            DataReader.GetStringValueFromUser(out string answer, "Продолжить? [y/n]", "Продолжить? [y/n]", "^[yn]{1}$");
                            if (answer.Equals("n"))
                            {
                                Console.WriteLine("Операция прервана.");
                                isDeleteSuccess = false;
                                break;
                            }
                        }
                        isDeleteSuccess = pharmacyRepository.Value.Delete(nameof(Pharmacy), pharmacyId);

                    }
                    else
                    {
                        Console.WriteLine("Нет аптек для удаления");
                        isDeleteSuccess = false;
                    }
                    break;
                case "store":
                    List<Store> stores = storeRepository.Value.GetAll(nameof(Store)).ToList();
                    if (stores.Count > 0)
                    {
                        Console.WriteLine("Введите id удаляемого склада. Список складов:");
                        stores.ForEach(Console.WriteLine);
                        DataReader.GetIntValueFromUser(out int storeId, "Введите Id:", "Выберите Id из предложенных выше:", stores.Select(s => s.Id).Contains);

                        List<Shipment> linkedShipments = shipmentRepository.Value.GetAll(nameof(Shipment), s => s.Id == storeId).ToList();
                        if (linkedShipments.Count > 0)
                        {
                            ConsoleExtension.WriteColorLine("При удалении данного склада будут также удалены следующие партии:", ConsoleColor.Yellow);
                            linkedShipments.ForEach(Console.WriteLine);
                            DataReader.GetStringValueFromUser(out string answer, "Продолжить? [y/n]", "Продолжить? [y/n]", "^[yn]{1}$");
                            if (answer.Equals("n"))
                            {
                                Console.WriteLine("Операция прервана.");
                                isDeleteSuccess = false;
                                break;
                            }
                        }

                        isDeleteSuccess = storeRepository.Value.Delete(nameof(Store), storeId);
                    }
                    else
                    {
                        Console.WriteLine("Нет складов для удаления");
                        isDeleteSuccess = false;
                    }
                    break;
                case "shipment":
                    List<Shipment> shimpents = shipmentRepository.Value.GetAll(nameof(Shipment)).ToList();
                    if (shimpents.Count > 0)
                    {
                        Console.WriteLine("Введите id удаляемой партии. Список партий:");
                        shimpents.ForEach(Console.WriteLine);
                        DataReader.GetIntValueFromUser(out int shipmentId, "Введите Id:", "Выберите Id из предложенных выше:", shimpents.Select(s => s.Id).Contains);
                        isDeleteSuccess = shipmentRepository.Value.Delete(nameof(Shipment), shipmentId);
                    }
                    else
                    {
                        Console.WriteLine("Нет партий для удаления");
                        isDeleteSuccess = false;
                    }
                    break;
                default:
                    ConsoleExtension.WriteColorLine("Неизвестная команда. Попробуйте ещё раз", ConsoleColor.Red);
                    break;
            }

            if (isDeleteSuccess)
            {
                ConsoleExtension.WriteColorLine("Успешно", ConsoleColor.Green);
            }
            else
            {
                ConsoleExtension.WriteColorLine("Ошибка при выполнении операции", ConsoleColor.Red);
            }

            break;
        case "show":
            switch (commandArray[1])
            {
                case "all":
                    List<Product> products = productRepository.Value.GetAll(nameof(Product)).ToList();
                    Console.WriteLine("Продукты:");
                    products.ForEach(Console.WriteLine);

                    List<Pharmacy> pharmacies = pharmacyRepository.Value.GetAll(nameof(Pharmacy)).ToList();
                    Console.WriteLine("Аптеки:");
                    pharmacies.ForEach(Console.WriteLine);

                    List<Store> stores = storeRepository.Value.GetAll(nameof(Store)).ToList();
                    Console.WriteLine("Склады:");
                    stores.ForEach(Console.WriteLine);

                    List<Shipment> shipments = shipmentRepository.Value.GetAll(nameof(Shipment)).ToList();
                    Console.WriteLine("Партии:");
                    shipments.ForEach(Console.WriteLine);
                    break;
                case "task":
                    List<Product> allProducts = productRepository.Value.GetAll(nameof(Product)).ToList();
                    Console.WriteLine("Список всех товаров");
                    allProducts.ForEach(Console.WriteLine);

                    List<Pharmacy> allPharmacies = pharmacyRepository.Value.GetAll(nameof(Pharmacy)).ToList();
                    Console.WriteLine("Список всех аптек");
                    allPharmacies.ForEach(Console.WriteLine);

                    Console.WriteLine("Выберите аптеку:");
                    DataReader.GetIntValueFromUser(out int value, string.Empty, "Выберите Id из аптек выше", allPharmacies.Select(p => p.Id).Contains);

                    IEnumerable<int> storesIds = storeRepository.Value.GetAll(nameof(Store), s => s.PharmacyId == value).Select(s => s.Id);
                    List<Shipment> linkedShipments = shipmentRepository.Value.GetAll(nameof(Shipment), s => storesIds.Contains(s.StoreId)).ToList();

                    Console.WriteLine("Склады выбранной аптеки:");
                    linkedShipments.ForEach(Console.WriteLine);

                    int count = linkedShipments.Select(s => s.Count).Aggregate((acc, i) => acc + i);
                    Console.WriteLine($"Количество товара: {count}");

                    break;
                default:
                    ConsoleExtension.WriteColorLine("Неизвестная команда. Попробуйте ещё раз", ConsoleColor.Red);
                    break;
            }
            break;
        case "help":
            Console.WriteLine(helpCommand);
            break;
        default:
            ConsoleExtension.WriteColorLine("Неизвестная команда. Попробуйте ещё раз", ConsoleColor.Red);
            break;
    }

    Console.WriteLine("Введите команду:");
    command = Console.ReadLine();
}

Console.WriteLine("Выход из приложения.");