using H4_TrashPlusPlus.Entities;
using XPowerClassLibrary.Users.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XPowerClassLibrary.Users.Tokens.RefreshTokens
{
    interface IRefreshTokenGenerator
    {
        RefreshToken CreateRefreshToken(IUser selectedUser, string ipAddress);
        RefreshToken ReplaceRefreshToken(IUser selectedUser, string ipAddress, string token);
        bool RevokeRefreshToken(IUser selectedUser, string ipAddress, string token);
    }
}
