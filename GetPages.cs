using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GetPagesQuery;

namespace GetPages
{
    public class GetPages
    {
        public List<Models.Page> Pages { get; set; } = new List<Models.Page>();
        public List<Models.Parent> Parents { get; set; } = new List<Models.Parent>();
        public List<Models.Category> Categories { get; set; } = new List<Models.Category>();

        public async Task FetchAsync()
        {
            try
            {
                var response = await NetworkUtility.MakeContentfulRequest<ApiResponse>(Queries.GetPagesQuery);

                if (response.Data.ParentCollection.Parents != null)
                {
                    foreach (var p in response.Data.ParentCollection.Parents.Where(p => p != null))
                    {
                        var parent = new Models.Parent(p.Slug);
                        Parents.Add(parent);

                        if (p.CategoryCollection.Categories != null)
                        {
                            foreach (var c in p.CategoryCollection.Categories.Where(c => c != null))
                            {
                                var category = new Models.Category(c.Slug);
                                Categories.Add(category);

                                if (c.InternalPageCollection.Pages != null)
                                {
                                    foreach (var page in c.InternalPageCollection.Pages.Where(page => page != null))
                                    {
                                        Pages.Add(new Models.Page(page.Sys.Id, page.Slug, parent, category));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}