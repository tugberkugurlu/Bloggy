using AutoMapper;

namespace Bloggy.Client.Web.Infrastructure.AutoMapper
{
    public class IntTypeConverter : TypeConverter<string, int>
    {
        protected override int ConvertCore(string source)
        {
            if (source == null)
            {
                throw new AutoMapperMappingException("Cannot convert null string to non-nullable return type.");
            }

            if (source.Contains("/"))
            {
                return source.ToIntId();
            }

            return int.Parse(source);
        }
    }
}