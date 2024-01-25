using Newtonsoft.Json;

namespace Models {

    public class Parent
    {
        public string Slug { get; }

        public Parent(string slug)
        {
            Slug = slug;
        }
    }

    public class Category
    {
        public string Slug { get; }

        public Category(string slug)
        {
            Slug = slug;
        }
    }

    public class Region
    {
        public string Slug { get; }

        public Region(string slug)
        {
            Slug = slug;
        }
    }

    public class City
    {
        public string Slug { get; }

        public City(string slug)
        {
            Slug = slug;
        }
    }

    public class Building
    {
        public string Slug { get; }

        public Building(string slug)
        {
            Slug = slug;
        }
    }

    public class Page
    {
        public string Id { get; }
        public string Slug { get; }
        public Parent Parent { get; }
        public Category Category { get; }

        public Page(string id, string slug, Parent parent, Category category)
        {
            Id = id;
            Slug = slug;
            Parent = parent;
            Category = category;
        }
    }

    public class WorkplacePage
    {
        public string Id { get; }
        public Category Category { get; }
        public City City { get; }
        public Region Region { get; }
        public Building Building { get; }

        public WorkplacePage(string id, Category category, Region region, City city, Building building)
        {
            Id = id;
            Category = category;
            Region = region;
            City = city;
            Building = building;
        }
    }

    public class Slugs {
        [JsonProperty("parent")]
        public string Parent { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("building")]
        public string Building { get; set; }

        public Slugs(string parent, string category, string city, string building)
        {
            Parent = parent;
            Category = category;
            City = city;
            Building = building;
        }
    }

    public class ContentToIndex
    {
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("pageId")]
        public string PageId { get; set; }
        [JsonProperty("pageSlug")]
        public string PageSlug { get; set; }
        [JsonProperty("urlPath")]
        public string UrlPath { get; set; }
        [JsonProperty("contentId")]
        public string ContentId { get; set; }
        [JsonProperty("pageTitle")]
        public string PageTitle { get; set; }
        [JsonProperty("locationSlug")]
        public string LocationSlug { get; set; }
        [JsonProperty("slugs")]
        public Slugs Slugs { get; set; }
    }
}