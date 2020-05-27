namespace PiControlPanel.Api.GraphQL.Types.Output.Authentication
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Authentication;

    public class LoginResponseType : ObjectGraphType<LoginResponse>
    {
        public LoginResponseType()
        {
            this.Field(x => x.Username);
            this.Field("jwt", x => x.JsonWebToken);
            this.Field(x => x.Roles, false, typeof(ListGraphType<StringGraphType>));
        }
    }
}
