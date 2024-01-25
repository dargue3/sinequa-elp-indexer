using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

namespace GetPageData
{
    public static class Queries
    {
        public const string GetPageDataQuery = @"
            query GetPageById($id: String!) {
              internalPage(id: $id) {
                sys {
                  id
                }
                slug
                cmsTitle
                pageIntroDescription
                sectionContentCollection(limit: 20) {
                  total
                  items {
                    ... on SectionContent {
                      ...SectionContent
                    }
                    ... on SectionGroup {
                      ...SectionGroup
                    }
                    ... on QaSection {
                      ...FAQ
                    }
                  }
                }
                locationBasedContentCollection(limit: 20) {
                  total
                  localContent: items {
                    ... on LocationBasedContent {
                      location {
                        slug
                      }
                      locationSpecificContentCollection(limit: 20) {
                        items {
                          ... on SectionContent {
                            ...SectionContent
                          }
                          ... on SectionGroup {
                            ...SectionGroup
                          }
                          ... on QaSection {
                            ...FAQ
                          }
                        }
                      }
                    }
                  }
                }
              }
            }

            fragment FAQ on QaSection {
              sys {
                id
              }
              title
              questionsCollection(limit: 10) {
                questions: items {
                  question
                  answersCollection(limit: 1) {
                    answers: items {
                      content
                    }
                  }
                }
              }
              __typename
            }

            fragment SectionContent on SectionContent {
              sys {
                id
              }
              sectionTitle
              sectionDescription
              __typename
            }

            fragment SectionGroup on SectionGroup {
              title
              sys {
                id
              }
              contentCollection(limit: 20) {
                total
                content: items {
                  ... on SectionContent {
                    ...SectionContent
                  }
                }
              }
              __typename
            }
        ";
    }

    public class Sys
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Question
    {
        [JsonProperty("question")]
        public string QuestionText { get; set; }

        [JsonProperty("answersCollection")]
        public AnswersCollection AnswersCollection { get; set; }
    }

    public class Answer
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class AnswersCollection
    {
        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }
    }

    public class QuestionsCollection
    {
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class ContentCollection
    {
        [JsonProperty("content")]
        public List<SectionContent> Content { get; set; }
    }

    public abstract class ContentItem
    {
        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        [JsonProperty("__typename")]
        public string Typename { get; set; }
    }

    public class SectionContent : ContentItem
    {
        [JsonProperty("sectionTitle")]
        public string SectionTitle { get; set; }

        [JsonProperty("sectionDescription")]
        public string SectionDescription { get; set; }
    }

    public class SectionGroup : ContentItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("contentCollection")]
        public ContentCollection ContentCollection { get; set; }
    }

    public class QaSection : ContentItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("questionsCollection")]
        public QuestionsCollection QuestionsCollection { get; set; }
    }

    public class LocationBasedContent
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("locationSpecificContentCollection")]
        [JsonConverter(typeof(ContentItemListConverter))]
        public ContentItemList LocationSpecificContentCollection { get; set; }
    }

    public class Location
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }
    }

    public class LocationBasedContentCollection
    {
        [JsonProperty("localContent")]
        public List<LocationBasedContent> LocalContent { get; set; }
    }

    public class ContentItemList
    {
        [JsonProperty("items")]
        public List<ContentItem> Items { get; set; }
    }

    public class InternalPage
    {
        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("cmsTitle")]
        public string CmsTitle { get; set; }

        [JsonProperty("pageIntroDescription")]
        public string PageIntroDescription { get; set; }

        [JsonProperty("sectionContentCollection")]
        [JsonConverter(typeof(ContentItemListConverter))]
        public ContentItemList SectionContentCollection { get; set; }

        [JsonProperty("locationBasedContentCollection")]
        public LocationBasedContentCollection LocationBasedContentCollection { get; set; }
    }

    public class PageByIdResponse
    {
        [JsonProperty("internalPage")]
        public InternalPage InternalPage { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("data")]
        public PageByIdResponse Data { get; set; }
    }

    // For ensuring we can deserialize the content items to the right type
    public class ContentItemListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ContentItemList));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var array = (JArray)obj["items"];
            var itemList = new ContentItemList { Items = new List<ContentItem>() };

            foreach (var item in array)
            {
                if (item is JObject jo && jo.ContainsKey("__typename"))
                {
                    switch (jo["__typename"].Value<string>())
                    {
                        case "SectionContent":
                            itemList.Items.Add(jo.ToObject<SectionContent>(serializer));
                            break;
                        case "SectionGroup":
                            itemList.Items.Add(jo.ToObject<SectionGroup>(serializer));
                            break;
                        case "QaSection":
                            itemList.Items.Add(jo.ToObject<QaSection>(serializer));
                            break;
                        default:
                            throw new ApplicationException($"The type {jo["__typename"].Value<string>()} is not supported!");
                    }
                }
            }

            return itemList;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}