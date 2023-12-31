﻿namespace ThreadsBackend.Api.Application.Services;

using ThreadsBackend.Api.Domain.DTOs.Community;

public interface ICommunityService
{
    Task<CommunityDTO> CreateCommunity(CreateCommunityDTO data);

    Task<List<CommunityDTO>> ListCommunities(ListCommunitiesQueryDTO query);

    Task AddMemberToCommunity(string communityId, string userId);

    Task RemoveMemberFromCommunity(string communityId, string userId);

    Task<GetCommunityProfileResponseDTO> GetCommunityProfile(string id, GetCommunityProfileQueryDTO query);

    Task<List<CommunityDTO>> GetSuggestCommunities(GetSuggestCommunitiesQueryDTO query);
}