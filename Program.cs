using System;
using System.Net.Http;
using System.Threading.Tasks;
using ArticleServiceClient;
using Grpc.Net.Client;

namespace ArticleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // Ignore SSL - bad, do not use, self signed ssl cert tooling is borked for linux

            using var channel =
                GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler }); // Actually ignore SSL this time

            var client = new Articles.ArticlesClient(channel);

            Console.WriteLine("1. Retrieve articles");
            Console.WriteLine("2. Get Single Article");

            string input = Console.ReadLine();

            if (input.ToLower() == "1")
            {
                ArticleList list = await client.GetArticleListAsync(new EmptyRequest());

                foreach (var listArticle in list.Articles)
                {
                    Console.WriteLine("GUID:");
                    Console.WriteLine(listArticle.Guid);
                    Console.WriteLine("---");
                    Console.WriteLine("Title:");
                    Console.WriteLine(listArticle.Title);
                    Console.WriteLine("---");
                    Console.WriteLine("Content:");
                    Console.WriteLine(listArticle.Content);
                    Console.WriteLine("---");
                    Console.WriteLine("Published:");
                    Console.WriteLine((listArticle.Published) ? "Yes: " + listArticle.PublishedAt : "No");
                    Console.WriteLine("---");
                    Console.WriteLine();
                }
            }
        }
    }
}
