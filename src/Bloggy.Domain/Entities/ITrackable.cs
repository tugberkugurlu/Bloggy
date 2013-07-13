using System;

namespace Bloggy.Domain.Entities
{
    public interface ITrackable
    {
        DateTimeOffset CreatedOn { get; set; }
        string CreationIp { get; set; }
        DateTimeOffset LastUpdatedOn { get; set; }
        string LastUpdateIp { get; set; }
    }
}
