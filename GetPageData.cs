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
        public readonly int PAGE_SIZE = 5;

        public List<ContentToIndex> ContentToIndex { get; set; } = new List<ContentToIndex>();

        public async Task<InternalPage> FetchPage(string id, int skip)
        {
            var Response = await NetworkUtility.MakeContentfulRequest<ApiResponse>(
                Queries.GetPageDataQuery,
                new { id = id, skip = skip }
            );
            return Response.Data.InternalPage;
        }

        public async Task<List<InternalPage>> PaginateThroughContent(string pageId)
        {
            // We're going to accumulate multiple InternalPage objects of the same page.
            // However that's fine since we index the _content_ of the page, not the page itself.
            var PageContentList = new List<InternalPage>();
            try
            {
                // The skip parameter is used to paginate through the content
                // The limit in GetPageDataQuery is set to 5, so we'll jump in pages of 5.
                int skip = 0;
                while (skip < 100)
                {
                    var PageData = await FetchPage(pageId, skip);

                    PageContentList.Add(PageData);

                    int remainingGlobalContent = PageData.SectionContentCollection.Items.Count;
                    int remainingLocalContent = PageData.LocationBasedContentCollection.Items.Count;
                    if (remainingGlobalContent < PAGE_SIZE && remainingLocalContent < PAGE_SIZE) break;

                    skip += PAGE_SIZE;
                }
            }
            catch (Exception ex)
            {
                Sinequa.Common.Sys.LogError(ex); 
            }
            return PageContentList;
        }

        public async Task FetchParentCategoryPages(List<Page> pages)
        {
            foreach (var page in pages)
            {
                try
                {
                    (await PaginateThroughContent(page.Id)).ForEach(pageData =>
                    {
                        var urlPath = $"/{page.Parent.Slug}/{page.Category.Slug}/{page.Slug}";
                        IndexContent(pageData, urlPath);
                    });
                    Console.WriteLine($"Indexed {ContentToIndex.Count} items");
                }
                catch (Exception ex)
                {
                    Sinequa.Common.Sys.LogError(ex); 
                }
            }
        }

        public async Task FetchWorkplacePages(List<WorkplacePage> pages)
        {
            foreach (var page in pages)
            {
                try
                {
                    (await PaginateThroughContent(page.Id)).ForEach(pageData =>
                    {
                        var urlPath = $"/workplace/{page.Category.Slug}/{page.Slug}?city={page.City.Slug}&bldg={page.Building.Slug}";
                        IndexContent(pageData, urlPath);
                    });
                    Console.WriteLine($"Indexed {ContentToIndex.Count} items");
                }
                catch (Exception ex)
                {
                    Sinequa.Common.Sys.LogError(ex); 
                }
            }
        }

        public void IndexContent(InternalPage p, string urlPath)
        {
            toContent(p, p.SectionContentCollection, urlPath, null, null);

            p.LocationBasedContentCollection.Items.Where(l => l != null && l?.LocationSpecificContentCollection != null)
                .ToList()
                .ForEach(LocalContent =>
                {
                    var LocationName = LocalContent.Location != null ? LocalContent.Location.Name : null;
                    var LocationSlug = LocalContent.Location != null ? LocalContent.Location.Slug : null;
                    toContent(p, LocalContent.LocationSpecificContentCollection, urlPath, LocationName, LocationSlug);
                });
        }

        public void toContent(InternalPage p, ContentItemList List, string urlPath, string LocationName, string LocationSlug)
        {
            if (p?.Sys?.Id == null) return;

            foreach (var content in List.Items.Where(content => content != null))
            {
                var NewContent = new ContentToIndex
                {
                    UrlPath = urlPath,
                    PageSlug = p.Slug,
                    PageId = p.Sys.Id,
                    PageTitle = p.PageName,
                    LocationName = LocationName,
                    LocationSlug = LocationSlug,
                    PublishedAt = content.Sys.PublishedAt
                };

                switch (content)
                {
                    case SectionContent c:
                        NewContent.ContentId = c.Sys.Id;
                        NewContent.Data = JsonConvert.SerializeObject(new { title = c.SectionTitle, description = c.SectionDescription });
                        ContentToIndex.Add(NewContent);
                        break;

                    case SectionGroup co:
                        if (co.ContentCollection?.Content == null) continue;
                        foreach (var c in co.ContentCollection.Content.Where(c1 => c1 != null))
                        {
                            if (c.SectionDescription == null || c.Sys == null) continue;
                            NewContent.ContentId = c.Sys.Id;
                            NewContent.Data = JsonConvert.SerializeObject(new { title = c.SectionTitle, description = c.SectionDescription });
                            ContentToIndex.Add(NewContent);
                        }
                        break;

                    case QaSection c:
                        if (c.QuestionsCollection?.Questions == null) continue;
                        NewContent.ContentId = c.Sys.Id;
                        NewContent.Data = JsonConvert.SerializeObject(c.QuestionsCollection.Questions
                            .Where(q => q != null)
                            .Select(q => new
                            {
                                question = q.QuestionText,
                                answer = q.AnswersCollection.Answers.Where(a => a != null).Select(a => a.Content).ToList()
                            }));
                        ContentToIndex.Add(NewContent);
                        break;
                }
            }
        }
    }
}