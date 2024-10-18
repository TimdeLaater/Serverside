using Domain.Models;
using Domain.Services;

namespace Tests
{
    public class PersonDomainTests
    {
        private readonly PersonValidationService _personValidationService;
        public PersonDomainTests()
        {
            _personValidationService = new PersonValidationService();
        }
        [Fact]
        public void ValidatePerson_BirthdateInFuture_ReturnsError()
        {
            // Arrange
            var futureBirthDate = DateTime.Now.AddYears(1);
            var person = new Person
            {
                Name = "Future Person",
                Email = "futureperson@example.com",
                BirthDate = futureBirthDate,
                Gender = Gender.M
            };

            // Act
            var validationResult = _personValidationService.ValidatePerson(person);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains("Birthdate cannot be in the future.", validationResult.Errors);
        }

        [Fact]
        public void ValidatePerson_AgeLessThan16_ReturnsError()
        {
            // Arrange
            var underageBirthDate = DateTime.Now.AddYears(-15); // 15 years old
            var person = new Person
            {
                Name = "Underage Person",
                Email = "underageperson@example.com",
                BirthDate = underageBirthDate,
                Gender = Gender.V
            };

            // Act
            var validationResult = _personValidationService.ValidatePerson(person);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains("Person must be at least 16 years old to register.", validationResult.Errors);
        }



        [Fact]
        public void ValidatePerson_ValidPerson_NoErrors()
        {
            // Arrange
            var validBirthDate = DateTime.Now.AddYears(-20); // 20 years old
            var person = new Person
            {
                Name = "Valid Person",
                Email = "validperson@example.com",
                BirthDate = validBirthDate,
                Gender = Gender.M,
                Address = new Address(), 
                Participations = new List<BoardGameNight>(),
                Reviews = new List<Review>(),
                DietaryPreferences = new List<DietaryPreference> { DietaryPreference.Vegetarian }
            };

            // Act
            var validationResult = _personValidationService.ValidatePerson(person);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Empty(validationResult.Errors); // No errors should be present
        }
    }
}