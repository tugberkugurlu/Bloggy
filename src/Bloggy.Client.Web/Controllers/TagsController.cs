using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class TagsController : RavenController
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IMappingEngine _mapper;

        public TagsController(IMvcLogger logger, IAsyncDocumentSession documentSession, IMappingEngine mapper)
            : base(logger)
        {

            _documentSession = documentSession;
            _mapper = mapper;
        }
   
        // HTTP GET /tags
        public async Task<ActionResult> Index()
        {
            var tags = await RetrieveTagsAsync();

            if (tags.Count() < 1)
            {
                return HttpNotFound();
            }

            TagsViewModel viewModel = await ConstructorTagsViewModelAsync(tags);

            return View(viewModel);
        }

        private async Task<TagsViewModel> ConstructorTagsViewModelAsync(IEnumerable<Tags_Count.ReduceResult> tagsCount)
        {
            return new TagsViewModel()
            {
                Tags = _mapper.Map<IEnumerable<Tags_Count.ReduceResult>, IEnumerable<TagModel>>(tagsCount)
            };
        }

        private async Task<IEnumerable<Tags_Count.ReduceResult>> RetrieveTagsAsync()
        {
            IList<Tags_Count.ReduceResult> tags= await _documentSession
                .Query<Tags_Count.ReduceResult, Tags_Count>()
                .OrderByDescending(tag => tag.Count)
                .ToListAsync();

            return tags;
        }
    }
}