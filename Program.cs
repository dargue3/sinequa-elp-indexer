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
                var GetWorkplacesResponse = new GetWorkplaces.GetWorkplaces();

                var PagesTask = GetPagesResponse.FetchAsync();
                var WorkplacesPagesTask = GetWorkplacesResponse.FetchAllWorkplaces();

                await Task.WhenAll(PagesTask, WorkplacesPagesTask);

                Console.WriteLine($"Pages From Parents: {GetPagesResponse.Pages.Count}\n");
                Console.WriteLine($"Pages From Workplaces: {GetWorkplacesResponse.Pages.Count}\n");

                var PageGetter = new GetPageData.GetPageDataForIndexing();

                await PageGetter.FetchParentCategoryPages(GetPagesResponse.Pages);
                await PageGetter.FetchWorkplacePages(GetWorkplacesResponse.Pages);

                Console.WriteLine($"Blocks of Content To Index: {PageGetter.ContentToIndex.Count}\n");

                // Print out the response
                var json = JsonConvert.SerializeObject(PageGetter.ContentToIndex, Formatting.Indented);

                File.WriteAllText("results.json", json);
                Console.WriteLine("Results written to results.json\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}