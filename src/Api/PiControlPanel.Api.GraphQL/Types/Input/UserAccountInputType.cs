namespace PiControlPanel.Api.GraphQL.Types.Input
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Authentication;

    public class UserAccountInputType : InputObjectGraphType<UserAccount>
    {
        public UserAccountInputType()
        {
            this.Field<StringGraphType>("Username");
            this.Field<StringGraphType>("Password");
        }
    }
}
