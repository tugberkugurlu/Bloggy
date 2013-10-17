using AutoMapper;
using Bloggy.Client.Web.RequestModels;
using Bloggy.Domain.Entities;
using Bloggy.Wrappers.Akismet.RequestModels;

namespace Bloggy.Client.Web
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<CommentPostRequestModel, AkismetCommentRequestModel>()
                .ForMember(dest => dest.CommentAuthor, opt => opt.MapFrom(src => src.CommentAuthorName))
                .ForMember(dest => dest.CommentContent, opt => opt.MapFrom(src => src.SanitizedCommentContent));

            Mapper.CreateMap<CommentPostRequestModel, BlogPostComment>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.SanitizedCommentContent))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.CommentAuthorEmail))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommentAuthorName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.CommentAuthorUrl));
        }
    }
}