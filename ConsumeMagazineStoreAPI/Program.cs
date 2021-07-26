using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumeMagazineStoreAPI.Models;
using Newtonsoft.Json;

namespace ConsumeMagazineStoreAPI
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static Category categories = null;
        private static SubscriberList subscribersList = null;
        static async Task Main(string[] args)
        {
            client.BaseAddress = new Uri("http://magazinestore.azurewebsites.net/api/"); // Set the base address of the URI of magazinestore

            // Get Token from /api/token
            var resToken = await CallMagazineStore("token");
            var myToken = JsonConvert.DeserializeObject<MyToken>(resToken);

            Task t1 = Task.Run(async () => {
                // Get Categories using valid token
                var resCategories = await CallMagazineStore("categories/" + myToken.Token);
                categories = JsonConvert.DeserializeObject<Category>(resCategories);
            });
            Task t2 = Task.Run(async () => {
                // Get Subscribers using valid token
                var resSubscribersList = await CallMagazineStore("subscribers/" + myToken.Token);
                subscribersList = JsonConvert.DeserializeObject<SubscriberList>(resSubscribersList);
            });
            
            await Task.WhenAll(t1);
                        
            string resMagazines = string.Empty;
            List<Magazine> magazines = new List<Magazine>();
            var tasks = new List<Task>();
            // Get Magazines using valid token and recieved categories
            foreach (var name in categories.Data)
            {
                var categoryName = name;
                tasks.Add(Task.Run(async () => {                    
                    resMagazines = await CallMagazineStore("magazines/" + myToken.Token + "/" + categoryName);
                    magazines.AddRange(JsonConvert.DeserializeObject<MagazineList>(resMagazines).Data);
                }));
            }
                        
            await Task.WhenAll(tasks.ToArray());
            await Task.WhenAll(t2);

            var groupedResult = magazines.GroupBy(m => m.Category);
            List<string> SubscriberIds = new List<string>(); 

            foreach (var subscriber in subscribersList.Data)
            {
                int i = 0;
                foreach (var group in groupedResult)
                {
                    foreach (Magazine m in group)
                    {
                        if (subscriber.MagazineIds.Contains(m.Id))
                        {
                            i++;
                            break;
                        }
                    }
                    if (i == 3)
                    {
                        SubscriberIds.Add(subscriber.Id);
                        break;
                    }
                }
            }

            PostList pl = new PostList();
            pl.Subscribers = SubscriberIds;

            HttpResponseMessage response = await client.PostAsJsonAsync("answer/" + myToken.Token, pl);
            response.EnsureSuccessStatusCode();

            // Deserialize the output from the response body.
            var answerList = await response.Content.ReadAsAsync<AnswerList>();
            if (answerList.Success)
            {
                Console.WriteLine("totalTime: " + answerList.Data.TotalTime);
                Console.WriteLine("answerCorrect: " + answerList.Data.AnswerCorrect);
            }
            else
                Console.WriteLine("Post request failed");

            // Print the result in console
            Console.WriteLine("\nSubscribers with atleast 1 magazine in each category:");
            Console.WriteLine("\"" + string.Join("\",\n\"", SubscriberIds) + "\"\n");

            Console.ReadLine();
        }

        private static async Task<string> CallMagazineStore(string action)
        {
            var stringTask = client.GetStringAsync(action);

            return await stringTask;
        }
    }
}
