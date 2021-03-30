using AutoMapper;
using PublicElections.Contracts.Requests.Identity;
using PublicElections.Contracts.Response.Candidates;
using PublicElections.Contracts.Response.Elections;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;

namespace PublicElections.Api.MappingProfiles
{
    public class EntitiesToResponse : Profile
    {
        public EntitiesToResponse()
        {
            CreateMap<Election, ElectionResponse>();
            CreateMap<Candidate, CandidateResponse>();
            CreateMap<UserRegistrationRequest, NewUser>();
        }
    }
}



//CreateMap<WorkspaceUser, WorkspaceUsersResponse>()
//    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
//    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
//    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));