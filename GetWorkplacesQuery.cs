using Newtonsoft.Json;
using System.Collections.Generic;

namespace GetWorkplaces
{
    public static class Queries
    {
        public const string GetWorkplacesQuery = @"
            query GetWorkplaces($skip: Int) {
              workplaceRegionCollection(limit: 1, skip: $skip) {
                total
                regions: items {
                  slug
                  citiesCollection(limit: 15) {
                    total
                    cities: items {
                      sys {
                        id
                      }
                      slug
                      title
                      buildingsCollection(limit: 5) {
                        total
                        buildings: items {
                          sys {
                            id
                          }
                          slug
                          categoriesCollection(limit: 7) {
                            total
                            categories: items {
                              slug
                              internalPageCollection(limit: 15) {
                                total
                                pages: items {
                                  slug
                                  sys {
                                    id
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
        ";
    }
    public class Sys
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class InternalPage
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }
    }

    public class InternalPageCollection
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("pages")]
        public List<InternalPage> Pages { get; set; }
    }

    public class Category
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("internalPageCollection")]
        public InternalPageCollection InternalPageCollection { get; set; }
    }

    public class CategoriesCollection
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }
    }

    public class Building
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        [JsonProperty("categoriesCollection")]
        public CategoriesCollection CategoriesCollection { get; set; }
    }

    public class BuildingsCollection
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("buildings")]
        public List<Building> Buildings { get; set; }
    }

    public class City
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("title")]
        public string Name { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        [JsonProperty("buildingsCollection")]
        public BuildingsCollection BuildingsCollection { get; set; }
    }

    public class CitiesCollection
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("cities")]
        public List<City> Cities { get; set; }
    }

    public class Region
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("citiesCollection")]
        public CitiesCollection CitiesCollection { get; set; }
    }

    public class WorkplaceRegionCollection
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("regions")]
        public List<Region> Regions { get; set; }
    }

    public class GetWorkplacesResponse
    {
        [JsonProperty("workplaceRegionCollection")]
        public WorkplaceRegionCollection WorkplaceRegionCollection { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("data")]
        public GetWorkplacesResponse Data { get; set; }
    }

}