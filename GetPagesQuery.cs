using Newtonsoft.Json;
using System.Collections.Generic;

namespace GetPagesQuery
{
    public static class Queries
    {
        public const string GetPagesQuery = @"
            query GetPages {
                parentCollection(limit: 20) {
                    total
                    parents: items {
                        slug
                        categoryCollection(limit: 20) {
                            total
                            categories: items {
                                ... on Category {
                                    slug
                                    internalPageCollection(limit: 20) {
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
        ";
    }

    public class Sys
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Page
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }
    }

    public class PageCollection
    {

        [JsonProperty("pages")]
        public List<Page> Pages { get; set; }
    }

    public class Category
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("internalPageCollection")]
        public PageCollection InternalPageCollection { get; set; }
    }

    public class CategoryCollection
    {
        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }
    }

    public class Parent
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("categoryCollection")]
        public CategoryCollection CategoryCollection { get; set; }
    }

    public class ParentCollection
    {
        [JsonProperty("parents")]
        public List<Parent> Parents { get; set; }
    }

    public class GetPagesResponse
    {
        [JsonProperty("parentCollection")]
        public ParentCollection ParentCollection { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("data")]
        public GetPagesResponse Data { get; set; }
    }

}