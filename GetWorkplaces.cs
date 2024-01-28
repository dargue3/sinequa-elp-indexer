using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace GetWorkplaces
{
  public class GetWorkplaces
  {
      public List<Models.City> Cities { get; } = new List<Models.City>();
      public List<Models.Region> Regions { get; } = new List<Models.Region>();
      public List<Models.Building> Buildings { get; } = new List<Models.Building>();
      public List<Models.WorkplacePage> Pages { get; } = new List<Models.WorkplacePage>();

      public async Task FetchAllWorkplaces()
      {
          int skip = 0;

          while (skip < 100)
          {
              var done = await FetchPage(skip);
              skip++;
              if (done) break;
          }
      }

      public async Task<bool> FetchPage(int skip)
      {
          try
          {
              var response = await NetworkUtility.MakeContentfulRequest<ApiResponse>(Queries.GetWorkplacesQuery, new { skip = skip });

              if (!response.Data.WorkplaceRegionCollection.Regions.Any()) return true;

              foreach (var r in response.Data.WorkplaceRegionCollection.Regions)
              {
                  if (r == null) continue;
                  var region = new Models.Region(r.Slug);
                  Regions.Add(region);

                  foreach (var c in r.CitiesCollection.Cities)
                  {
                      if (c == null) continue;
                      var city = new Models.City(c.Slug, c.Name);
                      Cities.Add(city);

                      foreach (var b in c.BuildingsCollection.Buildings)
                      {
                          if (b == null) continue;
                          var building = new Models.Building(b.Slug);
                          Buildings.Add(building);

                          foreach (var cat in b.CategoriesCollection.Categories)
                          {
                              if (cat == null) continue;
                              var category = new Models.Category(cat.Slug);

                              foreach (var page in cat.InternalPageCollection.Pages)
                              {
                                  if (page == null) continue;
                                  Pages.Add(new Models.WorkplacePage(
                                    page.Sys.Id, 
                                    page.Slug,
                                    category, 
                                    region, 
                                    city, 
                                    building 
                                ));
                              }
                          }
                      }
                  }
              }
          }
          catch (Exception ex)
          {
            Console.Error.WriteLine(ex);
          }
          return true;
      }
  }

}