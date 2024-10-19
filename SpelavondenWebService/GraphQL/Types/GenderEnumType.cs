using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class GenderEnumType : EnumerationGraphType<Gender>
    {
        public GenderEnumType()
        {
            Name = "Gender";
            Description = "The gender of the person.";
        }
    }
}
