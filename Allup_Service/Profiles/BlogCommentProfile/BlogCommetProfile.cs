using Allup_Core.Entities;
using Allup_Service.Dtos.AppUserDtos;
using Allup_Service.Dtos.BlogCommentDtos;
using AutoMapper;

namespace Allup_Service.Profiles.BlogCommentProfile
{
    public class BlogCommetProfile:Profile
    {
        public BlogCommetProfile()
        {

            //BlogComment Profiles
            CreateMap<BlogComment, BlogCommentCreateDto>().ReverseMap();
            CreateMap<BlogComment, BlogCommentUpdateDto>().ReverseMap();
            CreateMap<BlogComment, BlogCommentGetDto>()
                .ForMember(dest => dest.AppUser, opt => opt.MapFrom(src => src.AppUser))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children)) // children map
                .ReverseMap();
            CreateMap<BlogComment, BlogCommentReplyDto>().ReverseMap();
            CreateMap<AppUser, UserGetDto>().ReverseMap();
        }
    }
}
