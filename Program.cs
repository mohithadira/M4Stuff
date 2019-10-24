using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace AzureTableStorage
{
    class GuestEntity: TableEntity
    {
        public string City { get; set; }
        public string Contact { get; set; }

        public GuestEntity() { }
        public GuestEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
    }
    
    class Program
    {
        static CloudStorageAccount storageAccount;
        static CloudTableClient tableClient;
        static CloudTable table;

        static void Main(string[] args)
        {
        start:
            Console.WriteLine("0 Create table\n1 Add Entity\n2 Retrieve Entity\n3 Update Entity\n4 Delete Entity\n5 Delete Table");
            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "0":
                        {
                            CreateAzureStorageTable();
                            Console.ReadKey();
                            break;
                        }

                    case "1":
                        {
                            AddGuestEntityDyn();
                            Console.ReadKey();
                            break;
                        }
                    case "2":
                        {
                            RetrieveGuestEntity();
                            Console.ReadKey();
                            break;
                        }
                    case "3":
                        {
                            UpdateGuestEntity();
                            Console.ReadKey();
                            break;
                        }
                    case "4":
                        {
                            DeleteGuestEntity();
                            Console.ReadKey();
                            break;
                        }
                    case "5":
                        {
                            DeleteAzureStorageTable();
                            Console.ReadKey();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Not Found!");
                            Console.ReadKey();
                            break;
                        }

                }
                goto start;
            }
            catch (StorageException exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private static void CreateAzureStorageTable()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("guesto");
            table.CreateIfNotExists();
            Console.WriteLine("Table Created");
        }

        private static void AddGuestEntity()
        {
            GuestEntity guestEntity = new GuestEntity("nono", "nonono");
            guestEntity.City = "molando";
            guestEntity.Contact = "12334";
            TableOperation insertOperation = TableOperation.InsertOrReplace(guestEntity);
            table.Execute(insertOperation);
            Console.WriteLine("Entity Added!");
        }

        private static void AddGuestEntityDyn()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lname = Console.ReadLine();

            GuestEntity guestEntity = new GuestEntity(fname, lname);

            Console.Write("Enter City: ");
            guestEntity.City = Console.ReadLine();
            Console.Write("Enter Contact: ");
            guestEntity.Contact = Console.ReadLine();

            TableOperation insertOperation = TableOperation.Insert(guestEntity);
            table.Execute(insertOperation);
            Console.WriteLine("Entity Added!");
        }

        private static void RetrieveGuestEntity()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lname = Console.ReadLine();
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>(fname, lname);
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if(retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                Console.WriteLine($"City: {guest.City}\t Contact: {guest.Contact}");
            }
            else
            {
                Console.WriteLine("Could not be retrieved! :(");
            }
        }

        private static void UpdateGuestEntity()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lname = Console.ReadLine();
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>(fname, lname);
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if(retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                Console.Write("Enter Contact Number");
                guest.Contact = Console.ReadLine();
                TableOperation updateOperation = TableOperation.Replace(guest);
                table.Execute(updateOperation);
                Console.WriteLine("Entity Updated!");
            }
            else
            {
                Console.WriteLine("Could not be retrieved! :(");
            }
        }

        private static void DeleteGuestEntity()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lname = Console.ReadLine();

            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>(fname, lname);
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                TableOperation deleteOperation = TableOperation.Delete(guest);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity Deleted!");
            }
            else
            {
                Console.WriteLine("Could not be retrieved! :(");
            }
        }

        private static void DeleteAzureStorageTable()
        {
            table.DeleteIfExists();
            Console.WriteLine("Table Deleted");
        }
    }
}
