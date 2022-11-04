using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Messages.Parameters.Users;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Permission;
using TN.TNM.DataAccess.Messages.Results.Users;

namespace TN.TNM.Api.Controllers
{
    public class AuthController : Controller
    {
        private IAuthDataAccess AuthDataAccess;
        private IConfiguration Configuration;        

        public AuthController(IConfiguration configuration, IAuthDataAccess iAuthDataAccess)
        {
            this.Configuration = configuration;
            this.AuthDataAccess = iAuthDataAccess;
        }

        [Route("api/auth")]
        [HttpPost]
        [AllowAnonymous]
        public LoginResult GetAuthToken([FromBody]LoginParameter request)
        {
            var response = this.AuthDataAccess.Login(request,
                this.Configuration["secret-key-name"],
                this.Configuration["token-valid-issuer"],
                this.Configuration["token-valid-audience"]);

            return response;
        }

        [Route("api/auth/getUserPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserPermissionResult GetUserPermission([FromBody]GetUserPermissionParameter request)
        {
            var response = this.AuthDataAccess.GetUserPermission(request);
            return response;
        }

        [Route("api/auth/changePassword")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public ChangePasswordResult ChangePassword([FromBody]ChangePasswordParameter request)
        {
            var response = this.AuthDataAccess.ChangePassword(request);
            return response;
        }

        [Route("api/auth/getUserProfile")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileResult GetUserProfile([FromBody]GetUserProfileParameter request)
        {
            var response = this.AuthDataAccess.GetUserProfile(request);
            return response;
        }

        [Route("api/auth/getUserProfileByEmail")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileByEmailResult GetUserProfileByEmail([FromBody]GetUserProfileByEmailParameter request)
        {
            var response = this.AuthDataAccess.GetUserProfileByEmail(request);
            return response;
        }

        [Route("api/auth/editUserProfile")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditUserProfileResult EditUserProfile([FromBody]EditUserProfileParameter request)
        {
            var response = this.AuthDataAccess.EditUserProfile(request);
            return response;
        }

        [Route("api/auth/getAllUser")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllUserResult GetAllUser(
            [FromBody]GetAllUserParameter request)
        {
            var response = this.AuthDataAccess.GetAllUser(request);
            return response;
        }

        [Route("api/auth/getCheckUserName")]
        [HttpPost]
        [AllowAnonymous]
        public GetCheckUserNameResult GetCheckUserName(
            [FromBody]GetCheckUserNameParameter request)
        {
            var response = this.AuthDataAccess.GetCheckUserName(request);
            return response;
        }

        [Route("api/auth/getCheckResetCodeUser")]
        [HttpPost]
        [AllowAnonymous]
        public GetCheckResetCodeUserResult GetCheckResetCodeUser(
            [FromBody]GetCheckResetCodeUserParameter request)
        {
            var response = this.AuthDataAccess.GetCheckResetCodeUser(request);
            return response;
        }

        [Route("api/auth/resetPassword")]
        [HttpPost]
        [AllowAnonymous]
        public ResetPasswordResult ResetPassword(
            [FromBody]ResetPasswordParameter request)
        {
            var response = this.AuthDataAccess.ResetPassword(request);
            return response;
        }

        [Route("api/auth/getPositionCodeByPositionId")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPositionCodeByPositionIdResult GetPositionCodeByPositionId(
            [FromBody]GetPositionCodeByPositionIdParameter request)
        {
            var response = this.AuthDataAccess.GetPositionCodeByPositionId(request);
            return response;
        }

        [Route("api/auth/getAllRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllRoleResult GetAllRole([FromBody]GetAllRoleParameter request)
        {
            var response = this.AuthDataAccess.GetAllRole(request);
            return response;
        }

        [Route("api/auth/getCreatePermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetCreatePermissionResult GetCreatePermission([FromBody]GetCreatePermissionParameter request)
        {
            var response = this.AuthDataAccess.GetCreatePermission(request);
            return response;
        }

        [Route("api/auth/createRoleAndPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public CreateRoleAndPermissionResult CreateRoleAndPermission([FromBody]CreateRoleAndPermissionParameter request)
        {
            var response = this.AuthDataAccess.CreateRoleAndPermission(request);
            return response;
        }

        [Route("api/auth/getDetailPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetDetailPermissionResult GetDetailPermission([FromBody]GetDetailPermissionParameter request)
        {
            var response = this.AuthDataAccess.GetDetailPermission(request);
            return response;
        }

        [Route("api/auth/editRoleAndPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditRoleAndPermissionResult EditRoleAndPermission([FromBody]EditRoleAndPermissionParameter request)
        {
            var response = this.AuthDataAccess.EditRoleAndPermission(request);
            return response;
        }

        [Route("api/auth/addUserRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public AddUserRoleResult AddUserRole([FromBody]AddUserRoleParameter request)
        {
            var response = this.AuthDataAccess.AddUserRole(request);
            return response;
        }

        [Route("api/auth/deleteRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public DeleteRoleResult DeleteRole([FromBody]DeleteRoleParameter request)
        {
            var response = this.AuthDataAccess.DeleteRole(request);
            return response;
        }

    }
}
