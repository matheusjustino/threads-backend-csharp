﻿namespace ThreadsBackend.Api.Application.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ThreadsBackend.Api.Domain.DTOs.Community;
using ThreadsBackend.Api.Domain.DTOs.Thread;
using ThreadsBackend.Api.Domain.DTOs.User;
using ThreadsBackend.Api.Domain.Enums;
using ThreadsBackend.Api.Domain.Entities;
using ThreadsBackend.Api.Infrastructure.Persistence;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;

    private readonly AppDbContext _context;

    private readonly IMapper _mapper;

    private readonly IManageImageService _manageImageService;

    private readonly IServiceProvider _serviceProvider;

    public UserService(
        ILogger<UserService> logger,
        AppDbContext context,
        IMapper mapper,
        IManageImageService manageImageService,
        IServiceProvider serviceProvider)
    {
        this._logger = logger;
        this._context = context;
        this._mapper = mapper;
        this._manageImageService = manageImageService;
        this._serviceProvider = serviceProvider;
    }

    public async Task<List<UserDTO>> ListUsers(ListUsersQueryDTO query)
    {
        this._logger.LogInformation($"List Users - query: {query.ToString()}");

        var usersQuery = this._context.Users
            .Where(u => u.Id != query.UserId);
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            usersQuery = usersQuery.Where(u =>
                u.Name.ToLower().Contains(query.SearchTerm.ToLower()) ||
                u.Username.ToLower().Contains(query.SearchTerm.ToLower()));
        }

        var users = await usersQuery
            .OrderBy(u => u.Name)
            .Skip(query.Skip * query.Take)
            .Take(query.Take)
            .Select(u => this._mapper.Map<UserDTO>(u))
            .ToListAsync();
        return users;
    }

    public async Task<UserDTO> GetUser(string id)
    {
        var user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            throw new BadHttpRequestException("User not found");
        }

        return this._mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> UpdateUser(string userId, UpdateUserDTO data)
    {
        this._logger.LogInformation($"Update User - userId: {userId} data: {data.ToString()}");

        await using var transaction = await this._context.Database.BeginTransactionAsync();
        string? filename = null;

        var user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var baseUrlUserImg = isDevelopment
            ? "http://localhost:8080/api/images/"
            : "https://threads-backend-b8daee83f8bc.herokuapp.com/api/images/";
        try
        {
            if (user is null)
            {
                if (data.ProfilePhoto != null)
                {
                    filename = await this._manageImageService.UploadFile(data.ProfilePhoto);
                    filename = baseUrlUserImg + filename;
                }

                var newUser = new User
                {
                    Id = userId,
                    Name = data.Name ?? string.Empty,
                    Username = data.Username ?? string.Empty,
                    Bio = data.Bio ?? string.Empty,
                    ProfilePhoto = filename ?? string.Empty,
                    Onboarded = true,
                };

                await this._context.Users.AddAsync(newUser);
                await this._context.SaveChangesAsync();
                await transaction.CommitAsync();

                return this._mapper.Map<UserDTO>(newUser);
            }

            if (data.ProfilePhoto != null)
            {
                filename = await this._manageImageService.UploadFile(data.ProfilePhoto);
                filename = baseUrlUserImg + filename;
                this._manageImageService.DeleteImage(user.ProfilePhoto);
            }

            user.Name = data.Name ?? user.Name;
            user.Username = data.Username ?? user.Username;
            user.Bio = data.Bio ?? user.Bio;
            user.ProfilePhoto = filename ?? user.ProfilePhoto;
            user.Onboarded = true;

            this._context.Users.Update(user);
            await this._context.SaveChangesAsync();
            await transaction.CommitAsync();

            return this._mapper.Map<UserDTO>(user);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            if (!string.IsNullOrEmpty(filename))
            {
                this._manageImageService.DeleteImage(filename);
            }

            throw;
        }
    }

    public async Task<List<ThreadDTO>> GetUserActivity(string userId)
    {
        this._logger.LogInformation($"Get User Activity - userId: {userId}");

        var userThreads = await this._context.Threads
            .Where(t => t.AuthorId == userId)
            .Include(t => t.Community)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
            .ToListAsync();

        var allComments = userThreads.SelectMany(t => t.Comments.Where(c => c.AuthorId != userId)).ToList();

        return this._mapper.Map<List<ThreadDTO>>(allComments);
    }

    public async Task<GetUserProfileResponseDTO> GetUserProfile(string userId, GetUserProfileQueryDTO query)
    {
        this._logger.LogInformation("Get User Profile - query: {Query}", query);

        var user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            throw new BadHttpRequestException("User not found");
        }

        var threadsContext = this._serviceProvider.GetService<AppDbContext>();
        var repliesContext = this._serviceProvider.GetService<AppDbContext>();

        var threadsTask = threadsContext.Threads
            .Where(t => t.AuthorId == userId)
            .Where(t => t.ParentThreadId == null)
            .Select(t => new ThreadDTO
            {
                Id = t.Id,
                Text = t.Text,
                AuthorId = t.AuthorId,
                Author = new UserDTO
                {
                    Id = t.Author.Id,
                    Name = t.Author.Name,
                    Username = t.Author.Username,
                    ProfilePhoto = t.Author.ProfilePhoto,
                },
                ParentThreadId = t.ParentThreadId,
                CommunityId = t.CommunityId,
                Community = t.Community == null
                    ? null
                    : new CommunityDTO
                    {
                        Id = t.Community.Id,
                        Name = t.Community.Name,
                        Username = t.Community.Username,
                        Image = t.Community.Image,
                        CreatedAt = t.Community.CreatedAt,
                    },
                CommentsCount = t.Comments.Count,
                CreatedAt = t.CreatedAt,
            }).ToListAsync();
        var repliesTask = repliesContext.Threads
            .Where(t => t.AuthorId == userId)
            .Where(t => t.ParentThreadId != null)
            .Select(t => new ThreadDTO
            {
                Id = t.Id,
                Text = t.Text,
                AuthorId = t.AuthorId,
                Author = new UserDTO
                {
                    Id = t.Author.Id,
                    Name = t.Author.Name,
                    Username = t.Author.Username,
                    ProfilePhoto = t.Author.ProfilePhoto,
                },
                ParentThreadId = t.ParentThreadId,
                CommunityId = t.CommunityId,
                Community = t.Community == null
                    ? null
                    : new CommunityDTO
                    {
                        Id = t.Community.Id,
                        Name = t.Community.Name,
                        Username = t.Community.Username,
                        Image = t.Community.Image,
                        CreatedAt = t.Community.CreatedAt,
                    },
                CommentsCount = t.Comments.Count,
                CreatedAt = t.CreatedAt,
            }).ToListAsync();
        await Task.WhenAll(threadsTask, repliesTask);

        return new GetUserProfileResponseDTO
        {
            Profile = this._mapper.Map<UserDTO>(user),
            Threads = this._mapper.Map<List<ThreadDTO>>(threadsTask.Result),
            Replies = this._mapper.Map<List<ThreadDTO>>(repliesTask.Result),
        };
    }

    public async Task<List<UserDTO>> GetSuggestUsers(GetSuggestUsersQueryDTO query)
    {
        this._logger.LogInformation($"Get Suggest Users - query: {query.ToString()}");

        if (query.Count is 0)
        {
            return new List<UserDTO>();
        }

        var random = new Random();
        var userIds = await this._context.Users.Select(user => user.Id).ToListAsync();
        var shuffledUserIds = userIds.OrderBy(_ => random.Next()).ToList();

        var randomUsers = await this._context.Users
            .Where(user => shuffledUserIds.Contains(user.Id))
            .Take(query.Count ?? 4)
            .ToListAsync();

        return this._mapper.Map<List<UserDTO>>(randomUsers);
    }
}