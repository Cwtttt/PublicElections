using AutoMapper;

namespace PublicElections.Api.MappingProfiles
{
    public class EntitiesToDto : Profile
    {
        public EntitiesToDto()
        {
            //CreateMap<WorkspaceUser, WorkspaceUsersResponse>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            //    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            //    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
        }
    }
}