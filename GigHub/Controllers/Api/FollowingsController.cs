﻿using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Http;
using GigHub.Core;
using GigHub.Core.Dtos;
using GigHub.Core.Models;

namespace GigHub.Controllers.Api
{
    [Authorize]
    public class FollowingsController : ApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public FollowingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IHttpActionResult Follow(FollowingDto dto)
        {
            var userId = User.Identity.GetUserId();

            var followingExists = _unitOfWork.Followings
                            .GetFollowing(userId, dto.FolloweeId)
                            .Any();

            if (followingExists)
                return BadRequest("Following already exists.");

            var following = new Following
            {
                FollowerId = userId,
                FolloweeId = dto.FolloweeId
            };

            _unitOfWork.Followings.Add(following);
            _unitOfWork.Complete();
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult UnFollow(string id)
        {
            var userId = User.Identity.GetUserId();

            var following = _unitOfWork.Followings
                .GetFollowing(userId, id).SingleOrDefault();

            if (following == null)
            {
                return NotFound();
            }
            _unitOfWork.Followings.Remove(following);
            _unitOfWork.Complete();

            return Ok();
        }
    }
}