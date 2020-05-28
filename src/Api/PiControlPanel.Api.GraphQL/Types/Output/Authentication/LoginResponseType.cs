namespace PiControlPanel.Api.GraphQL.Types.Output.Authentication
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Authentication;

    /// <summary>
    /// The LoginResponse GraphQL output type.
    /// </summary>
    public class LoginResponseType : ObjectGraphType<LoginResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResponseType"/> class.
        /// </summary>
        public LoginResponseType()
        {
            this.Field(x => x.Username);
            this.Field("jwt", x => x.JsonWebToken);
            this.Field(x => x.Roles, false, typeof(ListGraphType<StringGraphType>));
        }
    }
}
