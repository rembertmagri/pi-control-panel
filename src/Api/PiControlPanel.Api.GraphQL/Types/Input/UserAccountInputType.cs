namespace PiControlPanel.Api.GraphQL.Types.Input
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Authentication;

    /// <summary>
    /// The UserAccount GraphQL input type.
    /// </summary>
    public class UserAccountInputType : InputObjectGraphType<UserAccount>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountInputType"/> class.
        /// </summary>
        public UserAccountInputType()
        {
            this.Field<StringGraphType>("Username");
            this.Field<StringGraphType>("Password");
        }
    }
}
