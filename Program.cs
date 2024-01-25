using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SinequaElpIndexer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var GetPagesResponse = new GetPages.GetPages();

                await GetPagesResponse.FetchAsync();

                var GetPageResponse = new GetPageData.GetPageDataForIndexing();

                await GetPageResponse.FetchParentCategoryPages(GetPagesResponse.Pages);

                

                // Print out the response
                var json = JsonConvert.SerializeObject(GetPageResponse.ContentToIndex, Formatting.Indented);

                File.WriteAllText("results.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}