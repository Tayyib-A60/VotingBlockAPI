using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers.DTOs;
using API.Core;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [ApiController]
    // [Authorize]
    [Route ("/api/voting/election")]
    public class ElectionController : Controller {
        private IElectionRepository _repository { get; }
        private IUnitOfWork _unitOfWork { get; }
        private IMapper _mapper { get; }
        public ElectionController (IElectionRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateElection ([FromBody] ElectionDTO electionDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest (ModelState);
            var election = _mapper.Map<Election>(electionDTO);
            election.DateCreated = DateTime.Now;
            _repository.CreateElection(election);
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpGet("getAll/{userId}")]
        public async Task<IEnumerable<Election>> GetAll(int userId)
        {
            var elections = await _repository.GetElections(userId);
            return elections;
        }
        [HttpGet("getOne/{electionId}/{userId}")]
        public async Task<Election> GetOne(int electionId, int userId)
        {
            return await _repository.GetElection(electionId, userId);
        }
    }
}