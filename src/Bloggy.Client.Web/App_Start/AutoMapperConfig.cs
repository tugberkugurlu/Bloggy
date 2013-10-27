using AutoMapper;
using Bloggy.Client.Web.Areas.Admin.Models;
using Bloggy.Client.Web.Areas.Admin.RequestModels;
using Bloggy.Client.Web.Infrastructure.Mapping;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.RequestModels;
using Bloggy.Domain.Entities;
using Bloggy.Wrappers.Akismet.RequestModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bloggy.Client.Web
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            // For mapping ravendb string ids to integer
            Mapper.CreateMap<string, int>().ConvertUsing(new IntTypeConverter());
            Mapper.CreateMap<string, int?>().ConvertUsing(new NullIntTypeConverter());

            Mapper.CreateMap<CommentPostRequestModel, AkismetCommentRequestModel>()
                .ForMember(dest => dest.CommentAuthor, opt => opt.MapFrom(src => src.CommentAuthorName))
                .ForMember(dest => dest.CommentContent, opt => opt.MapFrom(src => src.SanitizedCommentContent));

            Mapper.CreateMap<CommentPostRequestModel, BlogPostComment>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.SanitizedCommentContent))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.CommentAuthorEmail))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommentAuthorName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.CommentAuthorUrl));

            Mapper.CreateMap<BlogPostRequestModel, BlogPost>()
                .ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Slugs, opt => opt.MapFrom(src => new[] { new Slug { Path = src.Title.ToSlug(), IsDefault = true, CreatedOn = DateTimeOffset.Now } }))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => new Tag { Name = tag, Slug = tag.ToSlug() })));

            Mapper.CreateMap<BlogPost, BlogPostModel>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Name)))
                .ForMember(dest => dest.Slugs, opt => opt.MapFrom(src => src.Slugs.Select(slug => slug.Path)));

            Mapper.CreateMap<BlogPostComment, BlogPostCommentModel>();
            Mapper.CreateMap<BlogPost, BlogPostModelLight>()
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slugs.Select(slug => slug.Path).FirstOrDefault()));
            Mapper.CreateMap<Tag, TagModel>();
        }
    }
}