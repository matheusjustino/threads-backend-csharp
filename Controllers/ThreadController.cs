﻿namespace ThreadsBackend.Controllers;

using Microsoft.AspNetCore.Mvc;
using ThreadsBackend.DTOs.Thread;
using ThreadsBackend.Services;

[Route("api/threads")]
[ApiController]
public class ThreadController : ControllerBase
{
    private readonly IThreadService _threadService;

    public ThreadController(IThreadService threadService)
    {
        this._threadService = threadService;
    }

    [HttpPost]
    public async Task<ActionResult<ThreadDTO>> CreateThread([FromBody] CreateThreadDTO body)
    {
        var thread = await this._threadService.CreateThread(body);
        return Ok(thread);
    }

    [HttpGet]
    public async Task<ActionResult<List<ThreadDTO>>> ListThreads([FromQuery] ListThreadsQueryDTO query)
    {
        var threads = await this._threadService.ListThreads(query);
        return Ok(threads);
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<GetUserThreadsResponseDTO>> GetUserThreads([FromRoute] string id)
    {
        var userThreads = await this._threadService.GetUserThreads(id);
        return Ok(userThreads);
    }

    [HttpGet("community/{id}")]
    public async Task<ActionResult<GetCommunityThreadsResponseDTO>> GetCommunityThreads([FromRoute] string id)
    {
        var communityThreads = await this._threadService.GetCommunityThreads(id);
        return Ok(communityThreads);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ThreadDTO>> GetThread([FromRoute] Guid id)
    {
        var thread = await this._threadService.GetThread(id);
        return Ok(thread);
    }

    [HttpPost("add/comment")]
    public async Task<ActionResult<ThreadDTO>> AddCommentToThread([FromBody] AddCommentDTO body)
    {
        var thread = await this._threadService.AddCommentToThread(body);
        return Ok(thread);
    }
}