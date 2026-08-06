using Atlas.Template.Core.Dtos;
using Atlas.Template.Core.Dtos.AppUserDtos;
using Atlas.Template.Core.Models;
using Atlas.Template.Services.IServices;
using Atlas.Template.Services.Responses;
using Atlas.Template.Services.SpecificationParams;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Atlas.Template.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<Response> GetAllAsync(UserSpecParams specParams)
        {
            var usersCount = await _userManager.Users.Where(user=>
            string.IsNullOrEmpty(specParams.UserName) ||
            (user.FirstName+ ' ' +user.LastName).ToLower().Contains(specParams.UserName.ToLower())
            ).CountAsync();

            var users = await _userManager.Users.AsQueryable().Where(user =>
            string.IsNullOrEmpty(specParams.UserName) ||
            (user.FirstName + ' ' + user.LastName).ToLower().Contains(specParams.UserName.ToLower())
            ).Skip((specParams.PageIndex-1) * specParams.PageSize)
            .Take(specParams.PageSize)
            .ToListAsync();


            var pagination = new Pagination(
                specParams.PageIndex,
                specParams.PageSize,
                usersCount,
                users.Adapt<List<UserDetailsDto>>());

            return Response.Success(data: pagination);
        }

        public async Task<Response> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
                return Response.Fail(message: "User not found", 
                    statusCode: (int)HttpStatusCode.NotFound);

            return Response.Success(data: user.Adapt<UserDetailsDto>());
        }
    }
}
