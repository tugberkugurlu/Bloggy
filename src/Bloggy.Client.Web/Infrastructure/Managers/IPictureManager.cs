using System.IO;
using System.Threading.Tasks;

namespace Bloggy.Client.Web.Infrastructure.Managers
{
    public interface IPictureManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="containerReferenceName"></param>
        /// <param name="pictureReferenceName"></param>
        /// <returns>Returns the URI of the created picture.</returns>
        Task<string> UploadAsync(Stream inputStream, string containerReferenceName, string pictureReferenceName, string contentType);
    }
}