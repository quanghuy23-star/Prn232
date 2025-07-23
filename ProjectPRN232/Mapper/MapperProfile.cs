using AutoMapper;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;

namespace ProjectPRN232.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewsArticle, NewsArticleDTO>()
                .ForMember(dest => dest.NewsStatusName, opt => opt.MapFrom(src => src.NewsStatus.ToString()))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
    .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.Category.ParentCategory.CategoryName))
    .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.AccountName))
    .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedBy.AccountName))
                 .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

            CreateMap<NewsArticleCreateDTO, NewsArticle>()
     .ForMember(dest => dest.NewsStatus, opt => opt.Ignore());

            CreateMap<Tag, TagDTO>();

            CreateMap<NewsArticleUpdateDTO, NewsArticle>();

            CreateMap<SystemAccount, LoginResponseDTO>();
            CreateMap<LoginRequestDTO, SystemAccount>();

            // Comment
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.AccountName))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies.Where(r => r.IsActive)));

            CreateMap<CommentCreateDTO, Comment>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            // NewsLike
            CreateMap<NewsLike, NewsLikeDTO>();
        }
    }
}
