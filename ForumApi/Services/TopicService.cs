using AutoMapper;
using ForumApi.Models.Dtos.Topic;
using ForumApi.Models.Entities;
using ForumApi.Repositories;
using ForumApi.Repositories.Interfaces;
using ForumApi.Services.Interfaces;

namespace ForumApi.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _repository;
        private readonly IMapper _mapper;

        public TopicService(ITopicRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TopicResponseDto>> GetAllAsync()
        {
            var topics = await _repository.GetAllAsync();
            return _mapper.Map<List<TopicResponseDto>>(topics);
        }

        public async Task<TopicResponseDto?> GetByIdAsync(int id)
        {
            var topic = await _repository.GetByIdWithUserAndLikesAsync(id);
            if (topic == null) return null;

           

            return _mapper.Map<TopicResponseDto>(topic);
        }


        public async Task<TopicResponseDto> CreateAsync(CreateTopicDto dto, int userId)
        {
            var topic = _mapper.Map<Topic>(dto);
            topic.UserId = userId;
            topic.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(topic);
            await _repository.SaveChangesAsync();

            return _mapper.Map<TopicResponseDto>(topic);
        }

        public async Task<bool> DeleteAsync(int id, int requestingUserId, bool isAdmin)
        {
            var topic = await _repository.GetByIdAsync(id);
            if (topic == null) return false;

            if (topic.UserId != requestingUserId && !isAdmin) return false;

            await _repository.DeleteAsync(topic);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
