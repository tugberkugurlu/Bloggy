using System;
using AutoMapper;

namespace Bloggy.Client.Web.Infrastructure.AutoMapper
{
    public class NullIntTypeConverter : TypeConverter<string, int?>
    {
        protected override int? ConvertCore(string source)
        {
            int? nullableValue = new Nullable<int>();
            if (source != null)
            {
                if (source.Contains("/"))
                {
                    nullableValue = source.ToIntId();
                }
                else
                {
                    int result;
                    if (int.TryParse(source, out result))
                    {
                        nullableValue = result;
                    }
                }
            }

            return nullableValue;
        }
    }
}