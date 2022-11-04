﻿using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Messages.Parameters.Users;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Permission;
using TN.TNM.DataAccess.Messages.Results.Users;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IAuthDataAccess
    {
        LoginResult Login(LoginParameter paramater, string secretKey, string issuer, string audience);
        GetUserPermissionResult GetUserPermission(GetUserPermissionParameter parameter);
        ChangePasswordResult ChangePassword(ChangePasswordParameter parameter);
        GetUserProfileResult GetUserProfile(GetUserProfileParameter parameter);
        GetUserProfileByEmailResult GetUserProfileByEmail(GetUserProfileByEmailParameter parameter);
        EditUserProfileResult EditUserProfile(EditUserProfileParameter parameter);
        GetAllUserResult GetAllUser(GetAllUserParameter parameter);
        GetCheckUserNameResult GetCheckUserName(GetCheckUserNameParameter parameter);
        GetCheckResetCodeUserResult GetCheckResetCodeUser(GetCheckResetCodeUserParameter parameter);
        ResetPasswordResult ResetPassword(ResetPasswordParameter parameter);
        GetPositionCodeByPositionIdResult GetPositionCodeByPositionId(GetPositionCodeByPositionIdParameter parameter);
        GetAllRoleResult GetAllRole(GetAllRoleParameter parameter);
        GetCreatePermissionResult GetCreatePermission(GetCreatePermissionParameter parameter);
        CreateRoleAndPermissionResult CreateRoleAndPermission(CreateRoleAndPermissionParameter parameter);
        GetDetailPermissionResult GetDetailPermission(GetDetailPermissionParameter parameter);
        EditRoleAndPermissionResult EditRoleAndPermission(EditRoleAndPermissionParameter parameter);
        AddUserRoleResult AddUserRole(AddUserRoleParameter parameter);
        DeleteRoleResult DeleteRole(DeleteRoleParameter parameter);
    }
}
