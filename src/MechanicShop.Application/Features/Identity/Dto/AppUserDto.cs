using System.Security.Claims;
using MechanicShop.Domain.Identity;

namespace MechanicShop.Application.Common.Interfaces;

public sealed record AppUserDto(string UserId, string Email, IList<string> Roles , IList<Claim> Claims);