using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloggy.Domain.Managers
{
    public class DynamicPageManager : IDynamicPageManager
    {
        private readonly IAsyncDocumentSession _documentSession;

        public DynamicPageManager(IAsyncDocumentSession documentSession)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            _documentSession = documentSession;
        }
    }
}
