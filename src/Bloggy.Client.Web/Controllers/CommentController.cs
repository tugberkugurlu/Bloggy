using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class CommentController : RavenController
    {
        public CommentController(IMvcLogger logger) : base(logger)
        {
        }

        [HttpPost]
        [ActionName("Create")]
        // HTTP POST /comment/create/{id}
        public Task<ActionResult> CreateComment(int id, CommentPostRequestModel requestModel)
        {
            // TODO: 1-) Check whether post exists.
            //       2-) Check whether model state is valid.
            //       3-) Sanitize the HTML input.
            //       4-) Check whether the spam check is enabled or not. If enabled, check against spam.
            //           4.1-) If spam, log the info as warning and mark the comment as spam.
            //           4.2-) If spam check throws an exception, log the error message and continue our way as the comment is not spam.
            //       5-) If not spam, 

            throw new NotImplementedException();
        }
    }
}