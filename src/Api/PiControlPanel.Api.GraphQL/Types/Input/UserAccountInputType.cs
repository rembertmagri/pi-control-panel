namespace PiControlPanel.Api.GraphQL.Types.Input
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Authentication;

    public class UserAccountInputType : InputObjectGraphType<UserAccount>
    {
        public UserAccountInputType()
        {
            Field<StringGraphType>("Username");
            Field<StringGraphType>("Password");
        }
    }
}
