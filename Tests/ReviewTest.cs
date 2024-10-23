using Domain.Models;
using Domain.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class ReviewTest
    {
        private readonly ReviewValidationService _reviewValidationService;
        private Address _defaultAddress;
        private Person _defaultOrganizer;
        private Person _defaultReviewer;
        private BoardGameNight _defaultBoardGameNight;

        public ReviewTest()
        {
            _reviewValidationService = new ReviewValidationService();
            _defaultAddress = CreateDefaultAddress();
            _defaultOrganizer = CreateDefaultOrganizer();
            _defaultReviewer = CreateDefaultReviewer();
            _defaultBoardGameNight = CreateDefaultBoardGameNight(_defaultOrganizer, DateTime.Now.AddDays(-1)); // Past date by default
        }

        private Address CreateDefaultAddress()
        {
            return new Address
            {
                Street = "123 Main St",
                HouseNumber = "1",
                City = "Sample City"
            };
        }

        private Person CreateDefaultOrganizer()
        {
            return new Person
            {
                PersonId = 1,
                Name = "Default Organizer",
                Email = "organizer@example.com",
                Address = _defaultAddress
            };
        }

        private Person CreateDefaultReviewer()
        {
            return new Person
            {
                PersonId = 2,
                Name = "Default Reviewer",
                Email = "reviewer@example.com",
                Address = _defaultAddress
            };
        }

        private BoardGameNight CreateDefaultBoardGameNight(Person organizer, DateTime dateTime)
        {
            return new BoardGameNight
            {
                Organizer = organizer,
                Address = _defaultAddress,
                DateTime = dateTime,
                Participants = new List<Person>(),
                Reviews = new List<Review>()
            };
        }

        [Fact]
        public void ValidateReview_PersonNotParticipating_ReturnsError()
        {
            // Arrange
            var nonParticipant = new Person { PersonId = 3, Name = "Non-Participant", Email = "nonparticipant@example.com", Address = _defaultAddress };
            _defaultBoardGameNight.Participants.Add(new Person { PersonId = 4, Name = "Other Participant", Email= "Test2mail.com", Address = _defaultAddress }); // Add a different participant

            // Act
            var result = _reviewValidationService.ValidateReview(_defaultBoardGameNight, nonParticipant);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person is not participating in this board game night.", result.Errors);
        }

        [Fact]
        public void ValidateReview_BoardGameNightNotOccurred_ReturnsError()
        {
            // Arrange
            var futureBoardGameNight = CreateDefaultBoardGameNight(_defaultOrganizer, DateTime.Now.AddDays(1)); // Future date
            futureBoardGameNight.Participants.Add(_defaultReviewer);

            // Act
            var result = _reviewValidationService.ValidateReview(futureBoardGameNight, _defaultReviewer);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Board game night has not yet happened.", result.Errors);
        }

        [Fact]
        public void ValidateReview_PersonAlreadyReviewed_ReturnsError()
        {
            // Arrange
            _defaultBoardGameNight.Participants.Add(_defaultReviewer);
            _defaultBoardGameNight.Reviews.Add(new Review
            {
                ReviewerId = _defaultReviewer.PersonId,
                ReviewText = "", // Provide some text or leave it empty if that's your requirement
                Reviewer = _defaultReviewer,
                BoardGameNight = _defaultBoardGameNight // Setting the BoardGameNight property
            });
            // Act
            var result = _reviewValidationService.ValidateReview(_defaultBoardGameNight, _defaultReviewer);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person has already reviewed this board game night.", result.Errors);
        }

        [Fact]
        public void ValidateReview_ValidReview_ReturnsValid()
        {
            // Arrange
            _defaultBoardGameNight.Participants.Add(_defaultReviewer);

            // Act
            var result = _reviewValidationService.ValidateReview(_defaultBoardGameNight, _defaultReviewer);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
