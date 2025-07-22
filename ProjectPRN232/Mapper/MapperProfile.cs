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
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
               .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.AccountName))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedBy.AccountName))
                .ForMember(dest => dest.TagNames, opt => opt.MapFrom(src => src.Tags.Select(t=>t.TagName)));
                
            CreateMap<NewsArticleCreateDTO, NewsArticle>();
            CreateMap<NewsArticleUpdateDTO, NewsArticle>();

            CreateMap<SystemAccount, LoginResponseDTO>();
            CreateMap<LoginRequestDTO, SystemAccount>();
        }
    }
}
