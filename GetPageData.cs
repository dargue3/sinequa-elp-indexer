using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;

namespace GetPageData
{
    public class GetPageDataForIndexing
    {
        public List<ContentToIndex> ContentToIndex { get; set; } = new List<ContentToIndex>();

        public async Task FetchParentCategoryPages(List<Page> pages)
        {
            foreach (var page in pages)
            {
                try
                {
                    var response = await NetworkUtility.MakeContentfulRequest<ApiResponse>(Queries.GetPageDataQuery, new { id = page.Id });
                    var p = response.Data.InternalPage;
                    var urlPath = $"/{page.Parent.Slug}/{page.Category.Slug}/{p.Slug}";
                    IndexContent(response, urlPath, new Slugs(page.Parent.Slug, page.Category.Slug, null, null));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        public void IndexContent(ApiResponse pageData, string urlPath, Slugs slugs)
        {
            var p = pageData.Data.InternalPage;
            var title = p.CmsTitle;

            // Process global content
            foreach (var content in p.SectionContentCollection.Items.Where(content => content != null))
            {
                switch (content)
                {
                    case SectionContent c:
                        ContentToIndex.Add(new ContentToIndex
                        {
                            UrlPath = urlPath,
                            Slugs = slugs,
                            PageSlug = p.Slug,
                            PageId = p.Sys.Id,
                            PageTitle = title,
                            ContentId = c.Sys.Id,
                            Data = JsonConvert.SerializeObject(new { title = c.SectionTitle, description = c.SectionDescription })
                        });
                        break;
                    case SectionGroup c:
                        if (c.ContentCollection?.Content == null) continue;
                        foreach (var co in c.ContentCollection.Content.Where(co => co != null))
                        {
                            if (co.SectionDescription == null || co.Sys == null || p.Sys == null) continue;
                            ContentToIndex.Add(new ContentToIndex
                            {
                                UrlPath = urlPath,
                                Slugs = slugs,
                                PageSlug = p.Slug,
                                PageId = p.Sys.Id,
                                PageTitle = title,
                                ContentId = co.Sys.Id,
                                Data = JsonConvert.SerializeObject(new { title = co.SectionTitle, description = co.SectionDescription })
                            });
                        }
                        break;
                    case QaSection c:
                        ContentToIndex.Add(new ContentToIndex
                        {
                            UrlPath = urlPath,
                            Slugs = slugs,
                            PageSlug = p.Slug,
                            PageId = p.Sys.Id,
                            PageTitle = title,
                            ContentId = c.Sys.Id,
                            Data = JsonConvert.SerializeObject(c.QuestionsCollection)
                        });
                        break;
                }
            }

            // Process local content
            foreach (var locationBased in p.LocationBasedContentCollection.LocalContent.Where(locationBased => locationBased != null))
            {
                var locationSlug = locationBased.Location.Slug;

                foreach (var content in locationBased.LocationSpecificContentCollection.Items.Where(content => content != null))
                {
                    switch (content)
                    {
                        case SectionContent c:
                            ContentToIndex.Add(new ContentToIndex
                            {
                                UrlPath = urlPath,
                                PageSlug = p.Slug,
                                PageId = p.Sys.Id,
                                PageTitle = title,
                                ContentId = c.Sys.Id,
                                Data = JsonConvert.SerializeObject(c),
                                LocationSlug = locationSlug
                            });
                            break;
                        case SectionGroup c:
                            if (c.ContentCollection?.Content == null) continue;
                            foreach (var co in c.ContentCollection.Content.Where(co => co != null))
                            {
                                if (co.SectionDescription == null || co.Sys == null || p.Sys == null) continue;
                                ContentToIndex.Add(new ContentToIndex
                                {
                                    UrlPath = urlPath,
                                    PageSlug = p.Slug,
                                    PageId = p.Sys.Id,
                                    PageTitle = title,
                                    ContentId = co.Sys.Id,
                                    Data = co.SectionDescription,
                                    LocationSlug = locationSlug
                                });
                            }
                            break;
                        case QaSection c:
                            ContentToIndex.Add(new ContentToIndex
                            {
                                UrlPath = urlPath,
                                PageSlug = p.Slug,
                                PageId = p.Sys.Id,
                                PageTitle = title,
                                ContentId = c.Sys.Id,
                                Data = JsonConvert.SerializeObject(c.QuestionsCollection.Questions
                                    .Where(q => q != null)
                                    .Select(q => new
                                    {
                                        question = q.QuestionText,
                                        answer = q.AnswersCollection.Answers.Where(a => a != null).Select(a => a.Content).ToList()
                                    })),
                                LocationSlug = locationSlug
                            });
                            break;
                    }
                }
            }
        }
    }
}