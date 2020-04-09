namespace PiControlPanel.Api.GraphQL.Types.Output.Authentication
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Authentication;

    public class LoginResponseType : ObjectGraphType<LoginResponse>
    {
        public LoginResponseType()
        {
            Field(x => x.Username);
            Field("jwt", x => x.JsonWebToken);
            Field(x => x.Roles, false, typeof(ListGraphType<StringGraphType>));
        }
    }
}
