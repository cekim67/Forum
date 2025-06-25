using AutoMapper;
using ForumApi.Models.Entities;
using ForumApi.Models.Dtos.Topic;
using ForumApi.Models.Dtos.Reply;
using ForumApi.Models.Dtos.User;

namespace ForumApi.MappingProfiles
{
    public class ForumProfile : Profile
    {
        public ForumProfile()
        {
            // Create -> Entity
            CreateMap<CreateTopicDto, Topic>();

            // Entity -> Response
            CreateMap<Topic, TopicResponseDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount)); ;

            CreateMap<CreateReplyDto, Reply>();

            CreateMap<Reply, ReplyResponseDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.ChildReplies, opt => opt.MapFrom(src => src.ChildReplies));
            CreateMap<User, UserDto>();


        }
    }
}