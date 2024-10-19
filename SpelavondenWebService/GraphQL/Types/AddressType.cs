using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class AddressType : ObjectGraphType<Address>
    {
        public AddressType()
        {
            Field(x => x.Street).Description("The street of the address.");
            Field(x => x.City).Description("The city of the address.");
            Field(x => x.HouseNumber).Description("The house number of the address.");
        }
    }
}
