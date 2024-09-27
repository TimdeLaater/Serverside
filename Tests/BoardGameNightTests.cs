using Xunit;
using Domain.Models;
using Domain.Services;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class BoardGameNightTests
    {
        private readonly BoardGameNightValidationService _validator;

        public BoardGameNightTests()
        {
            _validator = new BoardGameNightValidationService();
        }

        [Fact]
        public void ValidateBoardGameNight_OrganizerUnder18_ReturnsError()
        {
            // Arrange
            var underageOrganizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-17), // 17 years old
                Name = "Young Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = underageOrganizer,
                Date = DateTime.Now.AddDays(1), // Future date
                MaxPlayers = 5
            };

            // Act
            var result = _validator.ValidateBoardGameNight(boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Organizer must be at least 18 years old.", result.Errors);
        }

        [Fact]
        public void ValidateBoardGameNight_PastDate_ReturnsError()
        {
            // Arrange
            var organizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = organizer,
                Date = DateTime.Now.AddDays(-1), // Past date
                MaxPlayers = 5
            };

            // Act
            var result = _validator.ValidateBoardGameNight(boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Board game night date cannot be in the past.", result.Errors);
        }

        [Fact]
        public void ValidateBoardGameNight_MaxPlayersZero_ReturnsError()
        {
            // Arrange
            var organizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = organizer,
                Date = DateTime.Now.AddDays(1),
                MaxPlayers = 0 // Invalid max players
            };

            // Act
            var result = _validator.ValidateBoardGameNight(boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Max players must be greater than 0.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_ParticipantUnder16_ReturnsError()
        {
            // Arrange
            var participant = new Person
            {
                BirthDate = DateTime.Now.AddYears(-15), // 15 years old
                Name = "Young Participant"
            };
            var organizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = organizer,
                Participants = new List<Person>(),
                Is18Plus = false,
                MaxPlayers = 5
            };

            // Act
            var result = _validator.ValidateParticipant(participant, boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person must be at least 16 years old to participate.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_ParticipantIsOrganizer_ReturnsError()
        {
            // Arrange
            var organizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = organizer,
                Participants = new List<Person>(),
                Is18Plus = false,
                MaxPlayers = 5
            };

            // Act
            var result = _validator.ValidateParticipant(organizer, boardGameNight); // Organizer as participant

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Organizer cannot participate in their own board game night.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_AlreadyParticipating_ReturnsError()
        {
            // Arrange
            var participant = new Person
            {
                PersonId = 1,
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Participant"
            };
            var organizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = organizer,
                Participants = new List<Person> { participant },
                Is18Plus = false,
                MaxPlayers = 5
            };

            // Act
            var result = _validator.ValidateParticipant(participant, boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person is already participating in this board game night.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_Under18For18PlusGame_ReturnsError()
        {
            // Arrange
            var participant = new Person
            {
                BirthDate = DateTime.Now.AddYears(-17), // 17 years old
                Name = "Young Participant"
            };
            var organizer = new Person
            {
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Organizer"
            };
            var boardGameNight = new BoardGameNight
            {
                Organizer = organizer,
                Participants = new List<Person>(),
                Is18Plus = true, // 18+ game
                MaxPlayers = 5
            };

            // Act
            var result = _validator.ValidateParticipant(participant, boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person must be at least 18 years old to participate in this board game night.", result.Errors);
        }
    }
}
