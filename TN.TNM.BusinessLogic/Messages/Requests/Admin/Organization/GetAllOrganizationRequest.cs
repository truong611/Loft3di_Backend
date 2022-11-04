using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetAllOrganizationRequest : BaseRequest<GetAllOrganizationParameter>
    {
        public string Type { get; set; }
        public override GetAllOrganizationParameter ToParameter()
        {
            return new GetAllOrganizationParameter()
            {
                Type = this.Type,
                UserId = UserId,
            };
        }
    }

}
