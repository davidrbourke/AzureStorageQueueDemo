using System;
using System.Collections.Generic;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.Azure.Storage; // Namespace for CloudStorageAccount
using Microsoft.Azure.Storage.Queue; // Namespace for Queue storage types
using Newtonsoft.Json;


namespace AzureStorageQueueDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //var conn = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var conn = "";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                conn);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("plans");

            queue.CreateIfNotExists();


            //for (int j = 0; j < 40; j++)
            //{
            //    var plansManager = new PlansManager
            //    {
            //        BatchId = j,
            //        PlansToCreate = new List<PlanToCreate>()
            //    };

            //    for (int i = 0; i < 50; i++)
            //    {
            //        var planToCreate = new PlanToCreate
            //        {
            //            StoreId = i,
            //            PlanDate = new DateTime(2019, 08, 12)
            //        };
            //        plansManager.PlansToCreate.Add(planToCreate);
            //    }

            //    var planMessage = JsonConvert.SerializeObject(plansManager);

            //    CloudQueueMessage message = new CloudQueueMessage(planMessage);
            //    queue.AddMessage(message);
            //}
            Console.WriteLine($"Done. Queued: {queue.ApproximateMessageCount}");

            bool hasItems = true;

            while (hasItems)
            {
                CloudQueueMessage receivedMessage = queue.GetMessage();
                if (receivedMessage == null)
                {
                    hasItems = false;
                    break;
                }
                PlansManager planFromQueue = JsonConvert.DeserializeObject<PlansManager>(receivedMessage.AsString);
                Console.WriteLine(planFromQueue.ToString());
                queue.DeleteMessage(receivedMessage);
            }
        }
    }


    public class PlansManager
    {
        public int BatchId { get; set; }
        public List<PlanToCreate> PlansToCreate { get; set; }
        public override string ToString()
        {
            return $"{BatchId}: {PlansToCreate.Count}";
        }
    }

    public class PlanToCreate
    {
        public int StoreId { get; set; }
        public DateTime PlanDate { get; set; }
        public override string ToString()
        {
            return $"{StoreId}: {PlanDate}";
        }
    }
}
